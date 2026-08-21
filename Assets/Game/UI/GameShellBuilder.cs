using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using StartupEmpire.Core;
using StartupEmpire.UI.Screens;

namespace StartupEmpire.UI
{
    /// Composição visual raiz: Canvas, EventSystem, barra de status no topo, área
    /// de conteúdo (onde ScreenManager alterna telas) e barra de navegação embaixo.
    /// Cada tela é uma classe própria (IScreenPanel) sem nenhuma regra de negócio —
    /// só chama a API que já existe em GameRoot e mostra o resultado.
    public sealed class GameShellBuilder : MonoBehaviour
    {
        private Text _topBarText;
        private ScreenManager _screenManager;
        private readonly Dictionary<string, IScreenPanel> _panelsById = new();

        private void Start()
        {
            EnsureEventSystem();
            var canvas = BuildCanvas();
            var root = UiFactory.CreatePanel(canvas.transform, UiFactory.PanelBackground, "Root");

            _topBarText = UiFactory.CreateText(root.transform, "", 24, TextAnchor.MiddleLeft,
                new Vector2(0.02f, 0.945f), new Vector2(0.98f, 0.99f), "TopBarText");

            var contentGo = new GameObject("Content", typeof(RectTransform));
            contentGo.transform.SetParent(root.transform, false);
            UiFactory.Stretch(contentGo.GetComponent<RectTransform>(), new Vector2(0f, 0.10f), new Vector2(1f, 0.935f));

            _screenManager = gameObject.AddComponent<ScreenManager>();

            RegisterPanel("Office", new OfficeScreenPanel(), contentGo.transform);
            RegisterPanel("Products", new ProductsScreenPanel(), contentGo.transform);
            RegisterPanel("Employees", new EmployeesScreenPanel(), contentGo.transform);
            RegisterPanel("Upgrades", new UpgradesScreenPanel(), contentGo.transform);
            RegisterPanel("Store", new StoreScreenPanel(), contentGo.transform);
            RegisterPanel("Finances", new FinancesScreenPanel(), contentGo.transform);
            RegisterPanel("Statistics", new StatisticsScreenPanel(), contentGo.transform);
            RegisterPanel("Achievements", new AchievementsScreenPanel(), contentGo.transform);
            RegisterPanel("Missions", new MissionsScreenPanel(), contentGo.transform);
            RegisterPanel("Settings", new SettingsScreenPanel(), contentGo.transform);

            BuildNavBar(root.transform);

            ShowScreen("Office");
        }

        private void RegisterPanel(string id, IScreenPanel panel, Transform contentParent)
        {
            panel.Build(contentParent, _screenManager);
            _panelsById[id] = panel;
        }

        private void ShowScreen(string id)
        {
            _screenManager.Show(id);
            if (_panelsById.TryGetValue(id, out var panel)) panel.Refresh();
        }

        private void Update()
        {
            if (GameRoot.Instance == null) return;
            var state = GameRoot.Instance.State;
            _topBarText.text =
                $"Caixa: R$ {state.Economy.Cash:F0}   Valuation: R$ {state.Economy.Valuation:F0}   " +
                $"Gems: {state.GemWallet.Balance}   {state.Stage}";
        }

        private void BuildNavBar(Transform parent)
        {
            var navGo = UiFactory.CreatePanel(parent, UiFactory.NavBarColor, "NavBar");
            UiFactory.Stretch(navGo.GetComponent<RectTransform>(), new Vector2(0, 0), new Vector2(1, 0.095f));

            var labels = new[] { "Office", "Products", "Employees", "Upgrades", "Store", "Finances", "Stats", "Achv", "Missions", "Settings" };
            var ids = new[] { "Office", "Products", "Employees", "Upgrades", "Store", "Finances", "Statistics", "Achievements", "Missions", "Settings" };

            var width = 1f / labels.Length;
            for (var i = 0; i < labels.Length; i++)
            {
                var id = ids[i];
                var minX = i * width;
                var maxX = minX + width;
                UiFactory.CreateButton(navGo.transform, labels[i], new Vector2(minX + 0.002f, 0.08f), new Vector2(maxX - 0.002f, 0.92f),
                    () => ShowScreen(id));
            }
        }

        private static void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() != null) return;
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        private Canvas BuildCanvas()
        {
            var canvasGo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
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
    }
}
