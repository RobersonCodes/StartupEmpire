namespace StartupEmpire.Store
{
    /// Instância em runtime de um boost comprado, contando os ciclos restantes.
    public sealed class ActiveBoost
    {
        public string SourceItemId { get; }
        public StoreItemEffectType EffectType { get; }
        public double Magnitude { get; }
        public int RemainingCycles { get; internal set; }

        public ActiveBoost(string sourceItemId, StoreItemEffectType effectType, double magnitude, int remainingCycles)
        {
            SourceItemId = sourceItemId;
            EffectType = effectType;
            Magnitude = magnitude;
            RemainingCycles = remainingCycles;
        }
    }
}
