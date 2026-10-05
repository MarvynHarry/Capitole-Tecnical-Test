using System;
using FluentAssertions;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.Domain.Vehicles
{
    public class ValueObjectsTests
    {
        [Theory]
        [InlineData("1234-abc")]
        [InlineData("1234 ABC")]
        [InlineData(" 1234abc ")]
        public void LicensePlate_IsNormalized(string raw)
        {
            new LicensePlate(raw).Should().Be(new LicensePlate("1234ABC"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("1234567890ABCDEF")]
        public void LicensePlate_WhenInvalid_ThrowsDomainException(string raw)
        {
            var act = () => new LicensePlate(raw);

            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void RenterId_IsNormalized()
        {
            new RenterId(" 12345678z ").Should().Be(new RenterId("12345678Z"));
        }

        [Fact]
        public void RenterId_WhenEmpty_ThrowsDomainException()
        {
            var act = () => new RenterId(" ");

            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void VehicleId_WhenEmpty_ThrowsDomainException()
        {
            var act = () => new VehicleId(Guid.Empty);

            act.Should().Throw<DomainException>();
        }
    }
}
