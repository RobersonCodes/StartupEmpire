namespace StartupEmpire.Upgrades
{
    /// Dado de design de um upgrade (seção 10 da missão): nível, custo, benefício,
    /// requisito e curva de progressão vivem aqui — nunca hardcoded no código de sistema.
    public sealed class UpgradeDefinition
    {
        public string Id { get; }
        public string DisplayName { get; }
        public UpgradeCategory Category { get; }
        public double BaseCost { get; }
        public double CostGrowthFactor { get; }
        public int MaxLevel { get; }
        public UpgradeEffectType EffectType { get; }
        public double EffectPerLevel { get; }

        public UpgradeDefinition(string id, string displayName, UpgradeCategory category,
            double baseCost, double costGrowthFactor, int maxLevel,
            UpgradeEffectType effectType, double effectPerLevel)
        {
            Id = id;
            DisplayName = displayName;
            Category = category;
            BaseCost = baseCost;
            CostGrowthFactor = costGrowthFactor;
            MaxLevel = maxLevel;
            EffectType = effectType;
            EffectPerLevel = effectPerLevel;
        }
    }
}
