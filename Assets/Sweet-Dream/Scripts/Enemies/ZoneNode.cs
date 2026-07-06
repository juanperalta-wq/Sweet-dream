using UnityEngine;
using System.Collections.Generic;

public class ZoneNode : MonoBehaviour
{
    [Header("Zone")]
    public PatrolZone Zone;

    [Header("Connected Nodes")]
    public List<ZoneNode> Neighbors = new List<ZoneNode>();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.2f);

        Gizmos.color = Color.green;

        foreach (ZoneNode neighbor in Neighbors)
        {
            if (neighbor != null)
            {
                Gizmos.DrawLine(transform.position, neighbor.transform.position);
            }
        }
    }
}