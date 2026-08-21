namespace StartupEmpire.Economy
{
    /// Todos os números de balanceamento da economia centralizados aqui —
    /// nunca espalhados como "números mágicos" pelo código de sistema.
    public sealed class EconomyConfigValues
    {
        public double StartingCash = 500.0;
        public double BaseSalaryPerEmployee = 50.0;
        public double BaseInfraCostPerProduct = 5.0;
        public double MarketingCostMultiplier = 1.0;
        public double ReputationSensitivity = 0.05;
        public double ValuationMrrMultiple = 12.0;
        public double ValuationSectorMultiplier = 3.0;
        public double MaxOfflineHours = 12.0;
        public double OfflineEarningsEfficiency = 0.5;
    }
}
