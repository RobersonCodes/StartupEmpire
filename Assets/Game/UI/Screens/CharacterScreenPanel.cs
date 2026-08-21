using System.Text;
using UnityEngine;
using UnityEngine.UI;
using StartupEmpire.Core;

namespace StartupEmpire.UI.Screens
{
    public sealed class CharacterScreenPanel : IScreenPanel
    {
        private Text _text;

        public void Build(Transform contentParent, ScreenManager screenManager)
        {
            var root = UiFactory.CreatePanel(contentParent, UiFactory.PanelBackground, "CharacterPanel");

            _text = UiFactory.CreateText(root.transform, "", 26, TextAnchor.UpperLeft,
                new Vector2(0.03f, 0.20f), new Vector2(0.97f, 0.97f), "CharacterText");

            UiFactory.CreateButton(root.transform, "Enviar Ranking", new Vector2(0.03f, 0.11f), new Vector2(0.97f, 0.18f), OnSubmitRanking);

            screenManager.Register("Character", root);
        }

        private static async void OnSubmitRanking()
        {
            await GameRoot.Instance.SubmitRankingAsync();
        }

        public void Refresh()
        {
            if (GameRoot.Instance == null) return;
            var player = GameRoot.Instance.State.Player;
            var sb = new StringBuilder();
            sb.AppendLine("PERFIL\n");
            sb.AppendLine($"Nome: {player.Name}");
            sb.AppendLine($"ID: {player.PlayerId}");
            sb.AppendLine($"Ciclos de trabalho por dia: {player.WorkCyclesPerDay}");
            _text.text = sb.ToString();
        }
    }
}
