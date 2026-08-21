# Startup Empire — Backend

ASP.NET Core (Minimal API) + PostgreSQL, usado **somente** para Ranking (seção 23) e
Referrals (seção 24) da missão, por enquanto. O jogo principal continua funcionando
100% offline sem este backend — nada aqui é dependência obrigatória para jogar.

## Projetos

- `StartupEmpire.Api/` — a API. `net10.0`, Minimal API, EF Core + Npgsql.
- `StartupEmpire.Api.Tests/` — 22 testes reais via `dotnet test`:
  - `Unit/` — `RankingService`/`ReferralService` testados com repositórios fake em memória, sem banco nenhum.
  - `Integration/` — sobem a API inteira (`Program.cs` real) via `WebApplicationFactory<Program>`, com o `AppDbContext` trocado para **SQLite em memória** (um motor relacional de verdade, não um mock) — cobre HTTP → EF Core → banco ponta a ponta.

## Por que os testes automatizados não usam Postgres

Esta máquina já tem um PostgreSQL 16 **nativo** rodando (serviço Windows
`postgresql-x64-16`, porta 5432), mas exige senha (`scram-sha-256` em `pg_hba.conf`,
sem trust auth) que o agente não tem — e não tentou adivinhar, porque é um Postgres
que provavelmente serve outros projetos seus, não só este. Por isso a suíte de
testes (`dotnet test`) usa SQLite em memória: evidência real de execução, sem tocar
em nada seu.

Isso **não é** o mesmo caso do Postgres usado pra rodar a API de verdade — ver abaixo.

## Rodando contra PostgreSQL de verdade (via Docker, já verificado nesta sessão)

Em vez de mexer no Postgres nativo, este projeto sobe seu **próprio** container
Postgres isolado (`backend/docker-compose.yml`), na porta `5442` — 5432 já está
ocupada pelo nativo, e outros projetos seus já usam 5555/5455 (`docker ps`).

1. Suba o container (Docker Desktop precisa estar rodando):

   ```
   cd backend
   docker compose up -d
   ```

2. Configure a connection string via user-secrets (nunca commitado — vive em
   `%APPDATA%\Microsoft\UserSecrets\<UserSecretsId>\secrets.json`, fora do repo):

   ```
   cd StartupEmpire.Api
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5442;Database=startup_empire;Username=startup_empire;Password=startup_empire_dev"
   ```

   (A senha acima é a definida em `docker-compose.yml` — só vale para este container local, não é segredo real. Em produção, use uma connection string própria via variável de ambiente `ConnectionStrings__Default`.)

3. Aplique a migration:

   ```
   dotnet ef database update
   ```

4. Rode a API **em modo Development** (só assim os user-secrets são carregados):

   ```
   ASPNETCORE_ENVIRONMENT=Development dotnet run
   ```

   `GET /health` deve responder `{"status":"ok"}`.

**Isso já foi executado e verificado nesta sessão**, de ponta a ponta, contra o
Postgres real do container: submissão de ranking, consulta de top/rank, geração de
código de indicação, resgate com sucesso e rejeição correta de um segundo resgate
pelo mesmo convidado — todos confirmados também via `psql` direto no banco (não só
pela resposta HTTP). Não é uma alegação sem evidência.

Para parar o container quando não precisar mais dele: `docker compose down` (ou
`docker compose down -v` para apagar os dados também).

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
