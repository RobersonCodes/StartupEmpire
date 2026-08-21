namespace StartupEmpire.Core
{
    public enum TutorialStep
    {
        LearnFundamentals,
        DevelopProduct,
        TestProduct,
        FixKnownBugs,
        LaunchProduct,
        AcquireFirstCustomer,
        Completed
    }

    public static class TutorialGuidance
    {
        public static string MessageFor(TutorialStep step) => step switch
        {
            TutorialStep.LearnFundamentals => "Próximo passo: estude Fundamentos.",
            TutorialStep.DevelopProduct => "Agora desenvolva seu primeiro produto.",
            TutorialStep.TestProduct => "O desenvolvimento terminou. Teste o produto.",
            TutorialStep.FixKnownBugs => "Corrija os bugs descobertos ou assuma o risco.",
            TutorialStep.LaunchProduct => "Produto testado. Faça o primeiro lançamento.",
            TutorialStep.AcquireFirstCustomer => "Encerre dias para conquistar o primeiro cliente pagante.",
            _ => "Tutorial concluído. Construa seu império."
        };
    }
}
