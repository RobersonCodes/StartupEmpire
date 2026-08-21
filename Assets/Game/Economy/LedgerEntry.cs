using System;

namespace StartupEmpire.Economy
{
    public enum LedgerCategory
    {
        Revenue,
        Salary,
        Infrastructure,
        Marketing,
        Equipment,
        Investment,
        Other
    }

    public readonly struct LedgerEntry
    {
        public readonly DateTime TimestampUtc;
        public readonly LedgerCategory Category;
        public readonly double Amount;
        public readonly string Description;

        public LedgerEntry(DateTime timestampUtc, LedgerCategory category, double amount, string description)
        {
            TimestampUtc = timestampUtc;
            Category = category;
            Amount = amount;
            Description = description;
        }
    }
}
