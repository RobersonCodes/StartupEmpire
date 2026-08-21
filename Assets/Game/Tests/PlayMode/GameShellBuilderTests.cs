using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using StartupEmpire.Audio;
using StartupEmpire.Core;
using StartupEmpire.Products;
using StartupEmpire.UI;

namespace StartupEmpire.Tests.PlayMode
{
    public class GameShellBuilderTests
    {
        /// Object.Destroy() só é processado no fim do frame, o que é ambíguo demais
        /// entre um [UnityTest] e o próximo (já pegou um bug real: o singleton do
        /// teste anterior ainda existia quando o Awake() do próximo rodava, e o
        /// GameObject inteiro se autodestruía antes do Start() montar a UI).
        /// DestroyImmediate + teardown garantido elimina essa ambiguidade.
        [UnityTearDown]
        public IEnumerator TearDown()
        {
            foreach (var root in Object.FindObjectsByType<GameRoot>(FindObjectsSortMode.None))
            {
                Object.DestroyImmediate(root.gameObject);
            }
            yield return null;
        }

        private static IEnumerator BuildAndStartFreshGame()
        {
            new GameObject("GameRoot", typeof(GameRoot), typeof(AudioManager), typeof(GameShellBuilder));
            yield return null;

            GameObject.Find("Button_SkipSplash").GetComponent<Button>().onClick.Invoke();
            yield return null;
            GameObject.Find("Button_Novo Jogo").GetComponent<Button>().onClick.Invoke();
            yield return null;
        }

        [UnityTest]
        public IEnumerator StartupFlow_RequiresEntry_AndDisablesContinueWithoutSave()
        {
            new GameObject("GameRoot", typeof(GameRoot), typeof(AudioManager), typeof(GameShellBuilder));
            yield return null;

            var splash = GameObject.Find("Splash");
            var gameplay = GameObject.Find("Canvas").transform.Find("Root").gameObject;
            Assert.IsTrue(splash.activeInHierarchy);
            Assert.IsFalse(gameplay.activeSelf);

            GameObject.Find("Button_SkipSplash").GetComponent<Button>().onClick.Invoke();
            yield return null;
            Assert.IsFalse(splash.activeSelf);
            Assert.IsTrue(GameObject.Find("MainMenu").activeInHierarchy);
            Assert.IsFalse(GameObject.Find("Button_Continuar").GetComponent<Button>().interactable);

            GameObject.Find("Button_Novo Jogo").GetComponent<Button>().onClick.Invoke();
            yield return null;
            Assert.IsTrue(gameplay.activeInHierarchy);
            Assert.AreEqual(TutorialStep.LearnFundamentals, GameRoot.Instance.State.TutorialProgress);
        }

        [UnityTest]
        public IEnumerator GameShellBuilder_CreatesOfficeScreen_WithoutErrors()
        {
            yield return BuildAndStartFreshGame();

            var statusGo = GameObject.Find("StatusText");
            Assert.IsNotNull(statusGo, "StatusText não foi criado");

            var text = statusGo.GetComponent<Text>();
            Assert.IsNotNull(text);
            StringAssert.Contains("Caixa", text.text);
            StringAssert.Contains("Estágio", text.text);

            Assert.IsNotNull(GameObject.Find("Button_Estudar Fundamentos"));
            Assert.IsNotNull(GameObject.Find("Button_Testar Produto"));
            Assert.IsNotNull(GameObject.Find("Button_Encerrar Dia"));
            Assert.IsNotNull(GameObject.Find("Button_Produtos"));
            Assert.IsNotNull(GameObject.Find("Button_Mais"));
            Assert.IsNotNull(GameObject.Find("Root").GetComponent<SafeAreaFitter>());
            Assert.AreEqual(5, GameObject.Find("NavBar").transform.childCount,
                "A navegação principal deve manter cinco alvos de toque grandes");
            foreach (var navButton in GameObject.Find("NavBar").GetComponentsInChildren<Button>())
            {
                var rect = navButton.GetComponent<RectTransform>();
                Assert.GreaterOrEqual(rect.anchorMax.x - rect.anchorMin.x, 0.19f,
                    $"{navButton.name} ficou estreito demais para toque");
            }
        }

        [UnityTest]
        public IEnumerator StudyButton_Click_IncreasesKnowledge_AndRefreshesStatus()
        {
            yield return BuildAndStartFreshGame();

            var knowledgeBefore = GameRoot.Instance.State.Player.GetKnowledge(Research.KnowledgeTracks.Fundamentos);

            var buttonGo = GameObject.Find("Button_Estudar Fundamentos");
            Assert.IsNotNull(buttonGo, "Button_Estudar Fundamentos não foi encontrado");
            buttonGo.GetComponent<Button>().onClick.Invoke();

            var knowledgeAfter = GameRoot.Instance.State.Player.GetKnowledge(Research.KnowledgeTracks.Fundamentos);
            Assert.Greater(knowledgeAfter, knowledgeBefore);
            Assert.AreEqual(TutorialStep.DevelopProduct, GameRoot.Instance.State.TutorialProgress);
            Assert.AreEqual(GameRoot.Instance.State.Player.WorkCyclesPerDay - 1,
                GameRoot.Instance.State.Player.RemainingWorkCycles);
        }

        [UnityTest]
        public IEnumerator WorkTime_BlocksExtraActions_AndEndDayRestoresCycles()
        {
            yield return BuildAndStartFreshGame();

            var player = GameRoot.Instance.State.Player;
            var studyButton = GameObject.Find("Button_Estudar Fundamentos").GetComponent<Button>();
            for (var i = 0; i < player.WorkCyclesPerDay; i++) studyButton.onClick.Invoke();

            var knowledgeAtLimit = player.GetKnowledge(Research.KnowledgeTracks.Fundamentos);
            studyButton.onClick.Invoke();
            Assert.AreEqual(knowledgeAtLimit, player.GetKnowledge(Research.KnowledgeTracks.Fundamentos),
                "Uma ação sem tempo não pode alterar o estado");
            Assert.AreEqual(0, player.RemainingWorkCycles);
            StringAssert.Contains("Sem ciclos", GameObject.Find("StatusText").GetComponent<Text>().text);

            GameObject.Find("Button_Encerrar Dia").GetComponent<Button>().onClick.Invoke();
            Assert.AreEqual(2, player.CurrentDay);
            Assert.AreEqual(player.WorkCyclesPerDay, player.RemainingWorkCycles);

            studyButton.onClick.Invoke();
            Assert.Greater(player.GetKnowledge(Research.KnowledgeTracks.Fundamentos), knowledgeAtLimit);
        }

        [UnityTest]
        public IEnumerator NavBar_SwitchesToProductsScreen_AndShowsProductData()
        {
            yield return BuildAndStartFreshGame();

            // Precisa capturar a referência ANTES de trocar de tela: GameObject.Find
            // não encontra objetos inativos, e o Office vai ficar inativo depois do clique.
            var officeStatusGo = GameObject.Find("StatusText");
            Assert.IsNotNull(officeStatusGo, "StatusText não foi encontrado antes da troca de tela");

            var navButtonGo = GameObject.Find("Button_Produtos");
            Assert.IsNotNull(navButtonGo, "Button_Produtos não foi encontrado");
            navButtonGo.GetComponent<Button>().onClick.Invoke();
            yield return null;

            var productsTextGo = GameObject.Find("ProductsText");
            Assert.IsNotNull(productsTextGo);
            Assert.IsTrue(productsTextGo.activeInHierarchy, "A tela Products deveria estar visível depois do clique na navegação");

            var text = productsTextGo.GetComponent<Text>();
            StringAssert.Contains("PRODUTOS", text.text);
            StringAssert.Contains("Meu Primeiro Site", text.text);

            Assert.IsFalse(officeStatusGo.activeInHierarchy, "A tela Office deveria ter sido escondida");
        }

        [UnityTest]
        public IEnumerator HireButton_Click_AddsEmployee_OnEmployeesScreen()
        {
            yield return BuildAndStartFreshGame();

            GameObject.Find("Button_Equipe").GetComponent<Button>().onClick.Invoke();
            yield return null;

            var employeesBefore = GameRoot.Instance.State.Employees.Employees.Count;

            var hireButtonGo = GameObject.Find("Button_backend");
            Assert.IsNotNull(hireButtonGo, "Button_backend não foi encontrado");
            hireButtonGo.GetComponent<Button>().onClick.Invoke();

            var employeesAfter = GameRoot.Instance.State.Employees.Employees.Count;
            Assert.AreEqual(employeesBefore + 1, employeesAfter);
        }

        [UnityTest]
        public IEnumerator ProductLifecycle_BlocksEarlyLaunch_ThenRequiresTesting()
        {
            yield return BuildAndStartFreshGame();

            var product = GameRoot.Instance.State.Products[0];
            GameObject.Find("Button_Lançar Produto").GetComponent<Button>().onClick.Invoke();
            Assert.AreEqual(ProductStage.Planning, product.Stage,
                "O produto não pode ser lançado antes de desenvolver e testar");

            var developButton = GameObject.Find("Button_Desenvolver Produto").GetComponent<Button>();
            var endDayButton = GameObject.Find("Button_Encerrar Dia").GetComponent<Button>();
            for (var i = 0; i < 10; i++)
            {
                if (GameRoot.Instance.State.Player.RemainingWorkCycles == 0) endDayButton.onClick.Invoke();
                developButton.onClick.Invoke();
            }
            Assert.AreEqual(ProductStage.Testing, product.Stage);
            Assert.AreEqual(TutorialStep.TestProduct, GameRoot.Instance.State.TutorialProgress,
                "O tutorial deve reconhecer desenvolvimento feito fora da ordem sugerida");

            GameObject.Find("Button_Lançar Produto").GetComponent<Button>().onClick.Invoke();
            Assert.AreEqual(ProductStage.Testing, product.Stage,
                "Concluir o desenvolvimento não substitui uma rodada de testes");

            GameObject.Find("Button_Testar Produto").GetComponent<Button>().onClick.Invoke();
            Assert.IsTrue(product.HasBeenTested);
            Assert.AreEqual(product.KnownBugCount > 0 ? TutorialStep.FixKnownBugs : TutorialStep.LaunchProduct,
                GameRoot.Instance.State.TutorialProgress);

            GameObject.Find("Button_Lançar Produto").GetComponent<Button>().onClick.Invoke();
            Assert.AreEqual(ProductStage.Launched, product.Stage);
            Assert.AreEqual(TutorialStep.AcquireFirstCustomer, GameRoot.Instance.State.TutorialProgress,
                "Lançar com bugs conhecidos não pode prender o tutorial em Corrigir");
        }

        [UnityTest]
        public IEnumerator NewScreens_AreReachable_ViaNavBar()
        {
            yield return BuildAndStartFreshGame();

            GameObject.Find("Button_Mais").GetComponent<Button>().onClick.Invoke();
            yield return null;
            var moreMenu = GameObject.Find("MoreMenu");
            Assert.IsNotNull(moreMenu);
            Assert.IsTrue(moreMenu.activeInHierarchy);

            GameObject.Find("Button_Pesquisa").GetComponent<Button>().onClick.Invoke();
            yield return null;
            var researchText = GameObject.Find("ResearchText").GetComponent<Text>();
            StringAssert.Contains("CONHECIMENTO", researchText.text);

            GameObject.Find("Button_Empresa").GetComponent<Button>().onClick.Invoke();
            yield return null;
            var companyText = GameObject.Find("CompanyText").GetComponent<Text>();
            StringAssert.Contains("EMPRESA", companyText.text);
            StringAssert.Contains("CONCORRENTES", companyText.text);

            GameObject.Find("Button_Mais").GetComponent<Button>().onClick.Invoke();
            yield return null;
            GameObject.Find("Button_Perfil").GetComponent<Button>().onClick.Invoke();
            yield return null;
            var characterText = GameObject.Find("CharacterText").GetComponent<Text>();
            StringAssert.Contains("PERFIL", characterText.text);
        }

        [UnityTest]
        public IEnumerator EventModal_AppearsWhenEventTriggers_AndResolvingHidesIt()
        {
            yield return BuildAndStartFreshGame();

            // Os eventos do capítulo 1 só ficam elegíveis depois que existe um
            // produto lançado ou um cliente pagante.
            var product = GameRoot.Instance.State.Products[0];
            GameRoot.Instance.Development.Develop(product, GameRoot.Instance.State.Player,
                Research.KnowledgeTracks.Fundamentos, 10);
            GameRoot.Instance.Development.TestForBugs(product, 1);
            Assert.IsTrue(GameRoot.Instance.Development.Launch(product));

            // BaseTriggerChancePerCycle é 5% por ciclo; 300 ciclos deixa a chance de
            // nenhum evento disparar em torno de 0,95^300 ≈ 0,00002%.
            var triggered = false;
            for (var i = 0; i < 300 && !triggered; i++)
            {
                GameRoot.Instance.RunGameCycle(1);
                if (GameRoot.Instance.PendingEvent != null) triggered = true;
            }
            Assert.IsTrue(triggered, "Nenhum evento disparou em 300 ciclos — muito improvável, verificar EventService");

            var pendingEvent = GameRoot.Instance.PendingEvent;
            for (var i = 0; i < 300; i++) GameRoot.Instance.RunGameCycle(1);
            Assert.AreSame(pendingEvent, GameRoot.Instance.PendingEvent,
                "Um evento sem resposta não deve ser substituído por ciclos posteriores");

            yield return null;

            var modalGo = GameObject.Find("EventModal");
            Assert.IsNotNull(modalGo);
            Assert.IsTrue(modalGo.activeInHierarchy, "O modal deveria estar visível com um evento pendente");

            var titleGo = GameObject.Find("EventTitle");
            Assert.IsFalse(string.IsNullOrEmpty(titleGo.GetComponent<Text>().text));

            var firstChoiceLabel = GameRoot.Instance.PendingEvent.Choices[0].Label;
            var choiceButtonGo = GameObject.Find($"Button_{firstChoiceLabel}");
            Assert.IsNotNull(choiceButtonGo, $"Botão da escolha '{firstChoiceLabel}' não foi encontrado");
            choiceButtonGo.GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.IsNull(GameRoot.Instance.PendingEvent, "PendingEvent deveria ter sido limpo depois de resolver a escolha");
            Assert.IsFalse(modalGo.activeInHierarchy, "O modal deveria se esconder depois do evento resolvido");
        }
    }
}
