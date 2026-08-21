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
        private Button _studyButton;
        private Button _developButton;
        private Button _testButton;
        private Button _fixButton;
        private Button _launchButton;
        private Button _endDayButton;
        private string _lastActionMessage = "Escolha como usar seu tempo hoje.";

        public void Build(Transform contentParent, ScreenManager screenManager)
        {
            var root = UiFactory.CreatePanel(contentParent, UiFactory.PanelBackground, "OfficePanel");

            _statusText = UiFactory.CreateText(root.transform, "", 30, TextAnchor.UpperLeft,
                new Vector2(0.03f, 0.50f), new Vector2(0.97f, 0.97f), "StatusText");

            _studyButton = UiFactory.CreateButton(root.transform, "Estudar Fundamentos", new Vector2(0.03f, 0.42f), new Vector2(0.97f, 0.48f), OnStudy);
            _developButton = UiFactory.CreateButton(root.transform, "Desenvolver Produto", new Vector2(0.03f, 0.34f), new Vector2(0.97f, 0.40f), OnDevelop);
            _testButton = UiFactory.CreateButton(root.transform, "Testar Produto", new Vector2(0.03f, 0.26f), new Vector2(0.97f, 0.32f), OnTest);
            _fixButton = UiFactory.CreateButton(root.transform, "Corrigir Bugs", new Vector2(0.03f, 0.18f), new Vector2(0.97f, 0.24f), OnFixBugs);
            _launchButton = UiFactory.CreateButton(root.transform, "Lançar Produto", new Vector2(0.03f, 0.10f), new Vector2(0.97f, 0.16f), OnLaunch);
            _endDayButton = UiFactory.CreateButton(root.transform, "Encerrar Dia", new Vector2(0.03f, 0.02f), new Vector2(0.97f, 0.08f), OnEndDay);

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
                $"{_lastActionMessage}\n" +
                $"{TutorialGuidance.MessageFor(state.TutorialProgress)}";

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
            UpdateTutorialHighlight(state.TutorialProgress);
        }

        private void UpdateTutorialHighlight(TutorialStep step)
        {
            var recommended = step switch
            {
                TutorialStep.LearnFundamentals => _studyButton,
                TutorialStep.DevelopProduct => _developButton,
                TutorialStep.TestProduct => _testButton,
                TutorialStep.FixKnownBugs => _fixButton,
                TutorialStep.LaunchProduct => _launchButton,
                TutorialStep.AcquireFirstCustomer => _endDayButton,
                _ => null
            };

            ResetButtonColor(_studyButton);
            ResetButtonColor(_developButton);
            ResetButtonColor(_testButton);
            ResetButtonColor(_fixButton);
            ResetButtonColor(_launchButton);
            ResetButtonColor(_endDayButton);
            if (recommended != null)
            {
                recommended.GetComponent<Image>().color = new Color(0.93f, 0.58f, 0.12f, 1f);
            }
        }

        private static void ResetButtonColor(Button button)
        {
            if (button != null) button.GetComponent<Image>().color = UiFactory.ButtonColor;
        }
    }
}
