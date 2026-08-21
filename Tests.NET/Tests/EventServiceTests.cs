using System;
using System.Collections.Generic;
using System.Linq;
using StartupEmpire.Core;
using StartupEmpire.Domain.Tests.TestSupport;
using StartupEmpire.Economy;
using StartupEmpire.Events;
using StartupEmpire.Products;
using Xunit;

namespace StartupEmpire.Domain.Tests
{
    public class EventServiceTests
    {
        [Fact]
        public void TryTriggerRandomEvent_ReturnsNull_WhenNoEligibleEvents()
        {
            var config = new EventConfigValues { BaseTriggerChancePerCycle = 1.0 };
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var definitions = new List<GameEventDefinition>
            {
                new GameEventDefinition { Id = "e1", TriggerCondition = _ => false, Choices = new List<EventChoice>() }
            };
            var service = new EventService(definitions, config, economy, null);
            var state = new GameState(new PlayerState(), new EconomyState(0));

            var result = service.TryTriggerRandomEvent(state);

            Assert.Null(result);
        }

        [Fact]
        public void TryTriggerRandomEvent_ReturnsEligibleEvent_WhenChanceGuaranteed()
        {
            var config = new EventConfigValues { BaseTriggerChancePerCycle = 1.0 };
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var definitions = new List<GameEventDefinition>
            {
                new GameEventDefinition { Id = "e1", TriggerCondition = _ => true, Choices = new List<EventChoice>() }
            };
            var eventBus = new EventBus();
            GameEventTriggeredEvent? received = null;
            eventBus.Subscribe<GameEventTriggeredEvent>(e => received = e);
            var service = new EventService(definitions, config, economy, eventBus);
            var state = new GameState(new PlayerState(), new EconomyState(0));

            var result = service.TryTriggerRandomEvent(state);

            Assert.NotNull(result);
            Assert.Equal("e1", result.Id);
            Assert.NotNull(received);
        }

        [Fact]
        public void ResolveChoice_AppliesConsequence_AndPublishesEvent()
        {
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var choiceApplied = false;
            var definition = new GameEventDefinition
            {
                Id = "e1",
                Choices = new List<EventChoice>
                {
                    new EventChoice
                    {
                        Id = "c1",
                        Apply = (state, econ) =>
                        {
                            choiceApplied = true;
                            econ.Earn(state.Economy, 10, "test");
                        }
                    }
                }
            };
            var eventBus = new EventBus();
            GameEventResolvedEvent? received = null;
            eventBus.Subscribe<GameEventResolvedEvent>(e => received = e);
            var service = new EventService(new List<GameEventDefinition> { definition }, new EventConfigValues(), economy, eventBus);
            var state = new GameState(new PlayerState(), new EconomyState(0));

            var result = service.ResolveChoice(state, definition, "c1");

            Assert.True(result);
            Assert.True(choiceApplied);
            Assert.Equal(10, state.Economy.Cash);
            Assert.NotNull(received);
        }

        [Fact]
        public void EventCatalog_ServerDown_InvestInfra_SpendsCashAndImprovesStability()
        {
            var economy = new EconomyEngine(new EconomyConfigValues(), new FakeClock(DateTime.UtcNow), null);
            var definitions = EventCatalog.CreateChapter1Catalog();
            var serverDown = definitions.First(d => d.Id == "server_down");
            var service = new EventService(definitions, new EventConfigValues(), economy, null);
            var state = new GameState(new PlayerState(), new EconomyState(500));
            var def = new ProductDefinition("p", "P", ProductCategory.Website, 100, 10, 0.08);
            state.Products.Add(new ProductState(def) { Stage = ProductStage.Launched, Stability = 0.5 });

            service.ResolveChoice(state, serverDown, "invest_infra");

            Assert.Equal(300, state.Economy.Cash);
            Assert.Equal(0.7, state.Products[0].Stability, 5);
        }
    }
}
