using System.Text;
using UnityEngine;
using UnityEngine.UI;
using StartupEmpire.Core;

namespace StartupEmpire.UI.Screens
{
    public sealed class EmployeesScreenPanel : IScreenPanel
    {
        private static readonly string[] HireableRoles =
        {
            "backend_junior", "frontend_junior", "mobile_junior", "designer_junior", "qa_junior"
        };

        private Text _text;

        public void Build(Transform contentParent, ScreenManager screenManager)
        {
            var root = UiFactory.CreatePanel(contentParent, UiFactory.PanelBackground, "EmployeesPanel");

            _text = UiFactory.CreateText(root.transform, "", 24, TextAnchor.UpperLeft,
                new Vector2(0.03f, 0.32f), new Vector2(0.97f, 0.97f), "EmployeesText");

            var x = 0.02f;
            const float width = 0.192f;
            foreach (var roleId in HireableRoles)
            {
                var capturedId = roleId;
                UiFactory.CreateButton(root.transform, ShortLabel(roleId), new Vector2(x, 0.22f), new Vector2(x + width, 0.29f),
                    () => { GameRoot.Instance.HireEmployee(capturedId); Refresh(); });
                x += width + 0.006f;
            }

            screenManager.Register("Employees", root);
        }

        private static string ShortLabel(string id) => id.Replace("_junior", "").Replace("_", " ");

        public void Refresh()
        {
            if (GameRoot.Instance == null) return;
            var sb = new StringBuilder();
            sb.AppendLine("EQUIPE\n");

            var roster = GameRoot.Instance.State.Employees;
            if (roster.Employees.Count == 0)
            {
                sb.AppendLine("Nenhum funcionário contratado ainda.\n");
            }

            foreach (var employee in roster.Employees)
            {
                sb.AppendLine($"{employee.Definition.DisplayName} ({employee.Definition.Role})");
                sb.AppendLine($"  Salário: R$ {employee.Definition.BaseSalary:F0}   Produtividade: {employee.Productivity:F2}   Satisfação: {employee.Satisfaction:P0}\n");
            }

            sb.AppendLine($"\nFolha mensal: R$ {roster.TotalMonthlySalary():F2}");
            _text.text = sb.ToString();
        }
    }
}
