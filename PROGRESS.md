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
- [PENDING] Upgrades
- [PENDING] Employees
- [PENDING] Research
- [PENDING] Events
- [PENDING] Competitors
- [PENDING] Investment
- [PENDING] Premium currency / Store
- [PENDING] Ad service abstraction
- [PENDING] Ranking/backend
- [PENDING] Referrals
- [PENDING] Audio manager
- [PENDING] UI final de todas as telas
- [PENDING] Art polish
- [PENDING] Android build (APK/AAB) — bloqueado pelo Editor

## Testes

- [COMPLETED] `Tests.NET` — 28 testes reais sobre a camada de domínio, executados via `dotnet test` nesta máquina (0 falhas). Cobrem: EconomyEngine (5), DevelopmentService (6), CustomerAcquisitionService (3), OfflineProgress/Idle (5), SaveService (4), ProgressionService (2), Missions/Achievements (3).
- [PENDING] Unity Test Framework (PlayMode/EditMode) — aguarda instalação do Editor. Os mesmos arquivos-fonte já compilam para isso; nenhuma reescrita será necessária.

## Bug real encontrado e corrigido nesta sessão

`SaveSerializer` usava `System.Text.Json` com `SaveDataV1` baseado em campos públicos (para manter compatibilidade futura com `UnityEngine.JsonUtility`). `System.Text.Json` por padrão só serializa **propriedades**, não campos — o teste `SaveThenLoad_RoundTripsGameState` pegou isso na primeira execução (nome do jogador voltava sempre como "Founder"). Corrigido com `JsonSerializerOptions.IncludeFields = true`. Suite voltou a 28/28 depois da correção.

## Nota sobre veracidade dos resultados

Todo item marcado `[COMPLETED]` neste arquivo foi de fato executado nesta máquina (compilado e/ou testado — ver saída de `dotnet test` acima). Itens que dependem do Unity Editor/Android SDK permanecem `[BLOCKED]` ou `[PENDING]` até que essas ferramentas estejam disponíveis — não há alegação de "build funcionando" ou "APK gerado" sem evidência real, conforme regra 35 da missão. UI de telas (Office, Products, Employees, etc.), Áudio, Arte e o build Android continuam pendentes e dependem do Editor instalado.
