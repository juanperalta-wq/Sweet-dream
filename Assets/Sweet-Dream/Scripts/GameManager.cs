using System;
using Unity.Cinemachine;
using UnityEngine;
using Sirenix.OdinInspector;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Systems")]
    public FlashlightSystem flashlightSystem;
    public CameraSystem cameraSystem;

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
