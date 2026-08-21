namespace StartupEmpire.Statistics
{
    /// Retrato legível do GameState num instante — base para a futura tela de
    /// Estatísticas e reaproveitável por outros sistemas (ex.: submissão de ranking).
    public sealed class StatisticsSnapshot
    {
        public double Cash;
        public double Valuation;
        public double MonthlyRecurringRevenue;
        public double FounderEquity;
        public int TotalUsers;
        public int TotalPayingCustomers;
        public int ProductCount;
        public int LaunchedProductCount;
        public int EmployeeCount;
        public int UpgradesPurchasedLevels;
        public int UnlockedAchievementCount;
        public int CompletedMissionCount;
        public int GemBalance;
        public int RaisedInvestmentRoundCount;
        public double PlayerMarketShare;
        public string CompanyStage;
    }
}
