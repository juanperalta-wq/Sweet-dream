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

    [Header("Player")]
    [SerializeField] private GameObject playerObject;

    private Coroutine routine;

    void Start()
    {
        if (itemPickUp == null)
            itemPickUp = GetComponent<ItemPickUp>();
    }

    public void Interact()
    {
        if (itemPickUp == null || !itemPickUp.IsEquipped)
            return;

        // Consumir la barnilla equipada
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ConsumeCurrentItem();

            Debug.Log("Hijos equipados: " +
                InventoryManager.Instance.equipPoint.childCount);

            foreach (Transform child in InventoryManager.Instance.equipPoint)
            {
                Debug.Log("Item hijo: " + child.name +
                          " Activo: " + child.gameObject.activeSelf);
            }
        }

        // Ocultar jugador
        if (playerObject != null)
        {
            playerObject.SetActive(false);
        }

        // Cambiar a cámara cinematográfica
        if (Player != null && Sillon != null)
        {
            Sillon.Priority = 20;
            Player.Priority = 10;
        }

        // Mostrar barnilla de la cinemática
        if (Barnilla != null)
        {
            Barnilla.SetActive(true);
        }

        InitiationCorrutine();
    }

    public void InitiationCorrutine()
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