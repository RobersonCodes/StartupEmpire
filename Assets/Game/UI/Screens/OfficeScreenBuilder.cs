using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using StartupEmpire.Core;
using StartupEmpire.Research;

namespace StartupEmpire.UI
{
    /// Constrói a UI da tela "Office" (hub) inteiramente em runtime, via código.
    /// Deliberadamente sem depender de uma cena/prefab pré-desenhados: mais fácil
    /// de gerar e validar sem interação manual no Editor. Toda a lógica de clique
    /// já vive em GameRoot — este componente só monta a árvore visual e chama os
    /// métodos correspondentes. Quando a arte/UX final existir, isso vira um
    /// prefab desenhado no Editor sem precisar mudar nenhuma lógica de jogo.
    public sealed class OfficeScreenBuilder : MonoBehaviour
    {
        private Text _statusText;

        private void Start()
        {
            EnsureEventSystem();
            var canvas = BuildCanvas();
            var panel = BuildPanel(canvas.transform);
            _statusText = BuildStatusText(panel.transform);

            BuildButton(panel.transform, "Estudar Fundamentos", 0.46f, OnStudy);
            BuildButton(panel.transform, "Desenvolver Produto", 0.38f, OnDevelop);
            BuildButton(panel.transform, "Corrigir Bugs", 0.30f, OnFixBugs);
            BuildButton(panel.transform, "Lançar Produto", 0.22f, OnLaunch);
            BuildButton(panel.transform, "Avançar Ciclo", 0.14f, OnRunCycle);

            RefreshStatus();
        }

        private void OnStudy()
        {
            GameRoot.Instance.StudyTrack(KnowledgeTracks.Fundamentos, 1);
            RefreshStatus();
        }

        private void OnDevelop()
        {
            var product = FirstProduct();
            if (product == null) return;
            GameRoot.Instance.DevelopProduct(product, KnowledgeTracks.Fundamentos, 1);
            RefreshStatus();
        }

        private void OnFixBugs()
        {
            var product = FirstProduct();
            if (product == null) return;
            GameRoot.Instance.Development.FixBugs(product, 1);
            RefreshStatus();
        }

        private void OnLaunch()
        {
            var product = FirstProduct();
            if (product == null) return;
            GameRoot.Instance.Development.Launch(product);
            RefreshStatus();
        }

        private void OnRunCycle()
        {
            GameRoot.Instance.RunGameCycle(1);
            RefreshStatus();
        }

        private static Products.ProductState FirstProduct()
        {
            var products = GameRoot.Instance.State.Products;
            return products.Count > 0 ? products[0] : null;
        }

        private void RefreshStatus()
        {
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

        private static void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() != null) return;
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        private Canvas BuildCanvas()
        {
            var canvasGo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            // Filho deste GameObject de propósito: destruir o GameRoot/OfficeScreenBuilder
            // (ex.: ao trocar de cena, ou em testes) precisa levar a UI inteira junto.
            canvasGo.transform.SetParent(transform, false);
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            return canvas;
        }

        private static GameObject BuildPanel(Transform parent)
        {
            var panelGo = new GameObject("Panel", typeof(Image));
            panelGo.transform.SetParent(parent, false);

            var image = panelGo.GetComponent<Image>();
            image.color = new Color(0.07f, 0.09f, 0.13f, 1f);

            var rect = panelGo.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            return panelGo;
        }

        private static Text BuildStatusText(Transform parent)
        {
            var go = new GameObject("StatusText", typeof(Text));
            go.transform.SetParent(parent, false);

            var text = go.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 32;
            text.color = Color.white;
            text.alignment = TextAnchor.UpperLeft;

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.05f, 0.55f);
            rect.anchorMax = new Vector2(0.95f, 0.98f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            return text;
        }

        private static void BuildButton(Transform parent, string label, float anchorY, UnityEngine.Events.UnityAction onClick)
        {
            var go = new GameObject($"Button_{label}", typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);

            var image = go.GetComponent<Image>();
            image.color = new Color(0.15f, 0.45f, 0.75f, 1f);

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.05f, anchorY);
            rect.anchorMax = new Vector2(0.95f, anchorY + 0.06f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var button = go.GetComponent<Button>();
            button.onClick.AddListener(onClick);

            var textGo = new GameObject("Text", typeof(Text));
            textGo.transform.SetParent(go.transform, false);

            var text = textGo.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 28;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            text.text = label;

            var textRect = textGo.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
        }
    }
}
