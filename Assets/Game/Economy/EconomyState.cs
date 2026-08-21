using System.Collections.Generic;

namespace StartupEmpire.Economy
{
    public sealed class EconomyState
    {
        public double Cash { get; private set; }
        public double MonthlyRecurringRevenue { get; internal set; }
        public double Valuation { get; internal set; }
        public double FounderEquity { get; internal set; } = 1.0;
        public List<LedgerEntry> Ledger { get; } = new();

        public EconomyState(double startingCash)
        {
            Cash = startingCash;
        }

        public void Apply(LedgerEntry entry)
        {
            Cash += entry.Amount;
            Ledger.Add(entry);
        }

        public bool CanAfford(double amount) => Cash >= amount;
    }
}
