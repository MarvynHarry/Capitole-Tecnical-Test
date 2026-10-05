using System.ComponentModel.DataAnnotations;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.RentVehicle
{
    /// <summary>
    /// Body of the rent request.
    /// </summary>
    public sealed class RentVehicleBody
    {
        /// <summary>
        /// Gets or sets the identity document of the person that rents the vehicle.
        /// </summary>
        [Required]
        public string RenterId { get; set; }
    }
}
