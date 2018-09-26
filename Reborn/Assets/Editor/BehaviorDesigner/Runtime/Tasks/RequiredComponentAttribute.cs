using System;

namespace FrameWork.BehaviorDesigner.Tasks
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class RequiredComponentAttribute : Attribute
    {
        public readonly Type m_ComponentType;

        public Type componentType
        {
            get
            {
                return this.m_ComponentType;
            }
        }

        public RequiredComponentAttribute(Type componentType)
        {
            this.m_ComponentType = componentType;
        }
    }
}
