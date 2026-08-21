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

### Added (continuação — Ranking, Referrals, backend)
- Novo projeto `backend/StartupEmpire.Api` (ASP.NET Core Minimal API, .NET 10) e `backend/StartupEmpire.Api.Tests` (xUnit), com domínio próprio e desacoplado de EF/ASP.NET, no mesmo espírito do cliente Unity.
- `Domain/Ranking`: `RankingService` (seção 23) — valida dados no servidor de verdade (rejeita negativos/NaN/Infinity), aplica rate-limit por jogador (`MinSubmissionInterval`) e um heurístico anti-cheat de crescimento implausível (`MaxPlausibleGrowthMultiple`). Endpoints `/api/ranking/submit`, `/top`, `/me/{playerId}` cobrindo as 5 métricas da missão (NetWorth, Valuation, MRR, Progress, Achievements).
- `Domain/Referrals`: `ReferralService` (seção 24) — gera código único por jogador, vincula inviter/invitee, aplica prevenção de abuso (rejeita auto-indicação e um segundo resgate do mesmo convidado, reforçado também por índice único no banco) e um teto de resgates por indicador. Endpoints `/api/referrals/code`, `/redeem`.
- Persistência via EF Core + Npgsql, com migration inicial gerada (`Data/Migrations/InitialCreate`).
- 22 testes reais no backend: 15 de unidade com repositórios fake em memória (sem banco nenhum) e 7 de integração HTTP ponta a ponta via `WebApplicationFactory<Program>` com SQLite em memória (motor relacional de verdade).
- Cliente Unity: `IRankingClient`/`IReferralClient` com `NullRankingClient`/`NullReferralClient` como padrão seguro (o ranking nunca bloqueia a campanha, seção 23), mais `HttpRankingClient`/`HttpReferralClient` reais via `UnityWebRequest`. `ReferralClientService` só credita Gems localmente depois que o backend confirma o resgate. `GameRoot` expõe `SubmitRankingAsync`/`RedeemReferralCodeAsync` e liga tudo com um `backendBaseUrl` opcional (vazio = 100% offline).
- `PlayerState.PlayerId`: identificador estável gerado no cliente (tipo device id), agora persistido no save, usado para falar com o backend. Sem autenticação real ainda (item futuro, seção 3).
- 5 novos testes reais no cliente (`RankingClientServiceTests`, `ReferralClientServiceTests`) — suíte total do cliente: 72/72 passando.

### Fixed (continuação)
- Testes de integração do backend falhavam com "Only a single database provider can be registered" ao trocar Npgsql por SQLite via `WebApplicationFactory`: `AddDbContext` registra tanto `DbContextOptions<AppDbContext>` quanto `IDbContextOptionsConfiguration<AppDbContext>`, e só o primeiro estava sendo removido. Corrigido removendo os dois antes de registrar o provedor de teste.
- O mesmo bug de `.gitignore` que excluía `Tests.NET/*.csproj` (ver seção anterior) estava prestes a se repetir com `backend/**/*.csproj` — generalizada a exceção antes do primeiro commit do backend.
- Pacotes com vulnerabilidades conhecidas nos templates padrão: `Microsoft.OpenApi` 2.0.0 (GHSA-v5pm-xwqc-g5wc, pinado em 2.12.2) e `SQLitePCLRaw.lib.e_sqlite3` 2.1.11 (GHSA-2m69-gcr7-jv3q, pinado em 3.53.3). Backend builda com 0 avisos e 0 vulnerabilidades conhecidas.

### Added (continuação — verificação contra Postgres real via Docker)
- `backend/docker-compose.yml`: Postgres 16 dedicado deste projeto, isolado do nativo (porta 5432, ocupada e com senha desconhecida) e dos containers de outros projetos (5555/5455 já em uso) — sobe na porta 5442.
- Connection string configurada via `dotnet user-secrets` (nunca commitada). Migration `InitialCreate` aplicada de verdade contra esse Postgres.
- API rodada de verdade (`dotnet run`, `ASPNETCORE_ENVIRONMENT=Development`) e testada via `curl` ponta a ponta: submissão de ranking, top/rank, geração de código de indicação, resgate com sucesso e rejeição correta de um segundo resgate pelo mesmo convidado — tudo confirmado também via `psql` direto nas tabelas, não só pela resposta HTTP.
- Isso resolve a limitação anterior ("backend não testado contra Postgres real") registrada nesta mesma sessão.

### Added (continuação — Ads, Statistics)
- `Ads`: `IAdService`/`AdRewardService` (seção 22) — `NullAdService` como adapter seguro padrão (nunca trava, nunca finge sucesso); Gems só creditados quando `AdRewardResult.Granted` é retornado. `GameRoot.WatchRewardedAd` já pronto pra ligar num SDK real depois, sem tocar no resto do domínio.
- `Statistics`: `StatisticsService.BuildSnapshot` agrega o `GameState` inteiro num `StatisticsSnapshot` (patrimônio, valuation, MRR, equity do fundador, usuários/clientes agregados, produtos lançados, funcionários, níveis de upgrade comprados, conquistas, missões, gems, rodadas de investimento captadas, participação de mercado). Sem estado próprio — sempre recalculado a partir da fonte de verdade.
- 10 novos testes reais (`AdRewardServiceTests`, `StatisticsServiceTests`) — suíte total do cliente: 82/82 passando.
- Com isso, todo o escopo de domínio da missão que não depende do Unity Editor está implementado e testado.

### Added (continuação — primeira compilação real no Unity Editor)
- Unity Editor 6000.0.82f1 (LTS) + Android Build Support completo instalados de verdade nesta máquina (ver `PROJECT-PLAN.md` para como o bloqueio foi contornado via CLI headless).
- Projeto aberto pela primeira vez no Editor real. Licença já estava ativa (sessão cacheada anterior), sem precisar de login.
- `.meta` files e `ProjectSettings/*.asset` gerados pelo Editor e commitados — antes só existia um `ProjectVersion.txt` escrito à mão como placeholder.

### Fixed (continuação)
- Primeiro erro de compilação real do projeto: `System.Text.Json` não existe no perfil de API padrão do Unity (`error CS0234`/`CS0246` em `SaveSerializer.cs`), exatamente o risco que o comentário original do arquivo já sinalizava. Trocado para `Newtonsoft.Json` (`com.unity.nuget.newtonsoft-json` no Unity, pacote NuGet padrão no `Tests.NET`). Recompilado no Editor real: 0 erros. 82/82 testes `.NET` continuam passando depois da troca.

### Added (continuação — testes reais dentro do Unity Test Runner)
- `Assets/Game/StartupEmpire.Game.asmdef`: assembly definition próprio para o código do jogo (antes compilava implicitamente em `Assembly-CSharp`) — necessário porque um asmdef externo não consegue referenciar `Assembly-CSharp` por nome.
- `Assets/Game/Tests/EditMode/`: suíte real do Unity Test Framework (NUnit), 24 testes portados do `Tests.NET` cobrindo Economy, Development, Customer Acquisition, Save, Idle, Progression e Missions/Achievements.
- `[assembly: InternalsVisibleTo("StartupEmpire.Tests.EditMode")]` em `Assets/Game/AssemblyInfo.cs` — necessário porque, com dois assemblies separados, os setters `internal` deixaram de ser visíveis para os testes (no `Tests.NET` isso nunca foi problema porque tudo compila numa única assembly).
- **Rodado de verdade** via `Unity.exe -batchmode -projectPath ... -runTests -testPlatform EditMode -testResults results.xml`: `test-run result="Passed" total="24" passed="24" failed="0" inconclusive="0" skipped="0"`, 0.37s. Não é alegação — resultado extraído do XML gerado pelo próprio Unity Test Runner.

### Added (continuação — Audio, tela jogável, APK Android real)
- `Audio`: `AudioManager`/`AudioMixState` (seção 28) — volume independente por categoria (música/UI/ambiente/eventos/conquistas). Sem clipes ainda: sem ferramenta para gerar áudio original ou obter licença compatível; sistema pronto para receber clipes quando existirem. 5 novos testes reais.
- Primeira tela jogável de verdade: `OfficeScreenBuilder` monta a UI do hub (Canvas, status, botões Estudar/Desenvolver/Corrigir Bugs/Lançar/Avançar Ciclo) em runtime, ligada direto na API existente de `GameRoot`. Cena `Assets/Game/UI/Scenes/Office.unity` gerada por um novo editor tool (`SceneBuilder`, também rodável headless).
- **APK Android real gerado**: `Builds/Android/StartupEmpire-debug.apk` (32.189.060 bytes), via novo `AndroidBuilder` editor tool rodado com `Unity.exe -batchmode -executeMethod ... -quit`. `result=Succeeded totalErrors=0 totalWarnings=0`, build IL2CPP+Gradle real de ~5m44s. Confirmado com o comando `file` do sistema, não é alegação sem evidência.
- 2 novos testes PlayMode reais (`OfficeScreenBuilderTests`) — rodam a UI de verdade em Play Mode dentro do Unity.

### Fixed (continuação)
- `OfficeScreenBuilder` criava o `Canvas` como GameObject raiz desconectado de `GameRoot`, então destruir o `GameRoot` nunca limpava a UI — um teste PlayMode pegou isso na primeira execução (segundo teste encontrava o botão órfão do primeiro, com listener já morto). Corrigido parentando o `Canvas` sob o `transform` do `GameRoot`.
- `AndroidBuilder.cs` não compilou de primeira: faltava `using UnityEditor.Build;` para `NamedBuildTarget`. Corrigido e recompilado antes do build real.

### Known limitations
- Unity Editor e Android SDK instalados, projeto compila sem erros, Audio implementado, 1 de 19 telas jogável, e um APK de debug real já foi gerado (ver acima). Restam: as outras 18 telas, arte final (visual atual é placeholder), otimização/AAB de publicação, balanceamento por playtesting — isso é o próximo trabalho, não mais um bloqueio de ambiente.
- IPO (`InvestmentRoundType.Ipo`) está modelado no enum mas ainda não tem uma oferta/mecânica própria — ela não é uma simples troca de caixa por equity como as demais rodadas.
- Gems ainda só são obtidos via recompensa de missão ou referral; não há vínculo com pagamento real (Google Play Billing) nem com anúncios recompensados (`IAdService` ainda não implementado).
- O backend não tem autenticação real — `PlayerId` é auto-declarado pelo cliente (item futuro, seção 3 da missão).
- `HttpRankingClient`/`HttpReferralClient`/`UnityWebRequestAsync` dependem de `UnityEngine`/`UnityWebRequest` e não foram compilados nem testados (aguardando o Unity Editor); a lógica pura (`RankingClientService`, `ReferralClientService`) está coberta por testes reais.
