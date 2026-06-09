using System;
using Unity.Cinemachine;
using UnityEngine;
using Sirenix.OdinInspector;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [FoldoutGroup("References")]
    public FlashlightSystem flashlightSystem;
    [FoldoutGroup("References")]
    public CameraSystem cameraSystem;
    [FoldoutGroup("References")]
    public MusicPool musicPool;

    [Header("Databases")]
    public MusicDatabase musicDatabase;
    private void Awake()
    {
        // Evita duplicados
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Persiste entre escenas
        DontDestroyOnLoad(gameObject);
    }
}
