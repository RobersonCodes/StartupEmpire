# Changelog

Formato baseado em [Keep a Changelog](https://keepachangelog.com/). Datas em AAAA-MM-DD.

## [Unreleased]

### Added
- Estrutura inicial do repositório e documentação (`PROJECT-PLAN.md`, `GAME-DESIGN-DOCUMENT.md`, `ARCHITECTURE.md`, `PROGRESS.md`).
- Esqueleto de projeto Unity (`ProjectSettings/`, `Packages/manifest.json`) pronto para abrir no Editor quando instalado.
- Domínio puro (sem `UnityEngine`) para: `Core` (GameState, EventBus, PlayerState, IClock), `Economy` (EconomyEngine, ledger, MRR, valuation), `Products` (ciclo de vida, dev/test/fix/launch, aquisição de clientes/churn), `Progression` (gates de estágio de empresa), `Research` (trilhas de conhecimento + LearningService), `Missions` (sistema genérico + missões do Capítulo 1), `Achievements` (Hello World, First Customer, MRR, Founder, Unicorn), `Idle` (progresso offline com teto de horas), `Save` (save versionado, migração, recuperação de corrupção).
- `GameRoot`: composition root MonoBehaviour que instancia e conecta todos os serviços de domínio na cena Unity, com autosave e save em pause/quit.
- `FileSaveStorage`: implementação de `ISaveStorage` baseada em arquivo (`Application.persistentDataPath`), com escrita atômica via arquivo temporário.
- `Tests.NET`: suíte de 28 testes `dotnet test` reais, linkando diretamente os arquivos de `Assets/Game/**` (mesmo `.cs`, não cópia), cobrindo Economy, Products/Bugs, Idle, Save e Progression.

### Fixed
- `SaveSerializer` não restaurava corretamente os dados do save porque `System.Text.Json` não serializa campos públicos por padrão (apenas propriedades), e `SaveDataV1` usa campos. Corrigido com `JsonSerializerOptions.IncludeFields = true`. Bug capturado pelo teste `SaveThenLoad_RoundTripsGameState`.

### Added (continuação — Upgrades, Employees, Events)
- `Upgrades`: `UpgradeDefinition`/`UpgradeState`/`UpgradeService` com catálogo do Capítulo 1 (Computador Melhor, Internet Melhor, Ferramentas de Produtividade, Cursos Online) — custo cresce por nível (`BaseCost * CostGrowthFactor^level`), efeitos agregados como multiplicadores (dev speed, redução de bugs, aquisição, ganho de conhecimento).
- `Employees`: `EmployeeDefinition`/`Employee`/`EmployeeRoster`/`HiringService` com os 10 cargos da seção 11 (Backend, Frontend, Mobile, Design, QA, DevOps, PM, Marketing, Sales, Support), contratação, demissão, folha de pagamento (satisfação cai se não há caixa para pagar, sobe e ganha experiência quando paga) e multiplicador de produtividade agregado por cargo.
- `Events`: `GameEventDefinition`/`EventService` com sorteio por chance configurável e resolução de escolhas com consequências reais; catálogo do Capítulo 1 com os 3 eventos de exemplo da seção 14 (Servidor caiu, Bug crítico em produção, Cliente importante).
- `GameRoot` agora expõe `DevelopProduct`, `StudyTrack`, `PurchaseUpgrade`, `HireEmployee`, `ResolveEvent` e dispara `PendingEvent` a cada `RunGameCycle`, todos já considerando os multiplicadores de Upgrades/Employees.
- Persistência de Upgrades e Employees no save (`SaveDataV1.UpgradeLevels`/`Employees`, com migração e produtos/funcionários órfãos ignorados com segurança).
- 18 novos testes reais (`UpgradeServiceTests`, `HiringServiceTests`, `EventServiceTests`, `LearningServiceTests`, mais 2 em `SaveServiceTests`) — suíte total: 46/46 passando.

### Fixed (continuação)
- `.gitignore` tinha um `*.csproj` genérico que excluía silenciosamente `Tests.NET/StartupEmpire.Domain.Tests.csproj` (hand-authored, não gerado). Os dois commits de teste anteriores nunca incluíram o `.csproj`. Corrigido com `!Tests.NET/**/*.csproj`.

### Added (continuação — Competitors, Investment)
- `Competitors`: `CompetitorDefinition`/`CompetitorState`/`CompetitorService` (seção 15) — 2 concorrentes do Capítulo 1 (RivalTech, MegaCorp Software), crescimento por taxa fixa configurável por ciclo (`UserGrowthRatePerCycle`/`ValuationGrowthRatePerCycle`, sem IA pesada) e participação de mercado recalculada comparando usuários do jogador contra a soma dos concorrentes.
- `Investment`: `InvestmentOffer`/`InvestmentService` (seção 17) — catálogo com Angel, Seed, Series A/B/C, cada oferta com estágio mínimo de empresa e valuation mínimo exigidos; `EconomyEngine.ApplyInvestment` dilui `FounderEquity` multiplicativamente (composição real entre rodadas, não uma soma ingênua) e cada rodada só pode ser aceita uma vez.
- `GameRoot.RunGameCycle` agora também simula concorrentes e recalcula participação de mercado a cada ciclo; novo método `AcceptInvestmentOffer`.
- Persistência de Competitors e das rodadas de investimento já captadas no save, com o mesmo padrão de segurança contra órfãos (definição removida do catálogo = entrada ignorada, não quebra o load).
- 10 novos testes reais (`CompetitorServiceTests`, `InvestmentServiceTests`, mais 1 em `SaveServiceTests`) — suíte total: 56/56 passando.

### Added (continuação — Gems, Store)
- `Premium`: `GemWalletState`/`GemWalletService` (seção 20) — saldo, ledger e grant/spend, sem qualquer conexão a pagamento real; a abstração já é o suficiente para plugar Google Play Billing depois sem tocar em quem consome `GemWalletState`.
- `Store`: `StoreItemDefinition`/`StoreService` (seção 21) — catálogo do Capítulo 1 com 4 itens (boost de desenvolvimento, boost de aquisição, aporte de caixa instantâneo, cosmético), efeitos e preços sempre visíveis antes da compra, sem caixas de recompensa aleatórias; cosméticos são posse permanente e não podem ser recomprados, boosts são consumíveis com duração em ciclos.
- `GameRoot.RunGameCycle` agora expira boosts automaticamente (`Store.TickBoosts`) e aplica os multiplicadores de boosts ativos junto com Upgrades/Employees em `DevelopProduct` e na aquisição de clientes; novo método `PurchaseStoreItem`.
- Persistência de saldo de gems, boosts ativos (com ciclos restantes) e cosméticos comprados no save.
- 11 novos testes reais (`GemWalletServiceTests`, `StoreServiceTests`, mais 1 em `MissionAndAchievementServiceTests`) — suíte total: 67/67 passando.

### Fixed (continuação)
- `MissionDefinition.RewardGems` existia desde o Capítulo 1 (a missão "MRR" já tinha `rewardGems: 10`) mas nunca era concedido — `MissionService.EvaluateAll` só processava `RewardCash`. Corrigido: `MissionService` agora recebe um `GemWalletService` opcional e concede gems junto com o cash.

### Known limitations
- Unity Editor e Android SDK não estão instalados nesta máquina (bloqueio de ambiente — ver `PROJECT-PLAN.md`); UI de telas, áudio, arte final e build Android (APK/AAB) ainda não foram implementados/gerados.
- IPO (`InvestmentRoundType.Ipo`) está modelado no enum mas ainda não tem uma oferta/mecânica própria — ela não é uma simples troca de caixa por equity como as demais rodadas.
- Gems ainda só são obtidos via recompensa de missão; não há vínculo com pagamento real (Google Play Billing) nem com anúncios recompensados (`IAdService` ainda não implementado).
