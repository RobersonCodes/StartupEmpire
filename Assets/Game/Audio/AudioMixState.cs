using System;
using System.Collections.Generic;

namespace StartupEmpire.Audio
{
    /// Estado puro dos volumes por categoria — controles independentes de volume
    /// (seção 28 da missão), testável sem depender de UnityEngine.AudioSource.
    public sealed class AudioMixState
    {
        private readonly Dictionary<AudioCategory, float> _volumeByCategory = new()
        {
            { AudioCategory.Music, 0.7f },
            { AudioCategory.UI, 1f },
            { AudioCategory.Ambient, 0.5f },
            { AudioCategory.Events, 1f },
            { AudioCategory.Achievements, 1f }
        };

        public float MasterVolume { get; private set; } = 1f;

        public float GetVolume(AudioCategory category) =>
            _volumeByCategory.TryGetValue(category, out var volume) ? volume : 1f;

        public void SetVolume(AudioCategory category, float volume) =>
            _volumeByCategory[category] = Math.Clamp(volume, 0f, 1f);

        public void SetMasterVolume(float volume) => MasterVolume = Math.Clamp(volume, 0f, 1f);

        public float GetEffectiveVolume(AudioCategory category) => GetVolume(category) * MasterVolume;
    }
}
