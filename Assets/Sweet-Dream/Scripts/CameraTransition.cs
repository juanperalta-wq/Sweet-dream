using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraTransition : MonoBehaviour, IInteractable
{
    [Header("Cameras")]
    public CinemachineCamera Player;
    public CinemachineCamera Sillon;
    public GameObject Barnilla;

    [Header("Scene")]
    [SerializeField] private string sceneToLoad = "scene_Home";
    [SerializeField] private float changeTime = 5f;

    [Header("Requirements")]
    [SerializeField] private ItemPickUp itemPickUp;

    private Coroutine routine;

    void Start()
    {
        if (itemPickUp == null)
            itemPickUp = GetComponent<ItemPickUp>();
    }

    public void Interact()
    {
        if (itemPickUp == null || !itemPickUp.IsEquipped) return;

        if (Player != null && Sillon != null && Barnilla != null)
        {
            Sillon.Priority = 20;
            Player.Priority = 10;
            Barnilla.SetActive(true);
        }

        IniciarCorrutina();
    }

    public void IniciarCorrutina()
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(ChargeScene());
    }

    private IEnumerator ChargeScene()
    {
        float t = 0f;
        while (t < changeTime)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        Debug.Log("Cargando escena");
        SceneManager.LoadScene("Sweet-Dream");
    }
}