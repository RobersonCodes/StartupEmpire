namespace StartupEmpire.Investment
{
    /// Progressão de captação (seção 17 da missão). Ipo é o estágio final e não é
    /// vendido como uma InvestmentOffer comum de caixa-por-equity — é tratado à parte
    /// quando o sistema de Company Progression alcançar CompanyStage.Ipo.
    public enum InvestmentRoundType
    {
        Bootstrapping,
        Angel,
        Seed,
        SeriesA,
        SeriesB,
        SeriesC,
        Ipo
    }
}
