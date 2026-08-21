namespace StartupEmpire.Store
{
    public enum StoreItemEffectType
    {
        /// Boost temporário (N ciclos) de velocidade de desenvolvimento.
        DevSpeedBoost,
        /// Boost temporário (N ciclos) de aquisição de clientes.
        AcquisitionBoost,
        /// Bônus de caixa instantâneo, uma vez.
        InstantCashBonus,
        /// Item puramente visual, sem efeito em regra de negócio.
        CosmeticOnly
    }
}
