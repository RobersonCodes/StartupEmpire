using System.Collections.Generic;

namespace StartupEmpire.Save
{
    /// Aplica migrações em cadeia para nunca perder progresso quando um campo novo
    /// é adicionado ao save (seção 25 da missão). Hoje só existe V1; futuras versões
    /// devem encadear MigrateVNToVN+1 aqui.
    public static class SaveMigrator
    {
        public const int CurrentSchemaVersion = 1;

        public static SaveDataV1 MigrateToCurrent(SaveDataV1 data)
        {
            if (data == null) return null;
            if (data.SchemaVersion < 1) data.SchemaVersion = 1;

            data.Products ??= new List<ProductSaveEntry>();
            data.Knowledge ??= new List<KnowledgeEntry>();
            data.CompletedMissionIds ??= new List<string>();
            data.UnlockedAchievementIds ??= new List<string>();

            return data;
        }
    }
}
