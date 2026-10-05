using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Exceptions;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GtMotive.Estimate.Microservice.Infrastructure.Repositories.MongoDb
{
    /// <summary>
    /// MongoDB adapter of <see cref="IVehicleRepository"/>.
    /// </summary>
    public sealed class MongoVehicleRepository : IVehicleRepository
    {
        public const string CollectionName = "vehicles";

        private const string LicensePlateIndexName = "ux_licensePlate";

        private readonly IMongoCollection<VehicleDocument> _collection;
        private readonly Lazy<Task> _indexesCreation;

        public MongoVehicleRepository(MongoService mongoService, IOptions<MongoDbSettings> options)
        {
            ArgumentNullException.ThrowIfNull(mongoService);
            ArgumentNullException.ThrowIfNull(options);

            _collection = mongoService.MongoClient
                .GetDatabase(options.Value.MongoDbDatabaseName)
                .GetCollection<VehicleDocument>(CollectionName);

            _indexesCreation = new Lazy<Task>(CreateIndexes);
        }

        public async Task Add(Vehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            await _indexesCreation.Value;

            try
            {
                await _collection.InsertOneAsync(ToDocument(vehicle));
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                throw new LicensePlateAlreadyExistsException($"A vehicle with license plate {vehicle.LicensePlate} already exists in the fleet.", ex);
            }
        }

        public async Task Update(Vehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            await _indexesCreation.Value;

            try
            {
                await _collection.ReplaceOneAsync(d => d.Id == ToId(vehicle.Id), ToDocument(vehicle));
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                // The partial unique index on "rentedBy" guarantees one active rental per person, even with concurrent requests.
                throw new RenterAlreadyHasVehicleException($"The person {vehicle.CurrentRental?.Renter} already has a vehicle rented.", ex);
            }
        }

        public async Task<Vehicle> GetById(VehicleId id)
        {
            var document = await _collection.Find(d => d.Id == ToId(id)).FirstOrDefaultAsync();
            return document is null ? null : ToVehicle(document);
        }

        public async Task<IReadOnlyCollection<Vehicle>> GetAvailable()
        {
            var documents = await _collection.Find(d => d.RentedBy == null).ToListAsync();
            return documents.Select(ToVehicle).ToList();
        }

        public async Task<bool> ExistsWithLicensePlate(LicensePlate licensePlate)
        {
            return await _collection.Find(d => d.LicensePlate == licensePlate.Value).AnyAsync();
        }

        public async Task<bool> HasActiveRental(RenterId renter)
        {
            return await _collection.Find(d => d.RentedBy == renter.Value).AnyAsync();
        }

        private static string ToId(VehicleId id) => id.Value.ToString("D", CultureInfo.InvariantCulture);

        private static VehicleDocument ToDocument(Vehicle vehicle) => new()
        {
            Id = ToId(vehicle.Id),
            Brand = vehicle.Brand,
            Model = vehicle.Model,
            LicensePlate = vehicle.LicensePlate.Value,
            ManufactureDate = DateTime.SpecifyKind(vehicle.ManufactureDate.Date, DateTimeKind.Utc),
            RentedBy = vehicle.CurrentRental?.Renter.Value,
            RentedAt = vehicle.CurrentRental?.StartedAt,
        };

        private static Vehicle ToVehicle(VehicleDocument document) => Vehicle.Restore(
            new VehicleId(Guid.Parse(document.Id, CultureInfo.InvariantCulture)),
            document.Brand,
            document.Model,
            new LicensePlate(document.LicensePlate),
            document.ManufactureDate.Date,
            document.RentedBy is null ? null : new Rental(new RenterId(document.RentedBy), document.RentedAt.GetValueOrDefault()));

        private async Task CreateIndexes()
        {
            var keys = Builders<VehicleDocument>.IndexKeys;

            var licensePlateIndex = new CreateIndexModel<VehicleDocument>(
                keys.Ascending(d => d.LicensePlate),
                new CreateIndexOptions { Name = LicensePlateIndexName, Unique = true });

            // Only documents with an active rental have "rentedBy", so a person can only appear once.
            var renterIndex = new CreateIndexModel<VehicleDocument>(
                keys.Ascending(d => d.RentedBy),
                new CreateIndexOptions<VehicleDocument>
                {
                    Name = "ux_rentedBy",
                    Unique = true,
                    PartialFilterExpression = Builders<VehicleDocument>.Filter.Type(d => d.RentedBy, MongoDB.Bson.BsonType.String),
                });

            await _collection.Indexes.CreateManyAsync([licensePlateIndex, renterIndex]);
        }
    }
}
