using System.Collections.Generic;

namespace StartupEmpire.Competitors
{
    public sealed class CompetitorDefinitionCatalog
    {
        private readonly Dictionary<string, CompetitorDefinition> _byId = new();

        public void Register(CompetitorDefinition definition) => _byId[definition.Id] = definition;

        public CompetitorDefinition Find(string id) => _byId.TryGetValue(id, out var def) ? def : null;

        public IReadOnlyCollection<CompetitorDefinition> All => _byId.Values;

        public static CompetitorDefinitionCatalog CreateChapter1Catalog()
        {
            var catalog = new CompetitorDefinitionCatalog();
            catalog.Register(new CompetitorDefinition(
                id: "rival_startup",
                displayName: "RivalTech",
                initialUsers: 200,
                initialValuation: 5000,
                initialReputation: 0.6,
                initialQuality: 0.6));
            catalog.Register(new CompetitorDefinition(
                id: "market_giant",
                displayName: "MegaCorp Software",
                initialUsers: 5000,
                initialValuation: 200000,
                initialReputation: 0.8,
                initialQuality: 0.75));
            return catalog;
        }
    }
}
