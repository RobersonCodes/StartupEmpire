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

### Known limitations
- Unity Editor e Android SDK não estão instalados nesta máquina (bloqueio de ambiente — ver `PROJECT-PLAN.md`); UI de telas, áudio, arte final e build Android (APK/AAB) ainda não foram implementados/gerados.
