using NUnit.Framework;
using StartupEmpire.Core;

namespace StartupEmpire.Tests.EditMode
{
    public class WorkScheduleTests
    {
        [Test]
        public void TryConsumeWorkCycles_RejectsWhenDayHasNoTime_WithoutChangingState()
        {
            var player = new PlayerState();

            Assert.IsTrue(player.TryConsumeWorkCycles(4));
            Assert.IsFalse(player.TryConsumeWorkCycles(1));
            Assert.AreEqual(0, player.RemainingWorkCycles);
        }

        [Test]
        public void StartNextDay_IncrementsDayAndRestoresAvailableTime()
        {
            var player = new PlayerState();
            player.TryConsumeWorkCycles(3);

            player.StartNextDay();

            Assert.AreEqual(2, player.CurrentDay);
            Assert.AreEqual(player.WorkCyclesPerDay, player.RemainingWorkCycles);
        }
    }
}
