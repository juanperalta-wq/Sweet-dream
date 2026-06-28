using UnityEngine;
using DG.Tweening;

public class CameraIdleRotation : MonoBehaviour
{
    [SerializeField] private float angle = 45f;
    [SerializeField] private float moveTime = 1000f;
    [SerializeField] private float waitTime = 1.5f;

    private void Start()
    {
        Sequence patrol = DOTween.Sequence();
        
        patrol.Append(transform.DOLocalRotate(new Vector3(0, angle, 0),moveTime));

        patrol.AppendInterval(waitTime);

        patrol.Append(transform.DOLocalRotate(new Vector3(0, -angle, 0),moveTime * 2));

        patrol.AppendInterval(waitTime);

        patrol.SetEase(Ease.Linear);
        patrol.SetLoops(-1);
    }
}