using System;

namespace GtMotive.Estimate.Microservice.UnitTests.TestDoubles
{
    /// <summary>
    /// Clock that always returns the same moment, so tests do not depend on the current date.
    /// </summary>
    public sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }
}
