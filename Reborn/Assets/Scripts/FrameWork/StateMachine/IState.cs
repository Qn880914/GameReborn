﻿namespace FrameWork.StateMachine
{
    /// <summary>
    ///     <para> Implement this interface to make sure it is a Updatable object.</para>
    /// </summary>
    public interface IUpdate
    {
        void OnUpdate(float deltaTime);
    }

    /// <summary>
    ///     <para>Extend IUpdate</para>
    ///     <para>Implement this interface to make sure it is a statable object</para>
    /// </summary>
    public interface IStatable : IUpdate
    {
        void OnEnter();

        void OnExit();
    }

    /// <summary>
    ///     <para>Extent IStatable</para>
    ///     <para>Implement this interface to make sure it is a state object</para>
    /// </summary>
    /// <typeparam name="KT"></typeparam>
    /// <typeparam name="OT"></typeparam>
    public interface IState<KT, OT> : IStatable
    {
        IStateMachine<KT, OT> GetCurrentStateMachine();

        void SetCurrentStateMachine(IStateMachine<KT, OT> stateMachine);
    }

    public interface IStateMone<KT, MonoBehaviour> { }
}
