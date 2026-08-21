using StartupEmpire.Core;
using StartupEmpire.Research;
using Xunit;

namespace StartupEmpire.Domain.Tests
{
    public class LearningServiceTests
    {
        [Fact]
        public void Study_AddsKnowledgeToTrack()
        {
            var service = new LearningService(new LearningConfigValues { KnowledgePointsPerCycle = 5 });
            var player = new PlayerState();

            service.Study(player, KnowledgeTracks.Fundamentos, cycles: 2);

            Assert.Equal(10, player.GetKnowledge(KnowledgeTracks.Fundamentos));
        }

        [Fact]
        public void Study_AppliesKnowledgeMultiplier()
        {
            var service = new LearningService(new LearningConfigValues { KnowledgePointsPerCycle = 10 });
            var player = new PlayerState();

            service.Study(player, KnowledgeTracks.Fundamentos, cycles: 1, knowledgeMultiplier: 1.5);

            Assert.Equal(15, player.GetKnowledge(KnowledgeTracks.Fundamentos));
        }
    }
}
