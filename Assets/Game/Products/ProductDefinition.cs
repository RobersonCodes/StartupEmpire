namespace StartupEmpire.Products
{
    /// Dados de design (imutáveis) de um tipo de produto. Instâncias em runtime
    /// são representadas por ProductState.
    public sealed class ProductDefinition
    {
        public string Id { get; }
        public string DisplayName { get; }
        public ProductCategory Category { get; }
        public double BaseDevPointsRequired { get; }
        public double BasePrice { get; }
        public double BugRatePerProgress { get; }

        public ProductDefinition(string id, string displayName, ProductCategory category,
            double baseDevPointsRequired, double basePrice, double bugRatePerProgress)
        {
            Id = id;
            DisplayName = displayName;
            Category = category;
            BaseDevPointsRequired = baseDevPointsRequired;
            BasePrice = basePrice;
            BugRatePerProgress = bugRatePerProgress;
        }
    }
}
