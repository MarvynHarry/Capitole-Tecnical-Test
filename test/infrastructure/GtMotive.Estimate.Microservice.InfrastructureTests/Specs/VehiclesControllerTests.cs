using System;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.InfrastructureTests.Infrastructure;
using Xunit;

namespace GtMotive.Estimate.Microservice.InfrastructureTests.Specs
{
    /// <summary>
    /// Infrastructure tests: the REST API is exercised through HTTP on an in-memory TestServer
    /// (routing, model binding, validation, filters and JSON serialization included).
    /// </summary>
    public sealed class VehiclesControllerTests(GenericInfrastructureTestServerFixture fixture) : InfrastructureTestBase(fixture)
    {
        private const string BaseUrl = "/api/vehicles";

        [Fact]
        public async Task Post_WithValidVehicle_Returns201AndVehicleIsListedAsAvailable()
        {
            using var client = Fixture.Server.CreateClient();
            var licensePlate = NewLicensePlate();

            using var response = await client.PostAsJsonAsync(BaseUrl, new
            {
                brand = "Seat",
                model = "Ibiza",
                licensePlate,
                manufactureDate = DateTime.UtcNow.AddYears(-1).ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            });

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await response.Content.ReadFromJsonAsync<JsonElement>();
            created.GetProperty("status").GetString().Should().Be("Available");

            var available = await client.GetFromJsonAsync<JsonElement[]>($"{BaseUrl}/available");
            available.Select(v => v.GetProperty("licensePlate").GetString()).Should().Contain(licensePlate.ToUpperInvariant());
        }

        [Fact]
        public async Task Post_WithVehicleOlderThanFiveYears_Returns400()
        {
            using var client = Fixture.Server.CreateClient();

            using var response = await client.PostAsJsonAsync(BaseUrl, new
            {
                brand = "Ford",
                model = "Focus",
                licensePlate = NewLicensePlate(),
                manufactureDate = "2010-01-01",
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Post_WithoutRequiredFields_Returns400()
        {
            using var client = Fixture.Server.CreateClient();

            using var response = await client.PostAsJsonAsync(BaseUrl, new { brand = "Ford" });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Rent_WhenPersonAlreadyHasAVehicle_Returns409()
        {
            using var client = Fixture.Server.CreateClient();
            var firstId = await CreateVehicle(client);
            var secondId = await CreateVehicle(client);
            var renter = new { renterId = $"P{Guid.NewGuid():N}"[..10] };

            using var firstRent = await client.PostAsJsonAsync($"{BaseUrl}/{firstId}/rent", renter);
            using var secondRent = await client.PostAsJsonAsync($"{BaseUrl}/{secondId}/rent", renter);

            firstRent.StatusCode.Should().Be(HttpStatusCode.OK);
            secondRent.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task Return_WhenVehicleDoesNotExist_Returns404()
        {
            using var client = Fixture.Server.CreateClient();

            using var response = await client.PostAsync($"{BaseUrl}/{Guid.NewGuid()}/return", null);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        private static string NewLicensePlate() => Guid.NewGuid().ToString("N")[..10];

        private static async Task<string> CreateVehicle(System.Net.Http.HttpClient client)
        {
            using var response = await client.PostAsJsonAsync(BaseUrl, new
            {
                brand = "Kia",
                model = "Ceed",
                licensePlate = NewLicensePlate(),
                manufactureDate = DateTime.UtcNow.AddYears(-1),
            });

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await response.Content.ReadFromJsonAsync<JsonElement>();
            return created.GetProperty("id").GetString();
        }
    }
}
