using Microsoft.EntityFrameworkCore;
using StartupEmpire.Api.Domain.Ranking;
using StartupEmpire.Api.Domain.Referrals;

namespace StartupEmpire.Api.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<RankingEntry> RankingEntries => Set<RankingEntry>();
    public DbSet<ReferralCode> ReferralCodes => Set<ReferralCode>();
    public DbSet<ReferralRedemption> ReferralRedemptions => Set<ReferralRedemption>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RankingEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.PlayerId).IsUnique();
            entity.Property(e => e.PlayerId).HasMaxLength(128);
            entity.Property(e => e.DisplayName).HasMaxLength(64);
        });

        modelBuilder.Entity<ReferralCode>(entity =>
        {
            entity.HasKey(e => e.Code);
            entity.Property(e => e.Code).HasMaxLength(16);
            entity.HasIndex(e => e.OwnerPlayerId).IsUnique();
            entity.Property(e => e.OwnerPlayerId).HasMaxLength(128);
        });

        modelBuilder.Entity<ReferralRedemption>(entity =>
        {
            entity.HasKey(e => e.Id);
            // Reforça no banco a mesma regra de negócio (um resgate por convidado)
            // já validada em ReferralService — defesa em profundidade.
            entity.HasIndex(e => e.InviteePlayerId).IsUnique();
            entity.Property(e => e.Code).HasMaxLength(16);
            entity.Property(e => e.InviterPlayerId).HasMaxLength(128);
            entity.Property(e => e.InviteePlayerId).HasMaxLength(128);
        });
    }
}
