using StartupEmpire.Core;
using Xunit;

namespace StartupEmpire.Domain.Tests
{
    public class WorkScheduleTests
    {
        [Fact]
        public void TryConsumeWorkCycles_RejectsWhenDayHasNoTime_WithoutChangingState()
        {
            var player = new PlayerState();

            Assert.True(player.TryConsumeWorkCycles(4));
            Assert.False(player.TryConsumeWorkCycles(1));
            Assert.Equal(0, player.RemainingWorkCycles);
        }

        [Fact]
        public void StartNextDay_IncrementsDayAndRestoresAvailableTime()
        {
            var player = new PlayerState();
            player.TryConsumeWorkCycles(3);

            player.StartNextDay();

            Assert.Equal(2, player.CurrentDay);
            Assert.Equal(player.WorkCyclesPerDay, player.RemainingWorkCycles);
        }
    }
}
