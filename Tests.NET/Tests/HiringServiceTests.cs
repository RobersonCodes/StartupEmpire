using System;
using StartupEmpire.Core;
using StartupEmpire.Domain.Tests.TestSupport;
using StartupEmpire.Economy;
using StartupEmpire.Employees;
using Xunit;

namespace StartupEmpire.Domain.Tests
{
    public class HiringServiceTests
    {
        [Fact]
        public void TryHire_DeductsHiringCost_AndAddsEmployee()
        {
            var eventBus = new EventBus();
            EmployeeHiredEvent? received = null;
            eventBus.Subscribe<EmployeeHiredEvent>(e => received = e);
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var service = new HiringService(new HiringConfigValues(), economy, eventBus);
            var roster = new EmployeeRoster();
            var economyState = new EconomyState(100);
            var def = new EmployeeDefinition("backend_junior", "Backend Jr.", EmployeeRole.Backend, baseSalary: 50, hiringCost: 40);

            var employee = service.TryHire(roster, economyState, def);

            Assert.NotNull(employee);
            Assert.Equal(60, economyState.Cash);
            Assert.Single(roster.Employees);
            Assert.NotNull(received);
        }

        [Fact]
        public void TryHire_ReturnsNull_WhenInsufficientFunds()
        {
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var service = new HiringService(new HiringConfigValues(), economy, null);
            var roster = new EmployeeRoster();
            var economyState = new EconomyState(10);
            var def = new EmployeeDefinition("backend_junior", "Backend Jr.", EmployeeRole.Backend, baseSalary: 50, hiringCost: 40);

            var employee = service.TryHire(roster, economyState, def);

            Assert.Null(employee);
            Assert.Empty(roster.Employees);
        }

        [Fact]
        public void Fire_RemovesEmployee_AndPublishesEvent()
        {
            var eventBus = new EventBus();
            var fired = false;
            eventBus.Subscribe<EmployeeFiredEvent>(_ => fired = true);
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var service = new HiringService(new HiringConfigValues(), economy, eventBus);
            var roster = new EmployeeRoster();
            var def = new EmployeeDefinition("backend_junior", "Backend Jr.", EmployeeRole.Backend, baseSalary: 50, hiringCost: 40);
            var employee = service.TryHire(roster, new EconomyState(1000), def);

            var result = service.Fire(roster, employee.InstanceId);

            Assert.True(result);
            Assert.Empty(roster.Employees);
            Assert.True(fired);
        }

        [Fact]
        public void PaySalaries_IncreasesSatisfactionAndExperience_WhenAffordable()
        {
            var config = new HiringConfigValues { SatisfactionRecoveryWhenPaid = 0.1, ExperienceGainPerCycle = 0.05 };
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var service = new HiringService(config, economy, null);
            var roster = new EmployeeRoster();
            var def = new EmployeeDefinition("backend_junior", "Backend Jr.", EmployeeRole.Backend, baseSalary: 50, hiringCost: 0);
            var economyState = new EconomyState(1000);
            var employee = service.TryHire(roster, economyState, def);
            employee.Satisfaction = 0.5;

            var paid = service.PaySalaries(roster, economyState);

            Assert.True(paid);
            Assert.Equal(950, economyState.Cash);
            Assert.Equal(0.6, employee.Satisfaction, 5);
            Assert.Equal(0.05, employee.Experience, 5);
        }

        [Fact]
        public void PaySalaries_DecreasesSatisfaction_WhenCannotAfford()
        {
            var config = new HiringConfigValues { SatisfactionDecayWhenUnpaid = 0.3 };
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var service = new HiringService(config, economy, null);
            var roster = new EmployeeRoster();
            var def = new EmployeeDefinition("backend_junior", "Backend Jr.", EmployeeRole.Backend, baseSalary: 500, hiringCost: 0);
            var economyState = new EconomyState(10);
            var employee = service.TryHire(roster, economyState, def);
            employee.Satisfaction = 0.5;

            var paid = service.PaySalaries(roster, economyState);

            Assert.False(paid);
            Assert.Equal(10, economyState.Cash);
            Assert.Equal(0.2, employee.Satisfaction, 5);
        }

        [Fact]
        public void GetProductivityMultiplier_OnlyCountsMatchingRole()
        {
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var config = new HiringConfigValues { ProductivityContributionPerEmployee = 0.2 };
            var service = new HiringService(config, economy, null);
            var roster = new EmployeeRoster();
            var backendDef = new EmployeeDefinition("b", "B", EmployeeRole.Backend, 50, 0);
            var designDef = new EmployeeDefinition("d", "D", EmployeeRole.Design, 50, 0);
            roster.Employees.Add(new Employee("b0", backendDef) { Productivity = 1.0, Satisfaction = 1.0 });
            roster.Employees.Add(new Employee("d0", designDef) { Productivity = 1.0, Satisfaction = 1.0 });

            var multiplier = service.GetProductivityMultiplier(roster, EmployeeRole.Backend);

            Assert.Equal(1.2, multiplier, 5);
        }
    }
}
