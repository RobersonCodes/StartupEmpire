using System.Collections.Generic;
using StartupEmpire.Core;
using StartupEmpire.Products;

namespace StartupEmpire.Missions
{
    /// Missões do Capítulo 1 (seção 6 e 18 da missão).
    public static class Chapter1Missions
    {
        public static List<MissionDefinition> Create()
        {
            return new List<MissionDefinition>
            {
                new MissionDefinition(
                    "hello_world",
                    "Hello World",
                    "Desenvolva sua primeira funcionalidade.",
                    AnyProductHasProgress),

                new MissionDefinition(
                    "first_launch",
                    "Primeiro Lançamento",
                    "Lance seu primeiro produto.",
                    AnyProductLaunched,
                    rewardCash: 50),

                new MissionDefinition(
                    "first_customer",
                    "First Customer",
                    "Conquiste seu primeiro cliente pagante.",
                    AnyPayingCustomer,
                    rewardCash: 100),

                new MissionDefinition(
                    "first_mrr",
                    "MRR",
                    "Alcance R$1.000 de receita recorrente mensal.",
                    state => state.Economy.MonthlyRecurringRevenue >= 1000,
                    rewardGems: 10),
            };
        }

        private static bool AnyProductHasProgress(GameState state)
        {
            foreach (var product in state.Products)
            {
                if (product.DevProgress > 0) return true;
            }
            return false;
        }

        private static bool AnyProductLaunched(GameState state)
        {
            foreach (var product in state.Products)
            {
                if (product.Stage == ProductStage.Launched || product.Stage == ProductStage.Maintenance) return true;
            }
            return false;
        }

        private static bool AnyPayingCustomer(GameState state)
        {
            foreach (var product in state.Products)
            {
                if (product.PayingCustomers > 0) return true;
            }
            return false;
        }
    }
}
