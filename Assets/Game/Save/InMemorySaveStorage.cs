namespace StartupEmpire.Save
{
    /// Implementação em memória de ISaveStorage — usada em testes automatizados
    /// e como referência para novas implementações (arquivo, cloud, etc).
    public sealed class InMemorySaveStorage : IRecoverableSaveStorage
    {
        private string _raw;
        private string _backupRaw;

        public bool Exists() => _raw != null;
        public string ReadRaw() => _raw;
        public bool BackupExists() => _backupRaw != null;
        public string ReadBackupRaw() => _backupRaw;

        public void WriteRaw(string json)
        {
            if (_raw != null) _backupRaw = _raw;
            _raw = json;
        }

        public void RestoreBackup()
        {
            if (_backupRaw != null) _raw = _backupRaw;
        }

        public void Delete()
        {
            _raw = null;
            _backupRaw = null;
        }
    }
}
