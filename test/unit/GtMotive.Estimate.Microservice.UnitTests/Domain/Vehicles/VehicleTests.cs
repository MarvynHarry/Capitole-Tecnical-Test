using System;
using FluentAssertions;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using GtMotive.Estimate.Microservice.Domain.Vehicles.Exceptions;
using GtMotive.Estimate.Microservice.UnitTests.TestDoubles;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.Domain.Vehicles
{
    public class VehicleTests
    {
        private static readonly DateTime Today = VehicleMother.Today;

        [Fact]
        public void Create_WithValidData_ReturnsAvailableVehicle()
        {
            var vehicle = Vehicle.Create(" Seat ", "Ibiza", new LicensePlate("1234-abc"), Today.AddYears(-2), Today);

            vehicle.Id.Value.Should().NotBeEmpty();
            vehicle.Brand.Should().Be("Seat");
            vehicle.LicensePlate.Value.Should().Be("1234ABC");
            vehicle.Status.Should().Be(VehicleStatus.Available);
            vehicle.IsAvailable.Should().BeTrue();
            vehicle.CurrentRental.Should().BeNull();
        }

        [Fact]
        public void Create_WithManufactureDateOlderThanFiveYears_ThrowsVehicleTooOldException()
        {
            var manufactureDate = Today.AddYears(-Vehicle.MaxAgeInYears).AddDays(-1);

            var act = () => Vehicle.Create("Seat", "Ibiza", new LicensePlate("1234ABC"), manufactureDate, Today);

            act.Should().Throw<VehicleTooOldException>();
        }

        [Fact]
        public void Create_WithManufactureDateExactlyFiveYearsAgo_IsAllowed()
        {
            var manufactureDate = Today.AddYears(-Vehicle.MaxAgeInYears);

            var vehicle = Vehicle.Create("Seat", "Ibiza", new LicensePlate("1234ABC"), manufactureDate, Today);

            vehicle.ManufactureDate.Should().Be(manufactureDate.Date);
        }

        [Fact]
        public void Create_WithFutureManufactureDate_ThrowsDomainException()
        {
            var act = () => Vehicle.Create("Seat", "Ibiza", new LicensePlate("1234ABC"), Today.AddDays(1), Today);

            act.Should().Throw<DomainException>();
        }

        [Theory]
        [InlineData("", "Ibiza")]
        [InlineData("Seat", " ")]
        public void Create_WithoutBrandOrModel_ThrowsDomainException(string brand, string model)
        {
            var act = () => Vehicle.Create(brand, model, new LicensePlate("1234ABC"), Today.AddYears(-1), Today);

            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void Rent_WhenAvailable_StartsRental()
        {
            var vehicle = VehicleMother.Available();
            var renter = new RenterId("12345678Z");

            vehicle.Rent(renter, Today);

            vehicle.Status.Should().Be(VehicleStatus.Rented);
            vehicle.CurrentRental.Should().Be(new Rental(renter, Today));
        }

        [Fact]
        public void Rent_WhenAlreadyRented_ThrowsVehicleNotAvailableException()
        {
            var vehicle = VehicleMother.RentedBy("11111111A");

            var act = () => vehicle.Rent(new RenterId("22222222B"), Today);

            act.Should().Throw<VehicleNotAvailableException>();
            vehicle.CurrentRental.Renter.Value.Should().Be("11111111A");
        }

        [Fact]
        public void Rent_WhenVehicleHasExceededMaxAge_ThrowsVehicleTooOldException()
        {
            var vehicle = VehicleMother.Available(manufactureDate: Today.AddYears(-6));

            var act = () => vehicle.Rent(new RenterId("12345678Z"), Today);

            act.Should().Throw<VehicleTooOldException>();
            vehicle.IsAvailable.Should().BeTrue();
        }

        [Fact]
        public void Return_WhenRented_FinishesRentalAndMakesVehicleAvailable()
        {
            var vehicle = VehicleMother.RentedBy("12345678Z");

            var finishedRental = vehicle.Return();

            finishedRental.Renter.Value.Should().Be("12345678Z");
            vehicle.IsAvailable.Should().BeTrue();
            vehicle.CurrentRental.Should().BeNull();
        }

        [Fact]
        public void Return_WhenNotRented_ThrowsVehicleNotRentedException()
        {
            var vehicle = VehicleMother.Available();

            var act = () => vehicle.Return();

            act.Should().Throw<VehicleNotRentedException>();
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(4, true)]
        [InlineData(5, true)]
        [InlineData(6, false)]
        public void IsEligibleForFleet_DependsOnAge(int ageInYears, bool expected)
        {
            var vehicle = VehicleMother.Available(manufactureDate: Today.AddYears(-ageInYears));

            vehicle.IsEligibleForFleet(Today).Should().Be(expected);
        }
    }
}
