using StartupEmpire.Progression;

namespace StartupEmpire.Investment
{
    /// Oferta de investimento: caixa em troca de uma fatia da empresa (seção 17 da
    /// missão). O trade-off é real — dinheiro recebido dilui a participação do fundador.
    public sealed class InvestmentOffer
    {
        public InvestmentRoundType RoundType { get; }
        public string DisplayName { get; }
        public double CashAmount { get; }
        public double EquityPercentRequested { get; }
        public double MinValuationRequired { get; }
        public CompanyStage MinStageRequired { get; }

        public InvestmentOffer(InvestmentRoundType roundType, string displayName, double cashAmount,
            double equityPercentRequested, double minValuationRequired, CompanyStage minStageRequired)
        {
            RoundType = roundType;
            DisplayName = displayName;
            CashAmount = cashAmount;
            EquityPercentRequested = equityPercentRequested;
            MinValuationRequired = minValuationRequired;
            MinStageRequired = minStageRequired;
        }
    }
}
