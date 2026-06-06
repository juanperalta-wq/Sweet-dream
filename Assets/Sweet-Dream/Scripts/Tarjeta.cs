using UnityEngine;

public class Tarjeta : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData itemData;
    public void Interact()
    {
        Debug.Log("Tarjeta interactuada");
        //PlayerStats.Instance.AddSanity(10);
        Destroy(gameObject);
    }
}
