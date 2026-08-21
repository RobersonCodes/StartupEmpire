using UnityEngine;
using StartupEmpire.Achievements;
using StartupEmpire.Economy;
using StartupEmpire.Idle;
using StartupEmpire.Missions;
using StartupEmpire.Products;
using StartupEmpire.Progression;
using StartupEmpire.Research;
using StartupEmpire.Save;

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
            Missions = new MissionService(Chapter1Missions.Create(), EventBus, Economy);
            Achievements = new AchievementService(AchievementCatalog.Create(), EventBus);
            Learning = new LearningService(new LearningConfigValues());
            Idle = new IdleService(new OfflineProgressCalculator(_economyConfig), clock, EventBus);

            var storage = new FileSaveStorage();
            Save = new SaveService(storage, clock, Catalog);
            State = Save.Load(_economyConfig.StartingCash);

            if (State.Products.Count == 0)
            {
                var definition = Catalog.Find("first_website");
                State.Products.Add(new ProductState(definition));
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

        /// Avança um ciclo de simulação (aquisição de clientes, receita, progressão, missões).
        public void RunGameCycle(int cycles = 1)
        {
            foreach (var product in State.Products)
            {
                CustomerAcquisition.RunCycle(product, Economy, State.Economy, cycles);
            }
            Economy.RecomputeRecurringRevenue(State.Economy, State.Products);
            Economy.RecomputeValuation(State.Economy);
            Progression.TryAdvance(State);
            Missions.EvaluateAll(State);
            Achievements.EvaluateAll(State);
        }

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
