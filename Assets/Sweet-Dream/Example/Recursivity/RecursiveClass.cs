using UnityEngine;
using Sirenix.OdinInspector;


public class RecursiveClass : MonoBehaviour
{
    public GameObject Cube;
    public int iterations;
    void Start()
    {
        for (int i = 0; i < iterations; i++)
        {

            int fiboValue = Fibonacci(i);
            GameObject cube = Instantiate(Cube, this.transform);
            cube.GetComponent<MeshRenderer>().material.color
                = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

            cube.transform.localScale = Vector3.one * fiboValue;

            cube.transform.localPosition = new Vector3(fiboValue / 2, fiboValue / 2, fiboValue * 2.5f);
        }
        //CountDown(5);

        // Debug.Log(Sum(5));

        // PrintFlow(3);
    }
    public void CountDown(int n)
    {
        if (n <= 0)
        {
            Debug.Log("Inicio la partida");
            return;
        }

        CountDown(n - 1);

        Debug.Log(n);
    }
    public int Sum(int n)
    {
        if (n == 1)
            return 1;

        return n + Sum(n - 1);
    }
    [Button]
    public void PrintFlow(int n)
    {
        if (n < 1)
            return;

        Debug.Log("Entrando: " + n);
        PrintFlow(n - 1);
        Debug.Log("Saliendo: " + n);
    }
    [Button]
    public int SumExpress(int n)
    {
        if (n == 1)
            return 1;

        return SumExpress(n - 1) + SumExpress(n - 1);
    }
    [Button]
    public int Factorial(int n)
    {
        if (n == 1)
            return 1;


        return n * Factorial(n - 1);

    }
    [Button]
    public int Fibonacci(int n)
    {
        if (n == 0)
            return 0;
        if (n == 1)
            return 1;


        return Fibonacci(n - 1) + Fibonacci(n - 2);
    }

}
