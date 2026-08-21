namespace StartupEmpire.Save
{
    /// Optional capability for storages that keep the previous valid snapshot.
    /// Cloud implementations may rely on provider-side history instead and only
    /// implement ISaveStorage.
    public interface IRecoverableSaveStorage : ISaveStorage
    {
        bool BackupExists();
        string ReadBackupRaw();
        void RestoreBackup();
    }
}
