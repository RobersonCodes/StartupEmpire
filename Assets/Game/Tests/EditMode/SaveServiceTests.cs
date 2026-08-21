using System;
using NUnit.Framework;
using StartupEmpire.Competitors;
using StartupEmpire.Core;
using StartupEmpire.Economy;
using StartupEmpire.Employees;
using StartupEmpire.Products;
using StartupEmpire.Progression;
using StartupEmpire.Research;
using StartupEmpire.Save;

namespace StartupEmpire.Tests.EditMode
{
    public class SaveServiceTests
    {
        [Test]
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
            var state = new GameState(player, economy)
            {
                Stage = CompanyStage.Freelancer,
                TutorialProgress = TutorialStep.DevelopProduct
            };
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

            saveService.Save(state);
            var loaded = saveService.Load(startingCashIfNew: 0);

            Assert.AreEqual("player-abc123", loaded.Player.PlayerId);
            Assert.AreEqual("Ana", loaded.Player.Name);
            Assert.AreEqual(42, loaded.Player.GetKnowledge(KnowledgeTracks.Fundamentos));
            Assert.AreEqual(1, loaded.Player.CurrentDay);
            Assert.AreEqual(2, loaded.Player.RemainingWorkCycles);
            Assert.AreEqual(1234.5, loaded.Economy.Cash);
            Assert.AreEqual(CompanyStage.Freelancer, loaded.Stage);
            Assert.AreEqual(TutorialStep.DevelopProduct, loaded.TutorialProgress);
            Assert.AreEqual(1, loaded.Products.Count);
            Assert.AreEqual(ProductStage.Launched, loaded.Products[0].Stage);
            Assert.AreEqual(7, loaded.Products[0].PayingCustomers);
            Assert.AreEqual(1, loaded.Products[0].KnownBugCount);
            Assert.IsTrue(loaded.Products[0].HasBeenTested);
        }

        [Test]
        public void Load_CreatesNewGame_WhenNoSaveExists()
        {
            var storage = new InMemorySaveStorage();
            var catalog = ProductDefinitionCatalog.CreateChapter1Catalog();
            var employeeCatalog = EmployeeDefinitionCatalog.CreateDefaultCatalog();
            var competitorCatalog = CompetitorDefinitionCatalog.CreateChapter1Catalog();
            var saveService = new SaveService(storage, new FakeClock(DateTime.UtcNow), catalog, employeeCatalog, competitorCatalog);

            var state = saveService.Load(startingCashIfNew: 500);

            Assert.AreEqual(500, state.Economy.Cash);
            Assert.IsEmpty(state.Products);
        }

        [Test]
        public void Load_FallsBackToNewGame_WhenSaveIsCorrupted()
        {
            var storage = new InMemorySaveStorage();
            storage.WriteRaw("{ isso nao e json valido");
            var catalog = ProductDefinitionCatalog.CreateChapter1Catalog();
            var employeeCatalog = EmployeeDefinitionCatalog.CreateDefaultCatalog();
            var competitorCatalog = CompetitorDefinitionCatalog.CreateChapter1Catalog();
            var saveService = new SaveService(storage, new FakeClock(DateTime.UtcNow), catalog, employeeCatalog, competitorCatalog);

            var state = saveService.Load(startingCashIfNew: 500);

            Assert.AreEqual(500, state.Economy.Cash);
        }

        [Test]
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

            Assert.AreEqual("Recuperável", loaded.Player.Name);
            Assert.AreEqual(250, loaded.Economy.Cash);
            Assert.DoesNotThrow(() => SaveSerializer.Deserialize(storage.ReadRaw()),
                "O snapshot de backup também deve restaurar o arquivo principal");
        }

        [Test]
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
            Assert.IsTrue(storage.BackupExists());

            var fresh = saveService.DeleteSaveAndCreateNew(500);

            Assert.IsFalse(storage.Exists());
            Assert.IsFalse(storage.BackupExists());
            Assert.IsFalse(saveService.HasSave);
            Assert.AreEqual(500, fresh.Economy.Cash);
        }

        [Test]
        public void FileSaveStorage_RotatesAndRestoresPreviousSnapshot()
        {
            var storage = new FileSaveStorage($"save_recovery_test_{Guid.NewGuid():N}.json");
            try
            {
                storage.WriteRaw("primeiro");
                storage.WriteRaw("segundo");

                Assert.AreEqual("segundo", storage.ReadRaw());
                Assert.IsTrue(storage.BackupExists());
                Assert.AreEqual("primeiro", storage.ReadBackupRaw());

                storage.RestoreBackup();
                Assert.AreEqual("primeiro", storage.ReadRaw());
            }
            finally
            {
                storage.Delete();
            }
        }

        [Test]
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

            Assert.AreEqual(4, loaded.Products[0].KnownBugCount);
            Assert.IsTrue(loaded.Products[0].HasBeenTested);
            Assert.AreEqual(1, loaded.Player.CurrentDay);
            Assert.AreEqual(4, loaded.Player.RemainingWorkCycles);
            Assert.AreEqual(TutorialStep.LearnFundamentals, loaded.TutorialProgress);
        }

        [Test]
        public void Load_MigratesLaunchedLegacySave_ToCompletedTutorial()
        {
            var storage = new InMemorySaveStorage();
            var legacy = new SaveDataV1 { SchemaVersion = 3, Cash = 500 };
            legacy.Products.Add(new ProductSaveEntry
            {
                DefinitionId = "first_website",
                Stage = ProductStage.Launched.ToString(),
                HasBeenTested = true
            });
            storage.WriteRaw(SaveSerializer.Serialize(legacy));
            var saveService = new SaveService(storage, new FakeClock(DateTime.UtcNow),
                ProductDefinitionCatalog.CreateChapter1Catalog(),
                EmployeeDefinitionCatalog.CreateDefaultCatalog(),
                CompetitorDefinitionCatalog.CreateChapter1Catalog());

            var loaded = saveService.Load(startingCashIfNew: 0);

            Assert.AreEqual(TutorialStep.Completed, loaded.TutorialProgress);
        }
    }
}
