using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [TabGroup("Referencias"), Required]
    [SerializeField] private Camera playerCamera;

    [TabGroup("Referencias"), Required]
    [SerializeField] private GameObject flashEffect;

    [TabGroup("Configuracion"), LabelWidth(120)]
    [PropertyRange(1f, 30f)]
    [SerializeField] private float photoDistance = 15f;

    [TabGroup("Configuracion"), LabelWidth(120)]
    [SerializeField] private LayerMask enemyLayer;

    [TabGroup("Debug"), ReadOnly]
    [SerializeField] private bool isTakingPhoto;

    private void OnEnable()
    {
        PlayerInputs.OnTakePhoto += TakePhoto;
    }

    private void OnDisable()
    {
        PlayerInputs.OnTakePhoto -= TakePhoto;
    }

    #region TakePhoto
    public void TakePhoto()
    {
        if (isTakingPhoto) return;
        Debug.Log("FOTO");
        StartCoroutine(FlashCoroutine());

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, photoDistance, enemyLayer))
        {
            // Antes buscaba "EnemyBase", que ya no existe.
            // Las Shadows viven en ShadowAI: la foto las hace desaparecer (ReturnToPool).
            ShadowAI shadow = hit.collider.GetComponent<ShadowAI>();
            if (shadow != null)
                shadow.OnPhotoHit();
        }
    }
    #endregion

    #region Flash
    IEnumerator FlashCoroutine()
    {
        isTakingPhoto = true;
        if (flashEffect != null)
        {
            flashEffect.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            flashEffect.SetActive(false);
        }
        isTakingPhoto = false;
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * photoDistance);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(playerCamera.transform.position + playerCamera.transform.forward * photoDistance, 0.1f);
        }
    }
    #endregion
}
