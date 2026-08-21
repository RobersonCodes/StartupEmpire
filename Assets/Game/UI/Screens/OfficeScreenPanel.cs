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
        private string _lastActionMessage = "Escolha como usar seu tempo hoje.";

        public void Build(Transform contentParent, ScreenManager screenManager)
        {
            var root = UiFactory.CreatePanel(contentParent, UiFactory.PanelBackground, "OfficePanel");

            _statusText = UiFactory.CreateText(root.transform, "", 30, TextAnchor.UpperLeft,
                new Vector2(0.03f, 0.50f), new Vector2(0.97f, 0.97f), "StatusText");

            UiFactory.CreateButton(root.transform, "Estudar Fundamentos", new Vector2(0.03f, 0.42f), new Vector2(0.97f, 0.48f), OnStudy);
            UiFactory.CreateButton(root.transform, "Desenvolver Produto", new Vector2(0.03f, 0.34f), new Vector2(0.97f, 0.40f), OnDevelop);
            UiFactory.CreateButton(root.transform, "Testar Produto", new Vector2(0.03f, 0.26f), new Vector2(0.97f, 0.32f), OnTest);
            UiFactory.CreateButton(root.transform, "Corrigir Bugs", new Vector2(0.03f, 0.18f), new Vector2(0.97f, 0.24f), OnFixBugs);
            UiFactory.CreateButton(root.transform, "Lançar Produto", new Vector2(0.03f, 0.10f), new Vector2(0.97f, 0.16f), OnLaunch);
            UiFactory.CreateButton(root.transform, "Encerrar Dia", new Vector2(0.03f, 0.02f), new Vector2(0.97f, 0.08f), OnEndDay);

            screenManager.Register("Office", root);
        }

        private void OnStudy()
        {
            Record(GameRoot.Instance.StudyTrack(KnowledgeTracks.Fundamentos, 1));
            Refresh();
        }

        private void OnDevelop()
        {
            var product = FirstProduct();
            if (product == null) return;
            Record(GameRoot.Instance.DevelopProduct(product, KnowledgeTracks.Fundamentos, 1));
            Refresh();
        }

        private void OnFixBugs()
        {
            var product = FirstProduct();
            if (product == null) return;
            Record(GameRoot.Instance.FixProductBugs(product, 1));
            Refresh();
        }

        private void OnTest()
        {
            var product = FirstProduct();
            if (product == null) return;
            Record(GameRoot.Instance.TestProduct(product, 1));
            Refresh();
        }

        private void OnLaunch()
        {
            var product = FirstProduct();
            if (product == null) return;
            _lastActionMessage = GameRoot.Instance.LaunchProduct(product)
                ? "Produto lançado! Agora conquiste clientes."
                : "Para lançar, conclua o desenvolvimento e faça ao menos um teste.";
            Refresh();
        }

        private void OnEndDay()
        {
            Record(GameRoot.Instance.EndWorkDay());
            Refresh();
        }

        private void Record(GameActionResult result) => _lastActionMessage = result.Message;

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
                $"Dia {state.Player.CurrentDay}   Tempo: {state.Player.RemainingWorkCycles}/{state.Player.WorkCyclesPerDay} ciclos\n" +
                $"Caixa: R$ {state.Economy.Cash:F2}\n" +
                $"Valuation: R$ {state.Economy.Valuation:F2}\n" +
                $"MRR: R$ {state.Economy.MonthlyRecurringRevenue:F2}\n" +
                $"Gems: {state.GemWallet.Balance}\n" +
                $"Estágio: {state.Stage}\n" +
                $"{_lastActionMessage}";

            if (product != null)
            {
                text +=
                    $"\n\nProduto: {product.Definition.DisplayName}\n" +
                    $"Fase: {product.Stage}\n" +
                    $"Progresso: {product.DevProgress:F0}/{product.Definition.BaseDevPointsRequired:F0}\n" +
                    $"Testado: {(product.HasBeenTested ? "Sim" : "Não")}\n" +
                    $"Bugs conhecidos: {product.KnownBugCount}\n" +
                    $"Usuários: {product.Users}   Pagantes: {product.PayingCustomers}";
            }

            _statusText.text = text;
        }
    }
}
