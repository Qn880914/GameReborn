using System;

namespace FrameWork.BehaviorDesigner.Tasks
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TaskDescriptionAttribute : Attribute
    {
        public readonly string m_Description;

        public string description
        {
            get
            {
                return this.m_Description;
            }
        }

        public TaskDescriptionAttribute(string description)
        {
            this.m_Description = description;
        }
    }
}
