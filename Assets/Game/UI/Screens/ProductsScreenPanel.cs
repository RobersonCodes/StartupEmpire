using System.Text;
using UnityEngine;
using UnityEngine.UI;
using StartupEmpire.Core;

namespace StartupEmpire.UI.Screens
{
    public sealed class ProductsScreenPanel : IScreenPanel
    {
        private Text _text;

        public void Build(Transform contentParent, ScreenManager screenManager)
        {
            var root = UiFactory.CreatePanel(contentParent, UiFactory.PanelBackground, "ProductsPanel");
            _text = UiFactory.CreateText(root.transform, "", 26, TextAnchor.UpperLeft,
                new Vector2(0.03f, 0.03f), new Vector2(0.97f, 0.97f), "ProductsText");
            screenManager.Register("Products", root);
        }

        public void Refresh()
        {
            if (GameRoot.Instance == null) return;
            var sb = new StringBuilder();
            sb.AppendLine("PRODUTOS\n");

            var products = GameRoot.Instance.State.Products;
            if (products.Count == 0) sb.AppendLine("Nenhum produto ainda.");

            foreach (var product in products)
            {
                sb.AppendLine($"{product.Definition.DisplayName} ({product.Definition.Category})");
                sb.AppendLine($"  Fase: {product.Stage}   Progresso: {product.DevProgress:F0}/{product.Definition.BaseDevPointsRequired:F0}");
                sb.AppendLine($"  Qualidade: {product.Quality:P0}   Estabilidade: {product.Stability:P0}   Bugs conhecidos: {product.KnownBugCount}");
                sb.AppendLine($"  Testado: {(product.HasBeenTested ? "Sim" : "Não")}");
                sb.AppendLine($"  Usuários: {product.Users}   Pagantes: {product.PayingCustomers}   Preço: R$ {product.Price:F2}");
                sb.AppendLine($"  Reputação: {product.Reputation:P0}   Popularidade: {product.Popularity:P0}\n");
            }

            _text.text = sb.ToString();
        }
    }
}
