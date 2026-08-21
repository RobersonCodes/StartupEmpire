using System;
using StartupEmpire.Core;
using StartupEmpire.Economy;
using StartupEmpire.Products;

namespace StartupEmpire.Idle
{
    /// Calcula progresso offline em lote (não tick-a-tick) — seção 9 da missão.
    /// Usa DateTime.UtcNow via IClock (não horário local) e aplica um teto configurável
    /// de horas para mitigar manipulação trivial do relógio, sem mecanismos invasivos.
    public sealed class OfflineProgressCalculator
    {
        private readonly EconomyConfigValues _economyConfig;

        public OfflineProgressCalculator(EconomyConfigValues economyConfig)
        {
            _economyConfig = economyConfig ?? throw new ArgumentNullException(nameof(economyConfig));
        }

        public OfflineSummary Calculate(GameState state, TimeSpan rawElapsed)
        {
            var cappedHours = Math.Min(rawElapsed.TotalHours, _economyConfig.MaxOfflineHours);
            var elapsed = TimeSpan.FromHours(Math.Max(0, cappedHours));
            if (elapsed <= TimeSpan.Zero) return new OfflineSummary(TimeSpan.Zero, 0, 0);

            double totalCash = 0;
            var bugsAccrued = 0;

            foreach (var product in state.Products)
            {
                if (product.Stage != ProductStage.Launched && product.Stage != ProductStage.Maintenance) continue;

                var hourlyRevenue = product.PayingCustomers * product.Price * _economyConfig.OfflineEarningsEfficiency;
                totalCash += hourlyRevenue * elapsed.TotalHours;

                var newBugs = (int)Math.Round((1 - product.Stability) * elapsed.TotalHours * 0.1);
                if (newBugs > 0)
                {
                    product.AddOfflineBugs(newBugs);
                    bugsAccrued += newBugs;
                }
            }

            if (totalCash > 0)
            {
                state.Economy.Apply(new LedgerEntry(DateTime.UtcNow, LedgerCategory.Revenue, totalCash, "offline_progress"));
            }

            return new OfflineSummary(elapsed, totalCash, bugsAccrued);
        }
    }
}
