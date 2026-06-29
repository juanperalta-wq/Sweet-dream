using UnityEngine;

public interface IDoor
{
    void Open(Transform opener);
    bool IsOpen { get; }
}