# Architecture — Startup Empire

## Princípio central: Core desacoplado da Engine

A regra mais importante desta arquitetura, e a que viabiliza testar o jogo **sem o Unity Editor instalado**, é:

> Toda regra de negócio (economia, save, idle, produtos, missões, progressão) é escrita em **C# puro**, sem `using UnityEngine;`, sem `MonoBehaviour`, sem `ScriptableObject`. Essas classes vivem em `Assets/Game/**/Core*.cs` (ou subpastas `Domain/`) e são referenciadas por **link** (mesmo arquivo físico) tanto pelo projeto Unity quanto por um projeto de teste `.NET` comum (`Tests.NET/StartupEmpire.Domain.Tests.csproj`).

Isso significa: o mesmo arquivo `Assets/Game/Economy/EconomyEngine.cs` é compilado pelo Unity (quando o Editor existir) E pelo `dotnet test` (agora, nesta sessão) — não são cópias, é o arquivo físico linkado via `<Compile Include="..\Assets\Game\..\*.cs" />` no `.csproj` do projeto de testes. Isso dá evidência real de execução (Regra 35 da missão) sem depender do Editor.

MonoBehaviours (`Assets/Game/**/*View.cs`, `*Controller.cs`, `*Presenter.cs`) são finos: leem estado do Core, desenham UI, encaminham input. **A UI nunca é fonte de verdade.**

## Camadas

```
┌─────────────────────────────────────────┐
│  UI Layer (MonoBehaviour, uGUI)          │  Assets/Game/UI/**
│  - Lê GameState via eventos/observers    │
│  - Nunca modifica estado diretamente     │
├─────────────────────────────────────────┤
│  Application/Services Layer              │  Assets/Game/*/​*Service.cs
│  - Orquestra casos de uso                │
│  - Ex: DevelopmentService, SalesService  │
├─────────────────────────────────────────┤
│  Domain Layer (C# puro, sem UnityEngine) │  Assets/Game/*/​Core/*.cs
│  - GameState, Economy, Product, Employee │
│  - Regras determinísticas e testáveis    │
├─────────────────────────────────────────┤
│  Data Layer (ScriptableObjects/DTO)      │  Assets/Game/*/​Data/*.cs
│  - EconomyConfig, ProductDefinition, ... │
│  - Configuração data-driven              │
├─────────────────────────────────────────┤
│  Persistence Layer                       │  Assets/Game/Save/**
│  - ISaveStorage (abstrai disco/cloud)    │
│  - SaveData versionado + migração        │
└─────────────────────────────────────────┘
```

Comunicação entre camadas é via **interfaces + eventos** (C# `event`/`Action`), não referências estáticas globais. `GameState` é injetado, não é um singleton onipresente sem justificativa — existe um único `GameRoot` (composition root) que instancia os serviços e os injeta nos MonoBehaviours via `[SerializeField]`/inicialização explícita.

## Estrutura de Pastas

```
StartupEmpire/
├── Assets/
│   └── Game/
│       ├── Core/           GameState, GameClock, GameRoot, EventBus
│       ├── Economy/        EconomyEngine, EconomyConfig, LedgerEntry
│       ├── Products/       Product, ProductService, ProductDefinition
│       ├── Employees/      Employee, HiringService, RoleDefinition
│       ├── Research/       ResearchTree, ResearchNodeData
│       ├── Progression/    CompanyStage, ProgressionService
│       ├── Missions/       MissionDefinition, MissionService
│       ├── Events/         GameEventData, EventService
│       ├── Idle/           IdleService, OfflineProgressCalculator
│       ├── Save/           SaveData, SaveService, ISaveStorage, migrations/
│       ├── Audio/          AudioManager, AudioConfig
│       ├── UI/             *View.cs, *Presenter.cs, screens/
│       ├── Narrative/      Chapter1 dialogue/tutorial data
│       ├── Analytics/      IAnalyticsService (abstração, sem SDK acoplado)
│       └── Tests/          Testes em Unity Test Framework (rodam quando o Editor existir)
├── Packages/manifest.json  Dependências UPM mínimas
├── ProjectSettings/        ProjectVersion.txt (gera o resto ao abrir no Editor)
├── Tests.NET/              Projeto de testes .NET puro (roda HOJE via `dotnet test`)
│   └── StartupEmpire.Domain.Tests.csproj  (linka os mesmos .cs do Domain)
├── backend/                ASP.NET Core (ranking, cloud save, referrals) — Fase 2
├── PROJECT-PLAN.md
├── GAME-DESIGN-DOCUMENT.md
├── ARCHITECTURE.md
├── PROGRESS.md
└── CHANGELOG.md
```

## Save System

- `SaveDataV1` (e futuras `SaveDataV2`, ...) são DTOs simples e versionados (`int SchemaVersion`).
- `ISaveStorage` abstrai onde o save vive (`PlayerPrefs`/arquivo local hoje; cloud no futuro).
- `SaveMigrator` aplica migrações em cadeia (`V1→V2→V3→V4`) para nunca perder progresso quando um campo novo é adicionado; V4 persiste o tutorial contextual.
- `StartupFlowBuilder` mantém Splash/Menu acima do shell e só libera a campanha por Continue ou New Game. Novo jogo apaga o save apenas após confirmação quando já existe progresso.
- Em `Application.isBatchMode`, o composition root usa `InMemorySaveStorage`; automação de PlayMode nunca toca no arquivo real do jogador.
- `IRecoverableSaveStorage` é uma capacidade opcional: `FileSaveStorage` rotaciona o arquivo anterior para `.bak`, e `SaveService` tenta esse snapshot e restaura o principal quando a leitura atual falha. Storages cloud podem implementar histórico próprio sem alterar o contrato básico.
- Autosave a cada N ciclos de jogo e em pontos de transição de tela; save manual disponível.
- Falha de leitura (JSON corrompido, campo ausente) cai em valores padrão por campo, nunca aborta o load inteiro.

## Sistema de Tempo / Idle

- `GameClock` é a única fonte de "agora" no domínio (`IClock.UtcNow`), injetável — permite testar sem depender de `DateTime.UtcNow` real.
- `OfflineProgressCalculator` é uma classe pura testada por `dotnet test`: recebe `elapsed`, `EconomyState`, `EconomyConfig`, devolve um `OfflineSummary` (dinheiro ganho, bugs surgidos, eventos perdidos) sem tocar em nada de Unity.

## Backend (Fase 2, fora do escopo do MVP local)

ASP.NET Core + PostgreSQL, usado **apenas** para autenticação futura, ranking, perfil online, referrals e cloud save. O jogo principal roda 100% offline sem o backend. Estrutura prevista em `backend/` (não implementada nesta fase — ver `PROGRESS.md`).

## Por que não há Android SDK/Gradle configurados ainda

Documentado em `PROJECT-PLAN.md` (bloqueio de ambiente). Quando o Unity Editor + módulo Android forem instalados pelo usuário, o pipeline padrão de build Android do Unity (que gera o projeto Gradle automaticamente) assume a configuração — não é necessário configurar Gradle manualmente antes disso.
