using UnityEngine;
using UnityEngine.UI;
using StartupEmpire.Core;
using StartupEmpire.Products;
using StartupEmpire.Research;

namespace StartupEmpire.UI.Screens
{
    /// Tela "Office" (hub): status resumido + as ações centrais do loop do
    /// Capítulo 1 (aprender, desenvolver, corrigir, lançar, avançar ciclo).
    public sealed class OfficeScreenPanel : IScreenPanel
    {
        private Text _statusText;

        public void Build(Transform contentParent, ScreenManager screenManager)
        {
            var root = UiFactory.CreatePanel(contentParent, UiFactory.PanelBackground, "OfficePanel");

            _statusText = UiFactory.CreateText(root.transform, "", 30, TextAnchor.UpperLeft,
                new Vector2(0.03f, 0.42f), new Vector2(0.97f, 0.97f), "StatusText");

            UiFactory.CreateButton(root.transform, "Estudar Fundamentos", new Vector2(0.03f, 0.34f), new Vector2(0.97f, 0.40f), OnStudy);
            UiFactory.CreateButton(root.transform, "Desenvolver Produto", new Vector2(0.03f, 0.26f), new Vector2(0.97f, 0.32f), OnDevelop);
            UiFactory.CreateButton(root.transform, "Corrigir Bugs", new Vector2(0.03f, 0.18f), new Vector2(0.97f, 0.24f), OnFixBugs);
            UiFactory.CreateButton(root.transform, "Lançar Produto", new Vector2(0.03f, 0.10f), new Vector2(0.97f, 0.16f), OnLaunch);
            UiFactory.CreateButton(root.transform, "Avançar Ciclo", new Vector2(0.03f, 0.02f), new Vector2(0.97f, 0.08f), OnRunCycle);

            screenManager.Register("Office", root);
        }

        private void OnStudy()
        {
            GameRoot.Instance.StudyTrack(KnowledgeTracks.Fundamentos, 1);
            Refresh();
        }

        private void OnDevelop()
        {
            var product = FirstProduct();
            if (product == null) return;
            GameRoot.Instance.DevelopProduct(product, KnowledgeTracks.Fundamentos, 1);
            Refresh();
        }

        private void OnFixBugs()
        {
            var product = FirstProduct();
            if (product == null) return;
            GameRoot.Instance.Development.FixBugs(product, 1);
            Refresh();
        }

        private void OnLaunch()
        {
            var product = FirstProduct();
            if (product == null) return;
            GameRoot.Instance.Development.Launch(product);
            Refresh();
        }

        private void OnRunCycle()
        {
            GameRoot.Instance.RunGameCycle(1);
            Refresh();
        }

        private static ProductState FirstProduct()
        {
            var products = GameRoot.Instance.State.Products;
            return products.Count > 0 ? products[0] : null;
        }

        public void Refresh()
        {
            if (GameRoot.Instance == null) return;
            var state = GameRoot.Instance.State;
            var product = FirstProduct();

            var text =
                $"Caixa: R$ {state.Economy.Cash:F2}\n" +
                $"Valuation: R$ {state.Economy.Valuation:F2}\n" +
                $"MRR: R$ {state.Economy.MonthlyRecurringRevenue:F2}\n" +
                $"Gems: {state.GemWallet.Balance}\n" +
                $"Estágio: {state.Stage}";

            if (product != null)
            {
                text +=
                    $"\n\nProduto: {product.Definition.DisplayName}\n" +
                    $"Fase: {product.Stage}\n" +
                    $"Progresso: {product.DevProgress:F0}/{product.Definition.BaseDevPointsRequired:F0}\n" +
                    $"Bugs: {product.BugCount}\n" +
                    $"Usuários: {product.Users}   Pagantes: {product.PayingCustomers}";
            }

            _statusText.text = text;
        }
    }
}
