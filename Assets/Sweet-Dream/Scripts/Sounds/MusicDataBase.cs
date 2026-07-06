using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MusicDatabase", menuName = "Scriptable Objects/MusicDatabase")]
public class MusicDatabase : SerializedScriptableObject
{
    [Title("Sounds")]
    public Dictionary<string, AudioClip> ClipDatabase = new();

    public AudioClip GetAudio(string audioName)
    {
        if (ClipDatabase.TryGetValue(audioName, out AudioClip clip))
            return clip;

        Debug.LogError($"El audio '{audioName}' no existe en la MusicDatabase.");
        return null;
    }
}