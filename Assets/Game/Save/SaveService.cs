using System;
using System.Globalization;
using StartupEmpire.Competitors;
using StartupEmpire.Core;
using StartupEmpire.Economy;
using StartupEmpire.Employees;
using StartupEmpire.Investment;
using StartupEmpire.Premium;
using StartupEmpire.Products;
using StartupEmpire.Progression;
using StartupEmpire.Store;

namespace StartupEmpire.Save
{
    /// Converte GameState <-> SaveDataV1 e orquestra leitura/escrita via ISaveStorage.
    /// Nunca perde o progresso inteiro por um save corrompido ou campo ausente:
    /// falhas de parsing caem em um novo jogo em vez de derrubar a aplicação, e
    /// produtos/funcionários cuja definição não existe mais são ignorados com segurança.
    public sealed class SaveService
    {
        private readonly ISaveStorage _storage;
        private readonly IClock _clock;
        private readonly ProductDefinitionCatalog _productCatalog;
        private readonly EmployeeDefinitionCatalog _employeeCatalog;
        private readonly CompetitorDefinitionCatalog _competitorCatalog;

        public SaveService(ISaveStorage storage, IClock clock, ProductDefinitionCatalog productCatalog,
            EmployeeDefinitionCatalog employeeCatalog, CompetitorDefinitionCatalog competitorCatalog)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _productCatalog = productCatalog ?? throw new ArgumentNullException(nameof(productCatalog));
            _employeeCatalog = employeeCatalog ?? throw new ArgumentNullException(nameof(employeeCatalog));
            _competitorCatalog = competitorCatalog ?? throw new ArgumentNullException(nameof(competitorCatalog));
        }

        public void Save(GameState state)
        {
            var data = new SaveDataV1
            {
                SchemaVersion = SaveMigrator.CurrentSchemaVersion,
                PlayerId = state.Player.PlayerId,
                PlayerName = state.Player.Name,
                Cash = state.Economy.Cash,
                MonthlyRecurringRevenue = state.Economy.MonthlyRecurringRevenue,
                Valuation = state.Economy.Valuation,
                FounderEquity = state.Economy.FounderEquity,
                CompanyStage = state.Stage.ToString(),
                LastSavedUtcIso = _clock.UtcNow.ToString("O", CultureInfo.InvariantCulture)
            };

            foreach (var kv in state.Player.KnowledgeByTrack)
            {
                data.Knowledge.Add(new KnowledgeEntry { Track = kv.Key, Amount = kv.Value });
            }

            foreach (var product in state.Products)
            {
                data.Products.Add(new ProductSaveEntry
                {
                    DefinitionId = product.Definition.Id,
                    Stage = product.Stage.ToString(),
                    DevProgress = product.DevProgress,
                    Quality = product.Quality,
                    Stability = product.Stability,
                    BugCount = product.BugCount,
                    Performance = product.Performance,
                    Security = product.Security,
                    Popularity = product.Popularity,
                    Users = product.Users,
                    PayingCustomers = product.PayingCustomers,
                    Price = product.Price,
                    Reputation = product.Reputation
                });
            }

            foreach (var id in state.Missions.CompletedMissionIds) data.CompletedMissionIds.Add(id);
            foreach (var id in state.UnlockedAchievements) data.UnlockedAchievementIds.Add(id);

            foreach (var kv in state.Upgrades.LevelByUpgradeId)
            {
                data.UpgradeLevels.Add(new UpgradeLevelEntry { UpgradeId = kv.Key, Level = kv.Value });
            }

            foreach (var employee in state.Employees.Employees)
            {
                data.Employees.Add(new EmployeeSaveEntry
                {
                    InstanceId = employee.InstanceId,
                    DefinitionId = employee.Definition.Id,
                    Experience = employee.Experience,
                    Productivity = employee.Productivity,
                    Quality = employee.Quality,
                    Speed = employee.Speed,
                    Specialization = employee.Specialization,
                    Satisfaction = employee.Satisfaction
                });
            }

            foreach (var competitor in state.Competitors)
            {
                data.Competitors.Add(new CompetitorSaveEntry
                {
                    DefinitionId = competitor.Definition.Id,
                    Users = competitor.Users,
                    Valuation = competitor.Valuation,
                    Reputation = competitor.Reputation,
                    Quality = competitor.Quality,
                    MarketShare = competitor.MarketShare
                });
            }

            foreach (var roundType in state.RaisedInvestmentRounds)
            {
                data.RaisedInvestmentRounds.Add(roundType.ToString());
            }

            data.GemBalance = state.GemWallet.Balance;

            foreach (var boost in state.Store.ActiveBoosts)
            {
                data.ActiveBoosts.Add(new ActiveBoostSaveEntry
                {
                    SourceItemId = boost.SourceItemId,
                    EffectType = boost.EffectType.ToString(),
                    Magnitude = boost.Magnitude,
                    RemainingCycles = boost.RemainingCycles
                });
            }

            foreach (var cosmeticId in state.Store.PurchasedCosmeticIds) data.PurchasedCosmeticIds.Add(cosmeticId);

            _storage.WriteRaw(SaveSerializer.Serialize(data));
        }

        public GameState Load(double startingCashIfNew)
        {
            if (!_storage.Exists())
            {
                return CreateNewGame(startingCashIfNew);
            }

            SaveDataV1 data;
            try
            {
                data = SaveSerializer.Deserialize(_storage.ReadRaw());
            }
            catch
            {
                return CreateNewGame(startingCashIfNew);
            }

            if (data == null) return CreateNewGame(startingCashIfNew);
            data = SaveMigrator.MigrateToCurrent(data);

            var player = new PlayerState { PlayerId = data.PlayerId, Name = data.PlayerName };
            foreach (var knowledge in data.Knowledge) player.AddKnowledge(knowledge.Track, knowledge.Amount);

            var economy = new EconomyState(data.Cash)
            {
                MonthlyRecurringRevenue = data.MonthlyRecurringRevenue,
                Valuation = data.Valuation,
                FounderEquity = data.FounderEquity
            };

            var state = new GameState(player, economy)
            {
                LastSavedUtc = DateTime.TryParse(data.LastSavedUtcIso, CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind, out var parsed)
                    ? parsed
                    : _clock.UtcNow
            };

            if (Enum.TryParse<CompanyStage>(data.CompanyStage, out var stage)) state.Stage = stage;

            foreach (var entry in data.Products)
            {
                var definition = _productCatalog.Find(entry.DefinitionId);
                if (definition == null) continue;

                var product = new ProductState(definition)
                {
                    Stage = Enum.TryParse<ProductStage>(entry.Stage, out var productStage)
                        ? productStage
                        : ProductStage.Planning,
                    DevProgress = entry.DevProgress,
                    Quality = entry.Quality,
                    Stability = entry.Stability,
                    BugCount = entry.BugCount,
                    Performance = entry.Performance,
                    Security = entry.Security,
                    Popularity = entry.Popularity,
                    Users = entry.Users,
                    PayingCustomers = entry.PayingCustomers,
                    Price = entry.Price,
                    Reputation = entry.Reputation
                };
                state.Products.Add(product);
            }

            foreach (var id in data.CompletedMissionIds) state.Missions.CompletedMissionIds.Add(id);
            foreach (var id in data.UnlockedAchievementIds) state.UnlockedAchievements.Add(id);

            foreach (var entry in data.UpgradeLevels) state.Upgrades.SetLevel(entry.UpgradeId, entry.Level);

            foreach (var entry in data.Employees)
            {
                var definition = _employeeCatalog.Find(entry.DefinitionId);
                if (definition == null) continue;

                var employee = new Employee(entry.InstanceId, definition)
                {
                    Experience = entry.Experience,
                    Productivity = entry.Productivity,
                    Quality = entry.Quality,
                    Speed = entry.Speed,
                    Specialization = entry.Specialization,
                    Satisfaction = entry.Satisfaction
                };
                state.Employees.Employees.Add(employee);
            }

            foreach (var entry in data.Competitors)
            {
                var definition = _competitorCatalog.Find(entry.DefinitionId);
                if (definition == null) continue;

                var competitor = new CompetitorState(definition)
                {
                    Users = entry.Users,
                    Valuation = entry.Valuation,
                    Reputation = entry.Reputation,
                    Quality = entry.Quality,
                    MarketShare = entry.MarketShare
                };
                state.Competitors.Add(competitor);
            }

            foreach (var roundName in data.RaisedInvestmentRounds)
            {
                if (Enum.TryParse<InvestmentRoundType>(roundName, out var roundType))
                {
                    state.RaisedInvestmentRounds.Add(roundType);
                }
            }

            state.GemWallet.Balance = data.GemBalance;

            foreach (var entry in data.ActiveBoosts)
            {
                if (!Enum.TryParse<StoreItemEffectType>(entry.EffectType, out var effectType)) continue;
                state.Store.ActiveBoosts.Add(
                    new ActiveBoost(entry.SourceItemId, effectType, entry.Magnitude, entry.RemainingCycles));
            }

            foreach (var cosmeticId in data.PurchasedCosmeticIds) state.Store.PurchasedCosmeticIds.Add(cosmeticId);

            return state;
        }

        private GameState CreateNewGame(double startingCash)
        {
            var player = new PlayerState();
            var economy = new EconomyState(startingCash);
            return new GameState(player, economy) { LastSavedUtc = _clock.UtcNow };
        }
    }
}
