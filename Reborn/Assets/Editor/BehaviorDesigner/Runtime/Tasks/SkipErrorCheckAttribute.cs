using System;

namespace FrameWork.BehaviorDesigner.Tasks
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SkipErrorCheckAttribute : Attribute
    {
    }
}
