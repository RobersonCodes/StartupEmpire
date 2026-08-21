namespace StartupEmpire.Competitors
{
    /// Instância em runtime de um concorrente: valuation, usuários, reputação,
    /// qualidade e participação de mercado (seção 15 da missão).
    public sealed class CompetitorState
    {
        public string Id { get; }
        public CompetitorDefinition Definition { get; }
        public double Users { get; internal set; }
        public double Valuation { get; internal set; }
        public double Reputation { get; internal set; }
        public double Quality { get; internal set; }
        public double MarketShare { get; internal set; }

        public CompetitorState(CompetitorDefinition definition)
        {
            Definition = definition;
            Id = definition.Id;
            Users = definition.InitialUsers;
            Valuation = definition.InitialValuation;
            Reputation = definition.InitialReputation;
            Quality = definition.InitialQuality;
        }
    }
}
