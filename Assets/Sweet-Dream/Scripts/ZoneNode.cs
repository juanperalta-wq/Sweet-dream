using UnityEngine;
using System.Collections.Generic;

public class ZoneNode : MonoBehaviour
{
    [Header("Connected Nodes")]
    public List<ZoneNode> Neighbors = new List<ZoneNode>();
}