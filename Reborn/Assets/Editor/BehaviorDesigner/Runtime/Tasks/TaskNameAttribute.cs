using System;

namespace FrameWork.BehaviorDesigner.Tasks
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TaskNameAttribute : Attribute
    {
        public readonly string m_Name;

        public string name
        {
            get
            {
                return this.m_Name;
            }
        }

        public TaskNameAttribute(string name)
        {
            this.m_Name = name;
        }
    }
}
