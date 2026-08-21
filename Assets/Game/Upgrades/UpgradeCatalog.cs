using System.Collections.Generic;

namespace StartupEmpire.Upgrades
{
    /// Upgrades do Capítulo 1 (seção 10 da missão): computador, internet, ferramentas
    /// de produtividade e cursos de conhecimento. Extensível — novas categorias (servidores,
    /// automação, marketing) entram como novas entradas aqui sem tocar em UpgradeService.
    public static class UpgradeCatalog
    {
        public static List<UpgradeDefinition> CreateChapter1Catalog()
        {
            return new List<UpgradeDefinition>
            {
                new UpgradeDefinition(
                    id: "better_computer",
                    displayName: "Computador Melhor",
                    category: UpgradeCategory.Computer,
                    baseCost: 100,
                    costGrowthFactor: 1.6,
                    maxLevel: 5,
                    effectType: UpgradeEffectType.DevSpeedMultiplier,
                    effectPerLevel: 0.10),

                new UpgradeDefinition(
                    id: "better_internet",
                    displayName: "Internet Melhor",
                    category: UpgradeCategory.Internet,
                    baseCost: 80,
                    costGrowthFactor: 1.5,
                    maxLevel: 5,
                    effectType: UpgradeEffectType.AcquisitionRateMultiplier,
                    effectPerLevel: 0.08),

                new UpgradeDefinition(
                    id: "productivity_tools",
                    displayName: "Ferramentas de Produtividade",
                    category: UpgradeCategory.Tools,
                    baseCost: 150,
                    costGrowthFactor: 1.7,
                    maxLevel: 5,
                    effectType: UpgradeEffectType.BugRateReduction,
                    effectPerLevel: -0.05),

                new UpgradeDefinition(
                    id: "online_courses",
                    displayName: "Cursos Online",
                    category: UpgradeCategory.Knowledge,
                    baseCost: 60,
                    costGrowthFactor: 1.4,
                    maxLevel: 5,
                    effectType: UpgradeEffectType.KnowledgeGainMultiplier,
                    effectPerLevel: 0.15),
            };
        }
    }
}
