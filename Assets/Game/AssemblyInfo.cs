using System.Runtime.CompilerServices;

// Os setters `internal` de GameState/ProductState/EconomyState etc. são pensados
// para serem mutáveis só dentro do domínio (services) — nunca pela UI. O Tests.NET
// (dotnet test) compila tudo numa única assembly, então nunca precisou disso; o
// Unity, com StartupEmpire.Game e StartupEmpire.Tests.EditMode como assemblies
// separadas, precisa desta exceção explícita só para os testes.
[assembly: InternalsVisibleTo("StartupEmpire.Tests.EditMode")]
