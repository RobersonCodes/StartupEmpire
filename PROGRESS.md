# PROGRESS

Legenda: `[COMPLETED]` `[IN PROGRESS]` `[PENDING]` `[BLOCKED]`

## Ambiente

- [COMPLETED] Auditoria do ambiente (Git, .NET, JDK, Unity, Android SDK)
- [COMPLETED] Unity Hub instalado via winget
- [BLOCKED] Unity Editor + módulo Android — requer login/ativação interativa do usuário. Ver `PROJECT-PLAN.md` seção "Bloqueio".
- [BLOCKED] Android SDK — instalado junto ao módulo Android do Unity Hub (mesmo bloqueio acima)
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
- [PENDING] Premium currency / Store
- [PENDING] Ad service abstraction
- [PENDING] Ranking/backend
- [PENDING] Referrals
- [PENDING] Audio manager
- [PENDING] UI final de todas as telas
- [PENDING] Art polish
- [PENDING] Android build (APK/AAB) — bloqueado pelo Editor

## Testes

- [COMPLETED] `Tests.NET` — 56 testes reais sobre a camada de domínio, executados via `dotnet test` nesta máquina (0 falhas). Cobrem: EconomyEngine (5), DevelopmentService (6), CustomerAcquisitionService (3), OfflineProgress/Idle (5), SaveService (7), ProgressionService (2), Missions/Achievements (3), UpgradeService (5), HiringService (6), EventService (4), LearningService (2), CompetitorService (4), InvestmentService (5).
- [PENDING] Unity Test Framework (PlayMode/EditMode) — aguarda instalação do Editor. Os mesmos arquivos-fonte já compilam para isso; nenhuma reescrita será necessária.

## Bugs reais encontrados e corrigidos nesta sessão

1. `SaveSerializer` usava `System.Text.Json` com `SaveDataV1` baseado em campos públicos (para manter compatibilidade futura com `UnityEngine.JsonUtility`). `System.Text.Json` por padrão só serializa **propriedades**, não campos — o teste `SaveThenLoad_RoundTripsGameState` pegou isso na primeira execução (nome do jogador voltava sempre como "Founder"). Corrigido com `JsonSerializerOptions.IncludeFields = true`.
2. O `.gitignore` tinha um padrão genérico `*.csproj` (para ignorar `.csproj` gerados pelo Unity/Visual Studio) que também estava excluindo silenciosamente `Tests.NET/StartupEmpire.Domain.Tests.csproj` — um arquivo escrito à mão, não gerado. Os dois commits anteriores de teste incluíram os arquivos `.cs` mas nunca o `.csproj` em si; `dotnet test` continuava funcionando localmente porque o arquivo existia em disco, mas um `git clone` limpo ficaria sem o projeto. Corrigido com uma exceção `!Tests.NET/**/*.csproj` no `.gitignore`.

## Nota sobre veracidade dos resultados

Todo item marcado `[COMPLETED]` neste arquivo foi de fato executado nesta máquina (compilado e/ou testado — ver saída de `dotnet test` acima). Itens que dependem do Unity Editor/Android SDK permanecem `[BLOCKED]` ou `[PENDING]` até que essas ferramentas estejam disponíveis — não há alegação de "build funcionando" ou "APK gerado" sem evidência real, conforme regra 35 da missão. UI de telas (Office, Products, Employees, etc.), Áudio, Arte e o build Android continuam pendentes e dependem do Editor instalado.
