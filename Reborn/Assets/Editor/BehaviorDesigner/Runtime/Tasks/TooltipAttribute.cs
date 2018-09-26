using System;

namespace FrameWork.BehaviorDesigner.Tasks
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class TooltipAttribute : Attribute
    {
        public readonly string m_Tooltip;

        public string tooltip
        {
            get
            {
                return this.m_Tooltip;
            }
        }

        public TooltipAttribute(string tooltip)
        {
            this.m_Tooltip = tooltip;
        }
    }
}
