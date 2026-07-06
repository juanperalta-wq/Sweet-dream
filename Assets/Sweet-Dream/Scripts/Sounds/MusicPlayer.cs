using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance { get; private set; }

    [SerializeField] private AudioSource source;
    [SerializeField] private AudioMixerGroup musicGroup;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        source.outputAudioMixerGroup = musicGroup;
        source.loop = true;
        source.playOnAwake = false;
    }

    public void Play(AudioClip clip, float fadeInDuration = 1.5f)
    {
        if (clip == null)
        {
            Debug.LogWarning($"{nameof(MusicPlayer)}: clip null, no se reproduce nada.");
            return;
        }

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        source.clip = clip;
        source.volume = 0f;
        source.Play();

        fadeCoroutine = StartCoroutine(FadeRoutine(0f, 1f, fadeInDuration));
    }

    public void Stop(float fadeOutDuration = 1f)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOutAndStop(fadeOutDuration));
    }

    private IEnumerator FadeRoutine(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        source.volume = to;
    }

    private IEnumerator FadeOutAndStop(float duration)
    {
        yield return FadeRoutine(source.volume, 0f, duration);
        source.Stop();
        source.clip = null;
    }
}