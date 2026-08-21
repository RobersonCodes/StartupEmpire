namespace StartupEmpire.Employees
{
    /// Instância em runtime de um funcionário contratado, com atributos configuráveis
    /// (salário vem de Definition; os demais evoluem com experiência e satisfação).
    public sealed class Employee
    {
        public string InstanceId { get; }
        public EmployeeDefinition Definition { get; }
        public double Experience { get; internal set; }
        public double Productivity { get; internal set; } = 1.0;
        public double Quality { get; internal set; } = 1.0;
        public double Speed { get; internal set; } = 1.0;
        public double Specialization { get; internal set; } = 0.5;
        public double Satisfaction { get; internal set; } = 1.0;

        public Employee(string instanceId, EmployeeDefinition definition)
        {
            InstanceId = instanceId;
            Definition = definition;
        }
    }
}
