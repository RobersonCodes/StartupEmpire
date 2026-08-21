using System;
using StartupEmpire.Core;
using StartupEmpire.Domain.Tests.TestSupport;
using StartupEmpire.Economy;
using StartupEmpire.Employees;
using StartupEmpire.Products;
using StartupEmpire.Progression;
using StartupEmpire.Research;
using StartupEmpire.Save;
using Xunit;

namespace StartupEmpire.Domain.Tests
{
    public class SaveServiceTests
    {
        [Fact]
        public void SaveThenLoad_RoundTripsGameState()
        {
            var clock = new FakeClock(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            var storage = new InMemorySaveStorage();
            var catalog = ProductDefinitionCatalog.CreateChapter1Catalog();
            var employeeCatalog = EmployeeDefinitionCatalog.CreateDefaultCatalog();
            var saveService = new SaveService(storage, clock, catalog, employeeCatalog);

            var player = new PlayerState { Name = "Ana" };
            player.AddKnowledge(KnowledgeTracks.Fundamentos, 42);
            var economy = new EconomyState(1234.5) { MonthlyRecurringRevenue = 99, Valuation = 5000 };
            var state = new GameState(player, economy) { Stage = CompanyStage.Freelancer };
            var def = catalog.Find("first_website");
            var product = new ProductState(def) { Stage = ProductStage.Launched, DevProgress = 100, BugCount = 2, PayingCustomers = 7 };
            state.Products.Add(product);
            state.Missions.CompletedMissionIds.Add("hello_world");
            state.UnlockedAchievements.Add("hello_world");
            state.Upgrades.SetLevel("better_computer", 3);
            state.Employees.Employees.Add(new Employee("backend_junior_0", employeeCatalog.Find("backend_junior"))
            {
                Experience = 0.5,
                Productivity = 1.5,
                Satisfaction = 0.8
            });

            saveService.Save(state);
            var loaded = saveService.Load(startingCashIfNew: 0);

            Assert.Equal("Ana", loaded.Player.Name);
            Assert.Equal(42, loaded.Player.GetKnowledge(KnowledgeTracks.Fundamentos));
            Assert.Equal(1234.5, loaded.Economy.Cash);
            Assert.Equal(CompanyStage.Freelancer, loaded.Stage);
            Assert.Single(loaded.Products);
            Assert.Equal(ProductStage.Launched, loaded.Products[0].Stage);
            Assert.Equal(7, loaded.Products[0].PayingCustomers);
            Assert.Contains("hello_world", loaded.Missions.CompletedMissionIds);
            Assert.Contains("hello_world", loaded.UnlockedAchievements);
            Assert.Equal(3, loaded.Upgrades.GetLevel("better_computer"));
            Assert.Single(loaded.Employees.Employees);
            Assert.Equal("backend_junior", loaded.Employees.Employees[0].Definition.Id);
            Assert.Equal(1.5, loaded.Employees.Employees[0].Productivity);
        }

        [Fact]
        public void Load_CreatesNewGame_WhenNoSaveExists()
        {
            var storage = new InMemorySaveStorage();
            var catalog = ProductDefinitionCatalog.CreateChapter1Catalog();
            var employeeCatalog = EmployeeDefinitionCatalog.CreateDefaultCatalog();
            var saveService = new SaveService(storage, new FakeClock(DateTime.UtcNow), catalog, employeeCatalog);

            var state = saveService.Load(startingCashIfNew: 500);

            Assert.Equal(500, state.Economy.Cash);
            Assert.Empty(state.Products);
        }

        [Fact]
        public void Load_FallsBackToNewGame_WhenSaveIsCorrupted()
        {
            var storage = new InMemorySaveStorage();
            storage.WriteRaw("{ isso nao e json valido");
            var catalog = ProductDefinitionCatalog.CreateChapter1Catalog();
            var employeeCatalog = EmployeeDefinitionCatalog.CreateDefaultCatalog();
            var saveService = new SaveService(storage, new FakeClock(DateTime.UtcNow), catalog, employeeCatalog);

            var state = saveService.Load(startingCashIfNew: 500);

            Assert.Equal(500, state.Economy.Cash);
        }

        [Fact]
        public void Load_IgnoresOrphanProduct_WhenDefinitionNoLongerExists()
        {
            var clock = new FakeClock(DateTime.UtcNow);
            var storage = new InMemorySaveStorage();
            var catalog = ProductDefinitionCatalog.CreateChapter1Catalog();
            var employeeCatalog = EmployeeDefinitionCatalog.CreateDefaultCatalog();
            var saveService = new SaveService(storage, clock, catalog, employeeCatalog);
            var state = new GameState(new PlayerState(), new EconomyState(0));
            var def = catalog.Find("first_website");
            state.Products.Add(new ProductState(def));
            saveService.Save(state);

            var newCatalog = new ProductDefinitionCatalog();
            var saveServiceWithNewCatalog = new SaveService(storage, clock, newCatalog, employeeCatalog);

            var loaded = saveServiceWithNewCatalog.Load(startingCashIfNew: 0);

            Assert.Empty(loaded.Products);
        }

        [Fact]
        public void Load_IgnoresOrphanEmployee_WhenDefinitionNoLongerExists()
        {
            var clock = new FakeClock(DateTime.UtcNow);
            var storage = new InMemorySaveStorage();
            var catalog = ProductDefinitionCatalog.CreateChapter1Catalog();
            var employeeCatalog = EmployeeDefinitionCatalog.CreateDefaultCatalog();
            var saveService = new SaveService(storage, clock, catalog, employeeCatalog);
            var state = new GameState(new PlayerState(), new EconomyState(0));
            state.Employees.Employees.Add(new Employee("backend_junior_0", employeeCatalog.Find("backend_junior")));
            saveService.Save(state);

            var newEmployeeCatalog = new EmployeeDefinitionCatalog();
            var saveServiceWithNewCatalog = new SaveService(storage, clock, catalog, newEmployeeCatalog);

            var loaded = saveServiceWithNewCatalog.Load(startingCashIfNew: 0);

            Assert.Empty(loaded.Employees.Employees);
        }
    }
}
