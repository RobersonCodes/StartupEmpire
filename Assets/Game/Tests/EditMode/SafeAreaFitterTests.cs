using NUnit.Framework;
using UnityEngine;
using StartupEmpire.UI;

namespace StartupEmpire.Tests.EditMode
{
    public class SafeAreaFitterTests
    {
        [Test]
        public void CalculateAnchors_NormalizesNotchAndGestureInsets()
        {
            SafeAreaFitter.CalculateAnchors(
                new Rect(0, 80, 1080, 2240),
                new Vector2Int(1080, 2400),
                out var min,
                out var max);

            Assert.AreEqual(0f, min.x, 0.0001f);
            Assert.AreEqual(80f / 2400f, min.y, 0.0001f);
            Assert.AreEqual(1f, max.x, 0.0001f);
            Assert.AreEqual(2320f / 2400f, max.y, 0.0001f);
        }

        [Test]
        public void CalculateAnchors_InvalidScreenSize_FallsBackToFullScreen()
        {
            SafeAreaFitter.CalculateAnchors(new Rect(), Vector2Int.zero, out var min, out var max);

            Assert.AreEqual(Vector2.zero, min);
            Assert.AreEqual(Vector2.one, max);
        }
    }
}
