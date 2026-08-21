using System.Collections.Generic;

namespace StartupEmpire.Store
{
    /// Catálogo do Capítulo 1. Preços e efeitos deliberadamente modestos e
    /// transparentes — nenhum item bloqueia progresso normal do jogo offline.
    public static class StoreCatalog
    {
        public static List<StoreItemDefinition> CreateChapter1Catalog()
        {
            return new List<StoreItemDefinition>
            {
                new StoreItemDefinition(
                    id: "dev_boost_small",
                    displayName: "Café Turbo (Boost de Desenvolvimento)",
                    category: StoreItemCategory.Boost,
                    gemCost: 20,
                    effectType: StoreItemEffectType.DevSpeedBoost,
                    effectMagnitude: 0.5,
                    durationCycles: 3),

                new StoreItemDefinition(
                    id: "marketing_boost_small",
                    displayName: "Campanha Relâmpago (Boost de Aquisição)",
                    category: StoreItemCategory.Acceleration,
                    gemCost: 20,
                    effectType: StoreItemEffectType.AcquisitionBoost,
                    effectMagnitude: 0.5,
                    durationCycles: 3),

                new StoreItemDefinition(
                    id: "cash_injection",
                    displayName: "Aporte Rápido",
                    category: StoreItemCategory.Special,
                    gemCost: 50,
                    effectType: StoreItemEffectType.InstantCashBonus,
                    effectMagnitude: 500,
                    durationCycles: 0),

                new StoreItemDefinition(
                    id: "cosmetic_dark_theme",
                    displayName: "Tema Escuro do Escritório",
                    category: StoreItemCategory.Cosmetic,
                    gemCost: 30,
                    effectType: StoreItemEffectType.CosmeticOnly,
                    effectMagnitude: 0,
                    durationCycles: 0),
            };
        }
    }
}
