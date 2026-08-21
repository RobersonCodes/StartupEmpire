using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StartupEmpire.Api.Data;

namespace StartupEmpire.Api.Tests.Integration;

/// Sobe a API inteira (Program.cs real, endpoints reais) mas troca o AppDbContext
/// de Npgsql por SQLite em memória — um motor relacional de verdade, não um mock,
/// então os testes de integração são evidência real de HTTP -> EF Core -> banco.
/// Isso evita depender do PostgreSQL local (que exige credenciais que este agente
/// não tem e não deve tentar adivinhar — ver handoff/CHECKPOINT.md).
public sealed class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();

        builder.ConfigureServices(services =>
        {
            // Program.cs registra AppDbContext com Npgsql via AddDbContext(Action<DbContextOptionsBuilder>),
            // que internamente adiciona tanto DbContextOptions<AppDbContext> quanto
            // IDbContextOptionsConfiguration<AppDbContext>. Se só o primeiro for removido,
            // o EF Core enxerga configurações de dois provedores (Npgsql + Sqlite) ao
            // montar as opções finais e lança "Only a single database provider can be
            // registered". Por isso os dois precisam ser removidos aqui.
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();

            services.AddDbContext<AppDbContext>(options => options.UseSqlite(_connection));

            var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing) _connection.Dispose();
    }
}
