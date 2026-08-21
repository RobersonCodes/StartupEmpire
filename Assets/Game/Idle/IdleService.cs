using System;
using StartupEmpire.Core;

namespace StartupEmpire.Idle
{
    public sealed class IdleService
    {
        private readonly OfflineProgressCalculator _calculator;
        private readonly IClock _clock;
        private readonly EventBus _eventBus;

        public IdleService(OfflineProgressCalculator calculator, IClock clock, EventBus eventBus)
        {
            _calculator = calculator ?? throw new ArgumentNullException(nameof(calculator));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _eventBus = eventBus;
        }

        public OfflineSummary ApplyOfflineProgress(GameState state)
        {
            var elapsed = _clock.UtcNow - state.LastSavedUtc;
            if (elapsed <= TimeSpan.Zero)
            {
                state.LastSavedUtc = _clock.UtcNow;
                return new OfflineSummary(TimeSpan.Zero, 0, 0);
            }

            var summary = _calculator.Calculate(state, elapsed);
            state.LastSavedUtc = _clock.UtcNow;
            _eventBus?.Publish(new OfflineProgressAppliedEvent(summary.ElapsedApplied, summary.CashEarned));
            return summary;
        }
    }
}
