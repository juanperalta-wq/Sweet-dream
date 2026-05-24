using Unity.VisualScripting;
using UnityEngine;

public class MyStack<T> //->strings
{

    #region Privates
    private StackNode<T> top;
    private int count;
    #endregion
    #region Public Methods
    public virtual void Push(T value)
    {
        StackNode<T> newNode = new StackNode<T>(value);
        //->pila vacia
        if (top == null)
        {
            top = newNode;
            count++;
            return;
        }
        //->pila ya tiene un elemtno
        newNode.SetNext(top);
        top = newNode;

        count++;
    }

    public virtual T Pop()
    {
        if (top == null)
            throw new System.Exception("Trying to pop a empty stack");

        T tempValue = top.Value;
        // StackNode<T> tempValue = top;

        top = top.Next;
        count--;

        return tempValue;
    }
    public virtual T Peek()//-> +1  Jeremy , arribasplata, traviz 
    {
        if (top == null)
            throw new System.Exception("Trying to peek a empty stack");

        T tempValue = top.Value;
        return tempValue;
    }


    public virtual void Clear()//+1  coretin, juan diego, guerrero erwin !
    {
        top = null;
        count = 0;
    }
    #endregion
    #region Getters
    public StackNode<T> Top => top;
    public int Count => count;
    #endregion



}