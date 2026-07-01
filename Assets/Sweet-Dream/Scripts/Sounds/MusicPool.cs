using DulceSueño.Collections;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class MusicPool : MonoBehaviour
{
    // public MusicDatabase Database;
    public SoundPlayer SoundPlayerPrefab;

    public Queue<SoundPlayer> Pool = new();

    public int size = 20;

    public static Action<SoundPlayer> OnFinishAudio;

    private void OnEnable()
    {
        OnFinishAudio += EnqueueAudio;
    }

    void Start()
    {
        CreateSoundPlayerObjs(size);
    }

    // Sobrecarga #1 (comportamiento original): busca el audio en la base de datos y lo
    // reproduce con el volumen guardado en PlayerPrefs.
    public void PlayAudio(string audioName)
    {
        AudioClip clip = GameManager.Instance.musicDatabase.GetAudio(audioName);
        PlayClip(clip, PlayerPrefs.GetFloat("SFXVolume", 1f));
    }

    // Sobrecarga #2: igual que la anterior, pero permite forzar un volumen específico.
    // Útil para sonidos que SIEMPRE deben sonar fuerte (ej. jumpscares) sin importar
    // la configuración de audio del jugador.
    public void PlayAudio(string audioName, float volumeOverride)
    {
        AudioClip clip = GameManager.Instance.musicDatabase.GetAudio(audioName);
        PlayClip(clip, volumeOverride);
    }

    // Sobrecarga #3: reproduce un AudioClip directo, sin pasar por la base de datos.
    // Útil para un clip propio de un objeto puntual que no necesita estar registrado
    // globalmente en MusicDatabase.
    public void PlayAudio(AudioClip clip)
    {
        PlayClip(clip, PlayerPrefs.GetFloat("SFXVolume", 1f));
    }

    private void PlayClip(AudioClip clip, float volume)
    {
        if (Pool.Head == null || Pool.Count == 0)
        {
            Debug.Log("Se agrando la lista");
            CreateSoundPlayerObjs(5);
            return;
        }

        SoundPlayer soundPlayer = Pool.Dequeue();
        soundPlayer.gameObject.SetActive(true);
        soundPlayer.PlayAudio(clip, volume);
    }

    private void EnqueueAudio(SoundPlayer soundPlayer)
    {
        soundPlayer.gameObject.SetActive(false);
        Pool.Enqueue(soundPlayer);
    }

    [Button]
    private void CreateSoundPlayerObjs(int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            SoundPlayer obj = Instantiate(SoundPlayerPrefab, transform);
            obj.gameObject.SetActive(false);
            Pool.Enqueue(obj);
        }
    }

    [Button]
    public void Test(string audioName)
    {
        PlayAudio(audioName);
        Debug.Log(Pool.Count);
    }

    [Button]
    public void Test2()
    {
        Debug.Log(Pool.Count);
    }
}