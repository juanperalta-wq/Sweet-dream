
public class StateMachine
{
    private IState currentState;

    // Útil para debug y para que otros sistemas sepan en qué estado está el enemigo
    public IState CurrentState => currentState;

    public void Initialize(IState startingState)
    {
        currentState = startingState;
        currentState.Enter();
    }
    public void ChangeState(IState newState)
    {
        // Protección: no cambiar al mismo estado que ya estamos
        if (newState == currentState) return;

        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void Update()
    {
        currentState?.Update();
    }
}