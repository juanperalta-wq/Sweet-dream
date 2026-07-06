using DulceSueño.Collections;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class MusicPool : MonoBehaviour
{
    // public MusicDatabase Database;
    public SoundPlayer SoundPlayerPrefab;

    [ShowInInspector, ReadOnly]
    private Queue<SoundPlayer> pool = new();

    public int size = 20;

    public static Action<SoundPlayer> OnFinishAudio;

    private void OnEnable()
    {
        OnFinishAudio += EnqueueAudio;
    }

    private void OnDisable()
    {
        OnFinishAudio -= EnqueueAudio;
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
        if (pool.Count == 0)
        {
            Debug.Log("Se agrandó la lista");
            CreateSoundPlayerObjs(5);
        }

        SoundPlayer soundPlayer = pool.Dequeue();
        soundPlayer.gameObject.SetActive(true);
        soundPlayer.PlayAudio(clip, volume);
    }

    private void EnqueueAudio(SoundPlayer soundPlayer)
    {
        soundPlayer.gameObject.SetActive(false);
        pool.Enqueue(soundPlayer);
    }

    [Button]
    private void CreateSoundPlayerObjs(int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            SoundPlayer obj = Instantiate(SoundPlayerPrefab, transform);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    [Button]
    public void Test(string audioName)
    {
        PlayAudio(audioName);
        Debug.Log(pool.Count);
    }

    [Button]
    public void Test2()
    {
        Debug.Log(pool.Count);
    }
}