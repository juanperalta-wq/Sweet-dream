using UnityEngine;

public class Barnilla : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private float duration = 30f;
    [SerializeField] private float sanityAmount = 10f;

    public void Interact()
    {
        if (PlayerStats.Instance == null) return;

        BuffManager buffManager = PlayerStats.Instance.GetComponent<BuffManager>();
        if (buffManager == null) return;

        buffManager.AddBuff(new SanityBuff(duration, sanityAmount));
        Destroy(gameObject);
    }
}