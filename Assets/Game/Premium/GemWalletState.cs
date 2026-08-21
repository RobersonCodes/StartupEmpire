using System.Collections.Generic;

namespace StartupEmpire.Premium
{
    /// Abstração de moeda premium (seção 20 da missão). Sem conexão a pagamento
    /// real ainda — a arquitetura (saldo + ledger + serviço) já comporta plugar
    /// Google Play Billing depois sem mudar quem consome GemWalletState.
    public sealed class GemWalletState
    {
        public int Balance { get; internal set; }
        public List<GemLedgerEntry> Ledger { get; } = new();

        public void Apply(GemLedgerEntry entry)
        {
            Balance += entry.Amount;
            Ledger.Add(entry);
        }

        public bool CanAfford(int amount) => Balance >= amount;
    }
}
