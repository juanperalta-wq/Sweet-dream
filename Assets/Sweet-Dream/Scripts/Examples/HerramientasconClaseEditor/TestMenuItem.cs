using UnityEditor;
using UnityEngine;
public class TestMenuItem
{
    public static void SnapToGround()
    {
        //-> Obtiene el objeto seleccion en la escena
        GameObject obj = Selection.activeGameObject;
        if (obj == null)
        {
            Debug.LogWarning("Tienes que tener un objeto seleccionado");
            return;
        }

        //->Permite deshacer el cambio con ctrl + z
        Undo.RegisterCompleteObjectUndo(obj.transform, "Simular caida");

        //Asegurando un collider
        Collider coll = obj.GetComponent<Collider>();
        if (coll == null)
            coll = obj.AddComponent<Collider>();

        //Aseguramos Rigidbody
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        bool rbAdded = false;

        if (rb == null)
        {
            rbAdded = true; //->saber que nosotros añadimos el script
            rb = obj.AddComponent<Rigidbody>();
        }


        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        //->Activamos la simulacion fisica manual del editor
        Physics.simulationMode = SimulationMode.Script; //->Nosotros vamos a controlar la simulaon de fisicas

        int maxSteps = 600;//->numero maximo de limite de segurar
        float dt = 0.0166f;//->Duracion de cada paso de simulacion equivale maso 1 frame
        float sleepThreshold = 0.001f;//->valor minimo para considerar un objeto quieto

        for (int i = 0; i < maxSteps; i++)
        {
            Physics.Simulate(dt);

            if (rb.IsSleeping() || rb.linearVelocity.sqrMagnitude < sleepThreshold && rb.angularVelocity.sqrMagnitude < sleepThreshold && i < 5)
            {
                break;
            }
            
        }
        //-> restauramos el modo simulacion por defecto
        Physics.simulationMode = SimulationMode.FixedUpdate;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (rbAdded) Object.DestroyImmediate(rb);

        //-> marca objeto como modificado en la escena
        EditorUtility.SetDirty(obj.transform);
        Debug.Log("Simulación de caida finalizada");
    }
}