using System.Collections.Generic;

namespace StartupEmpire.Products
{
    public sealed class ProductDefinitionCatalog
    {
        private readonly Dictionary<string, ProductDefinition> _byId = new();

        public void Register(ProductDefinition definition) => _byId[definition.Id] = definition;

        public ProductDefinition Find(string id) => _byId.TryGetValue(id, out var def) ? def : null;

        public static ProductDefinitionCatalog CreateChapter1Catalog()
        {
            var catalog = new ProductDefinitionCatalog();
            catalog.Register(new ProductDefinition(
                id: "first_website",
                displayName: "Meu Primeiro Site",
                category: ProductCategory.Website,
                baseDevPointsRequired: 100,
                basePrice: 9.90,
                bugRatePerProgress: 0.08));
            return catalog;
        }
    }
}
