using UnityEngine;

public class Barnilla : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData itemData;

    public void Interact()
    {
        Debug.Log("Barnilla interactuada");
        if (itemData != null)
        {
            Debug.Log("Sanity a agregar: " + itemData.Sanity);
            PlayerStats.Instance.AddSanity(itemData.Sanity);
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("itemData es null");
        }
    }
}
