using System;
using StartupEmpire.Core;
using StartupEmpire.Economy;

namespace StartupEmpire.Employees
{
    /// Contratação, demissão, folha de pagamento e produtividade agregada por cargo
    /// (seção 11 da missão). Simulação deliberadamente simples no MVP — multiplicadores
    /// lineares, sem simular relações interpessoais ou hierarquia.
    public sealed class HiringService
    {
        private readonly HiringConfigValues _config;
        private readonly EconomyEngine _economy;
        private readonly EventBus _eventBus;
        private int _nextInstanceId;

        public HiringService(HiringConfigValues config, EconomyEngine economy, EventBus eventBus)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _economy = economy ?? throw new ArgumentNullException(nameof(economy));
            _eventBus = eventBus;
        }

        public Employee TryHire(EmployeeRoster roster, EconomyState economyState, EmployeeDefinition definition)
        {
            if (!_economy.TrySpend(economyState, definition.HiringCost, LedgerCategory.Equipment, $"hire:{definition.Id}"))
                return null;

            var employee = new Employee($"{definition.Id}_{_nextInstanceId++}", definition);
            roster.Employees.Add(employee);
            _eventBus?.Publish(new EmployeeHiredEvent(employee.InstanceId, definition.Id));
            return employee;
        }

        public bool Fire(EmployeeRoster roster, string employeeInstanceId)
        {
            var employee = roster.Employees.Find(e => e.InstanceId == employeeInstanceId);
            if (employee == null) return false;

            roster.Employees.Remove(employee);
            _eventBus?.Publish(new EmployeeFiredEvent(employeeInstanceId));
            return true;
        }

        /// Paga a folha do ciclo. Se o caixa não cobrir tudo, ninguém é pago
        /// parcialmente (evita estado inconsistente) e a satisfação de todos cai.
        public bool PaySalaries(EmployeeRoster roster, EconomyState economyState)
        {
            var total = roster.TotalMonthlySalary();
            var paid = total <= 0 || _economy.TrySpend(economyState, total, LedgerCategory.Salary, "salaries");

            foreach (var employee in roster.Employees)
            {
                if (paid)
                {
                    employee.Satisfaction = Math.Clamp(employee.Satisfaction + _config.SatisfactionRecoveryWhenPaid, 0, 1);
                    employee.Experience += _config.ExperienceGainPerCycle;
                    employee.Productivity = 1.0 + employee.Experience;
                }
                else
                {
                    employee.Satisfaction = Math.Clamp(employee.Satisfaction - _config.SatisfactionDecayWhenUnpaid, 0, 1);
                }
            }

            return paid;
        }

        /// Multiplicador de produtividade agregado que um cargo específico contribui
        /// para o resto do domínio (ex.: Backend/Frontend/Mobile aceleram DevelopmentService).
        public double GetProductivityMultiplier(EmployeeRoster roster, EmployeeRole role)
        {
            double bonus = 0;
            foreach (var employee in roster.Employees)
            {
                if (employee.Definition.Role != role) continue;
                bonus += employee.Productivity * _config.ProductivityContributionPerEmployee * employee.Satisfaction;
            }
            return 1.0 + bonus;
        }
    }
}
