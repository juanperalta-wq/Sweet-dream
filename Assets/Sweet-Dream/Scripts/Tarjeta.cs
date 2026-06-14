using UnityEngine;

public class Tarjeta : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Tarjeta interactuada");
        //PlayerStats.Instance.AddSanity(10);
    }
}
