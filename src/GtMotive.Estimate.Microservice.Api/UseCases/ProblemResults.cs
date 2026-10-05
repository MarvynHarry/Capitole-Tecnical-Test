using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases
{
    /// <summary>
    /// Builds RFC 7807 responses for the alternative flows of the use cases.
    /// </summary>
    public static class ProblemResults
    {
        public static IActionResult NotFound(string message) => new NotFoundObjectResult(new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Status = StatusCodes.Status404NotFound,
            Title = "Not Found",
            Detail = message,
        });

        public static IActionResult Conflict(string message) => new ConflictObjectResult(new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            Status = StatusCodes.Status409Conflict,
            Title = "Conflict",
            Detail = message,
        });
    }
}
