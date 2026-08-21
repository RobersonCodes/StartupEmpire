# Checkpoint — onde paramos e por onde continuar

Última atualização: 2026-08-20, sessão em andamento implementando **Ranking/Backend** e **Referrals** (seções 23 e 24 da missão).

## Estado exato agora

Estou no meio da implementação do backend (`backend/StartupEmpire.Api` + `backend/StartupEmpire.Api.Tests`), ainda **nada disso foi commitado** (`git status` mostra `?? backend/` — pasta inteira não rastreada). Antes disso, o cliente Unity (Assets/Game/**) está com **26 commits**, 67/67 testes reais passando (`Tests.NET`), cobrindo Core, Economy, Products, Progression, Research, Missions, Achievements, Upgrades, Employees, Events, Competitors, Investment, Premium (Gems) e Store. Isso está tudo commitado e estável — não precisa retrabalhar.

### O que já existe em `backend/` (criado, ainda não commitado)

- `StartupEmpire.Api/StartupEmpire.Api.csproj` — projeto ASP.NET Core Minimal API, `net10.0`, com `Microsoft.AspNetCore.OpenApi`, `Microsoft.OpenApi` (pinado em 2.12.2 — a versão 2.0.0 default tinha vulnerabilidade alta, e a 3.x quebra o source generator do OpenApi 10.0.9), `Microsoft.EntityFrameworkCore` 10.0.4, `Npgsql.EntityFrameworkCore.PostgreSQL`, `Microsoft.EntityFrameworkCore.Design` 10.0.4. Builda limpo (0 avisos, 0 erros).
- `StartupEmpire.Api.Tests/StartupEmpire.Api.Tests.csproj` — projeto xUnit referenciando o Api, com `Microsoft.EntityFrameworkCore.Sqlite` 10.0.4 (para testes de integração reais contra um motor relacional de verdade, sem depender do Postgres local) e `Microsoft.AspNetCore.Mvc.Testing` 10.0.4. Também precisou pinar `SQLitePCLRaw.lib.e_sqlite3` em 3.53.3 (a versão transitiva 2.1.11 tinha vulnerabilidade alta). Builda limpo.
- Domínio puro (sem EF/ASP.NET), já escrito e compilando:
  - `Domain/Common/IClock.cs`, `SystemClock.cs`
  - `Domain/Ranking/RankingMetric.cs` (NetWorth, Valuation, MonthlyRecurringRevenue, Progress, Achievements — as 5 métricas da seção 23)
  - `Domain/Ranking/RankingEntry.cs`, `RankingConfigValues.cs`, `RankingSubmissionResult.cs`
  - `Domain/Ranking/RankingService.cs` — **já implementa validação server-side de verdade** (seção 23: "valide dados importantes no servidor"): rejeita dados inválidos (negativos/NaN/Infinity), rate-limit por `MinSubmissionInterval`, e um heurístico anti-cheat de "crescimento implausível" (`MaxPlausibleGrowthMultiple`).
  - `Domain/Referrals/ReferralCode.cs`, `ReferralRedemption.cs`, `ReferralConfigValues.cs`, `ReferralRedemptionResult.cs`
  - `Domain/Referrals/ReferralService.cs` — **já implementa geração de código, vínculo inviter/invitee, recompensa e prevenção de abuso** (seção 24): rejeita auto-indicação, no máximo 1 resgate por convidado na vida toda, teto de resgates por indicador (`MaxRedemptionsPerInviter`).
- `Data/AppDbContext.cs` — mapeamento EF Core (`RankingEntry`, `ReferralCode`, `ReferralRedemption`), com índices únicos (`PlayerId`, `OwnerPlayerId`, `InviteePlayerId` — o índice único em `InviteePlayerId` reforça no banco a mesma regra "um resgate por convidado" já validada no `ReferralService`, defesa em profundidade).
- `Data/Repositories/EfRankingRepository.cs` — implementação EF de `IRankingRepository` (upsert, top N por métrica, cálculo de posição/rank).

### O que falta (nesta ordem)

1. **`Data/Repositories/EfReferralRepository.cs`** — implementação EF de `IReferralRepository` (símile do `EfRankingRepository`, já desenhada no plano da sessão — ver seção "Design já decidido" abaixo).
2. **Contracts** (`Contracts/Ranking/*.cs`, `Contracts/Referrals/*.cs`) — DTOs de request/response (`SubmitRankingRequest`, `RankingEntryResponse`, `SubmitRankingResponse`, `GetOrCreateReferralCodeRequest`, `ReferralCodeResponse`, `RedeemReferralRequest`, `RedeemReferralResponse`).
3. **Endpoints** (`Endpoints/RankingEndpoints.cs`, `Endpoints/ReferralEndpoints.cs`) — Minimal API, `MapGroup("/api/ranking")` e `MapGroup("/api/referrals")`. Mapear `RankingSubmissionStatus.RejectedRateLimited` para HTTP 429, o resto de rejeição para 400.
4. **Reescrever `Program.cs`** por completo — hoje ainda é o template padrão (`/weatherforecast`). Precisa: `AddDbContext<AppDbContext>` com `UseNpgsql(connectionString)` (connection string vinda de `builder.Configuration.GetConnectionString("Default")`, com fallback óbvio só para dev), registrar `IClock`, `RankingConfigValues`, `ReferralConfigValues`, `IRankingRepository→EfRankingRepository`, `IReferralRepository→EfReferralRepository`, `RankingService`, `ReferralService`, chamar `app.MapRankingEndpoints()` / `app.MapReferralEndpoints()`, endpoint `/health`, e terminar com `public partial class Program { }` (necessário para `WebApplicationFactory<Program>` nos testes de integração).
5. **Migração inicial EF Core** (`dotnet ef migrations add InitialCreate` dentro de `StartupEmpire.Api`, targetando Npgsql).
6. **Testes reais**:
   - `RankingServiceTests.cs` — testes de unidade puros usando um `IRankingRepository` fake em memória (sem banco), cobrindo: validação rejeita dados inválidos, rate-limit rejeita submissão muito rápida, heurístico de crescimento implausível rejeita, submissão válida é aceita e faz upsert.
   - `ReferralServiceTests.cs` — idem com `IReferralRepository` fake: gera código único, rejeita auto-indicação, rejeita segundo resgate do mesmo convidado, rejeita ao atingir `MaxRedemptionsPerInviter`, sucesso retorna as recompensas configuradas.
   - `RankingEndpointsIntegrationTests.cs` / `ReferralEndpointsIntegrationTests.cs` — via `WebApplicationFactory<Program>`, substituindo o `AppDbContext` real por um configurado com `UseSqlite` em conexão SQLite em memória aberta manualmente (padrão: abrir uma `SqliteConnection("DataSource=:memory:")`, chamar `.Open()`, e registrar via `options.UseSqlite(connection)` no `ConfigureTestServices`) — isso dá evidência real de execução ponta a ponta (HTTP → EF → banco relacional de verdade) sem depender do Postgres local.
7. Rodar `dotnet build` e `dotnet test` no backend inteiro, corrigir o que aparecer, confirmar saída real (sem alegar sucesso sem rodar).
8. Commitar em pedaços semânticos, seguindo o padrão já usado no resto do repo (`feat(ranking): ...`, `feat(referrals): ...`, `test: ...`).
9. **Cliente Unity** (`Assets/Game/Ranking/`, `Assets/Game/Referrals/`) — abstrações puras espelhando o padrão `IAdService` da missão (seção 22): `IRankingClient`/`IReferralClient` com uma implementação nula/offline segura por padrão (ranking "nunca deverá bloquear a campanha" — seção 23) e um adapter `UnityWebRequest` real (não testável sem o Editor, mas escrito e coerente com o resto do código).
10. Atualizar `PROGRESS.md`, `PROJECT-PLAN.md`, `CHANGELOG.md` (seções 23 e 24 → `[COMPLETED]`), documentar o bloqueio de credenciais do Postgres (ver abaixo) e commitar.

## Bloqueio de ambiente conhecido (não é erro, é decisão consciente)

Há um **PostgreSQL 16 nativo rodando de verdade nesta máquina** (serviço Windows `postgresql-x64-16`, escutando em `127.0.0.1:5432`, cliente `psql` em `C:\Program Files\PostgreSQL\16\bin\psql.exe`). `pg_hba.conf` exige `scram-sha-256` (senha) em todas as conexões locais — **não há trust auth**. Eu não tenho a senha e **não devo tentar adivinhar/forçar** (é um Postgres que provavelmente serve outros projetos do usuário, não só este). Por isso:

- O backend é escrito 100% Postgres-ready (Npgsql, migrations EF Core normais).
- A verificação automatizada real nesta sessão usa **SQLite em memória** nos testes de integração (motor relacional de verdade, não um mock — dá evidência real de execução).
- Conectar de fato no Postgres local (rodar a migration, validar end-to-end) fica pendente até o usuário fornecer uma connection string (usuário/senha) — via variável de ambiente `ConnectionStrings__Default` ou `dotnet user-secrets`, nunca hardcoded no repo.
- Isso é o mesmo padrão já usado para o bloqueio do Unity Editor (documentar, seguir em frente com o que dá pra verificar de verdade agora, não fingir que "funciona" sem rodar).

## Design já decidido (para não redecidir do zero)

- `EfReferralRepository` deve seguir exatamente o mesmo estilo do `EfRankingRepository`: injeta `AppDbContext`, cada método do `IReferralRepository` vira uma query/gravação direta, sem lógica de negócio (isso já mora no `ReferralService`).
- Enums de rejeição (`RankingSubmissionStatus`, `ReferralRedemptionStatus`) devem virar strings na resposta HTTP (`.ToString()`), não códigos numéricos — mais legível pro cliente Unity.
- `RankingSubmissionStatus.RejectedRateLimited` → HTTP 429; outras rejeições → HTTP 400; sucesso → 200.
- Nenhuma autenticação real ainda (`PlayerId` é um GUID gerado pelo cliente, tipo device id) — a missão trata "autenticação futura" como item separado (seção 3), então isso é dívida técnica documentada, não esquecimento.
- `Program.cs` precisa terminar com `public partial class Program { }` para o `WebApplicationFactory<Program>` funcionar nos testes de integração — não esquecer, é uma pegadinha comum do minimal API.

## Como retomar

Se esta sessão for interrompida, o próximo passo é literalmente o item 1 da lista "O que falta" acima: criar `Data/Repositories/EfReferralRepository.cs`. Depois seguir a lista em ordem. Ao terminar cada bloco funcional, rodar:

```
cd backend && dotnet build StartupEmpire.Api && dotnet build StartupEmpire.Api.Tests
cd backend/StartupEmpire.Api.Tests && dotnet test
```

e só then commitar. Este arquivo (`handoff/CHECKPOINT.md`) deve ser atualizado ou apagado quando o trabalho de Ranking/Referrals estiver completo e commitado — ele existe só como ponto de retomada, não é documentação permanente do projeto (essa função é do `PROGRESS.md`/`PROJECT-PLAN.md` na raiz).
