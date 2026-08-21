namespace StartupEmpire.Employees
{
    /// Vaga/perfil de contratação (seção 11 da missão). Instâncias em runtime são Employee.
    public sealed class EmployeeDefinition
    {
        public string Id { get; }
        public string DisplayName { get; }
        public EmployeeRole Role { get; }
        public double BaseSalary { get; }
        public double HiringCost { get; }

        public EmployeeDefinition(string id, string displayName, EmployeeRole role, double baseSalary, double hiringCost)
        {
            Id = id;
            DisplayName = displayName;
            Role = role;
            BaseSalary = baseSalary;
            HiringCost = hiringCost;
        }
    }
}
