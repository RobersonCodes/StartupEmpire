namespace StartupEmpire.Save
{
    /// Abstrai ONDE o save vive (arquivo local hoje, cloud save no futuro).
    public interface ISaveStorage
    {
        bool Exists();
        string ReadRaw();
        void WriteRaw(string json);
        void Delete();
    }
}
