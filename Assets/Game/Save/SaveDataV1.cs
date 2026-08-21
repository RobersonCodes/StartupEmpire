using System;
using System.Collections.Generic;

namespace StartupEmpire.Save
{
    [Serializable]
    public sealed class SaveDataV1
    {
        public int SchemaVersion = 1;
        public string PlayerId;
        public string PlayerName = "Founder";
        public int WorkCyclesPerDay = 4;
        public int CurrentDay = 1;
        public int RemainingWorkCycles = 4;
        public string TutorialStep = "LearnFundamentals";
        public List<KnowledgeEntry> Knowledge = new();
        public double Cash;
        public double MonthlyRecurringRevenue;
        public double Valuation;
        public double FounderEquity = 1.0;
        public List<ProductSaveEntry> Products = new();
        public List<string> CompletedMissionIds = new();
        public List<string> UnlockedAchievementIds = new();
        public List<UpgradeLevelEntry> UpgradeLevels = new();
        public List<EmployeeSaveEntry> Employees = new();
        public List<CompetitorSaveEntry> Competitors = new();
        public List<string> RaisedInvestmentRounds = new();
        public int GemBalance;
        public List<ActiveBoostSaveEntry> ActiveBoosts = new();
        public List<string> PurchasedCosmeticIds = new();
        public string CompanyStage = "PessoaFisica";
        public string LastSavedUtcIso;
    }

    [Serializable]
    public sealed class ActiveBoostSaveEntry
    {
        public string SourceItemId;
        public string EffectType;
        public double Magnitude;
        public int RemainingCycles;
    }

    [Serializable]
    public sealed class CompetitorSaveEntry
    {
        public string DefinitionId;
        public double Users;
        public double Valuation;
        public double Reputation;
        public double Quality;
        public double MarketShare;
    }

    [Serializable]
    public sealed class UpgradeLevelEntry
    {
        public string UpgradeId;
        public int Level;
    }

    [Serializable]
    public sealed class EmployeeSaveEntry
    {
        public string InstanceId;
        public string DefinitionId;
        public double Experience;
        public double Productivity;
        public double Quality;
        public double Speed;
        public double Specialization;
        public double Satisfaction;
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
        public int KnownBugCount;
        public bool HasBeenTested;
        public double Performance;
        public double Security;
        public double Popularity;
        public int Users;
        public int PayingCustomers;
        public double Price;
        public double Reputation;
    }
}
