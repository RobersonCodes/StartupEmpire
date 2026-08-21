using System;
using StartupEmpire.Competitors;
using StartupEmpire.Core;
using StartupEmpire.Domain.Tests.TestSupport;
using StartupEmpire.Economy;
using StartupEmpire.Employees;
using StartupEmpire.Investment;
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
            var competitorCatalog = CompetitorDefinitionCatalog.CreateChapter1Catalog();
            var saveService = new SaveService(storage, clock, catalog, employeeCatalog, competitorCatalog);

            var player = new PlayerState { PlayerId = "player-abc123", Name = "Ana" };
            player.AddKnowledge(KnowledgeTracks.Fundamentos, 42);
            player.TryConsumeWorkCycles(2);
            var economy = new EconomyState(1234.5) { MonthlyRecurringRevenue = 99, Valuation = 5000 };
            var state = new GameState(player, economy) { Stage = CompanyStage.Freelancer };
            var def = catalog.Find("first_website");
            var product = new ProductState(def)
            {
                Stage = ProductStage.Launched,
                DevProgress = 100,
                BugCount = 2,
                KnownBugCount = 1,
                HasBeenTested = true,
                PayingCustomers = 7
            };
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
            state.Competitors.Add(new CompetitorState(competitorCatalog.Find("rival_startup")) { Users = 999 });
            state.RaisedInvestmentRounds.Add(InvestmentRoundType.Angel);

            saveService.Save(state);
            var loaded = saveService.Load(startingCashIfNew: 0);

            Assert.Equal("player-abc123", loaded.Player.PlayerId);
            Assert.Equal("Ana", loaded.Player.Name);
            Assert.Equal(42, loaded.Player.GetKnowledge(KnowledgeTracks.Fundamentos));
            Assert.Equal(1, loaded.Player.CurrentDay);
            Assert.Equal(2, loaded.Player.RemainingWorkCycles);
            Assert.Equal(1234.5, loaded.Economy.Cash);
            Assert.Equal(CompanyStage.Freelancer, loaded.Stage);
            Assert.Single(loaded.Products);
            Assert.Equal(ProductStage.Launched, loaded.Products[0].Stage);
            Assert.Equal(7, loaded.Products[0].PayingCustomers);
            Assert.Equal(1, loaded.Products[0].KnownBugCount);
            Assert.True(loaded.Products[0].HasBeenTested);
            Assert.Contains("hello_world", loaded.Missions.CompletedMissionIds);
            Assert.Contains("hello_world", loaded.UnlockedAchievements);
            Assert.Equal(3, loaded.Upgrades.GetLevel("better_computer"));
            Assert.Single(loaded.Employees.Employees);
            Assert.Equal("backend_junior", loaded.Employees.Employees[0].Definition.Id);
            Assert.Equal(1.5, loaded.Employees.Employees[0].Productivity);
            Assert.Single(loaded.Competitors);
            Assert.Equal("rival_startup", loaded.Competitors[0].Definition.Id);
            Assert.Equal(999, loaded.Competitors[0].Users);
            Assert.Contains(InvestmentRoundType.Angel, loaded.RaisedInvestmentRounds);
        }

        [Fact]
        public void Load_CreatesNewGame_WhenNoSaveExists()
        {
            var storage = new InMemorySaveStorage();
            var catalog = ProductDefinitionCatalog.CreateChapter1Catalog();
            var employeeCatalog = EmployeeDefinitionCatalog.CreateDefaultCatalog();
            var competitorCatalog = CompetitorDefinitionCatalog.CreateChapter1Catalog();
            var saveService = new SaveService(storage, new FakeClock(DateTime.UtcNow), catalog, employeeCatalog, competitorCatalog);

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
            var competitorCatalog = CompetitorDefinitionCatalog.CreateChapter1Catalog();
            var saveService = new SaveService(storage, new FakeClock(DateTime.UtcNow), catalog, employeeCatalog, competitorCatalog);

            var state = saveService.Load(startingCashIfNew: 500);

            Assert.Equal(500, state.Economy.Cash);
        }

        [Fact]
        public void Load_RecoversPreviousSnapshot_WhenPrimarySaveIsCorrupted()
        {
            var storage = new InMemorySaveStorage();
            var saveService = new SaveService(storage, new FakeClock(DateTime.UtcNow),
                ProductDefinitionCatalog.CreateChapter1Catalog(),
                EmployeeDefinitionCatalog.CreateDefaultCatalog(),
                CompetitorDefinitionCatalog.CreateChapter1Catalog());

            saveService.Save(new GameState(new PlayerState { Name = "Primeiro" }, new EconomyState(100)));
            saveService.Save(new GameState(new PlayerState { Name = "Recuperável" }, new EconomyState(250)));
            storage.WriteRaw("{ save principal corrompido");

            var loaded = saveService.Load(startingCashIfNew: 500);

            Assert.Equal("Recuperável", loaded.Player.Name);
            Assert.Equal(250, loaded.Economy.Cash);
            Assert.NotNull(SaveSerializer.Deserialize(storage.ReadRaw()));
        }

        [Fact]
        public void DeleteSaveAndCreateNew_RemovesPrimaryAndBackup()
        {
            var storage = new InMemorySaveStorage();
            var saveService = new SaveService(storage, new FakeClock(DateTime.UtcNow),
                ProductDefinitionCatalog.CreateChapter1Catalog(),
                EmployeeDefinitionCatalog.CreateDefaultCatalog(),
                CompetitorDefinitionCatalog.CreateChapter1Catalog());
            var state = new GameState(new PlayerState(), new EconomyState(100));
            saveService.Save(state);
            saveService.Save(state);
            Assert.True(storage.BackupExists());

            var fresh = saveService.DeleteSaveAndCreateNew(500);

            Assert.False(storage.Exists());
            Assert.False(storage.BackupExists());
            Assert.False(saveService.HasSave);
            Assert.Equal(500, fresh.Economy.Cash);
        }

        [Fact]
        public void Load_MigratesV1ProductTestingState_WithoutLosingKnownBugs()
        {
            var storage = new InMemorySaveStorage();
            var legacy = new SaveDataV1 { SchemaVersion = 1, Cash = 500 };
            legacy.Products.Add(new ProductSaveEntry
            {
                DefinitionId = "first_website",
                Stage = ProductStage.Testing.ToString(),
                BugCount = 4
            });
            storage.WriteRaw(SaveSerializer.Serialize(legacy));
            var saveService = new SaveService(storage, new FakeClock(DateTime.UtcNow),
                ProductDefinitionCatalog.CreateChapter1Catalog(),
                EmployeeDefinitionCatalog.CreateDefaultCatalog(),
                CompetitorDefinitionCatalog.CreateChapter1Catalog());

            var loaded = saveService.Load(startingCashIfNew: 0);

            Assert.Equal(4, loaded.Products[0].KnownBugCount);
            Assert.True(loaded.Products[0].HasBeenTested);
            Assert.Equal(1, loaded.Player.CurrentDay);
            Assert.Equal(4, loaded.Player.RemainingWorkCycles);
        }

        [Fact]
        public void Load_IgnoresOrphanProduct_WhenDefinitionNoLongerExists()
        {
            var clock = new FakeClock(DateTime.UtcNow);
            var storage = new InMemorySaveStorage();
            var catalog = ProductDefinitionCatalog.CreateChapter1Catalog();
            var employeeCatalog = EmployeeDefinitionCatalog.CreateDefaultCatalog();
            var competitorCatalog = CompetitorDefinitionCatalog.CreateChapter1Catalog();
            var saveService = new SaveService(storage, clock, catalog, employeeCatalog, competitorCatalog);
            var state = new GameState(new PlayerState(), new EconomyState(0));
            var def = catalog.Find("first_website");
            state.Products.Add(new ProductState(def));
            saveService.Save(state);

            var newCatalog = new ProductDefinitionCatalog();
            var saveServiceWithNewCatalog = new SaveService(storage, clock, newCatalog, employeeCatalog, competitorCatalog);

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
            var competitorCatalog = CompetitorDefinitionCatalog.CreateChapter1Catalog();
            var saveService = new SaveService(storage, clock, catalog, employeeCatalog, competitorCatalog);
            var state = new GameState(new PlayerState(), new EconomyState(0));
            state.Employees.Employees.Add(new Employee("backend_junior_0", employeeCatalog.Find("backend_junior")));
            saveService.Save(state);

            var newEmployeeCatalog = new EmployeeDefinitionCatalog();
            var saveServiceWithNewCatalog = new SaveService(storage, clock, catalog, newEmployeeCatalog, competitorCatalog);

            var loaded = saveServiceWithNewCatalog.Load(startingCashIfNew: 0);

            Assert.Empty(loaded.Employees.Employees);
        }

        [Fact]
        public void Load_IgnoresOrphanCompetitor_WhenDefinitionNoLongerExists()
        {
            var clock = new FakeClock(DateTime.UtcNow);
            var storage = new InMemorySaveStorage();
            var catalog = ProductDefinitionCatalog.CreateChapter1Catalog();
            var employeeCatalog = EmployeeDefinitionCatalog.CreateDefaultCatalog();
            var competitorCatalog = CompetitorDefinitionCatalog.CreateChapter1Catalog();
            var saveService = new SaveService(storage, clock, catalog, employeeCatalog, competitorCatalog);
            var state = new GameState(new PlayerState(), new EconomyState(0));
            state.Competitors.Add(new CompetitorState(competitorCatalog.Find("rival_startup")));
            saveService.Save(state);

            var newCompetitorCatalog = new CompetitorDefinitionCatalog();
            var saveServiceWithNewCatalog = new SaveService(storage, clock, catalog, employeeCatalog, newCompetitorCatalog);

            var loaded = saveServiceWithNewCatalog.Load(startingCashIfNew: 0);

            Assert.Empty(loaded.Competitors);
        }
    }
}
