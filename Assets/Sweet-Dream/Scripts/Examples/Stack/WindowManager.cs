using System;

public class WindowManager : MyStack<Window>
{
    public Action<Window> OnElementAdded;
    public Action<Window> OnElementRemoved;



    public override void Push(Window value)
    {
        base.Push(value);
        OnElementAdded?.Invoke(Peek());

    }

    public override Window Pop()//+1 jeremy , coreting , arribasplata
    {
        //->si el elemento que popeo es esta activo tiro el OnElementRemoved , caso contrario no lo realizo.    (IF)

        if (Peek().window.activeSelf == true)
        {
            OnElementRemoved?.Invoke(Peek());
        }


        return base.Pop();
    }




}