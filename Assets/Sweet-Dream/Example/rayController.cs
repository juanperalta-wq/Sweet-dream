using UnityEngine;

public class rayController : MonoBehaviour
{
    public float AliveTime;

    void Start()
    {
        Destroy(gameObject, AliveTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
