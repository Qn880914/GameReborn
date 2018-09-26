using System;

namespace FrameWork.BehaviorDesigner.Tasks
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class HelpURLAttribute : Attribute
    {
        private readonly string m_URL;

        public string url
        {
            get
            {
                return this.m_URL;
            }
        }

        public HelpURLAttribute(string url)
        {
            this.m_URL = url;
        }
    }
}
