using System;
using StartupEmpire.Core;

namespace StartupEmpire.Premium
{
    public sealed class GemWalletService
    {
        private readonly IClock _clock;
        private readonly EventBus _eventBus;

        public GemWalletService(IClock clock, EventBus eventBus)
        {
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _eventBus = eventBus;
        }

        public void Grant(GemWalletState wallet, int amount, GemLedgerCategory category, string description)
        {
            if (amount <= 0) return;
            wallet.Apply(new GemLedgerEntry(_clock.UtcNow, category, amount, description));
            _eventBus?.Publish(new GemsGrantedEvent(amount, description));
        }

        public bool TrySpend(GemWalletState wallet, int amount, string description)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
            if (!wallet.CanAfford(amount)) return false;

            wallet.Apply(new GemLedgerEntry(_clock.UtcNow, GemLedgerCategory.Spend, -amount, description));
            _eventBus?.Publish(new GemsSpentEvent(amount, description));
            return true;
        }
    }
}
