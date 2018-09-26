using System;

namespace FrameWork.BehaviorDesigner.Tasks
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TaskIconAttribute : Attribute
    {
        public readonly string m_IconPath;

        public string iconPath
        {
            get
            {
                return this.m_IconPath;
            }
        }

        public TaskIconAttribute(string iconPath)
        {
            this.m_IconPath = iconPath;
        }
    }
}
