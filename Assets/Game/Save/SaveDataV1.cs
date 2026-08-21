using System;
using System.Collections.Generic;

namespace StartupEmpire.Save
{
    [Serializable]
    public sealed class SaveDataV1
    {
        public int SchemaVersion = 1;
        public string PlayerName = "Founder";
        public List<KnowledgeEntry> Knowledge = new();
        public double Cash;
        public double MonthlyRecurringRevenue;
        public double Valuation;
        public double FounderEquity = 1.0;
        public List<ProductSaveEntry> Products = new();
        public List<string> CompletedMissionIds = new();
        public List<string> UnlockedAchievementIds = new();
        public string CompanyStage = "PessoaFisica";
        public string LastSavedUtcIso;
    }

    [Serializable]
    public sealed class KnowledgeEntry
    {
        public string Track;
        public int Amount;
    }

    [Serializable]
    public sealed class ProductSaveEntry
    {
        public string DefinitionId;
        public string Stage;
        public double DevProgress;
        public double Quality;
        public double Stability;
        public int BugCount;
        public double Performance;
        public double Security;
        public double Popularity;
        public int Users;
        public int PayingCustomers;
        public double Price;
        public double Reputation;
    }
}
