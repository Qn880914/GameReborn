using FrameWork.StateMachine;

namespace FrameWork.Game
{
    public enum PlayerStateType
    {
        Idle,
        Move,
        Dead
    }

    public class Player
    {
        StateMachine<PlayerStateType, Player> m_PlayerStateMachine = new StateMachine<PlayerStateType, Player>();

        public void Init()
        {
            RegisterState();
        }

        public void RegisterState()
        {
            m_PlayerStateMachine.AddState(PlayerStateType.Idle, new PlayerIdle());
            m_PlayerStateMachine.AddState(PlayerStateType.Move, new PlayerMove());
            m_PlayerStateMachine.AddState(PlayerStateType.Dead, new PlayerDead());
        }

        public void UnRegisterState()
        {
            m_PlayerStateMachine.RemoveState(PlayerStateType.Idle);
            m_PlayerStateMachine.RemoveState(PlayerStateType.Move);
            m_PlayerStateMachine.RemoveState(PlayerStateType.Dead);
        }

        public void OnIdle()
        {
            m_PlayerStateMachine.SetState(PlayerStateType.Idle);
        }

        public void OnMove()
        {
            m_PlayerStateMachine.SetState(PlayerStateType.Move);
        }

        public void OnDead()
        {
            m_PlayerStateMachine.SetState(PlayerStateType.Dead);
        }
    }

    public abstract class PlayerState : State<PlayerStateType, Player>
    {
    }

    public class PlayerIdle : PlayerState
    {
        public override void OnEnter()
        {
            UnityEngine.Debug.Log("   PlayerIdle OnEnter ");
        }

        public override void OnUpdate(float deltaTime)
        {
            UnityEngine.Debug.Log("   PlayerIdle OnUpdate ");
        }

        public override void OnExit()
        {
            UnityEngine.Debug.Log("   PlayerIdle OnExit ");
        }
    }

    public class PlayerMove : PlayerState
    {
        public override void OnEnter()
        {
            UnityEngine.Debug.Log("   PlayerMove OnEnter ");
        }

        public override void OnUpdate(float deltaTime)
        {
            UnityEngine.Debug.Log("   PlayerMove OnUpdate ");
        }

        public override void OnExit()
        {
            UnityEngine.Debug.Log("   PlayerMove OnExit ");
        }
    }

    public class PlayerDead : PlayerState
    {
        public override void OnEnter()
        {
            UnityEngine.Debug.Log("   PlayerDead OnEnter ");
        }

        public override void OnUpdate(float deltaTime)
        {
            UnityEngine.Debug.Log("   PlayerDead OnUpdate ");
        }

        public override void OnExit()
        {
            UnityEngine.Debug.Log("   PlayerDead OnExit ");
        }
    }
}
