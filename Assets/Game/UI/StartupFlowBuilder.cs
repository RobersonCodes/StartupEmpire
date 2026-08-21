using System;
using UnityEngine;
using UnityEngine.UI;
using StartupEmpire.Core;

namespace StartupEmpire.UI
{
    /// Builds the launch flow above the game shell. New Game only replaces an
    /// existing save after an explicit confirmation.
    public sealed class StartupFlowBuilder
    {
        private GameObject _root;
        private GameObject _splash;
        private GameObject _mainMenu;
        private GameObject _confirmation;
        private Action _onEnterGame;

        public void Build(Transform canvasParent, Action onEnterGame)
        {
            _onEnterGame = onEnterGame;
            _root = UiFactory.CreatePanel(canvasParent, UiFactory.PanelBackground, "StartupFlow");
            _root.AddComponent<SafeAreaFitter>();

            BuildSplash();
            BuildMainMenu();
            BuildConfirmation();

            _splash.SetActive(true);
            _mainMenu.SetActive(false);
            _confirmation.SetActive(false);
            _root.transform.SetAsLastSibling();
        }

        private void BuildSplash()
        {
            _splash = UiFactory.CreatePanel(_root.transform, new Color(0.035f, 0.055f, 0.10f, 1f), "Splash");
            UiFactory.CreateText(_splash.transform, "STARTUP\nEMPIRE", 72, TextAnchor.MiddleCenter,
                new Vector2(0.08f, 0.45f), new Vector2(0.92f, 0.75f), "SplashTitle");
            UiFactory.CreateText(_splash.transform, "Do primeiro código ao IPO", 28, TextAnchor.MiddleCenter,
                new Vector2(0.08f, 0.34f), new Vector2(0.92f, 0.47f), "SplashSubtitle");
            var skip = UiFactory.CreateButton(_splash.transform, "Entrar", new Vector2(0.18f, 0.12f),
                new Vector2(0.82f, 0.20f), ShowMainMenu);
            skip.gameObject.name = "Button_SkipSplash";
        }

        private void BuildMainMenu()
        {
            _mainMenu = UiFactory.CreatePanel(_root.transform, new Color(0.05f, 0.075f, 0.13f, 1f), "MainMenu");
            UiFactory.CreateText(_mainMenu.transform, "STARTUP EMPIRE", 62, TextAnchor.MiddleCenter,
                new Vector2(0.05f, 0.62f), new Vector2(0.95f, 0.82f), "MainMenuTitle");
            UiFactory.CreateText(_mainMenu.transform, "Construa. Lance. Escale.", 28, TextAnchor.MiddleCenter,
                new Vector2(0.05f, 0.55f), new Vector2(0.95f, 0.65f), "MainMenuSubtitle");

            var continueButton = UiFactory.CreateButton(_mainMenu.transform, "Continuar",
                new Vector2(0.14f, 0.37f), new Vector2(0.86f, 0.45f), EnterGame);
            continueButton.interactable = GameRoot.Instance != null && GameRoot.Instance.HadExistingSave;

            UiFactory.CreateButton(_mainMenu.transform, "Novo Jogo",
                new Vector2(0.14f, 0.27f), new Vector2(0.86f, 0.35f), RequestNewGame);
            UiFactory.CreateText(_mainMenu.transform, "Seu progresso é salvo automaticamente.", 22,
                TextAnchor.MiddleCenter, new Vector2(0.08f, 0.10f), new Vector2(0.92f, 0.18f), "AutosaveHint");
        }

        private void BuildConfirmation()
        {
            _confirmation = UiFactory.CreatePanel(_root.transform, new Color(0.07f, 0.09f, 0.14f, 0.99f), "NewGameConfirmation");
            UiFactory.Stretch(_confirmation.GetComponent<RectTransform>(), new Vector2(0.08f, 0.30f), new Vector2(0.92f, 0.70f));
            UiFactory.CreateText(_confirmation.transform,
                "Começar um novo jogo?\n\nO progresso atual será substituído.", 30, TextAnchor.MiddleCenter,
                new Vector2(0.07f, 0.38f), new Vector2(0.93f, 0.88f), "ConfirmationText");
            UiFactory.CreateButton(_confirmation.transform, "Confirmar Novo Jogo",
                new Vector2(0.08f, 0.18f), new Vector2(0.92f, 0.34f), ConfirmNewGame);
            UiFactory.CreateButton(_confirmation.transform, "Cancelar",
                new Vector2(0.08f, 0.02f), new Vector2(0.92f, 0.15f), CancelNewGame);
        }

        private void ShowMainMenu()
        {
            _splash.SetActive(false);
            _mainMenu.SetActive(true);
        }

        private void RequestNewGame()
        {
            if (GameRoot.Instance != null && GameRoot.Instance.HadExistingSave)
            {
                _confirmation.SetActive(true);
                _confirmation.transform.SetAsLastSibling();
                return;
            }

            ConfirmNewGame();
        }

        private void ConfirmNewGame()
        {
            GameRoot.Instance.StartNewGame();
            EnterGame();
        }

        private void CancelNewGame() => _confirmation.SetActive(false);

        private void EnterGame()
        {
            _root.SetActive(false);
            _onEnterGame?.Invoke();
        }
    }
}
