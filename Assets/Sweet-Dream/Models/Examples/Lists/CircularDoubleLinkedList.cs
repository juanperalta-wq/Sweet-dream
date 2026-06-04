using System;
using UnityEngine;

public class CircularDoubleLinkedList<T>// : MonoBehaviour
{
    public Node<T> head = null;
    public Node<T> tail = null;
    public int Count;

    //->O(1)
    public virtual void Add(T value)
    {
        Node<T> newNode = new(value);

        //-> Cuando no hay nuingun elemento en la lista
        if (head == null)
        {
            head = newNode;
            tail = newNode;

            head.SetPrev(tail);
            tail.SetNext(head);

        }
        else if (head != null)
        {
            tail.SetNext(newNode);
            newNode.SetPrev(tail);
            tail = newNode;

            head.SetPrev(tail);
            tail.SetNext(head);
        }
        Count++;
    }


    //->O(1)
    public void RemoveLast()
    {

        //Node<T> Evaluator = head;

        if (Count == 0)
        {
            Debug.Log("La lista esta vacia");
            return;
        }
        else if (Count == 1)
        {
            head = null;
            tail = null;
            Count--;
        }
        else if (Count >= 2)
        {
            Node<T> Evaluator = tail.Prev;
            tail.SetPrev(null);
            Evaluator.SetNext(null);
            tail = Evaluator;

            head.SetPrev(tail);
            tail.SetNext(head);

            Count--;
        }


    }
    //-> O(1)
    public void RemoveFirst()
    {

        if (Count <= 1)
        {
            head = null;
            tail = null;
            Count--;
            return;
        }

        Node<T> Evaluator = head.Next;
        head.SetNext(null);
        head.SetPrev(null);
        head = Evaluator;

        head.SetPrev(tail);
        tail.SetNext(head);

        Count--;


    }

    public void TraverseInOrder(Action<Node<T>> action)
    {
        Node<T> Evaluator = head;
        int count = 0;
        while (count < Count)
        {
            //  Debug.Log(Evaluator.Value);
            action(Evaluator);

            Evaluator = Evaluator.Next;
            count++;
        }
    }
    public void TraverseInReverse(Action<Node<T>> action)
    {
        Node<T> Evaluator = tail;
        int count = 0;
        while (count < Count)
        {
            //  Debug.Log(Evaluator.Value);
            action(Evaluator);

            Evaluator = Evaluator.Prev;
            count++;
        }
    }


}
