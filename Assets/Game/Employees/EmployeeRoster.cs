using System.Collections.Generic;

namespace StartupEmpire.Employees
{
    public sealed class EmployeeRoster
    {
        public List<Employee> Employees { get; } = new();

        public double TotalMonthlySalary()
        {
            double total = 0;
            foreach (var employee in Employees) total += employee.Definition.BaseSalary;
            return total;
        }
    }
}
