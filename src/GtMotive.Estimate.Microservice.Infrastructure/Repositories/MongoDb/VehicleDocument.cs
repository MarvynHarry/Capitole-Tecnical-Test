using System;
using MongoDB.Bson.Serialization.Attributes;

namespace GtMotive.Estimate.Microservice.Infrastructure.Repositories.MongoDb
{
    /// <summary>
    /// Persistence model of a vehicle in MongoDB. Keeps the domain model free of BSON concerns.
    /// </summary>
    public sealed class VehicleDocument
    {
        [BsonId]
        public string Id { get; set; }

        [BsonElement("brand")]
        public string Brand { get; set; }

        [BsonElement("model")]
        public string Model { get; set; }

        [BsonElement("licensePlate")]
        public string LicensePlate { get; set; }

        [BsonElement("manufactureDate")]
        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime ManufactureDate { get; set; }

        [BsonElement("rentedBy")]
        [BsonIgnoreIfNull]
        public string RentedBy { get; set; }

        [BsonElement("rentedAt")]
        [BsonIgnoreIfNull]
        public DateTime? RentedAt { get; set; }
    }
}
