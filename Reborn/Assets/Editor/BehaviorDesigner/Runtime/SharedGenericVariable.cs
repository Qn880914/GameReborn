using System;

namespace FrameWork.BehaviorDesigner.Runtime
{
    [Serializable]
    public class SharedGenericVariable : SharedVariable<GenericVariable>
    {
        public SharedGenericVariable()
        {
            this.value = new GenericVariable();
        }

        public static implicit operator SharedGenericVariable(GenericVariable value)
        {
            return new SharedGenericVariable
            {
                value = value
            };
        }
    }
}
