using System.Text.Json;

namespace StartupEmpire.Save
{
    /// Isolado atrás de uma classe própria: se o runtime do Unity Editor instalado
    /// não incluir System.Text.Json, basta trocar a implementação interna por
    /// Newtonsoft.Json (pacote com.unity.nuget.newtonsoft-json) sem tocar no
    /// restante do save system.
    public static class SaveSerializer
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = false,
            // SaveDataV1 usa campos públicos (compatível com UnityEngine.JsonUtility);
            // System.Text.Json só serializa propriedades por padrão, então isso é obrigatório.
            IncludeFields = true
        };

        public static string Serialize(SaveDataV1 data) => JsonSerializer.Serialize(data, Options);

        public static SaveDataV1 Deserialize(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            return JsonSerializer.Deserialize<SaveDataV1>(json, Options);
        }
    }
}
