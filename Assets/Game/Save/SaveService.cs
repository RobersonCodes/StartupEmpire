using System;
using System.Globalization;
using StartupEmpire.Core;
using StartupEmpire.Economy;
using StartupEmpire.Products;
using StartupEmpire.Progression;

namespace StartupEmpire.Save
{
    /// Converte GameState <-> SaveDataV1 e orquestra leitura/escrita via ISaveStorage.
    /// Nunca perde o progresso inteiro por um save corrompido ou campo ausente:
    /// falhas de parsing caem em um novo jogo em vez de derrubar a aplicação, e
    /// produtos cuja definição não existe mais são ignorados com segurança.
    public sealed class SaveService
    {
        private readonly ISaveStorage _storage;
        private readonly IClock _clock;
        private readonly ProductDefinitionCatalog _catalog;

        public SaveService(ISaveStorage storage, IClock clock, ProductDefinitionCatalog catalog)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
        }

        public void Save(GameState state)
        {
            var data = new SaveDataV1
            {
                SchemaVersion = SaveMigrator.CurrentSchemaVersion,
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

            var player = new PlayerState { Name = data.PlayerName };
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
                var definition = _catalog.Find(entry.DefinitionId);
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
