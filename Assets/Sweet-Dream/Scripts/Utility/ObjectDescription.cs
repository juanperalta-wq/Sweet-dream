using UnityEngine;

/// <summary>
/// Coloca este script en cualquier GameObject que quieras que
/// muestre una descripción cuando el raycast de la cámara lo detecte.
/// Recuerda que el objeto también debe estar en el Layer correcto
/// (el mismo que configures en CameraRaycastDescription -> layerMask).
///
/// Además controla el efecto de Flash (shader Custom/URP/FlashLit) de uno
/// de los materiales del MeshRenderer, seleccionado por índice.
/// </summary>
public class ObjectDescription : MonoBehaviour
{
    [Header("Descripción")]
    [TextArea(2, 5)]
    [SerializeField] private string description;

    [Header("Material con efecto Flash")]
    [Tooltip("Si está desactivado, ActivarFlash() y DesactivarFlash() no hacen nada")]
    [SerializeField] private bool useFlash = true;

    [Tooltip("MeshRenderer que contiene el material a controlar. Si se deja vacío, se busca en este mismo GameObject.")]
    [SerializeField] private MeshRenderer meshRenderer;

    [Tooltip("Índice del material dentro de la lista de materiales del MeshRenderer")]
    [SerializeField] private int materialIndex = 0;

    // Nombre de la propiedad del shader que controla el flash (Custom/URP/FlashLit)
    private static readonly int IntensidadFlashID = Shader.PropertyToID("_IntensidadFlash");

    private Material targetMaterial;

    private void Awake()
    {
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        if (meshRenderer == null)
        {
            Debug.LogWarning($"{name}: no se encontró un MeshRenderer para controlar el material del flash.");
            return;
        }

        Material[] materials = meshRenderer.materials; // instancia los materiales (no comparte con otros objetos)

        if (materialIndex < 0 || materialIndex >= materials.Length)
        {
            Debug.LogWarning($"{name}: materialIndex ({materialIndex}) está fuera de rango. El MeshRenderer tiene {materials.Length} materiales.");
            return;
        }

        targetMaterial = materials[materialIndex];
    }

    public string GetDescription()
    {
        return description;
    }

    /// <summary>
    /// Activa el efecto de flash (parpadeo/interpolación) poniendo _IntensidadFlash en 1.
    /// </summary>
    public void ActivarFlash()
    {
        if (!useFlash || targetMaterial == null) return;
        targetMaterial.SetFloat(IntensidadFlashID, 1f);
    }

    /// <summary>
    /// Desactiva el efecto de flash, dejando el material en su color base (_IntensidadFlash = 0).
    /// </summary>
    public void DesactivarFlash()
    {
        if (!useFlash || targetMaterial == null) return;
        targetMaterial.SetFloat(IntensidadFlashID, 0f);
    }

    /// <summary>
    /// Llamar cuando el objeto es agarrado/tomado: apaga el flash de inmediato
    /// (sin importar el estado de useFlash) y bloquea que vuelva a activarse
    /// mientras esté en manos del jugador.
    /// </summary>
    public void Agarrar()
    {
        if (targetMaterial != null)
        {
            targetMaterial.SetFloat(IntensidadFlashID, 0f);
        }

        useFlash = false;
    }

    /// <summary>
    /// Llamar cuando el objeto es soltado: vuelve a permitir que ActivarFlash()/
    /// DesactivarFlash() (por ejemplo desde el sphere overlap) controlen el flash normalmente.
    /// </summary>
    public void Soltar()
    {
        useFlash = true;
    }
}