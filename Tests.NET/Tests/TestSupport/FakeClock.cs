using System;
using StartupEmpire.Core;

namespace StartupEmpire.Domain.Tests.TestSupport
{
    public sealed class FakeClock : IClock
    {
        public DateTime UtcNow { get; set; }

        public FakeClock(DateTime initial) => UtcNow = initial;

        public void Advance(TimeSpan by) => UtcNow += by;
    }
}
