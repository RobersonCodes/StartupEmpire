using NUnit.Framework;
using StartupEmpire.Core;
using StartupEmpire.Economy;
using StartupEmpire.Products;
using StartupEmpire.Progression;

namespace StartupEmpire.Tests.EditMode
{
    public class ProgressionServiceTests
    {
        [Test]
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

            Assert.IsTrue(advanced);
            Assert.AreEqual(CompanyStage.Freelancer, state.Stage);
            Assert.IsNotNull(received);
            Assert.AreEqual(CompanyStage.PessoaFisica, received.Value.PreviousStage);
        }

        [Test]
        public void TryAdvance_ReturnsFalse_WhenConditionNotMet()
        {
            var service = new ProgressionService(null);
            var state = new GameState(new PlayerState(), new EconomyState(0));

            var advanced = service.TryAdvance(state);

            Assert.IsFalse(advanced);
            Assert.AreEqual(CompanyStage.PessoaFisica, state.Stage);
        }
    }
}
