namespace StartupEmpire.Upgrades
{
    /// O que um upgrade efetivamente multiplica no resto do domínio. Novos efeitos
    /// só precisam de um novo valor aqui + um consumidor que chame UpgradeService.GetMultiplier.
    public enum UpgradeEffectType
    {
        DevSpeedMultiplier,
        BugRateReduction,
        AcquisitionRateMultiplier,
        KnowledgeGainMultiplier
    }
}
