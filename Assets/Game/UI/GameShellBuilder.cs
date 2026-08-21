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
        private EventModalBuilder _eventModal;
        private StartupFlowBuilder _startupFlow;
        private GameObject _gameplayRoot;
        private GameObject _moreMenu;
        private bool _gameplayVisible;
        private string _currentScreenId;
        private readonly Dictionary<string, IScreenPanel> _panelsById = new();

        private void Start()
        {
            EnsureEventSystem();
            var canvas = BuildCanvas();
            _gameplayRoot = UiFactory.CreatePanel(canvas.transform, UiFactory.PanelBackground, "Root");
            _gameplayRoot.AddComponent<SafeAreaFitter>();

            _topBarText = UiFactory.CreateText(_gameplayRoot.transform, "", 24, TextAnchor.MiddleLeft,
                new Vector2(0.02f, 0.945f), new Vector2(0.98f, 0.99f), "TopBarText");

            var contentGo = new GameObject("Content", typeof(RectTransform));
            contentGo.transform.SetParent(_gameplayRoot.transform, false);
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
            RegisterPanel("Research", new ResearchScreenPanel(), contentGo.transform);
            RegisterPanel("Company", new CompanyScreenPanel(), contentGo.transform);
            RegisterPanel("Character", new CharacterScreenPanel(), contentGo.transform);

            BuildNavBar(_gameplayRoot.transform);
            BuildMoreMenu(_gameplayRoot.transform);

            _eventModal = new EventModalBuilder();
            _eventModal.Build(canvas.transform);

            ShowScreen("Office");
            _gameplayRoot.SetActive(false);
            _startupFlow = new StartupFlowBuilder();
            _startupFlow.Build(canvas.transform, EnterGame);
        }

        private void RegisterPanel(string id, IScreenPanel panel, Transform contentParent)
        {
            panel.Build(contentParent, _screenManager);
            _panelsById[id] = panel;
        }

        private void ShowScreen(string id)
        {
            if (_moreMenu != null) _moreMenu.SetActive(false);
            _screenManager.Show(id);
            if (_panelsById.TryGetValue(id, out var panel))
            {
                _currentScreenId = id;
                panel.Refresh();
            }
        }

        private void Update()
        {
            if (!_gameplayVisible) return;
            if (Input.GetKeyDown(KeyCode.Escape)) HandleBack();
            if (GameRoot.Instance == null) return;
            var state = GameRoot.Instance.State;
            _topBarText.text =
                $"Caixa: R$ {state.Economy.Cash:F0}   Valuation: R$ {state.Economy.Valuation:F0}   " +
                $"Gems: {state.GemWallet.Balance}   Dia {state.Player.CurrentDay}   " +
                $"Tempo: {state.Player.RemainingWorkCycles}/{state.Player.WorkCyclesPerDay}   {state.Stage}";

            _eventModal.Tick();
        }

        private void EnterGame()
        {
            _gameplayRoot.SetActive(true);
            _gameplayVisible = true;
            ShowScreen("Office");
        }

        private void BuildNavBar(Transform parent)
        {
            var navGo = UiFactory.CreatePanel(parent, UiFactory.NavBarColor, "NavBar");
            UiFactory.Stretch(navGo.GetComponent<RectTransform>(), new Vector2(0, 0), new Vector2(1, 0.095f));

            var labels = new[] { "Início", "Produtos", "Equipe", "Empresa", "Mais" };
            var ids = new[] { "Office", "Products", "Employees", "Company", "More" };

            var width = 1f / labels.Length;
            for (var i = 0; i < labels.Length; i++)
            {
                var id = ids[i];
                var minX = i * width;
                var maxX = minX + width;
                UiFactory.CreateButton(navGo.transform, labels[i], new Vector2(minX + 0.002f, 0.08f), new Vector2(maxX - 0.002f, 0.92f),
                    () =>
                    {
                        if (id == "More") ToggleMoreMenu();
                        else ShowScreen(id);
                    });
            }
        }

        private void BuildMoreMenu(Transform parent)
        {
            _moreMenu = UiFactory.CreatePanel(parent, new Color(0.06f, 0.08f, 0.12f, 0.98f), "MoreMenu");
            UiFactory.Stretch(_moreMenu.GetComponent<RectTransform>(), new Vector2(0.02f, 0.105f), new Vector2(0.98f, 0.49f));

            var labels = new[]
            {
                "Pesquisa", "Melhorias", "Loja", "Finanças", "Estatísticas",
                "Conquistas", "Missões", "Perfil", "Config."
            };
            var ids = new[]
            {
                "Research", "Upgrades", "Store", "Finances", "Statistics",
                "Achievements", "Missions", "Character", "Settings"
            };

            const int columns = 3;
            const int rows = 3;
            for (var i = 0; i < labels.Length; i++)
            {
                var id = ids[i];
                var column = i % columns;
                var row = i / columns;
                var minX = column / (float)columns + 0.01f;
                var maxX = (column + 1) / (float)columns - 0.01f;
                var maxY = 1f - row / (float)rows - 0.02f;
                var minY = 1f - (row + 1) / (float)rows + 0.02f;
                UiFactory.CreateButton(_moreMenu.transform, labels[i], new Vector2(minX, minY), new Vector2(maxX, maxY),
                    () => ShowScreen(id));
            }

            _moreMenu.SetActive(false);
        }

        private void ToggleMoreMenu()
        {
            _moreMenu.SetActive(!_moreMenu.activeSelf);
            if (_moreMenu.activeSelf) _moreMenu.transform.SetAsLastSibling();
        }

        private void HandleBack()
        {
            if (_moreMenu != null && _moreMenu.activeSelf)
            {
                _moreMenu.SetActive(false);
                return;
            }

            if (_currentScreenId != "Office") ShowScreen("Office");
            else Application.Quit();
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
