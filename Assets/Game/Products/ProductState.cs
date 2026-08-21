namespace StartupEmpire.Products
{
    /// Instância em runtime de um produto, com todos os atributos vivos do jogo.
    public sealed class ProductState
    {
        public string Id { get; }
        public ProductDefinition Definition { get; }
        public ProductStage Stage { get; internal set; } = ProductStage.Planning;
        public double DevProgress { get; internal set; }
        public double Quality { get; internal set; } = 0.5;
        public double Stability { get; internal set; } = 1.0;
        public int BugCount { get; internal set; }
        public double Performance { get; internal set; } = 0.5;
        public double Security { get; internal set; } = 0.5;
        public double Popularity { get; internal set; }
        public int Users { get; internal set; }
        public int PayingCustomers { get; internal set; }
        public double Price { get; internal set; }
        public double Reputation { get; internal set; } = 0.5;

        public ProductState(ProductDefinition definition)
        {
            Definition = definition;
            Id = definition.Id;
            Price = definition.BasePrice;
        }

        public bool IsReadyToTest => Stage == ProductStage.Development && DevProgress >= Definition.BaseDevPointsRequired;

        public void AddOfflineBugs(int count)
        {
            if (count <= 0) return;
            BugCount += count;
        }
    }
}
