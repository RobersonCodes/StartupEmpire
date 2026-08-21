using System;

namespace StartupEmpire.Core
{
    public interface IClock
    {
        DateTime UtcNow { get; }
    }
}
