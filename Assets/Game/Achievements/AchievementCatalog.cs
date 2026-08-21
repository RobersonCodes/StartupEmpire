using System.Collections.Generic;
using StartupEmpire.Core;
using StartupEmpire.Progression;

namespace StartupEmpire.Achievements
{
    /// Conquistas de exemplo da seção 19 da missão.
    public static class AchievementCatalog
    {
        public static List<AchievementDefinition> Create()
        {
            return new List<AchievementDefinition>
            {
                new AchievementDefinition("hello_world", "Hello World",
                    "Escreva o primeiro código do seu primeiro produto.", HasAnyProgress),

                new AchievementDefinition("first_customer", "First Customer",
                    "Conquiste seu primeiro cliente pagante.", HasAnyPayingCustomer),

                new AchievementDefinition("mrr", "MRR",
                    "Alcance sua primeira receita recorrente mensal.",
                    state => state.Economy.MonthlyRecurringRevenue > 0),

                new AchievementDefinition("founder", "Founder",
                    "Sua empresa deixou de ser um projeto de pessoa física.",
                    state => state.Stage != CompanyStage.PessoaFisica),

                new AchievementDefinition("unicorn", "Unicorn",
                    "Alcance um valuation de $1 bilhão.",
                    state => state.Economy.Valuation >= 1_000_000_000),
            };
        }

        private static bool HasAnyProgress(GameState state)
        {
            foreach (var product in state.Products)
            {
                if (product.DevProgress > 0) return true;
            }
            return false;
        }

        private static bool HasAnyPayingCustomer(GameState state)
        {
            foreach (var product in state.Products)
            {
                if (product.PayingCustomers > 0) return true;
            }
            return false;
        }
    }
}
