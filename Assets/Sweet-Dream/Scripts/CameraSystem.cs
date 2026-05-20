using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSystem : MonoBehaviour
{
    [FoldoutGroup("Variables")]
    [SerializeField] private Camera playerCamera;
    [FoldoutGroup("Variables")]
    [SerializeField] private GameObject flashEffect;
    [FoldoutGroup("Variables")]
    [SerializeField] private float photoDistance = 15f;
    [FoldoutGroup("Variables")]
    private InputSystem_Actions inputs;

    private void Awake()
    {
        inputs = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputs.Enable();

        inputs.Player.Photo.performed += TakePhoto;
    }

    private void OnDisable()
    {
        inputs.Player.Photo.performed -= TakePhoto;

        inputs.Disable();
    }

    void TakePhoto(InputAction.CallbackContext context)
    {
        Debug.Log("FOTO");

        StartCoroutine(FlashCoroutine());

        Ray ray = new Ray(playerCamera.transform.position,playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, photoDistance))
        {
            EnemyBase enemy = hit.collider.GetComponent<EnemyBase>();

            if (enemy != null)
            {
                enemy.OnPhotoHit();
            }
        }
    }

    IEnumerator FlashCoroutine()
    {
        if (flashEffect != null)
        {
            flashEffect.SetActive(true);

            yield return new WaitForSeconds(0.1f);

            flashEffect.SetActive(false);
        }
    }
}