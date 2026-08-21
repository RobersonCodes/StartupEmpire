using System;
using StartupEmpire.Core;

namespace StartupEmpire.Progression
{
    public sealed class ProgressionGate
    {
        public CompanyStage FromStage;
        public CompanyStage ToStage;
        public Func<GameState, bool> Condition;
    }

    /// Gates de progressão de empresa (seção 16 da missão). Extensível por dados —
    /// novos estágios só precisam de uma nova entrada em DefaultGates.
    public sealed class ProgressionService
    {
        private readonly ProgressionGate[] _gates;
        private readonly EventBus _eventBus;

        public ProgressionService(EventBus eventBus, ProgressionGate[] gates = null)
        {
            _eventBus = eventBus;
            _gates = gates ?? DefaultGates();
        }

        public static ProgressionGate[] DefaultGates() => new[]
        {
            new ProgressionGate
            {
                FromStage = CompanyStage.PessoaFisica,
                ToStage = CompanyStage.Freelancer,
                Condition = AnyProductHasPayingCustomer
            },
            new ProgressionGate
            {
                FromStage = CompanyStage.Freelancer,
                ToStage = CompanyStage.Microempresa,
                Condition = state => state.Economy.MonthlyRecurringRevenue >= 1000
            },
            new ProgressionGate
            {
                FromStage = CompanyStage.Microempresa,
                ToStage = CompanyStage.Startup,
                Condition = state => state.Economy.MonthlyRecurringRevenue >= 10000
            }
        };

        private static bool AnyProductHasPayingCustomer(GameState state)
        {
            foreach (var product in state.Products)
            {
                if (product.PayingCustomers > 0) return true;
            }
            return false;
        }

        public bool TryAdvance(GameState state)
        {
            foreach (var gate in _gates)
            {
                if (gate.FromStage != state.Stage) continue;
                if (!gate.Condition(state)) continue;

                var previous = state.Stage;
                state.Stage = gate.ToStage;
                _eventBus?.Publish(new CompanyStageChangedEvent(previous, gate.ToStage));
                return true;
            }
            return false;
        }
    }
}
