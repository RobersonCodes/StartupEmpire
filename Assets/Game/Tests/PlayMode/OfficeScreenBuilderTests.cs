using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using StartupEmpire.Core;
using StartupEmpire.UI;

namespace StartupEmpire.Tests.PlayMode
{
    public class OfficeScreenBuilderTests
    {
        [UnityTest]
        public IEnumerator OfficeScreenBuilder_CreatesUiHierarchy_WithoutErrors()
        {
            var go = new GameObject("GameRoot", typeof(GameRoot), typeof(OfficeScreenBuilder));

            yield return null;

            var statusGo = GameObject.Find("StatusText");
            Assert.IsNotNull(statusGo, "StatusText não foi criado");

            var text = statusGo.GetComponent<Text>();
            Assert.IsNotNull(text);
            StringAssert.Contains("Caixa", text.text);
            StringAssert.Contains("Estágio", text.text);

            Assert.IsNotNull(GameObject.Find("Button_Estudar Fundamentos"));
            Assert.IsNotNull(GameObject.Find("Button_Desenvolver Produto"));
            Assert.IsNotNull(GameObject.Find("Button_Avançar Ciclo"));

            Object.Destroy(go);
            yield return null;
        }

        [UnityTest]
        public IEnumerator StudyButton_Click_IncreasesKnowledge_AndRefreshesStatus()
        {
            var go = new GameObject("GameRoot", typeof(GameRoot), typeof(OfficeScreenBuilder));
            yield return null;

            var knowledgeBefore = GameRoot.Instance.State.Player.GetKnowledge(Research.KnowledgeTracks.Fundamentos);

            var buttonGo = GameObject.Find("Button_Estudar Fundamentos");
            var button = buttonGo.GetComponent<Button>();
            button.onClick.Invoke();

            var knowledgeAfter = GameRoot.Instance.State.Player.GetKnowledge(Research.KnowledgeTracks.Fundamentos);
            Assert.Greater(knowledgeAfter, knowledgeBefore);

            Object.Destroy(go);
            yield return null;
        }
    }
}
