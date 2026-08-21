namespace StartupEmpire.Core
{
    /// Resultado explícito de uma ação iniciada pela UI. Falhas não alteram
    /// estado nem consomem tempo e sempre carregam um motivo apresentável ao jogador.
    public readonly struct GameActionResult
    {
        public bool Success { get; }
        public string Message { get; }
        public int Amount { get; }

        private GameActionResult(bool success, string message, int amount)
        {
            Success = success;
            Message = message;
            Amount = amount;
        }

        public static GameActionResult Completed(string message, int amount = 0) =>
            new(true, message, amount);

        public static GameActionResult Rejected(string message) => new(false, message, 0);
    }
}
