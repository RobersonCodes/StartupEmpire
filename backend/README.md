# Startup Empire — Backend

ASP.NET Core (Minimal API) + PostgreSQL, usado **somente** para Ranking (seção 23) e
Referrals (seção 24) da missão, por enquanto. O jogo principal continua funcionando
100% offline sem este backend — nada aqui é dependência obrigatória para jogar.

## Projetos

- `StartupEmpire.Api/` — a API. `net10.0`, Minimal API, EF Core + Npgsql.
- `StartupEmpire.Api.Tests/` — 22 testes reais via `dotnet test`:
  - `Unit/` — `RankingService`/`ReferralService` testados com repositórios fake em memória, sem banco nenhum.
  - `Integration/` — sobem a API inteira (`Program.cs` real) via `WebApplicationFactory<Program>`, com o `AppDbContext` trocado para **SQLite em memória** (um motor relacional de verdade, não um mock) — cobre HTTP → EF Core → banco ponta a ponta.

## Por que os testes não usam o PostgreSQL local

Esta máquina já tem um PostgreSQL 16 nativo rodando (serviço Windows `postgresql-x64-16`,
porta 5432), mas exige senha (`scram-sha-256` em `pg_hba.conf`, sem trust auth). O agente
que escreveu este backend não tem — e não tentou adivinhar — essa senha, porque é um
Postgres que provavelmente serve outros projetos seus, não só este. Por isso os testes
automatizados usam SQLite em memória (evidência real de execução, sem tocar em nada seu).
Ver `handoff/CHECKPOINT.md` na raiz do repositório para mais contexto.

## Rodando contra o PostgreSQL local de verdade

1. Crie um banco e um usuário dedicados a este projeto (não reutilize um login existente):

   ```
   "C:\Program Files\PostgreSQL\16\bin\psql.exe" -U postgres -c "CREATE USER startup_empire WITH PASSWORD 'escolha-uma-senha';"
   "C:\Program Files\PostgreSQL\16\bin\psql.exe" -U postgres -c "CREATE DATABASE startup_empire OWNER startup_empire;"
   ```

2. Configure a connection string (nunca hardcode senha em arquivo versionado):

   ```
   cd StartupEmpire.Api
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=startup_empire;Username=startup_empire;Password=escolha-uma-senha"
   ```

   (Ou defina a variável de ambiente `ConnectionStrings__Default` com o mesmo valor.)

3. Aplique a migration inicial:

   ```
   dotnet ef database update
   ```

4. Rode a API:

   ```
   dotnet run
   ```

   `GET /health` deve responder `{"status":"ok"}`.

## Endpoints

### Ranking (`/api/ranking`)

- `POST /submit` — `{ playerId, displayName, netWorth, valuation, monthlyRecurringRevenue, progressStageIndex, achievementCount }`. Validado no servidor (seção 23): rejeita dados inválidos (negativos/NaN/Infinity), aplica rate-limit (`RankingConfigValues.MinSubmissionInterval`, padrão 2 min) e um heurístico anti-cheat de crescimento implausível (`MaxPlausibleGrowthMultiple`, padrão 1000x desde a última submissão). Upsert por `PlayerId` — uma linha de ranking por jogador.
- `GET /top?metric=Valuation&limit=50` — top N ordenado pela métrica (`NetWorth`, `Valuation`, `MonthlyRecurringRevenue`, `Progress`, `Achievements`).
- `GET /me/{playerId}?metric=Valuation` — posição (rank) do jogador nessa métrica.

### Referrals (`/api/referrals`)

- `POST /code` — `{ playerId }` → gera (ou retorna o já existente) código de indicação do jogador.
- `POST /redeem` — `{ code, inviteePlayerId }` → resgata um código. Prevenção de abuso (seção 24): rejeita auto-indicação, no máximo um resgate por convidado na vida toda, teto de resgates por indicador (`ReferralConfigValues.MaxRedemptionsPerInviter`, padrão 20). Sucesso devolve `inviterRewardGems`/`inviteeRewardGems` — quem efetivamente credita os Gems é o cliente Unity (`GemWalletService`), o backend só autoriza e informa os valores.

## Autenticação

Não implementada ainda — `PlayerId` é um identificador gerado pelo próprio cliente (tipo
device/install id), sem verificação de posse. A seção 3 da missão trata autenticação como
item futuro separado; isso é dívida técnica documentada, não um esquecimento.
