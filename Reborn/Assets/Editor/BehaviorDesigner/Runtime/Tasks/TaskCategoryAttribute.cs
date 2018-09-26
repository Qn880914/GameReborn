using System;

namespace FrameWork.BehaviorDesigner.Tasks
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TaskCategoryAttribute : Attribute
    {
        public readonly string m_Category;

        public string category
        {
            get
            {
                return this.m_Category;
            }
        }

        public TaskCategoryAttribute(string category)
        {
            this.m_Category = category;
        }
    }
}
