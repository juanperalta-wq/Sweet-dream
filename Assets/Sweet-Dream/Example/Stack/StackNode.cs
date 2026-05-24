using UnityEngine;
using Sirenix.OdinInspector;

public class StackNode<T>
{
    #region Privates
    private T value;
    private StackNode<T> next;
    #endregion
    #region Constructor
    public StackNode(T value)
    {
        this.value = value;
        next = null;
    }
    #endregion
    #region Public Methods
    public void SetNext(StackNode<T> next)
    {
        this.next = next;
    }
    #endregion
    #region Getters
    public T Value => value;
    public StackNode<T> Next => next;
    #endregion
}