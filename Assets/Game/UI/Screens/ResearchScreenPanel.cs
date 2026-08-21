using System.Text;
using UnityEngine;
using UnityEngine.UI;
using StartupEmpire.Core;
using StartupEmpire.Research;

namespace StartupEmpire.UI.Screens
{
    public sealed class ResearchScreenPanel : IScreenPanel
    {
        private static readonly string[] Tracks =
        {
            KnowledgeTracks.Fundamentos, KnowledgeTracks.Web, KnowledgeTracks.BancoDeDados,
            KnowledgeTracks.Backend, KnowledgeTracks.Frontend, KnowledgeTracks.Mobile,
            KnowledgeTracks.Cloud, KnowledgeTracks.DevOps, KnowledgeTracks.Ia,
            KnowledgeTracks.Automacao, KnowledgeTracks.Seguranca
        };

        private Text _text;
        private string _lastActionMessage;

        public void Build(Transform contentParent, ScreenManager screenManager)
        {
            var root = UiFactory.CreatePanel(contentParent, UiFactory.PanelBackground, "ResearchPanel");

            _text = UiFactory.CreateText(root.transform, "", 20, TextAnchor.UpperLeft,
                new Vector2(0.03f, 0.03f), new Vector2(0.97f, 0.47f), "ResearchText");

            const int columns = 3;
            var rows = Mathf.CeilToInt(Tracks.Length / (float)columns);
            const float areaTop = 0.97f;
            const float areaHeight = 0.45f;
            var cellWidth = 1f / columns;
            var cellHeight = areaHeight / rows;

            for (var i = 0; i < Tracks.Length; i++)
            {
                var track = Tracks[i];
                var col = i % columns;
                var row = i / columns;
                var minX = col * cellWidth + 0.01f;
                var maxX = (col + 1) * cellWidth - 0.01f;
                var maxY = areaTop - row * cellHeight;
                var minY = maxY - cellHeight + 0.01f;

                UiFactory.CreateButton(root.transform, track, new Vector2(minX, minY), new Vector2(maxX, maxY),
                    () =>
                    {
                        _lastActionMessage = GameRoot.Instance.StudyTrack(track, 1).Message;
                        Refresh();
                    });
            }

            screenManager.Register("Research", root);
        }

        public void Refresh()
        {
            if (GameRoot.Instance == null) return;
            var sb = new StringBuilder();
            sb.AppendLine("CONHECIMENTO\n");

            var player = GameRoot.Instance.State.Player;
            sb.AppendLine($"Dia {player.CurrentDay}   Tempo: {player.RemainingWorkCycles}/{player.WorkCyclesPerDay}");
            if (!string.IsNullOrEmpty(_lastActionMessage)) sb.AppendLine(_lastActionMessage);
            sb.AppendLine();
            foreach (var track in Tracks)
            {
                sb.AppendLine($"{track}: {player.GetKnowledge(track)}");
            }

            _text.text = sb.ToString();
        }
    }
}
