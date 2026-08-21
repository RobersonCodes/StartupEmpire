using System;

namespace StartupEmpire.Idle
{
    public readonly struct OfflineSummary
    {
        public readonly TimeSpan ElapsedApplied;
        public readonly double CashEarned;
        public readonly int BugsAccumulated;

        public OfflineSummary(TimeSpan elapsedApplied, double cashEarned, int bugsAccumulated)
        {
            ElapsedApplied = elapsedApplied;
            CashEarned = cashEarned;
            BugsAccumulated = bugsAccumulated;
        }
    }
}
