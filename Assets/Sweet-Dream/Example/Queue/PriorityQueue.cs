/*using System;
using UnityEngine;

public class PriorityQueue<T>
{
    #region Properties/Privates
    private QueueNode<T> head;
    private QueueNode<T> tail;
    private int count;

    private Func<T, T, bool> hasHigherPriority;
    #endregion

    public PriorityQueue(Func<T, T, bool> rule)
    {
        hasHigherPriority = rule;
    }



    #region Public Methods
    //-> O(1)
    public void Enqueue(T value)//->O(1) O(n)
    {
        QueueNode<T> newNode = new(value);
        count++;

        if (head == null)
        {
            head = newNode;
            return;
        }
        //-> [10]
        //-> [15][10]     
        //-> [15][10][2] 
        //-> [15][10][2][1] 
        //-> [15][10][4][2][2] [1] [1] [1] [1] [1] [1] [1] [1] [1] 
        //->O1
        if (hasHigherPriority(value, head.Value))
        {
            newNode.SetNext(head);
            head = newNode;
            return;
        }
        QueueNode<T> evaluator = head;

        while (evaluator.Next != null && !hasHigherPriority(value, evaluator.Next.Value))
        {
            evaluator = evaluator.Next;
        }

        newNode.SetNext(evaluator.Next);
        evaluator.SetNext(newNode);

    }

    public T Dequeue()
    {
        if (head == null)
        {
            Clear();
            throw new System.Exception("Queue Empty");
        }


        T value = head.Value;
        head = head.Next;

        count--;
        if (head == null)
            tail = null;
        return value;
    }
    public T Peek()
    {
        if (head == null)
        {
            Clear();
            throw new System.Exception("Queue Empty");
        }


        return head.Value;
    }
    public void Clear()
    {
        head = null;
        count = 0;
    }
    #endregion

    #region Getters
    public int Count => count;
    #endregion
}*/