
using Sirenix.OdinInspector;
public class Node<T>
{
    private T value = default;

    private Node<T> next = null;
    private Node<T> prev = null;

    public Node(T value)
    {
        this.value = value;
    }
    public void SetNext(Node<T> next)
    {
        this.next = next;
    }
    public void SetPrev(Node<T> prev)
    {
        this.prev = prev;
    }
    [FoldoutGroup("Getters")]
    public Node<T> Next => next;
    [FoldoutGroup("Getters")]
    public Node<T> Prev => prev;
    [FoldoutGroup("Getters")]
    public T Value => value;
}