using System.Collections;
using UnityEngine;

public class BearCharger : MonoBehaviour, IInteractable
{

    [SerializeField] private Transform PointCharger;
    [SerializeField] private float chargeTime = 3f;
    [SerializeField] private float rechargeAmount = 100f;
    public FlashlightSystem Flash;

    public void Interact()
    {
        Flash = GetComponent<FlashlightSystem>();
    }

}