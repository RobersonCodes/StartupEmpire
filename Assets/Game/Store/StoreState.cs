using System.Collections.Generic;

namespace StartupEmpire.Store
{
    public sealed class StoreState
    {
        public List<ActiveBoost> ActiveBoosts { get; } = new();
        public HashSet<string> PurchasedCosmeticIds { get; } = new();
    }
}
