using System;
using System.Collections.Generic;
using StartupEmpire.Products;

namespace StartupEmpire.Save
{
    /// Aplica migrações em cadeia para nunca perder progresso quando um campo novo
    /// é adicionado ao save (seção 25 da missão). A V2 preserva bugs/testes;
    /// a V3 introduz o calendário e a V4 persiste o tutorial contextual.
    public static class SaveMigrator
    {
        public const int CurrentSchemaVersion = 4;

        public static SaveDataV1 MigrateToCurrent(SaveDataV1 data)
        {
            if (data == null) return null;
            if (data.SchemaVersion < 1) data.SchemaVersion = 1;

            // Saves anteriores à introdução de Ranking/Referrals não tinham PlayerId —
            // gera um novo aqui em vez de mandar string vazia pro backend.
            if (string.IsNullOrEmpty(data.PlayerId)) data.PlayerId = Guid.NewGuid().ToString("N");

            data.Products ??= new List<ProductSaveEntry>();
            data.Knowledge ??= new List<KnowledgeEntry>();
            data.CompletedMissionIds ??= new List<string>();
            data.UnlockedAchievementIds ??= new List<string>();
            data.UpgradeLevels ??= new List<UpgradeLevelEntry>();
            data.Employees ??= new List<EmployeeSaveEntry>();
            data.Competitors ??= new List<CompetitorSaveEntry>();
            data.RaisedInvestmentRounds ??= new List<string>();
            data.ActiveBoosts ??= new List<ActiveBoostSaveEntry>();
            data.PurchasedCosmeticIds ??= new List<string>();

            if (data.SchemaVersion < 2)
            {
                foreach (var product in data.Products)
                {
                    // Antes da V2 todos os bugs eram visíveis e o teste não era
                    // persistido. Preservar isso evita regredir saves existentes.
                    product.KnownBugCount = Math.Max(0, product.BugCount);
                    if (Enum.TryParse<ProductStage>(product.Stage, out var stage))
                    {
                        product.HasBeenTested = stage == ProductStage.Testing ||
                            stage == ProductStage.Launched || stage == ProductStage.Maintenance ||
                            stage == ProductStage.Discontinued;
                    }
                }
                data.SchemaVersion = 2;
            }

            if (data.SchemaVersion < 3)
            {
                data.WorkCyclesPerDay = 4;
                data.CurrentDay = 1;
                data.RemainingWorkCycles = data.WorkCyclesPerDay;
                data.SchemaVersion = 3;
            }

            if (data.SchemaVersion < 4)
            {
                var alreadyProgressed = false;
                foreach (var product in data.Products)
                {
                    if (Enum.TryParse<ProductStage>(product.Stage, out var stage) &&
                        (stage == ProductStage.Launched || stage == ProductStage.Maintenance ||
                         stage == ProductStage.Discontinued))
                    {
                        alreadyProgressed = true;
                        break;
                    }
                }
                data.TutorialStep = alreadyProgressed ? "Completed" : "LearnFundamentals";
                data.SchemaVersion = 4;
            }

            return data;
        }
    }
}
