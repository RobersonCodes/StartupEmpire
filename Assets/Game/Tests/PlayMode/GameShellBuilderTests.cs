using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using StartupEmpire.Audio;
using StartupEmpire.Core;
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

        [UnityTest]
        public IEnumerator GameShellBuilder_CreatesOfficeScreen_WithoutErrors()
        {
            new GameObject("GameRoot", typeof(GameRoot), typeof(AudioManager), typeof(GameShellBuilder));

            yield return null;

            var statusGo = GameObject.Find("StatusText");
            Assert.IsNotNull(statusGo, "StatusText não foi criado");

            var text = statusGo.GetComponent<Text>();
            Assert.IsNotNull(text);
            StringAssert.Contains("Caixa", text.text);
            StringAssert.Contains("Estágio", text.text);

            Assert.IsNotNull(GameObject.Find("Button_Estudar Fundamentos"));
            Assert.IsNotNull(GameObject.Find("Button_Products"));
            Assert.IsNotNull(GameObject.Find("Button_Settings"));
        }

        [UnityTest]
        public IEnumerator StudyButton_Click_IncreasesKnowledge_AndRefreshesStatus()
        {
            new GameObject("GameRoot", typeof(GameRoot), typeof(AudioManager), typeof(GameShellBuilder));
            yield return null;

            var knowledgeBefore = GameRoot.Instance.State.Player.GetKnowledge(Research.KnowledgeTracks.Fundamentos);

            var buttonGo = GameObject.Find("Button_Estudar Fundamentos");
            Assert.IsNotNull(buttonGo, "Button_Estudar Fundamentos não foi encontrado");
            buttonGo.GetComponent<Button>().onClick.Invoke();

            var knowledgeAfter = GameRoot.Instance.State.Player.GetKnowledge(Research.KnowledgeTracks.Fundamentos);
            Assert.Greater(knowledgeAfter, knowledgeBefore);
        }

        [UnityTest]
        public IEnumerator NavBar_SwitchesToProductsScreen_AndShowsProductData()
        {
            new GameObject("GameRoot", typeof(GameRoot), typeof(AudioManager), typeof(GameShellBuilder));
            yield return null;

            // Precisa capturar a referência ANTES de trocar de tela: GameObject.Find
            // não encontra objetos inativos, e o Office vai ficar inativo depois do clique.
            var officeStatusGo = GameObject.Find("StatusText");
            Assert.IsNotNull(officeStatusGo, "StatusText não foi encontrado antes da troca de tela");

            var navButtonGo = GameObject.Find("Button_Products");
            Assert.IsNotNull(navButtonGo, "Button_Products não foi encontrado");
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
            new GameObject("GameRoot", typeof(GameRoot), typeof(AudioManager), typeof(GameShellBuilder));
            yield return null;

            GameObject.Find("Button_Employees").GetComponent<Button>().onClick.Invoke();
            yield return null;

            var employeesBefore = GameRoot.Instance.State.Employees.Employees.Count;

            var hireButtonGo = GameObject.Find("Button_backend");
            Assert.IsNotNull(hireButtonGo, "Button_backend não foi encontrado");
            hireButtonGo.GetComponent<Button>().onClick.Invoke();

            var employeesAfter = GameRoot.Instance.State.Employees.Employees.Count;
            Assert.AreEqual(employeesBefore + 1, employeesAfter);
        }
    }
}
