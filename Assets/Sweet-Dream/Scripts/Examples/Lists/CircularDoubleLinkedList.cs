using System;
using UnityEngine;

public class CircularDoubleLinkedList<T>
{
    public Node<T> head = null;
    public Node<T> tail = null;
    public int Count;

    // O(1)
    public virtual void Add(T value)
    {
        Node<T> newNode = new(value);

        if (head == null)
        {
            head = newNode;
            tail = newNode;
            head.SetPrev(tail);
            tail.SetNext(head);
        }
        else
        {
            tail.SetNext(newNode);
            newNode.SetPrev(tail);
            tail = newNode;
            head.SetPrev(tail);
            tail.SetNext(head);
        }
        Count++;
    }

    // O(1)
    public void RemoveLast()
    {
        if (Count == 0)
        {
            Debug.Log("La lista esta vacia");
            return;
        }
        else if (Count == 1)
        {
            head = null;
            tail = null;
        }
        else
        {
            Node<T> newTail = tail.Prev;
            tail.SetPrev(null);
            tail.SetNext(null);
            tail = newTail;
            tail.SetNext(head);
            head.SetPrev(tail);
        }
        Count--;
    }

    // O(1)
    public void RemoveFirst()
    {
        if (Count == 0)
        {
            Debug.Log("La lista esta vacia");
            return;
        }
        else if (Count == 1)
        {
            head = null;
            tail = null;
        }
        else
        {
            Node<T> newHead = head.Next;
            head.SetNext(null);
            head.SetPrev(null);
            head = newHead;
            head.SetPrev(tail);
            tail.SetNext(head);
        }
        Count--;
    }
    public void RemoveNode(Node<T> node)
    {
        if (node == null || Count == 0) return;

        if (node == head)
        {
            RemoveFirst();
            return;
        }

        if (node == tail)
        {
            RemoveLast();
            return;
        }

        node.Prev.SetNext(node.Next);
        node.Next.SetPrev(node.Prev);
        node.SetNext(null);
        node.SetPrev(null);
        Count--;
    }

    public void TraverseInOrder(Action<Node<T>> action)
    {
        Node<T> current = head;
        for (int i = 0; i < Count; i++)
        {
            action(current);
            current = current.Next;
        }
    }

    public void TraverseInReverse(Action<Node<T>> action)
    {
        Node<T> current = tail;
        for (int i = 0; i < Count; i++)
        {
            action(current);
            current = current.Prev;
        }
    }
}