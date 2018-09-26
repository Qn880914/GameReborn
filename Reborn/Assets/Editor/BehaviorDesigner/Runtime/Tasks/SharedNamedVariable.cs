using System;

namespace FrameWork.BehaviorDesigner.Runtime
{
    [Serializable]
    public class SharedNamedVariable : SharedVariable<NamedVariable>
    {
        public SharedNamedVariable()
        {
            this.m_Value = new NamedVariable();
        }

        public static implicit operator SharedNamedVariable(NamedVariable value)
        {
            return new SharedNamedVariable
            {
                m_Value = value
            };
        }
    }
}
