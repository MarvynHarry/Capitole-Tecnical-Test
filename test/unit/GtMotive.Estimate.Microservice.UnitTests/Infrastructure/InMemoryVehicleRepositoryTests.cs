using System;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Exceptions;
using GtMotive.Estimate.Microservice.Infrastructure.Repositories.InMemory;
using GtMotive.Estimate.Microservice.UnitTests.TestDoubles;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.Infrastructure
{
    public class InMemoryVehicleRepositoryTests
    {
        private readonly InMemoryVehicleRepository _sut = new();

        [Fact]
        public async Task Changes_AreNotVisibleUntilUpdateIsCalled()
        {
            var vehicle = VehicleMother.Available();
            await _sut.Add(vehicle);

            vehicle.Rent(new RenterId("12345678Z"), VehicleMother.Today);

            (await _sut.GetById(vehicle.Id)).IsAvailable.Should().BeTrue();

            await _sut.Update(vehicle);

            (await _sut.GetById(vehicle.Id)).IsAvailable.Should().BeFalse();
            (await _sut.HasActiveRental(new RenterId("12345678Z"))).Should().BeTrue();
            (await _sut.GetAvailable()).Should().BeEmpty();
        }

        [Fact]
        public async Task Add_WithDuplicatedLicensePlate_ThrowsLicensePlateAlreadyExistsException()
        {
            await _sut.Add(VehicleMother.Available("1234ABC"));

            var act = () => _sut.Add(VehicleMother.Available("1234-abc"));

            await act.Should().ThrowAsync<LicensePlateAlreadyExistsException>();
        }

        [Fact]
        public async Task Update_WhenRenterAlreadyHasAnotherVehicle_ThrowsRenterAlreadyHasVehicleException()
        {
            var first = VehicleMother.Available("1111AAA");
            var second = VehicleMother.Available("2222BBB");
            await _sut.Add(first);
            await _sut.Add(second);

            first.Rent(new RenterId("12345678Z"), VehicleMother.Today);
            second.Rent(new RenterId("12345678Z"), VehicleMother.Today);
            await _sut.Update(first);

            var act = () => _sut.Update(second);

            await act.Should().ThrowAsync<RenterAlreadyHasVehicleException>();
        }

        [Fact]
        public async Task GetById_WhenItDoesNotExist_ReturnsNull()
        {
            (await _sut.GetById(new VehicleId(Guid.NewGuid()))).Should().BeNull();
        }
    }
}
