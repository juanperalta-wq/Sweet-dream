using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Se mantiene por compatibilidad: usa el volumen guardado en PlayerPrefs.
    public void PlayAudio(AudioClip clip)
    {
        PlayAudio(clip, PlayerPrefs.GetFloat("SFXVolume", 1f));
    }

    // Sobrecarga: permite especificar el volumen manualmente (la usa MusicPool
    // cuando quiere forzar un volumen distinto al de la configuración del jugador).
    public void PlayAudio(AudioClip clip, float volume)
    {
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();

        Invoke(nameof(ReturnToPool), audioSource.clip.length);
    }

    public void ReturnToPool()
    {
        audioSource.clip = null;
        MusicPool.OnFinishAudio?.Invoke(this);
    }
}