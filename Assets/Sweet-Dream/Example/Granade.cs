using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class Granade : MonoBehaviour
{
    public float timer;
    public float radius;
    public LayerMask mask;

    public UnityEvent OnExplotion;
    public ParticleSystem explotionFBX;
    void Start()
    {
        Invoke(nameof(OnExplode), timer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnExplode()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, radius, mask);

        foreach (var coll in colls)
        {
            //->mueran todos :D
            //->
        }
        //->siustema de particulas de explosion
        OnExplotion?.Invoke();

        Instantiate(explotionFBX,transform.position,quaternion.identity);
        

        Destroy(gameObject);
    }
}
