using System.Text;
using UnityEngine;
using UnityEngine.UI;
using StartupEmpire.Core;
using StartupEmpire.Missions;

namespace StartupEmpire.UI.Screens
{
    public sealed class MissionsScreenPanel : IScreenPanel
    {
        private Text _text;

        public void Build(Transform contentParent, ScreenManager screenManager)
        {
            var root = UiFactory.CreatePanel(contentParent, UiFactory.PanelBackground, "MissionsPanel");
            _text = UiFactory.CreateText(root.transform, "", 24, TextAnchor.UpperLeft,
                new Vector2(0.03f, 0.03f), new Vector2(0.97f, 0.97f), "MissionsText");
            screenManager.Register("Missions", root);
        }

        public void Refresh()
        {
            if (GameRoot.Instance == null) return;
            var sb = new StringBuilder();
            sb.AppendLine("MISSÕES\n");

            var completed = GameRoot.Instance.State.Missions.CompletedMissionIds;
            foreach (var definition in Chapter1Missions.Create())
            {
                var status = completed.Contains(definition.Id) ? "[X]" : "[ ]";
                sb.AppendLine($"{status} {definition.Title} — {definition.Description}");
            }

            _text.text = sb.ToString();
        }
    }
}
