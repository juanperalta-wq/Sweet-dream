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

    public void PlayAudio(string audioName)
    {
        if (Pool.Head == null || Pool.Count == 0)
        {
            Debug.Log("Se agrando la lista");
            CreateSoundPlayerObjs(5);
            // PlayAudio(audioName);
            return;
        }
        AudioClip clip = GameManager.Instance.musicDatabase.GetAudio(audioName);

        SoundPlayer soundPlayer = Pool.Dequeue();
        soundPlayer.gameObject.SetActive(true);
        soundPlayer.PlayAudio(clip);
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