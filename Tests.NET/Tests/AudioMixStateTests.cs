using StartupEmpire.Audio;
using Xunit;

namespace StartupEmpire.Domain.Tests
{
    public class AudioMixStateTests
    {
        [Fact]
        public void GetVolume_ReturnsConfiguredDefaults_PerCategory()
        {
            var mix = new AudioMixState();

            Assert.Equal(0.7f, mix.GetVolume(AudioCategory.Music));
            Assert.Equal(1f, mix.GetVolume(AudioCategory.UI));
            Assert.Equal(0.5f, mix.GetVolume(AudioCategory.Ambient));
        }

        [Fact]
        public void SetVolume_ClampsToZeroOneRange()
        {
            var mix = new AudioMixState();

            mix.SetVolume(AudioCategory.Music, 5f);
            Assert.Equal(1f, mix.GetVolume(AudioCategory.Music));

            mix.SetVolume(AudioCategory.Music, -5f);
            Assert.Equal(0f, mix.GetVolume(AudioCategory.Music));
        }

        [Fact]
        public void SetVolume_IsIndependentPerCategory()
        {
            var mix = new AudioMixState();

            mix.SetVolume(AudioCategory.Music, 0.2f);

            Assert.Equal(0.2f, mix.GetVolume(AudioCategory.Music));
            Assert.Equal(1f, mix.GetVolume(AudioCategory.UI));
        }

        [Fact]
        public void GetEffectiveVolume_MultipliesByMasterVolume()
        {
            var mix = new AudioMixState();
            mix.SetVolume(AudioCategory.Music, 0.5f);
            mix.SetMasterVolume(0.5f);

            Assert.Equal(0.25f, mix.GetEffectiveVolume(AudioCategory.Music), 5);
        }

        [Fact]
        public void SetMasterVolume_ClampsToZeroOneRange()
        {
            var mix = new AudioMixState();

            mix.SetMasterVolume(2f);
            Assert.Equal(1f, mix.MasterVolume);

            mix.SetMasterVolume(-1f);
            Assert.Equal(0f, mix.MasterVolume);
        }
    }
}
