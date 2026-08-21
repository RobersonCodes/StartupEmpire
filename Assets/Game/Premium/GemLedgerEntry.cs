using System;

namespace StartupEmpire.Premium
{
    public enum GemLedgerCategory
    {
        Reward,
        Purchase,
        Spend,
        Refund
    }

    public readonly struct GemLedgerEntry
    {
        public readonly DateTime TimestampUtc;
        public readonly GemLedgerCategory Category;
        public readonly int Amount;
        public readonly string Description;

        public GemLedgerEntry(DateTime timestampUtc, GemLedgerCategory category, int amount, string description)
        {
            TimestampUtc = timestampUtc;
            Category = category;
            Amount = amount;
            Description = description;
        }
    }
}
