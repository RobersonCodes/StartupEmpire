using System;
using System.Collections.Generic;
using StartupEmpire.Core;
using StartupEmpire.Products;

namespace StartupEmpire.Economy
{
    /// Motor de regras da economia: puramente C#, sem UnityEngine, para ser
    /// testável tanto pelo Unity Test Framework quanto por `dotnet test`.
    public sealed class EconomyEngine
    {
        private readonly EconomyConfigValues _config;
        private readonly IClock _clock;
        private readonly EventBus _eventBus;

        public EconomyEngine(EconomyConfigValues config, IClock clock, EventBus eventBus)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _eventBus = eventBus;
        }

        public bool TrySpend(EconomyState state, double amount, LedgerCategory category, string description)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
            if (!state.CanAfford(amount)) return false;
            state.Apply(new LedgerEntry(_clock.UtcNow, category, -amount, description));
            return true;
        }

        public void Earn(EconomyState state, double amount, string sourceProductId)
        {
            if (amount <= 0) return;
            state.Apply(new LedgerEntry(_clock.UtcNow, LedgerCategory.Revenue, amount, $"revenue:{sourceProductId}"));
            _eventBus?.Publish(new RevenueEarnedEvent(amount, sourceProductId));
        }

        public void RecomputeRecurringRevenue(EconomyState state, IEnumerable<ProductState> products)
        {
            double mrr = 0;
            foreach (var product in products)
            {
                if (product.Stage == ProductStage.Launched || product.Stage == ProductStage.Maintenance)
                {
                    mrr += product.PayingCustomers * product.Price;
                }
            }
            state.MonthlyRecurringRevenue = mrr;
        }

        /// Aplica uma rodada de investimento: soma o caixa recebido e dilui a
        /// participação do fundador multiplicativamente (nunca abaixo de 0).
        public void ApplyInvestment(EconomyState state, double cashAmount, double equityPercentGiven)
        {
            state.Apply(new LedgerEntry(_clock.UtcNow, LedgerCategory.Investment, cashAmount, "investment_round"));
            state.FounderEquity = Math.Max(0, state.FounderEquity * (1 - equityPercentGiven));
        }

        public void RecomputeValuation(EconomyState state)
        {
            state.Valuation = Math.Max(0,
                state.MonthlyRecurringRevenue * _config.ValuationMrrMultiple * _config.ValuationSectorMultiplier);
        }

        public double CashFlow(EconomyState state, TimeSpan window)
        {
            double sum = 0;
            var since = _clock.UtcNow - window;
            foreach (var entry in state.Ledger)
            {
                if (entry.TimestampUtc >= since) sum += entry.Amount;
            }
            return sum;
        }
    }
}
