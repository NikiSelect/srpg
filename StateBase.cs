
public class StateBase<T> where T : StateMachineBase<T>
{
    protected T machine;
    public StateBase(T _machine) { machine = _machine; }
    public virtual void OnEnterState() { }
    public virtual void OnUpdate() { }
    public virtual void OnExitState() { }

    public void ChangeState(StateBase<T> _nextState)
    {
        machine.ChangeState(_nextState);
    }
}
