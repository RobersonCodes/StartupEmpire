using System;
using System.Collections.Generic;
using StartupEmpire.Core;
using StartupEmpire.Economy;

namespace StartupEmpire.Investment
{
    public sealed class InvestmentService
    {
        private readonly List<InvestmentOffer> _offers;
        private readonly EconomyEngine _economy;
        private readonly EventBus _eventBus;

        public InvestmentService(List<InvestmentOffer> offers, EconomyEngine economy, EventBus eventBus)
        {
            _offers = offers ?? throw new ArgumentNullException(nameof(offers));
            _economy = economy ?? throw new ArgumentNullException(nameof(economy));
            _eventBus = eventBus;
        }

        public IReadOnlyList<InvestmentOffer> Offers => _offers;

        public InvestmentOffer Find(InvestmentRoundType roundType) => _offers.Find(o => o.RoundType == roundType);

        /// Uma rodada só pode ser aceita uma vez, exige o estágio mínimo de empresa
        /// e o valuation mínimo — captar não é uma recompensa gratuita.
        public bool IsEligible(InvestmentOffer offer, GameState state)
        {
            if (state.RaisedInvestmentRounds.Contains(offer.RoundType)) return false;
            if (state.Stage < offer.MinStageRequired) return false;
            if (state.Economy.Valuation < offer.MinValuationRequired) return false;
            return true;
        }

        public bool TryAcceptOffer(GameState state, InvestmentOffer offer)
        {
            if (!IsEligible(offer, state)) return false;

            _economy.ApplyInvestment(state.Economy, offer.CashAmount, offer.EquityPercentRequested);
            state.RaisedInvestmentRounds.Add(offer.RoundType);
            _eventBus?.Publish(new InvestmentAcceptedEvent(offer.RoundType, offer.CashAmount, offer.EquityPercentRequested));
            return true;
        }
    }
}
