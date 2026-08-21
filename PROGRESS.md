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
- [COMPLETED] Save System versionado (schema V3, migrações V1→V2→V3, recuperação de save corrompido, autosave)
- [COMPLETED] Calendário de trabalho — dia atual + 4 ciclos limitados por dia; estudar/desenvolver/testar/corrigir consomem tempo atomicamente, falhas não consomem nem alteram estado, e Encerrar Dia executa um ciclo econômico e restaura o tempo.
- [COMPLETED] Idle / Offline progress (teto de horas, cálculo em lote, bugs por instabilidade)
- [COMPLETED] Capítulo 1 (fluxo completo: aprender → dev → testar → bugs → corrigir → lançar → 1º cliente → MRR → transição de estágio)
- [COMPLETED] Products — lifecycle dev/test/fix/launch agora imposto pelo domínio: bugs nascem ocultos, testes os revelam, correção atua apenas nos conhecidos e o lançamento exige desenvolvimento completo + ao menos uma rodada de testes; `CustomerAcquisitionService` cobre aquisição/conversão/churn.
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
- [COMPLETED] Audio manager — `AudioManager`/`AudioMixState`, volume independente por categoria (música/UI/ambiente/eventos/conquistas). Sem clipes ainda (este agente não tem como gerar áudio original ou licenciado — seção 28 exige isso); sistema pronto para receber os clipes quando existirem.
- [IN PROGRESS] UI final de todas as telas — 14 de 19 experiências do GDD implementadas. A navegação portrait agora usa cinco alvos principais grandes (`Início`, `Produtos`, `Equipe`, `Empresa`, `Mais`) e grade secundária para nove destinos, com Voltar fechando o menu/retornando ao Início. Faltam: Splash, Main Menu, New Game, Continue e uma tela dedicada de Development; o loop de desenvolvimento já funciona no Office.
- [IN PROGRESS] Art polish — ícone original do app criado e integrado (`Assets/Game/Art/StartupEmpireAppIcon.png`); UI interna ainda usa retângulos/texto placeholder e precisa de direção visual completa.
- [IN PROGRESS] Android optimization — portrait travado, `renderOutsideSafeArea=false`, `SafeAreaFitter` responsivo a notch/barra de gestos e navegação com alvos largos; ainda faltam smoke test em aparelho, matriz visual de resoluções e medição de desempenho.
- [COMPLETED] Android build (APK/AAB) — **APK atual real gerado e confirmado**: `Builds/Android/StartupEmpire-debug.apk`, 45.610.490 bytes, SHA-256 `6D6373E0679613059F9BA445D0305BBED08E9B47C541C61BAE39FA131B013D6F`, `result=Succeeded totalErrors=0 totalWarnings=0`. `aapt2 dump xmltree` confirmou `screenOrientation=1` (portrait). AAB ainda não foi gerado e o APK ainda não foi instalado/testado em aparelho ou emulador online.

## Testes

- [COMPLETED] `Tests.NET` (cliente) — **92/92** testes reais sobre a camada de domínio, executados via `dotnet test` nesta máquina (0 falhas), incluindo lifecycle, calendário de trabalho e migrações de save.
- [COMPLETED] `backend/StartupEmpire.Api.Tests` — 22 testes reais via `dotnet test`: 15 de unidade (RankingService/ReferralService com repositórios fake em memória) + 7 de integração HTTP ponta a ponta (`WebApplicationFactory<Program>` + SQLite em memória, motor relacional de verdade).
- [COMPLETED] Unity Test Framework (EditMode + PlayMode) — **31/31 EditMode + 8/8 PlayMode reais passando** (131 testes reais no cliente com os 92 do `Tests.NET`). Safe area tem testes determinísticos para notch/gestos; PlayMode confirma cinco botões principais com largura mínima de 19% e acesso às telas secundárias pelo Mais.

## Bugs reais encontrados e corrigidos nesta sessão

1. `SaveSerializer` usava `System.Text.Json` com `SaveDataV1` baseado em campos públicos (para manter compatibilidade futura com `UnityEngine.JsonUtility`). `System.Text.Json` por padrão só serializa **propriedades**, não campos — o teste `SaveThenLoad_RoundTripsGameState` pegou isso na primeira execução (nome do jogador voltava sempre como "Founder"). Corrigido com `JsonSerializerOptions.IncludeFields = true`.
2. O `.gitignore` tinha um padrão genérico `*.csproj` (para ignorar `.csproj` gerados pelo Unity/Visual Studio) que também estava excluindo silenciosamente `Tests.NET/StartupEmpire.Domain.Tests.csproj` — um arquivo escrito à mão, não gerado. Os dois commits anteriores de teste incluíram os arquivos `.cs` mas nunca o `.csproj` em si; `dotnet test` continuava funcionando localmente porque o arquivo existia em disco, mas um `git clone` limpo ficaria sem o projeto. Corrigido com uma exceção `!Tests.NET/**/*.csproj` no `.gitignore`.
3. `MissionDefinition.RewardGems` existia desde o Capítulo 1 (a missão "MRR" já tinha `rewardGems: 10`), mas `MissionService.EvaluateAll` nunca chegou a conceder gems — só cash. O campo ficava sem efeito silenciosamente. Corrigido ao implementar Gems: `MissionService` agora recebe um `GemWalletService` opcional e concede `RewardGems` junto com `RewardCash`, coberto por um teste novo (`EvaluateAll_GrantsGemReward_WhenMissionHasRewardGems`).
4. Nos testes de integração do backend, trocar o `AppDbContext` de Npgsql para SQLite via `WebApplicationFactory` falhava com "Only a single database provider can be registered" mesmo removendo o descritor `DbContextOptions<AppDbContext>`. Causa: `AddDbContext` com uma `Action<DbContextOptionsBuilder>` também registra `IDbContextOptionsConfiguration<AppDbContext>`, e a chamada antiga (Npgsql) continuava lá. Corrigido removendo os dois descritores antes de registrar o Sqlite.
5. O mesmo bug de `.gitignore` que já tinha escondido o `.csproj` de `Tests.NET` (ver item 2 da sessão anterior) estava prestes a se repetir com `backend/**/*.csproj` — pego e corrigido antes do primeiro commit do backend, generalizando a exceção no `.gitignore`.
6. **Bug real de UI pego por teste PlayMode**: `OfficeScreenBuilder` criava o `Canvas` como GameObject raiz separado, sem ser filho de `GameRoot`. Um teste PlayMode (`OfficeScreenBuilder_CreatesUiHierarchy...`) destruía o `GameRoot` no fim, mas a UI (Canvas/botões) ficava órfã na cena; o teste seguinte (`StudyButton_Click...`) achava o botão órfão do teste anterior via `GameObject.Find`, clicava nele, mas o listener apontava pra um componente já destruído — Unity silenciosamente não invoca listeners de alvo destruído, então nada acontecia (`Expected: greater than 0, But was: 0`). Corrigido parentando o `Canvas` sob `transform` do próprio `GameRoot`/`OfficeScreenBuilder` — destruir um agora destrói o outro corretamente. 2/2 PlayMode passando depois da correção.
7. `AndroidBuilder.cs` não compilou na primeira tentativa: `NamedBuildTarget` precisa de `using UnityEditor.Build;`, que faltava. Corrigido, recompilado, build seguiu normalmente.
8. **Bug real de timing entre testes PlayMode**: ao expandir de 1 para 10 telas (e adicionar `AudioManager` como segundo singleton na mesma cena), `Object.Destroy()` no fim de um `[UnityTest]` não garantia que o objeto estivesse realmente destruído antes do `Awake()` do próximo teste rodar — o singleton antigo (`GameRoot.Instance` ou `AudioManager.Instance`) ainda existia, então o novo `GameObject` se autodestruía na hora (regra de "já existe uma instância") antes do `Start()` conseguir montar a UI. 2 de 4 testes falhavam com `NullReferenceException`. Corrigido trocando `Object.Destroy` por `Object.DestroyImmediate` dentro de um `[UnityTearDown]` garantido (roda depois de cada teste, sem depender de cada teste lembrar de limpar).
9. Não era bug do jogo, mas do meu próprio teste: `GameObject.Find` não encontra objetos inativos, e o teste de navegação tentava achar o "StatusText" do Office **depois** de já ter trocado para a tela Products (quando o Office já estava com `SetActive(false)`). Corrigido capturando a referência antes de clicar no botão de navegação.
10. `GameRoot.RunGameCycle` substituía silenciosamente um `PendingEvent` ainda não respondido ao sortear o ciclo seguinte. Corrigido para preservar o evento até `ResolveEvent`, com cobertura PlayMode. O primeiro cenário do teste também revelou uma pré-condição real do catálogo: eventos do Capítulo 1 exigem produto lançado ou cliente pagante; o teste agora prepara esse estado pela API de domínio antes do sorteio.
11. `DevelopmentService.Launch` aceitava qualquer fase, portanto o jogador podia lançar o primeiro produto imediatamente e pular todo o Capítulo 1. A UI também não oferecia a ação Testar e revelava todos os bugs antes do teste. Corrigido no domínio, UI e save V2, com testes de regressão nas três suítes do cliente.
12. `WorkCyclesPerDay` existia apenas como número decorativo: todas as ações podiam ser repetidas infinitamente e o ciclo econômico era um botão independente. Corrigido com agenda persistida no save V3, resultados explícitos e consumo atômico validado em PlayMode.

## 🎉 Primeira compilação real dentro do Unity Editor

O projeto abriu de verdade no Unity Editor 6000.0.82f1 pela primeira vez. A licença
Unity já estava ativa nesta máquina (sessão anterior cacheada), então nem foi preciso
fazer login. A primeira compilação real revelou um erro que eu já tinha sinalizado como
risco: `System.Text.Json` não existe no perfil de API padrão do Unity (`error CS0234`).
Troquei `SaveSerializer` para `Newtonsoft.Json` (client via `com.unity.nuget.newtonsoft-json`,
`Tests.NET` via pacote NuGet padrão) — reabri o projeto e a compilação terminou com
**zero erros**, módulo Android reconhecido (`Android Extension - Scanning For ADB Devices`
aparece no log), e os 82/82 testes do `Tests.NET` continuam passando depois da troca.

Isso está registrado em 4 commits: o fix em si, e três de "primeira importação real" com
todos os `.meta` files e `ProjectSettings/*.asset` que o Editor gerou (antes só existia o
`ProjectVersion.txt` escrito à mão como placeholder).

## Estado do escopo puramente C# (sem depender do Editor)

Com Ads e Statistics, todos os sistemas de domínio da missão que podem ser implementados
e testados de verdade sem o Unity Editor estão `[COMPLETED]`. Tudo que resta `[PENDING]`
(Audio manager, UI final de todas as telas, Art polish, Android build/APK/AAB, Balancing
por playtesting real) genuinely precisa do Editor instalado — não é falta de esforço,
é uma dependência real de ferramenta. Ver bloqueio na seção 1 acima.

## Nota sobre veracidade dos resultados

Todo item marcado `[COMPLETED]` neste arquivo foi de fato executado nesta máquina (compilado e/ou testado — ver saída de `dotnet test` acima). Itens que dependem do Unity Editor/Android SDK permanecem `[BLOCKED]` ou `[PENDING]` até que essas ferramentas estejam disponíveis — não há alegação de "build funcionando" ou "APK gerado" sem evidência real, conforme regra 35 da missão. UI de telas (Office, Products, Employees, etc.), Áudio, Arte e o build Android continuam pendentes e dependem do Editor instalado.
