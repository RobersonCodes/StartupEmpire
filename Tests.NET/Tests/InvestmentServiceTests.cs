using System;
using System.Collections.Generic;
using StartupEmpire.Core;
using StartupEmpire.Domain.Tests.TestSupport;
using StartupEmpire.Economy;
using StartupEmpire.Investment;
using StartupEmpire.Progression;
using Xunit;

namespace StartupEmpire.Domain.Tests
{
    public class InvestmentServiceTests
    {
        [Fact]
        public void IsEligible_ReturnsFalse_WhenStageTooLow()
        {
            var offer = new InvestmentOffer(InvestmentRoundType.Seed, "Seed", 1000, 0.1, 0, CompanyStage.Microempresa);
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var service = new InvestmentService(new List<InvestmentOffer> { offer }, economy, null);
            var state = new GameState(new PlayerState(), new EconomyState(0)) { Stage = CompanyStage.Freelancer };

            Assert.False(service.IsEligible(offer, state));
        }

        [Fact]
        public void IsEligible_ReturnsFalse_WhenValuationTooLow()
        {
            var offer = new InvestmentOffer(InvestmentRoundType.Seed, "Seed", 1000, 0.1, minValuationRequired: 5000, CompanyStage.Microempresa);
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var service = new InvestmentService(new List<InvestmentOffer> { offer }, economy, null);
            var economyState = new EconomyState(0) { Valuation = 1000 };
            var state = new GameState(new PlayerState(), economyState) { Stage = CompanyStage.Microempresa };

            Assert.False(service.IsEligible(offer, state));
        }

        [Fact]
        public void TryAcceptOffer_AddsCash_DilutesFounderEquity_AndMarksRoundRaised()
        {
            var offer = new InvestmentOffer(InvestmentRoundType.Angel, "Angel", cashAmount: 5000,
                equityPercentRequested: 0.2, minValuationRequired: 0, CompanyStage.Freelancer);
            var eventBus = new EventBus();
            InvestmentAcceptedEvent? received = null;
            eventBus.Subscribe<InvestmentAcceptedEvent>(e => received = e);
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var service = new InvestmentService(new List<InvestmentOffer> { offer }, economy, eventBus);
            var state = new GameState(new PlayerState(), new EconomyState(0)) { Stage = CompanyStage.Freelancer };

            var result = service.TryAcceptOffer(state, offer);

            Assert.True(result);
            Assert.Equal(5000, state.Economy.Cash);
            Assert.Equal(0.8, state.Economy.FounderEquity, 5);
            Assert.Contains(InvestmentRoundType.Angel, state.RaisedInvestmentRounds);
            Assert.NotNull(received);
        }

        [Fact]
        public void TryAcceptOffer_Fails_WhenRoundAlreadyRaised()
        {
            var offer = new InvestmentOffer(InvestmentRoundType.Angel, "Angel", 5000, 0.2, 0, CompanyStage.Freelancer);
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var service = new InvestmentService(new List<InvestmentOffer> { offer }, economy, null);
            var state = new GameState(new PlayerState(), new EconomyState(0)) { Stage = CompanyStage.Freelancer };
            service.TryAcceptOffer(state, offer);

            var secondAttempt = service.TryAcceptOffer(state, offer);

            Assert.False(secondAttempt);
            Assert.Equal(5000, state.Economy.Cash);
        }

        [Fact]
        public void ApplyInvestment_CompoundsDilutionAcrossMultipleRounds()
        {
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var economyState = new EconomyState(0);

            economy.ApplyInvestment(economyState, 1000, 0.1);
            economy.ApplyInvestment(economyState, 1000, 0.2);

            Assert.Equal(0.72, economyState.FounderEquity, 5);
            Assert.Equal(2000, economyState.Cash);
        }
    }
}
