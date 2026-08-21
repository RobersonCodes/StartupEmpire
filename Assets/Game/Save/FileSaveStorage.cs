using System.IO;
using UnityEngine;

namespace StartupEmpire.Save
{
    /// Único ponto do save system que depende do Unity (Application.persistentDataPath).
    /// Escreve em arquivo temporário e substitui o save real só depois — reduz o risco
    /// de corromper o save se o app fechar no meio da escrita.
    public sealed class FileSaveStorage : IRecoverableSaveStorage
    {
        private readonly string _path;
        private readonly string _backupPath;

        public FileSaveStorage(string fileName = "save_v1.json")
        {
            _path = Path.Combine(Application.persistentDataPath, fileName);
            _backupPath = _path + ".bak";
        }

        public bool Exists() => File.Exists(_path);

        public string ReadRaw() => File.ReadAllText(_path);
        public bool BackupExists() => File.Exists(_backupPath);
        public string ReadBackupRaw() => File.ReadAllText(_backupPath);

        public void WriteRaw(string json)
        {
            var tmpPath = _path + ".tmp";
            try
            {
                File.WriteAllText(tmpPath, json);
                if (File.Exists(_path)) File.Copy(_path, _backupPath, overwrite: true);
                File.Copy(tmpPath, _path, overwrite: true);
            }
            finally
            {
                if (File.Exists(tmpPath)) File.Delete(tmpPath);
            }
        }

        public void RestoreBackup()
        {
            if (!File.Exists(_backupPath)) return;
            var tmpPath = _path + ".restore.tmp";
            try
            {
                File.Copy(_backupPath, tmpPath, overwrite: true);
                File.Copy(tmpPath, _path, overwrite: true);
            }
            finally
            {
                if (File.Exists(tmpPath)) File.Delete(tmpPath);
            }
        }

        public void Delete()
        {
            if (File.Exists(_path)) File.Delete(_path);
            if (File.Exists(_backupPath)) File.Delete(_backupPath);
            var tmpPath = _path + ".tmp";
            if (File.Exists(tmpPath)) File.Delete(tmpPath);
            var restoreTmpPath = _path + ".restore.tmp";
            if (File.Exists(restoreTmpPath)) File.Delete(restoreTmpPath);
        }
    }
}
