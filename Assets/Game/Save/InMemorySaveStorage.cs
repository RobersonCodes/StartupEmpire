namespace StartupEmpire.Save
{
    /// Implementação em memória de ISaveStorage — usada em testes automatizados
    /// e como referência para novas implementações (arquivo, cloud, etc).
    public sealed class InMemorySaveStorage : ISaveStorage
    {
        private string _raw;

        public bool Exists() => _raw != null;
        public string ReadRaw() => _raw;
        public void WriteRaw(string json) => _raw = json;
        public void Delete() => _raw = null;
    }
}
