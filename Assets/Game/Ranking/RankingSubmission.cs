using System;

namespace StartupEmpire.Ranking
{
    [Serializable]
    public sealed class RankingSubmission
    {
        public string PlayerId;
        public string DisplayName;
        public double NetWorth;
        public double Valuation;
        public double MonthlyRecurringRevenue;
        public int ProgressStageIndex;
        public int AchievementCount;
    }
}
