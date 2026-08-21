using System.Text;
using UnityEngine;
using UnityEngine.UI;
using StartupEmpire.Achievements;
using StartupEmpire.Core;

namespace StartupEmpire.UI.Screens
{
    public sealed class AchievementsScreenPanel : IScreenPanel
    {
        private Text _text;

        public void Build(Transform contentParent, ScreenManager screenManager)
        {
            var root = UiFactory.CreatePanel(contentParent, UiFactory.PanelBackground, "AchievementsPanel");
            _text = UiFactory.CreateText(root.transform, "", 24, TextAnchor.UpperLeft,
                new Vector2(0.03f, 0.03f), new Vector2(0.97f, 0.97f), "AchievementsText");
            screenManager.Register("Achievements", root);
        }

        public void Refresh()
        {
            if (GameRoot.Instance == null) return;
            var sb = new StringBuilder();
            sb.AppendLine("CONQUISTAS\n");

            var unlocked = GameRoot.Instance.State.UnlockedAchievements;
            foreach (var definition in AchievementCatalog.Create())
            {
                var status = unlocked.Contains(definition.Id) ? "[X]" : "[ ]";
                sb.AppendLine($"{status} {definition.Title} — {definition.Description}");
            }

            _text.text = sb.ToString();
        }
    }
}
