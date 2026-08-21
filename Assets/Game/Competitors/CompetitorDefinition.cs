namespace StartupEmpire.Competitors
{
    /// Dado de design de um concorrente simulado (seção 15 da missão). Sem IA pesada —
    /// só um conjunto de estatísticas iniciais que evoluem por uma taxa de crescimento configurável.
    public sealed class CompetitorDefinition
    {
        public string Id { get; }
        public string DisplayName { get; }
        public double InitialUsers { get; }
        public double InitialValuation { get; }
        public double InitialReputation { get; }
        public double InitialQuality { get; }

        public CompetitorDefinition(string id, string displayName, double initialUsers,
            double initialValuation, double initialReputation, double initialQuality)
        {
            Id = id;
            DisplayName = displayName;
            InitialUsers = initialUsers;
            InitialValuation = initialValuation;
            InitialReputation = initialReputation;
            InitialQuality = initialQuality;
        }
    }
}
