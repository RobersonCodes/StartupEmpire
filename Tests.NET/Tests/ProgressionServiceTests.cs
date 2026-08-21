using StartupEmpire.Core;
using StartupEmpire.Economy;
using StartupEmpire.Products;
using StartupEmpire.Progression;
using Xunit;

namespace StartupEmpire.Domain.Tests
{
    public class ProgressionServiceTests
    {
        [Fact]
        public void TryAdvance_MovesToFreelancer_WhenAnyProductHasPayingCustomer()
        {
            var eventBus = new EventBus();
            CompanyStageChangedEvent? received = null;
            eventBus.Subscribe<CompanyStageChangedEvent>(e => received = e);
            var service = new ProgressionService(eventBus);
            var state = new GameState(new PlayerState(), new EconomyState(0));
            var def = new ProductDefinition("p", "P", ProductCategory.Website, 100, 10, 0.08);
            state.Products.Add(new ProductState(def) { PayingCustomers = 1 });

            var advanced = service.TryAdvance(state);

            Assert.True(advanced);
            Assert.Equal(CompanyStage.Freelancer, state.Stage);
            Assert.NotNull(received);
            Assert.Equal(CompanyStage.PessoaFisica, received.Value.PreviousStage);
        }

        [Fact]
        public void TryAdvance_ReturnsFalse_WhenConditionNotMet()
        {
            var service = new ProgressionService(null);
            var state = new GameState(new PlayerState(), new EconomyState(0));

            var advanced = service.TryAdvance(state);

            Assert.False(advanced);
            Assert.Equal(CompanyStage.PessoaFisica, state.Stage);
        }
    }
}
