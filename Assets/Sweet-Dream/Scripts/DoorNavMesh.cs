using UnityEngine;
using UnityEngine.AI;

public class DoorNavMesh : MonoBehaviour
{
    [SerializeField] private NavMeshObstacle obstacle;

    private void Awake()
    {
        if (obstacle == null)
        {
            obstacle = GetComponent<NavMeshObstacle>();
        }
    }

    public void OpenDoor()
    {
        if (obstacle != null)
        {
            obstacle.enabled = false;
        }
    }

    public void CloseDoor()
    {
        if (obstacle != null)
        {
            obstacle.enabled = true;
        }
    }
}