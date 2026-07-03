using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraEnd : MonoBehaviour
{
    [SerializeField] private Transform pointMove;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stopThreshold = 0.01f;

    private Coroutine moveRoutine;

    private void Start()
    {
        if (pointMove != null)
            StartMove();
    }

    public void StartMove()
    {
        if (pointMove == null) return;
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(MoveCamera(transform, pointMove));
    }

    public void StopMove()
    {
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            moveRoutine = null;
        }
    }

    private IEnumerator MoveCamera(Transform obj, Transform target)
    {
        while (Vector3.Distance(obj.position, target.position) > stopThreshold)
        {
            obj.position = Vector3.MoveTowards(obj.position, target.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        moveRoutine = null;
    }
}
