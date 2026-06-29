using UnityEngine;

[RequireComponent(typeof(InteractableObject))]
public class EnemyDoorAdapter : MonoBehaviour, IDoor
{
    private InteractableObject interactable;
    [SerializeField, Tooltip("Estado actual de la puerta (sólo lectura).")]
    private bool isOpen = false;

    public bool IsOpen => isOpen;

    private void Awake()
    {
        interactable = GetComponent<InteractableObject>();

        if (interactable == null)
            Debug.LogError($"EnemyDoorAdapter en '{name}': no encontró InteractableObject.");
    }

    public void Open(Transform opener)
    {
        if (isOpen) return;

        interactable.Interact();
        isOpen = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.NearbyDoor = this;
            Debug.Log($"Enemigo cerca de la puerta: {name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null && enemy.NearbyDoor == (IDoor)this)
        {
            enemy.NearbyDoor = null;
        }
    }
}