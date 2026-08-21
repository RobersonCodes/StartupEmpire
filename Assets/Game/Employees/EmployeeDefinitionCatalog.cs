using System.Collections.Generic;

namespace StartupEmpire.Employees
{
    public sealed class EmployeeDefinitionCatalog
    {
        private readonly Dictionary<string, EmployeeDefinition> _byId = new();

        public void Register(EmployeeDefinition definition) => _byId[definition.Id] = definition;

        public EmployeeDefinition Find(string id) => _byId.TryGetValue(id, out var def) ? def : null;

        public static EmployeeDefinitionCatalog CreateDefaultCatalog()
        {
            var catalog = new EmployeeDefinitionCatalog();
            catalog.Register(new EmployeeDefinition("backend_junior", "Desenvolvedor(a) Backend Jr.", EmployeeRole.Backend, baseSalary: 120, hiringCost: 50));
            catalog.Register(new EmployeeDefinition("frontend_junior", "Desenvolvedor(a) Frontend Jr.", EmployeeRole.Frontend, baseSalary: 110, hiringCost: 50));
            catalog.Register(new EmployeeDefinition("mobile_junior", "Desenvolvedor(a) Mobile Jr.", EmployeeRole.Mobile, baseSalary: 130, hiringCost: 60));
            catalog.Register(new EmployeeDefinition("designer_junior", "Designer Jr.", EmployeeRole.Design, baseSalary: 100, hiringCost: 40));
            catalog.Register(new EmployeeDefinition("qa_junior", "QA Jr.", EmployeeRole.QA, baseSalary: 90, hiringCost: 40));
            catalog.Register(new EmployeeDefinition("devops_junior", "DevOps Jr.", EmployeeRole.DevOps, baseSalary: 140, hiringCost: 70));
            catalog.Register(new EmployeeDefinition("pm_junior", "Product Manager Jr.", EmployeeRole.ProductManager, baseSalary: 120, hiringCost: 50));
            catalog.Register(new EmployeeDefinition("marketing_junior", "Marketing Jr.", EmployeeRole.Marketing, baseSalary: 100, hiringCost: 40));
            catalog.Register(new EmployeeDefinition("sales_junior", "Vendas Jr.", EmployeeRole.Sales, baseSalary: 100, hiringCost: 40));
            catalog.Register(new EmployeeDefinition("support_junior", "Suporte Jr.", EmployeeRole.Support, baseSalary: 90, hiringCost: 30));
            return catalog;
        }
    }
}
