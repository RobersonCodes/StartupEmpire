namespace StartupEmpire.Store
{
    /// Item da loja interna (seção 21 da missão): boosts, cosméticos, aceleração,
    /// itens especiais. Preço e efeito sempre visíveis antes da compra — sem caixas
    /// de recompensa aleatórias nem qualquer mecânica que esconda o que o jogador recebe.
    public sealed class StoreItemDefinition
    {
        public string Id { get; }
        public string DisplayName { get; }
        public StoreItemCategory Category { get; }
        public int GemCost { get; }
        public StoreItemEffectType EffectType { get; }
        public double EffectMagnitude { get; }
        public int DurationCycles { get; }

        public StoreItemDefinition(string id, string displayName, StoreItemCategory category, int gemCost,
            StoreItemEffectType effectType, double effectMagnitude, int durationCycles)
        {
            Id = id;
            DisplayName = displayName;
            Category = category;
            GemCost = gemCost;
            EffectType = effectType;
            EffectMagnitude = effectMagnitude;
            DurationCycles = durationCycles;
        }
    }
}
