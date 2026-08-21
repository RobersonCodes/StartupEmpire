namespace StartupEmpire.Api.Domain.Common;

public interface IClock
{
    DateTime UtcNow { get; }
}
