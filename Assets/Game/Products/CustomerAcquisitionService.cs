using System;
using StartupEmpire.Core;
using StartupEmpire.Economy;

namespace StartupEmpire.Products
{
    /// Aquisição, conversão e churn de clientes (seção 13 da missão).
    public sealed class CustomerAcquisitionService
    {
        private readonly CustomerAcquisitionConfigValues _config;
        private readonly EventBus _eventBus;

        public CustomerAcquisitionService(CustomerAcquisitionConfigValues config, EventBus eventBus)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _eventBus = eventBus;
        }

        /// acquisitionMultiplier vem de Upgrades/Employees (ex.: internet melhor, funcionário de marketing).
        public void RunCycle(ProductState product, EconomyEngine economy, EconomyState economyState, int cycles,
            double acquisitionMultiplier = 1.0)
        {
            if (cycles <= 0) return;
            if (product.Stage != ProductStage.Launched && product.Stage != ProductStage.Maintenance) return;

            var newUsers = (int)Math.Round(
                _config.BaseAcquisitionRate * product.Reputation * product.Quality * acquisitionMultiplier * cycles);
            product.Users += newUsers;

            var newPaying = (int)Math.Round(newUsers * _config.ConversionRateToPaying);
            if (newPaying > 0)
            {
                product.PayingCustomers += newPaying;
                _eventBus?.Publish(new CustomerAcquiredEvent(product.Id, newPaying));
            }

            var churnRate = _config.ChurnRateBase + _config.StabilityChurnPenalty * (1 - product.Stability);
            var churned = (int)Math.Round(product.PayingCustomers * churnRate * cycles);
            product.PayingCustomers = Math.Max(0, product.PayingCustomers - churned);

            var revenue = product.PayingCustomers * product.Price * cycles;
            if (revenue > 0)
            {
                economy.Earn(economyState, revenue, product.Id);
            }

            product.Popularity = Math.Clamp(product.Users / 1000.0, 0, 1);
        }
    }
}
