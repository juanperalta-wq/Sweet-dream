using System;
using System.Collections.Generic;

namespace DulceSueño.Algorithms
{
    // Algoritmos de ordenamiento genéricos implementados desde cero.
    // Se usa Comparison<T> para poder ordenar cualquier tipo sin depender de List.Sort()
    // (que es la implementación de .NET, no una hecha por nosotros).
    public static class SortAlgorithms
    {
        //-> Insertion Sort
        //-> Mejor caso: O(n)  -> cuando la lista ya está casi ordenada
        //-> Peor caso / promedio: O(n^2)
        //-> Se usa en SearchState porque las listas de nodos candidatos son pequeñas y,
        //   como la IA reconsulta cada pocos segundos, suelen llegar casi ordenadas del
        //   frame anterior: ahí Insertion Sort rinde cerca de su mejor caso.
        public static void InsertionSort<T>(List<T> list, Comparison<T> comparer)
        {
            for (int i = 1; i < list.Count; i++)
            {
                T current = list[i];
                int j = i - 1;

                while (j >= 0 && comparer(list[j], current) > 0)
                {
                    list[j + 1] = list[j];
                    j--;
                }

                list[j + 1] = current;
            }
        }

        //-> Selection Sort
        //-> Siempre O(n^2), sin importar si la lista ya está ordenada.
        //-> Se usa para ordenar el inventario porque las listas son muy pequeñas (máx. 6 slots),
        //   así que la diferencia real de rendimiento frente a Insertion Sort es insignificante,
        //   y es el algoritmo más simple de explicar/depurar frente al docente.
        public static void SelectionSort<T>(List<T> list, Comparison<T> comparer)
        {
            for (int i = 0; i < list.Count - 1; i++)
            {
                int minIndex = i;

                for (int j = i + 1; j < list.Count; j++)
                {
                    if (comparer(list[j], list[minIndex]) < 0)
                        minIndex = j;
                }

                if (minIndex != i)
                {
                    (list[i], list[minIndex]) = (list[minIndex], list[i]);
                }
            }
        }
    }
}