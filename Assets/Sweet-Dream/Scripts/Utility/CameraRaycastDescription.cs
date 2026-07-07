using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Combina dos comportamientos independientes:
///
/// 1) RAYCAST: desde la posición de la cámara en dirección forward, detecta el
///    objeto al que se está mirando directamente y muestra su descripción en un
///    TMP_Text de la UI. Solo controla el texto, no el flash.
///
/// 2) SPHERE OVERLAP: cada cierto intervalo, revisa qué objetos con ObjectDescription
///    están dentro de un radio alrededor de la cámara (el jugador) y activa/desactiva
///    su efecto de Flash según entren o salgan de ese radio.
/// </summary>
public class CameraRaycastDescription : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Cámara/jugador desde donde se lanza el raycast y se centra el sphere overlap. Si se deja vacío, se usa Camera.main")]
    [SerializeField] private Camera cam;

    [Tooltip("GameObject que contiene el componente TMP_Text de la UI (se activa/desactiva completo)")]
    [SerializeField] private GameObject descriptionUIObject;

    [Header("Configuración del Raycast (solo descripción)")]
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float rayDistance = 10f;

    [Header("Configuración del Flash por radio (Sphere Overlap)")]
    [Tooltip("Layer de los objetos que pueden activar su Flash al entrar en el radio")]
    [SerializeField] private LayerMask flashLayer;

    [Tooltip("Radio alrededor de la cámara/jugador dentro del cual se activa el Flash")]
    [SerializeField] private float flashRadius = 5f;

    [Tooltip("Cada cuántos segundos se revisa el sphere overlap (0 = cada frame)")]
    [SerializeField] private float flashCheckInterval = 0.1f;

    [Tooltip("Cantidad máxima de colliders que puede detectar el overlap en una sola revisión")]
    [SerializeField] private int maxOverlapResults = 20;

    private TMP_Text descriptionText;
    private ObjectDescription currentTarget;

    private Collider[] overlapResultsBuffer;
    private readonly HashSet<ObjectDescription> objetosConFlashActivo = new HashSet<ObjectDescription>();
    private readonly HashSet<ObjectDescription> objetosDetectadosEnRadio = new HashSet<ObjectDescription>();
    private readonly List<ObjectDescription> objetosParaDesactivar = new List<ObjectDescription>();
    private float flashCheckTimer;

    private void Awake()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }

        if (descriptionUIObject != null)
        {
            descriptionText = descriptionUIObject.GetComponent<TMP_Text>();

            if (descriptionText == null)
            {
                Debug.LogWarning("El GameObject asignado a descriptionUIObject no tiene un componente TMP_Text (TextMeshPro).");
            }

            // Empieza oculto hasta que se detecte un objeto
            descriptionUIObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("No se asignó descriptionUIObject en CameraRaycastDescription.");
        }

        overlapResultsBuffer = new Collider[Mathf.Max(1, maxOverlapResults)];
    }

    private void Update()
    {
        HandleRaycast();
        HandleFlashSphere();
    }

    // ---------------- Raycast: solo texto de descripción ----------------

    private void HandleRaycast()
    {
        if (cam == null) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance, interactableLayer))
        {
            ObjectDescription objDesc = hit.collider.GetComponent<ObjectDescription>();

            if (objDesc != null)
            {
                if (objDesc != currentTarget)
                {
                    currentTarget = objDesc;
                    ShowDescription(objDesc.GetDescription());
                }
            }
            else
            {
                ClearDescription();
            }
        }
        else
        {
            ClearDescription();
        }
    }

    private void ShowDescription(string description)
    {
        if (descriptionUIObject == null || descriptionText == null) return;

        descriptionText.text = description;
        descriptionUIObject.SetActive(true);
    }

    private void ClearDescription()
    {
        if (currentTarget == null && (descriptionUIObject == null || !descriptionUIObject.activeSelf))
        {
            return; // ya estaba limpio, evita trabajo innecesario cada frame
        }

        currentTarget = null;

        if (descriptionText != null)
        {
            descriptionText.text = string.Empty;
        }

        if (descriptionUIObject != null)
        {
            descriptionUIObject.SetActive(false);
        }
    }

    // ---------------- Sphere Overlap: control del Flash por radio ----------------

    private void HandleFlashSphere()
    {
        if (cam == null) return;

        flashCheckTimer -= Time.deltaTime;
        if (flashCheckTimer > 0f) return;
        flashCheckTimer = flashCheckInterval;

        objetosDetectadosEnRadio.Clear();

        int count = Physics.OverlapSphereNonAlloc(
            cam.transform.position,
            flashRadius,
            overlapResultsBuffer,
            flashLayer);

        for (int i = 0; i < count; i++)
        {
            ObjectDescription objDesc = overlapResultsBuffer[i].GetComponent<ObjectDescription>();
            if (objDesc != null)
            {
                objetosDetectadosEnRadio.Add(objDesc);
            }
        }

        // Activar el flash de los objetos nuevos que entraron al radio
        foreach (ObjectDescription objDesc in objetosDetectadosEnRadio)
        {
            if (!objetosConFlashActivo.Contains(objDesc))
            {
                objDesc.ActivarFlash();
                objetosConFlashActivo.Add(objDesc);
            }
        }

        // Marcar para desactivar el flash de los objetos que ya no están en el radio
        objetosParaDesactivar.Clear();
        foreach (ObjectDescription objDesc in objetosConFlashActivo)
        {
            if (!objetosDetectadosEnRadio.Contains(objDesc))
            {
                objetosParaDesactivar.Add(objDesc);
            }
        }

        foreach (ObjectDescription objDesc in objetosParaDesactivar)
        {
            objDesc.DesactivarFlash();
            objetosConFlashActivo.Remove(objDesc);
        }
    }

    // Opcional: dibuja el rayo y el radio de flash en el editor para depurar
    private void OnDrawGizmosSelected()
    {
        if (cam == null) return;

        Gizmos.color = Color.red;
        Vector3 origin = cam.transform.position;
        Vector3 direction = cam.transform.forward * rayDistance;
        Gizmos.DrawRay(origin, direction);

        Gizmos.color = new Color(0f, 1f, 1f, 0.25f);
        Gizmos.DrawWireSphere(cam.transform.position, flashRadius);
    }
}