using System.IO;
using UnityEngine;

namespace StartupEmpire.Save
{
    /// Único ponto do save system que depende do Unity (Application.persistentDataPath).
    /// Escreve em arquivo temporário e substitui o save real só depois — reduz o risco
    /// de corromper o save se o app fechar no meio da escrita.
    public sealed class FileSaveStorage : ISaveStorage
    {
        private readonly string _path;

        public FileSaveStorage(string fileName = "save_v1.json")
        {
            _path = Path.Combine(Application.persistentDataPath, fileName);
        }

        public bool Exists() => File.Exists(_path);

        public string ReadRaw() => File.ReadAllText(_path);

        public void WriteRaw(string json)
        {
            var tmpPath = _path + ".tmp";
            File.WriteAllText(tmpPath, json);
            File.Copy(tmpPath, _path, overwrite: true);
            File.Delete(tmpPath);
        }

        public void Delete()
        {
            if (File.Exists(_path)) File.Delete(_path);
        }
    }
}
