using Newtonsoft.Json;

namespace StartupEmpire.Save
{
    /// Isolado atrás de uma classe própria por design. Já trocou de implementação
    /// uma vez: começou com System.Text.Json (funcionava no `dotnet test`, mas o
    /// Unity Editor não tem esse namespace disponível no perfil de API padrão —
    /// erro CS0234 confirmado na primeira compilação real no Editor). Agora usa
    /// Newtonsoft.Json (pacote com.unity.nuget.newtonsoft-json no Unity, pacote
    /// NuGet Newtonsoft.Json no Tests.NET), que roda nos dois ambientes.
    public static class SaveSerializer
    {
        private static readonly JsonSerializerSettings Settings = new()
        {
            Formatting = Formatting.None
        };

        public static string Serialize(SaveDataV1 data) => JsonConvert.SerializeObject(data, Settings);

        public static SaveDataV1 Deserialize(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            return JsonConvert.DeserializeObject<SaveDataV1>(json, Settings);
        }
    }
}
