using Microsoft.EntityFrameworkCore;
using StartupEmpire.Api.Data;
using StartupEmpire.Api.Data.Repositories;
using StartupEmpire.Api.Domain.Common;
using StartupEmpire.Api.Domain.Ranking;
using StartupEmpire.Api.Domain.Referrals;
using StartupEmpire.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// A connection string real (com usuário/senha) nunca fica hardcoded aqui — vem de
// appsettings.Development.json (gitignored), variável de ambiente ConnectionStrings__Default,
// ou `dotnet user-secrets`. O fallback abaixo só existe para o projeto não quebrar ao abrir
// sem configuração nenhuma; ele aponta para um banco/usuário que não existe por padrão.
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? "Host=localhost;Port=5432;Database=startup_empire;Username=startup_empire;Password=CHANGE_ME";

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton(new RankingConfigValues());
builder.Services.AddSingleton(new ReferralConfigValues());
builder.Services.AddScoped<IRankingRepository, EfRankingRepository>();
builder.Services.AddScoped<IReferralRepository, EfReferralRepository>();
builder.Services.AddScoped<RankingService>();
builder.Services.AddScoped<ReferralService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapRankingEndpoints();
app.MapReferralEndpoints();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();

// Exposto para WebApplicationFactory<Program> nos testes de integração.
public partial class Program
{
}
