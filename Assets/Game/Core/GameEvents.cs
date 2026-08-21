using System;
using StartupEmpire.Investment;
using StartupEmpire.Progression;

namespace StartupEmpire.Core
{
    public readonly struct CustomerAcquiredEvent
    {
        public readonly string ProductId;
        public readonly int NewPayingCustomers;

        public CustomerAcquiredEvent(string productId, int newPayingCustomers)
        {
            ProductId = productId;
            NewPayingCustomers = newPayingCustomers;
        }
    }

    public readonly struct RevenueEarnedEvent
    {
        public readonly double Amount;
        public readonly string SourceProductId;

        public RevenueEarnedEvent(double amount, string sourceProductId)
        {
            Amount = amount;
            SourceProductId = sourceProductId;
        }
    }

    public readonly struct ProductLaunchedEvent
    {
        public readonly string ProductId;
        public ProductLaunchedEvent(string productId) => ProductId = productId;
    }

    public readonly struct BugFoundEvent
    {
        public readonly string ProductId;
        public readonly int Count;

        public BugFoundEvent(string productId, int count)
        {
            ProductId = productId;
            Count = count;
        }
    }

    public readonly struct BugFixedEvent
    {
        public readonly string ProductId;
        public readonly int Count;

        public BugFixedEvent(string productId, int count)
        {
            ProductId = productId;
            Count = count;
        }
    }

    public readonly struct CompanyStageChangedEvent
    {
        public readonly CompanyStage PreviousStage;
        public readonly CompanyStage NewStage;

        public CompanyStageChangedEvent(CompanyStage previousStage, CompanyStage newStage)
        {
            PreviousStage = previousStage;
            NewStage = newStage;
        }
    }

    public readonly struct MissionCompletedEvent
    {
        public readonly string MissionId;
        public MissionCompletedEvent(string missionId) => MissionId = missionId;
    }

    public readonly struct AchievementUnlockedEvent
    {
        public readonly string AchievementId;
        public AchievementUnlockedEvent(string achievementId) => AchievementId = achievementId;
    }

    public readonly struct OfflineProgressAppliedEvent
    {
        public readonly TimeSpan ElapsedApplied;
        public readonly double CashEarned;

        public OfflineProgressAppliedEvent(TimeSpan elapsedApplied, double cashEarned)
        {
            ElapsedApplied = elapsedApplied;
            CashEarned = cashEarned;
        }
    }

    public readonly struct UpgradePurchasedEvent
    {
        public readonly string UpgradeId;
        public readonly int NewLevel;

        public UpgradePurchasedEvent(string upgradeId, int newLevel)
        {
            UpgradeId = upgradeId;
            NewLevel = newLevel;
        }
    }

    public readonly struct EmployeeHiredEvent
    {
        public readonly string EmployeeInstanceId;
        public readonly string DefinitionId;

        public EmployeeHiredEvent(string employeeInstanceId, string definitionId)
        {
            EmployeeInstanceId = employeeInstanceId;
            DefinitionId = definitionId;
        }
    }

    public readonly struct EmployeeFiredEvent
    {
        public readonly string EmployeeInstanceId;
        public EmployeeFiredEvent(string employeeInstanceId) => EmployeeInstanceId = employeeInstanceId;
    }

    public readonly struct GameEventTriggeredEvent
    {
        public readonly string EventId;
        public GameEventTriggeredEvent(string eventId) => EventId = eventId;
    }

    public readonly struct GameEventResolvedEvent
    {
        public readonly string EventId;
        public readonly string ChoiceId;

        public GameEventResolvedEvent(string eventId, string choiceId)
        {
            EventId = eventId;
            ChoiceId = choiceId;
        }
    }

    public readonly struct InvestmentAcceptedEvent
    {
        public readonly InvestmentRoundType RoundType;
        public readonly double CashAmount;
        public readonly double EquityPercentGiven;

        public InvestmentAcceptedEvent(InvestmentRoundType roundType, double cashAmount, double equityPercentGiven)
        {
            RoundType = roundType;
            CashAmount = cashAmount;
            EquityPercentGiven = equityPercentGiven;
        }
    }
}
