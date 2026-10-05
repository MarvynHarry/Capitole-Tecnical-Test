using System;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.UnitTests.TestDoubles
{
    /// <summary>
    /// Object mother to build vehicles in a known state.
    /// </summary>
    public static class VehicleMother
    {
        public static readonly DateTime Today = new(2025, 6, 15, 10, 0, 0, DateTimeKind.Utc);

        public static Vehicle Available(string licensePlate = "1234ABC", DateTime? manufactureDate = null)
        {
            return Vehicle.Restore(
                VehicleId.New(),
                "Seat",
                "Ibiza",
                new LicensePlate(licensePlate),
                manufactureDate ?? Today.AddYears(-1),
                null);
        }

        public static Vehicle RentedBy(string renterId)
        {
            return Vehicle.Restore(
                VehicleId.New(),
                "Seat",
                "Leon",
                new LicensePlate("5678DEF"),
                Today.AddYears(-1),
                new Rental(new RenterId(renterId), Today.AddDays(-2)));
        }
    }
}
