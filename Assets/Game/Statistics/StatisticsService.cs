using System.Linq;
using StartupEmpire.Competitors;
using StartupEmpire.Core;
using StartupEmpire.Products;

namespace StartupEmpire.Statistics
{
    /// Agrega o GameState num StatisticsSnapshot — sem estado próprio, sem
    /// persistência (é sempre recalculado a partir da fonte de verdade).
    public sealed class StatisticsService
    {
        private readonly CompetitorService _competitors;

        public StatisticsService(CompetitorService competitors)
        {
            _competitors = competitors;
        }

        public StatisticsSnapshot BuildSnapshot(GameState state)
        {
            var totalUsers = 0;
            var totalPayingCustomers = 0;
            var launchedProductCount = 0;
            foreach (var product in state.Products)
            {
                totalUsers += product.Users;
                totalPayingCustomers += product.PayingCustomers;
                if (product.Stage == ProductStage.Launched || product.Stage == ProductStage.Maintenance)
                {
                    launchedProductCount++;
                }
            }

            var upgradeLevels = state.Upgrades.LevelByUpgradeId.Values.Sum();
            var playerMarketShare = _competitors?.GetPlayerMarketShare(state.Competitors, totalUsers) ?? 0;

            return new StatisticsSnapshot
            {
                Cash = state.Economy.Cash,
                Valuation = state.Economy.Valuation,
                MonthlyRecurringRevenue = state.Economy.MonthlyRecurringRevenue,
                FounderEquity = state.Economy.FounderEquity,
                TotalUsers = totalUsers,
                TotalPayingCustomers = totalPayingCustomers,
                ProductCount = state.Products.Count,
                LaunchedProductCount = launchedProductCount,
                EmployeeCount = state.Employees.Employees.Count,
                UpgradesPurchasedLevels = upgradeLevels,
                UnlockedAchievementCount = state.UnlockedAchievements.Count,
                CompletedMissionCount = state.Missions.CompletedMissionIds.Count,
                GemBalance = state.GemWallet.Balance,
                RaisedInvestmentRoundCount = state.RaisedInvestmentRounds.Count,
                PlayerMarketShare = playerMarketShare,
                CompanyStage = state.Stage.ToString()
            };
        }
    }
}
