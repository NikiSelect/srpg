using UnityEngine;
public class StateMachineBase<T> : MonoBehaviour where T : StateMachineBase<T>
{
    private StateBase<T> m_currentState;
    private StateBase<T> m_nextState;
    public string stateName;

    public bool ChangeState(StateBase<T> _nextState)
    {
        bool bRet = m_nextState == null;
        if (m_currentState != null)
        {
            m_currentState.OnExitState();
        }
        m_currentState = _nextState;
        m_currentState.OnEnterState();
        m_nextState = null;
        stateName = m_currentState.ToString();
        return bRet;
    }

    protected virtual void Update()
    {

        if (m_currentState != null)
        {
            m_currentState.OnUpdate();
        }
    }
}
