using System.Threading.Tasks;
using UnityEngine;
using StartupEmpire.Achievements;
using StartupEmpire.Competitors;
using StartupEmpire.Economy;
using StartupEmpire.Employees;
using StartupEmpire.Events;
using StartupEmpire.Idle;
using StartupEmpire.Investment;
using StartupEmpire.Missions;
using StartupEmpire.Premium;
using StartupEmpire.Products;
using StartupEmpire.Progression;
using StartupEmpire.Ranking;
using StartupEmpire.Referrals;
using StartupEmpire.Research;
using StartupEmpire.Save;
using StartupEmpire.Store;
using StartupEmpire.Upgrades;

namespace StartupEmpire.Core
{
    /// Composition root único da cena: instancia os serviços de domínio e os
    /// expõe para as MonoBehaviours de UI. Não contém regra de negócio própria —
    /// toda lógica vive nas classes puras de Assets/Game/*/​*.cs.
    public sealed class GameRoot : MonoBehaviour
    {
        public static GameRoot Instance { get; private set; }

        public GameState State { get; private set; }
        public EventBus EventBus { get; private set; }
        public EconomyEngine Economy { get; private set; }
        public DevelopmentService Development { get; private set; }
        public CustomerAcquisitionService CustomerAcquisition { get; private set; }
        public ProgressionService Progression { get; private set; }
        public MissionService Missions { get; private set; }
        public AchievementService Achievements { get; private set; }
        public LearningService Learning { get; private set; }
        public IdleService Idle { get; private set; }
        public SaveService Save { get; private set; }
        public ProductDefinitionCatalog Catalog { get; private set; }
        public UpgradeService Upgrades { get; private set; }
        public HiringService Hiring { get; private set; }
        public EventService Events { get; private set; }
        public EmployeeDefinitionCatalog EmployeeCatalog { get; private set; }
        public CompetitorService Competitors { get; private set; }
        public CompetitorDefinitionCatalog CompetitorCatalog { get; private set; }
        public InvestmentService Investment { get; private set; }
        public GemWalletService Gems { get; private set; }
        public StoreService Store { get; private set; }
        public RankingClientService Ranking { get; private set; }
        public ReferralClientService Referrals { get; private set; }

        /// URL do backend (seção 3 da missão: opcional, o jogo funciona sem isso).
        /// Deixe vazio para usar os clientes nulos (offline-safe) por padrão.
        [SerializeField] private string backendBaseUrl = "";

        private EconomyConfigValues _economyConfig;
        private float _autosaveTimer;
        private const float AutosaveIntervalSeconds = 60f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Bootstrap();
        }

        private void Bootstrap()
        {
            EventBus = new EventBus();
            var clock = new SystemClock();
            _economyConfig = new EconomyConfigValues();
            Catalog = ProductDefinitionCatalog.CreateChapter1Catalog();

            Economy = new EconomyEngine(_economyConfig, clock, EventBus);
            Development = new DevelopmentService(new DevelopmentConfigValues(), EventBus);
            CustomerAcquisition = new CustomerAcquisitionService(new CustomerAcquisitionConfigValues(), EventBus);
            Progression = new ProgressionService(EventBus);
            Gems = new GemWalletService(clock, EventBus);
            Missions = new MissionService(Chapter1Missions.Create(), EventBus, Economy, Gems);
            Achievements = new AchievementService(AchievementCatalog.Create(), EventBus);
            Learning = new LearningService(new LearningConfigValues());
            Idle = new IdleService(new OfflineProgressCalculator(_economyConfig), clock, EventBus);
            Upgrades = new UpgradeService(UpgradeCatalog.CreateChapter1Catalog(), Economy, EventBus);
            EmployeeCatalog = EmployeeDefinitionCatalog.CreateDefaultCatalog();
            Hiring = new HiringService(new HiringConfigValues(), Economy, EventBus);
            Events = new EventService(EventCatalog.CreateChapter1Catalog(), new EventConfigValues(), Economy, EventBus);
            Competitors = new CompetitorService(new CompetitorConfigValues());
            CompetitorCatalog = CompetitorDefinitionCatalog.CreateChapter1Catalog();
            Investment = new InvestmentService(InvestmentCatalog.CreateDefaultCatalog(), Economy, EventBus);
            Store = new StoreService(StoreCatalog.CreateChapter1Catalog(), Gems, Economy, EventBus);

            IRankingClient rankingClient = string.IsNullOrWhiteSpace(backendBaseUrl)
                ? new NullRankingClient()
                : new HttpRankingClient(backendBaseUrl);
            IReferralClient referralClient = string.IsNullOrWhiteSpace(backendBaseUrl)
                ? new NullReferralClient()
                : new HttpReferralClient(backendBaseUrl);
            Ranking = new RankingClientService(rankingClient);
            Referrals = new ReferralClientService(referralClient, Gems);

            var storage = new FileSaveStorage();
            Save = new SaveService(storage, clock, Catalog, EmployeeCatalog, CompetitorCatalog);
            State = Save.Load(_economyConfig.StartingCash);

            if (State.Products.Count == 0)
            {
                var definition = Catalog.Find("first_website");
                State.Products.Add(new ProductState(definition));
            }

            if (State.Competitors.Count == 0)
            {
                foreach (var competitorDefinition in CompetitorCatalog.All)
                {
                    State.Competitors.Add(new CompetitorState(competitorDefinition));
                }
            }

            var offlineSummary = Idle.ApplyOfflineProgress(State);
            if (offlineSummary.CashEarned > 0)
            {
                Debug.Log($"[StartupEmpire] Progresso offline: +{offlineSummary.CashEarned:F2} em {offlineSummary.ElapsedApplied.TotalHours:F1}h");
            }
        }

        private void Update()
        {
            _autosaveTimer += Time.unscaledDeltaTime;
            if (_autosaveTimer >= AutosaveIntervalSeconds)
            {
                _autosaveTimer = 0f;
                Save.Save(State);
            }
        }

        /// Avança um ciclo de simulação (aquisição de clientes, receita, folha de
        /// pagamento, progressão, missões e um possível evento aleatório).
        public void RunGameCycle(int cycles = 1)
        {
            var acquisitionMultiplier = Upgrades.GetMultiplier(UpgradeEffectType.AcquisitionRateMultiplier, State.Upgrades)
                * Hiring.GetProductivityMultiplier(State.Employees, EmployeeRole.Marketing)
                * Store.GetBoostMultiplier(State.Store, StoreItemEffectType.AcquisitionBoost);

            foreach (var product in State.Products)
            {
                CustomerAcquisition.RunCycle(product, Economy, State.Economy, cycles, acquisitionMultiplier);
            }
            Economy.RecomputeRecurringRevenue(State.Economy, State.Products);
            Economy.RecomputeValuation(State.Economy);

            Hiring.PaySalaries(State.Employees, State.Economy);

            Store.TickBoosts(State.Store, cycles);

            Competitors.RunCycle(State.Competitors, cycles);
            var playerUsers = 0.0;
            foreach (var product in State.Products) playerUsers += product.Users;
            Competitors.RecomputeMarketShare(State.Competitors, playerUsers);

            Progression.TryAdvance(State);
            Missions.EvaluateAll(State);
            Achievements.EvaluateAll(State);

            PendingEvent = Events.TryTriggerRandomEvent(State);
        }

        public bool AcceptInvestmentOffer(InvestmentRoundType roundType)
        {
            var offer = Investment.Find(roundType);
            return offer != null && Investment.TryAcceptOffer(State, offer);
        }

        /// Evento aleatório disparado no último RunGameCycle, aguardando a UI
        /// chamar ResolveEvent com a escolha do jogador (null se nenhum disparou).
        public GameEventDefinition PendingEvent { get; private set; }

        public void ResolveEvent(string choiceId)
        {
            if (PendingEvent == null) return;
            Events.ResolveChoice(State, PendingEvent, choiceId);
            PendingEvent = null;
        }

        public void DevelopProduct(ProductState product, string knowledgeTrack, int cycles)
        {
            var devSpeedMultiplier = Upgrades.GetMultiplier(UpgradeEffectType.DevSpeedMultiplier, State.Upgrades)
                * Hiring.GetProductivityMultiplier(State.Employees, EmployeeRole.Backend)
                * Hiring.GetProductivityMultiplier(State.Employees, EmployeeRole.Frontend)
                * Store.GetBoostMultiplier(State.Store, StoreItemEffectType.DevSpeedBoost);
            var bugRateMultiplier = Upgrades.GetMultiplier(UpgradeEffectType.BugRateReduction, State.Upgrades);
            Development.Develop(product, State.Player, knowledgeTrack, cycles, devSpeedMultiplier, bugRateMultiplier);
        }

        public void StudyTrack(string track, int cycles)
        {
            var knowledgeMultiplier = Upgrades.GetMultiplier(UpgradeEffectType.KnowledgeGainMultiplier, State.Upgrades);
            Learning.Study(State.Player, track, cycles, knowledgeMultiplier);
        }

        public bool PurchaseUpgrade(string upgradeId)
        {
            var definition = Upgrades.Find(upgradeId);
            return definition != null && Upgrades.TryPurchase(definition, State.Upgrades, State.Economy);
        }

        public bool HireEmployee(string employeeDefinitionId)
        {
            var definition = EmployeeCatalog.Find(employeeDefinitionId);
            if (definition == null) return false;
            return Hiring.TryHire(State.Employees, State.Economy, definition) != null;
        }

        public bool PurchaseStoreItem(string storeItemId)
        {
            var item = Store.Find(storeItemId);
            return item != null && Store.TryPurchase(State, item);
        }

        /// Nunca lança e nunca bloqueia o jogo se o backend estiver fora do ar
        /// (RankingClientService já engole falhas de rede internamente).
        public Task<bool> SubmitRankingAsync() => Ranking.SubmitAsync(State, State.Player.PlayerId, State.Player.Name);

        public Task<bool> RedeemReferralCodeAsync(string code) =>
            Referrals.RedeemAsync(State.GemWallet, code, State.Player.PlayerId);

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) Save.Save(State);
        }

        private void OnApplicationQuit()
        {
            Save.Save(State);
        }
    }
}
