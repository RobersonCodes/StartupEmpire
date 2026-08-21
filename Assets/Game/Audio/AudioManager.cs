using System;
using System.Collections.Generic;
using UnityEngine;

namespace StartupEmpire.Audio
{
    /// MonoBehaviour único que toca áudio por categoria, respeitando AudioMixState.
    /// Cada categoria tem sua própria AudioSource para permitir volume independente
    /// e, no caso de Music/Ambient, tocar em loop sem interromper as demais.
    /// Sem clipes ainda: o sistema está pronto, mas nenhum áudio original foi
    /// criado nesta sessão (seção 28 exige áudio original ou com licença compatível,
    /// que este agente não pode gerar) — plugar AudioClip nos campos quando existirem.
    public sealed class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        public AudioMixState MixState { get; } = new();

        private readonly Dictionary<AudioCategory, AudioSource> _sources = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            foreach (AudioCategory category in Enum.GetValues(typeof(AudioCategory)))
            {
                var source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.loop = category is AudioCategory.Music or AudioCategory.Ambient;
                source.volume = MixState.GetEffectiveVolume(category);
                _sources[category] = source;
            }
        }

        public void SetVolume(AudioCategory category, float volume)
        {
            MixState.SetVolume(category, volume);
            ApplyVolume(category);
        }

        public void SetMasterVolume(float volume)
        {
            MixState.SetMasterVolume(volume);
            foreach (AudioCategory category in Enum.GetValues(typeof(AudioCategory)))
            {
                ApplyVolume(category);
            }
        }

        private void ApplyVolume(AudioCategory category)
        {
            if (_sources.TryGetValue(category, out var source))
            {
                source.volume = MixState.GetEffectiveVolume(category);
            }
        }

        public void PlayOneShot(AudioCategory category, AudioClip clip)
        {
            if (clip == null || !_sources.TryGetValue(category, out var source)) return;
            source.PlayOneShot(clip, MixState.GetEffectiveVolume(category));
        }

        public void PlayLoop(AudioCategory category, AudioClip clip)
        {
            if (clip == null || !_sources.TryGetValue(category, out var source)) return;
            source.clip = clip;
            source.Play();
        }

        public void Stop(AudioCategory category)
        {
            if (_sources.TryGetValue(category, out var source)) source.Stop();
        }
    }
}
