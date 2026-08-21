# Startup Empire

> Tycoon mobile offline-first sobre construir uma empresa de tecnologia — do primeiro código, em um quarto simples, até uma companhia global e IPO.

[![Unity](https://img.shields.io/badge/Unity-6000.0.82f1_LTS-000000?logo=unity)](ProjectSettings/ProjectVersion.txt)
[![Android](https://img.shields.io/badge/Android-minSdk_23_%7C_targetSdk_36-3DDC84?logo=android)](Assets/Game/EditorTools/AndroidBuilder.cs)
[![C%23](https://img.shields.io/badge/C%23-.NET_8%2F10-512BD4?logo=dotnet)](Tests.NET/StartupEmpire.Domain.Tests.csproj)
[![Tests](https://img.shields.io/badge/tests-160_passing-success)](#qualidade-e-evidências)
[![Save](https://img.shields.io/badge/save-schema_V4-blue)](Assets/Game/Save/SaveMigrator.cs)

Startup Empire combina **Tycoon + Idle + Strategy + Business Simulator + Interactive Career**. A campanha principal funciona sem internet; backend é opcional e reservado a ranking, referrals e futuras funções de perfil/cloud save.

O repositório é um projeto de portfólio executável com fundação preparada para evolução comercial. Ele já possui APK Android real, domínio testado, persistência versionada, onboarding jogável, economia, produtos, funcionários, pesquisa, eventos, concorrentes, investimento, gems, loja e backend ASP.NET Core/PostgreSQL.

## Estado atual

| Área | Estado | Evidência |
|---|---|---|
| Gameplay de domínio | Funcional | 94 testes `.NET` + 35 EditMode |
| UI mobile | Jogável, em polish | Splash/Menu/onboarding + 13 telas internas |
| Save | Funcional, schema V4 | migrações V1→V4, escrita temporária e backup `.bak` |
| Backend opcional | Funcional | 22 testes, incluindo HTTP + SQLite em memória |
| APK debug | Gerado | 45.621.360 bytes, build Unity sem erros/avisos |
| Runtime em aparelho | Pendente | emulador local está offline |
| AAB/release signing | Pendente | exige configuração segura de distribuição |

Este documento distingue implementação de validação. “APK gerado” significa que o pipeline Unity/IL2CPP/Gradle terminou com sucesso; não significa que o binário já passou por smoke test em aparelho físico.

## Experiência de jogo

Progressão macro:

```text
Quarto → Freelancer → Primeiro Produto → Microempresa → Startup
       → Escritório → SaaS → Equipe → Investidores
       → Empresa Global → IPO
```

Loop principal:

```text
Aprender → Desenvolver → Testar → Descobrir bugs → Corrigir
         → Lançar → Adquirir clientes → Gerar receita
         → Reinvestir → Contratar → Automatizar → Escalar
```

O primeiro capítulo usa ciclos de trabalho limitados por dia. Ações inválidas não consomem tempo; encerrar o dia executa aquisição, receita, custos, salários, concorrentes, missões, conquistas e possíveis eventos. O tutorial é contextual, curto e derivado do estado real do jogo — o jogador pode agir fora da ordem sugerida sem prender a campanha.

## Funcionalidades implementadas

- Economia com caixa, ledger, MRR, valuation, custos, folha e equity do fundador.
- Lifecycle de produtos com planejamento, desenvolvimento, testes, bugs ocultos/conhecidos, correção e lançamento.
- Aquisição de usuários, conversão paga, satisfação, churn e reputação.
- Quatro ciclos de trabalho por dia, com consumo atômico e persistência.
- Progresso idle/offline limitado e calculado em lote.
- Upgrades data-driven de computador, internet, ferramentas e conhecimento.
- Dez cargos de funcionários, salários, experiência, produtividade e satisfação.
- Pesquisa extensível por trilhas de conhecimento.
- Missões e conquistas independentes da UI.
- Eventos data-driven com escolhas e consequências.
- Concorrentes simulados sem IA pesada.
- Rodadas Angel, Seed e Series A/B/C com requisitos e diluição composta.
- Gems, loja ética, boosts e cosméticos; sem pagamento real.
- Abstração `IAdService` com adapter nulo seguro.
- Ranking e referrals opcionais por backend.
- Volumes independentes por categoria de áudio.
- Splash, menu, novo jogo, continuar, confirmação destrutiva e 13 telas internas.
- Portrait, safe area, touchscreen e navegação adequada a smartphone.
- Ícone Android original e pipeline de APK reproduzível.

## Arquitetura

As regras de negócio são C# puro. Unity atua como shell de composição, entrada e apresentação; a UI nunca é fonte de verdade.

```text
┌──────────────────────────────────────────────────────────┐
│ UI / Unity                                               │
│ GameShellBuilder · StartupFlowBuilder · Screen panels    │
├──────────────────────────────────────────────────────────┤
│ Application / Composition                                │
│ GameRoot · serviços · EventBus                           │
├──────────────────────────────────────────────────────────┤
│ Domain                                                   │
│ Economy · Products · Employees · Research · Events ...   │
├──────────────────────────────────────────────────────────┤
│ Persistence / Integrations                               │
│ SaveService · ISaveStorage · HTTP clients · null adapters│
├──────────────────────────────────────────────────────────┤
│ Optional backend                                         │
│ ASP.NET Core · EF Core · PostgreSQL                      │
└──────────────────────────────────────────────────────────┘
```

Princípios aplicados:

- composição sobre hierarquia excessiva;
- serviços focados, sem God Object de regras;
- configurações centralizadas, evitando números mágicos espalhados;
- dependências online substituíveis e offline-safe;
- estado serializado por DTO versionado;
- sistemas determinísticos cobertos por testes rápidos fora do Editor;
- nenhuma lógica de domínio depende de botões, textos ou GameObjects.

Consulte [ARCHITECTURE.md](ARCHITECTURE.md) para decisões e fronteiras detalhadas.

## Estrutura do repositório

```text
Assets/Game/
├── Core/           estado, tempo, eventos e composition root
├── Economy/        caixa, ledger, MRR e valuation
├── Products/       catálogo, lifecycle e clientes
├── Employees/      catálogo, contratação e folha
├── Research/       conhecimento e aprendizado
├── Progression/    estágios da empresa
├── Missions/       objetivos orientados a dados
├── Events/         eventos e escolhas
├── Idle/           progresso offline
├── Save/           storage, DTO, serializer e migrações
├── Premium/        gems e ledger premium
├── Store/          itens, cosméticos e boosts
├── UI/             shell, telas, modal e safe area
├── Audio/          mixer lógico por categoria
├── EditorTools/    geração de cena e build Android
└── Tests/          EditMode e PlayMode

Tests.NET/          suíte rápida que linka o mesmo C# de domínio
backend/            API ASP.NET Core, EF Core e testes
ProjectSettings/    configuração versionada do Unity
Packages/           dependências Unity fixadas
```

## Pré-requisitos

- Windows 10/11.
- Git.
- Unity `6000.0.82f1` LTS.
- Android Build Support do Unity, incluindo SDK, NDK e OpenJDK.
- .NET SDK 8 para `Tests.NET`.
- .NET SDK 10 para backend.
- Docker Desktop apenas para validação opcional contra PostgreSQL real.

A versão exata do Editor está em [ProjectVersion.txt](ProjectSettings/ProjectVersion.txt). Abrir em outra versão pode migrar arquivos do projeto; faça isso conscientemente e em commit separado.

## Executar no Unity

1. Abra a pasta raiz no Unity Hub usando `6000.0.82f1`.
2. Abra `Assets/Game/UI/Scenes/Office.unity`.
3. Pressione Play.
4. Atravesse Splash → Novo Jogo.

O jogo inicia offline. `backendBaseUrl` vazio no `GameRoot` seleciona clientes nulos seguros para ranking/referrals.

## Testes

### Domínio C#

```powershell
dotnet test Tests.NET/StartupEmpire.Domain.Tests.csproj --nologo
```

### Backend

```powershell
dotnet test backend/StartupEmpire.Api.Tests/StartupEmpire.Api.Tests.csproj --nologo
```

### Unity EditMode

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.0.82f1\Editor\Unity.exe' `
  -batchmode -nographics -projectPath $PWD `
  -runTests -testPlatform EditMode `
  -testResults "$PWD\TestResults\editmode.xml" `
  -logFile "$PWD\TestResults\editmode.log"
```

### Unity PlayMode

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.0.82f1\Editor\Unity.exe' `
  -batchmode -nographics -projectPath $PWD `
  -runTests -testPlatform PlayMode `
  -testResults "$PWD\TestResults\playmode.xml" `
  -logFile "$PWD\TestResults\playmode.log"
```

Em batch mode, `GameRoot` seleciona `InMemorySaveStorage`. Isso impede que testes automatizados leiam, sobrescrevam ou apaguem o save real do jogador.

## Qualidade e evidências

Última validação local em 2026-08-21:

| Suíte | Passou | Falhou |
|---|---:|---:|
| Domínio `.NET` | 94 | 0 |
| Unity EditMode | 35 | 0 |
| Unity PlayMode | 9 | 0 |
| Backend | 22 | 0 |
| **Total** | **160** | **0** |

APK atual:

```text
Arquivo:  Builds/Android/StartupEmpire-debug.apk
Tamanho:  45.621.360 bytes
SHA-256:  BB9B2A69733CB99D300195AC781A7DF1A7AFF9F3F21AE607D0928D651E58153D
Package:  com.startupempire.game
minSdk:   23
target:   36
Orient.:  portrait
```

O APK e os resultados de teste são artefatos locais ignorados pelo Git. O estado verificável e as limitações ficam registrados em [PROGRESS.md](PROGRESS.md) e [CLAUDE.md](CLAUDE.md).

## Gerar APK debug

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.0.82f1\Editor\Unity.exe' `
  -batchmode -nographics -projectPath $PWD `
  -executeMethod StartupEmpire.EditorTools.AndroidBuilder.BuildDebugApk `
  -quit -logFile "$PWD\TestResults\android-build.log"
```

O método configura package id, ícone, portrait e safe area antes de chamar o `BuildPipeline`. Confira o `BuildReport` no log; não considere o artefato válido apenas porque um arquivo antigo existe.

## Save e compatibilidade

- Storage local por arquivo, atrás de `ISaveStorage`.
- Escrita preparada em arquivo temporário antes de substituir o principal.
- Rotação do snapshot anterior para `save_v1.json.bak`.
- Recuperação automática do backup quando o principal está ausente ou corrompido, com restauração best-effort do arquivo principal.
- Autosave periódico e em pause/quit.
- Schema atual V4.
- Migrações encadeadas preservam bugs/testes, calendário e tutorial.
- Campos e definições desconhecidas são tratados defensivamente.
- Novo Jogo remove principal, backup e temporários para não ressuscitar uma campanha apagada.
- Abstração permite cloud save posterior sem acoplar o domínio.

## Backend opcional

O backend em `backend/` fornece:

- submissão e consulta de ranking;
- validação de valores importantes no servidor;
- rate limiting e heurística básica anti-cheat;
- geração e resgate de referral;
- prevenção de autoindicação e resgate duplicado;
- persistência EF Core com PostgreSQL.

A campanha não depende da API. Configurações sensíveis não são commitadas. Veja [backend/README.md](backend/README.md) para execução local, migrations e Docker Compose.

## Segurança e monetização

- Não há secrets, tokens, senhas ou keystores no repositório.
- Gems não estão conectadas a dinheiro real.
- `NullAdService` não simula recompensa falsa.
- Google Play Billing e SDKs de anúncio devem entrar por adapters, não pelo domínio.
- Release signing deve usar segredo externo/CI; nunca commitar `.jks` ou senha.
- A loja evita loot boxes e mecânicas predatórias.

## Limitações conhecidas

- UI interna ainda usa linguagem visual funcional/placeholder.
- Falta tela dedicada de Development; o loop completo está no Office.
- Não há smoke test Android enquanto o emulador permanecer offline.
- Não há AAB, assinatura release ou pipeline CI de distribuição.
- Áudio possui arquitetura/mixagem, mas ainda não contém catálogo final de clipes originais/licenciados.
- Backend ainda não possui autenticação comercial e ownership forte de conta.
- Economia precisa de playtesting e balanceamento de unidade temporal.
- Evento aleatório de PlayMode ainda seria melhor com RNG injetável.

## Roadmap imediato

1. Smoke test em dispositivo/emulador online com logcat.
2. Development screen + scroll e feedback de bloqueios.
3. Balanceamento econômico orientado por testes/simulações.
4. Art/audio polish.
5. AAB, signing externo e CI.

O plano completo está em [PROJECT-PLAN.md](PROJECT-PLAN.md); decisões de design em [GAME-DESIGN-DOCUMENT.md](GAME-DESIGN-DOCUMENT.md); histórico em [CHANGELOG.md](CHANGELOG.md).

## Convenções de contribuição

- Commits pequenos e semânticos: `feat(economy): ...`, `fix(save): ...`, `test(ui): ...`.
- Regra de negócio nova deve ser testada fora da UI sempre que determinística.
- Mudanças no save exigem incremento de schema e migração.
- Integrações externas precisam de interface e fallback offline.
- Nunca marcar build/teste como concluído sem evidência executada.
- Preserve alterações não relacionadas existentes na working tree.

## Licença

Ainda não há licença pública definida. Até uma licença ser adicionada, todos os direitos permanecem reservados ao autor do projeto.
