using UnityEngine;

public class Barnilla : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private float duration = 30f;
    [SerializeField] private float sanityAmount = 10f;

    public void Interact()
    {
        Debug.Log("Interacted with Barnilla!");
        BuffManager buffManager = PlayerStats.Instance.GetComponent<BuffManager>();

        if (buffManager != null)
        {
            Debug.Log("BuffManager found, applying sanity buff.");
            Buff buff = new SanityBuff(duration, sanityAmount);
            buffManager.AddBuff(buff);
            Destroy(gameObject);
        }
    }
}