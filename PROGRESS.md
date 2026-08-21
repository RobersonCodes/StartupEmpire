# PROGRESS

Legenda: `[COMPLETED]` `[IN PROGRESS]` `[PENDING]` `[BLOCKED]`

## Ambiente

- [COMPLETED] Auditoria do ambiente (Git, .NET, JDK, Unity, Android SDK)
- [COMPLETED] Unity Hub instalado via winget
- [RESOLVED] Unity Editor 6000.0.82f1 (LTS) + Android Build Support — instalados de verdade nesta máquina via CLI headless do Unity Hub. O Hub via winget era um pacote MSIX sem CLI; baixei o instalador tradicional (`UnityHubSetup-x64.exe`, direto do CDN oficial) e reinstalei o Hub com isso. `Unity Hub.exe -- --headless install --version 6000.0.82f1 --module android` baixa e instala sem exigir login — só a *ativação de licença* (uso do Editor, não a instalação) exige login/conta Unity, e isso continua sendo o único passo que só o usuário pode fazer.
- [RESOLVED] Android SDK/NDK/OpenJDK — confirmados em disco (`.../AndroidPlayer/SDK` com build-tools/cmake/cmdline-tools/platform-tools/platforms, `.../AndroidPlayer/NDK` completo), 5.4GB ao todo. O `install-modules` do Hub CLI falhou na primeira tentativa ("No modules found for this editor" — o `install` inicial havia saído antes de registrar os módulos no manifesto do Hub); contornado rodando manualmente o instalador NSIS do módulo Android que o Hub já tinha baixado (`UnitySetup-Android-Support-for-Editor-6000.0.82f1.exe`, em `%AppData%\UnityHub\downloads\`), e depois reassociando o Editor (`editors --add`) para o `install-modules` do NDK/SDK completar normalmente.
- `ProjectSettings/ProjectVersion.txt` atualizado para `6000.0.82f1` (era um placeholder `6000.0.35f1`) — bate com o Editor de verdade agora instalado.
- [RESOLVED] Conexão com Postgres real — o PostgreSQL 16 **nativo** desta máquina (porta 5432) continua bloqueado por senha que o agente não tem e não tentou adivinhar (provavelmente serve outros projetos seus). Mas o Docker Desktop foi ligado durante a sessão, então subimos um Postgres **dedicado** deste projeto via `backend/docker-compose.yml` (porta 5442, isolado, não mexe no nativo nem nos containers de outros projetos). Migration aplicada de verdade (`dotnet ef database update`), API rodada de verdade (`dotnet run`), e os fluxos de Ranking e Referrals testados via `curl` ponta a ponta — inclusive a rejeição correta de um segundo resgate pelo mesmo convidado — com os dados confirmados também via `psql` direto no banco. Ver `backend/README.md`.
- [COMPLETED] Repositório Git inicializado

## Documentação

- [COMPLETED] PROJECT-PLAN.md
- [COMPLETED] GAME-DESIGN-DOCUMENT.md
- [COMPLETED] ARCHITECTURE.md
- [COMPLETED] PROGRESS.md (este arquivo)
- [COMPLETED] CHANGELOG.md

## Sistemas — Core

- [COMPLETED] Estrutura de pastas `Assets/Game/**`
- [COMPLETED] GameState / IClock / SystemClock / EventBus / GameRoot (composition root)
- [COMPLETED] Economy Engine + EconomyConfigValues (ledger, cash flow, MRR, valuation)
- [COMPLETED] Save System versionado (SaveDataV1, migração, recuperação de save corrompido, autosave)
- [COMPLETED] Idle / Offline progress (teto de horas, cálculo em lote, bugs por instabilidade)
- [COMPLETED] Capítulo 1 (fluxo completo: aprender → dev → testar → bugs → corrigir → lançar → 1º cliente → MRR → transição de estágio)
- [COMPLETED] Products — DevelopmentService (dev/test/fix/launch) + CustomerAcquisitionService (aquisição/conversão/churn)
- [COMPLETED] Missions — sistema genérico orientado a dados + 4 missões do Capítulo 1
- [COMPLETED] Achievements — Hello World, First Customer, MRR, Founder, Unicorn
- [COMPLETED] Progression — gates Pessoa Física → Freelancer → Microempresa → Startup
- [COMPLETED] Research — trilhas de conhecimento (constantes) + LearningService
- [COMPLETED] Upgrades — computador, internet, ferramentas de produtividade, cursos online; custo cresce por nível, multiplicadores agregados por efeito
- [COMPLETED] Employees — 10 cargos (seção 11), contratação, demissão, folha de pagamento com satisfação/experiência, multiplicador de produtividade por cargo
- [COMPLETED] Events — sistema data-driven com escolhas e consequências reais (Servidor caiu, Bug crítico, Cliente importante)
- [COMPLETED] Competitors — 2 concorrentes simulados (RivalTech, MegaCorp Software) com crescimento por taxa fixa e participação de mercado recalculada por ciclo; sem IA pesada
- [COMPLETED] Investment — rodadas Angel/Seed/Series A/B/C com elegibilidade por estágio e valuation, diluição real e composta de FounderEquity (Ipo modelado no enum, sem oferta própria ainda — é o estágio final de CompanyStage, não uma troca caixa-por-equity)
- [COMPLETED] Premium currency (Gems) — GemWalletState/GemWalletService com saldo, ledger, grant/spend; sem conexão a pagamento real ainda (arquitetura pronta para Google Play Billing depois)
- [COMPLETED] Store — 4 itens do Capítulo 1 (boost de dev, boost de aquisição, aporte de caixa instantâneo, cosmético), efeitos sempre visíveis antes da compra, cosméticos não podem ser recomprados
- [COMPLETED] Ranking/backend — ASP.NET Core + EF Core/Npgsql (`backend/StartupEmpire.Api`), validação server-side real (dados inválidos, rate-limit, crescimento implausível), endpoints `/api/ranking/submit`, `/top`, `/me/{playerId}`; cliente Unity com `NullRankingClient` por padrão (nunca bloqueia a campanha) + `HttpRankingClient` real
- [COMPLETED] Referrals — código de indicação, vínculo inviter/invitee, recompensa, limite por indicador e prevenção de abuso (auto-indicação e resgate duplicado rejeitados) no backend; cliente credita os Gems localmente só após confirmação do servidor
- [COMPLETED] Ad service abstraction — `IAdService`/`AdRewardService` (seção 22), `NullAdService` como adapter seguro padrão, recompensa em Gems só concedida quando o anúncio termina com `AdRewardResult.Granted`
- [COMPLETED] Statistics — `StatisticsService` agrega o `GameState` num `StatisticsSnapshot` (patrimônio, valuation, MRR, usuários, clientes pagantes, funcionários, níveis de upgrade, conquistas, missões, gems, rodadas de investimento, participação de mercado); base pronta para a futura tela e para reaproveitar em outras submissões
- [PENDING] Audio manager
- [PENDING] UI final de todas as telas
- [PENDING] Art polish
- [PENDING] Android build (APK/AAB) — bloqueado pelo Editor

## Testes

- [COMPLETED] `Tests.NET` (cliente) — 82 testes reais sobre a camada de domínio, executados via `dotnet test` nesta máquina (0 falhas). Cobrem: EconomyEngine (5), DevelopmentService (6), CustomerAcquisitionService (3), OfflineProgress/Idle (5), SaveService (7), ProgressionService (2), Missions/Achievements (4), UpgradeService (5), HiringService (6), EventService (4), LearningService (2), CompetitorService (4), InvestmentService (5), GemWalletService (4), StoreService (6), RankingClientService (2), ReferralClientService (3), AdRewardService (5), StatisticsService (5).
- [COMPLETED] `backend/StartupEmpire.Api.Tests` — 22 testes reais via `dotnet test`: 15 de unidade (RankingService/ReferralService com repositórios fake em memória) + 7 de integração HTTP ponta a ponta (`WebApplicationFactory<Program>` + SQLite em memória, motor relacional de verdade).
- [PENDING] Unity Test Framework (PlayMode/EditMode) — aguarda instalação do Editor. Os mesmos arquivos-fonte já compilam para isso; nenhuma reescrita será necessária.

## Bugs reais encontrados e corrigidos nesta sessão

1. `SaveSerializer` usava `System.Text.Json` com `SaveDataV1` baseado em campos públicos (para manter compatibilidade futura com `UnityEngine.JsonUtility`). `System.Text.Json` por padrão só serializa **propriedades**, não campos — o teste `SaveThenLoad_RoundTripsGameState` pegou isso na primeira execução (nome do jogador voltava sempre como "Founder"). Corrigido com `JsonSerializerOptions.IncludeFields = true`.
2. O `.gitignore` tinha um padrão genérico `*.csproj` (para ignorar `.csproj` gerados pelo Unity/Visual Studio) que também estava excluindo silenciosamente `Tests.NET/StartupEmpire.Domain.Tests.csproj` — um arquivo escrito à mão, não gerado. Os dois commits anteriores de teste incluíram os arquivos `.cs` mas nunca o `.csproj` em si; `dotnet test` continuava funcionando localmente porque o arquivo existia em disco, mas um `git clone` limpo ficaria sem o projeto. Corrigido com uma exceção `!Tests.NET/**/*.csproj` no `.gitignore`.
3. `MissionDefinition.RewardGems` existia desde o Capítulo 1 (a missão "MRR" já tinha `rewardGems: 10`), mas `MissionService.EvaluateAll` nunca chegou a conceder gems — só cash. O campo ficava sem efeito silenciosamente. Corrigido ao implementar Gems: `MissionService` agora recebe um `GemWalletService` opcional e concede `RewardGems` junto com `RewardCash`, coberto por um teste novo (`EvaluateAll_GrantsGemReward_WhenMissionHasRewardGems`).
4. Nos testes de integração do backend, trocar o `AppDbContext` de Npgsql para SQLite via `WebApplicationFactory` falhava com "Only a single database provider can be registered" mesmo removendo o descritor `DbContextOptions<AppDbContext>`. Causa: `AddDbContext` com uma `Action<DbContextOptionsBuilder>` também registra `IDbContextOptionsConfiguration<AppDbContext>`, e a chamada antiga (Npgsql) continuava lá. Corrigido removendo os dois descritores antes de registrar o Sqlite.
5. O mesmo bug de `.gitignore` que já tinha escondido o `.csproj` de `Tests.NET` (ver item 2 da sessão anterior) estava prestes a se repetir com `backend/**/*.csproj` — pego e corrigido antes do primeiro commit do backend, generalizando a exceção no `.gitignore`.

## Estado do escopo puramente C# (sem depender do Editor)

Com Ads e Statistics, todos os sistemas de domínio da missão que podem ser implementados
e testados de verdade sem o Unity Editor estão `[COMPLETED]`. Tudo que resta `[PENDING]`
(Audio manager, UI final de todas as telas, Art polish, Android build/APK/AAB, Balancing
por playtesting real) genuinely precisa do Editor instalado — não é falta de esforço,
é uma dependência real de ferramenta. Ver bloqueio na seção 1 acima.

## Nota sobre veracidade dos resultados

Todo item marcado `[COMPLETED]` neste arquivo foi de fato executado nesta máquina (compilado e/ou testado — ver saída de `dotnet test` acima). Itens que dependem do Unity Editor/Android SDK permanecem `[BLOCKED]` ou `[PENDING]` até que essas ferramentas estejam disponíveis — não há alegação de "build funcionando" ou "APK gerado" sem evidência real, conforme regra 35 da missão. UI de telas (Office, Products, Employees, etc.), Áudio, Arte e o build Android continuam pendentes e dependem do Editor instalado.
