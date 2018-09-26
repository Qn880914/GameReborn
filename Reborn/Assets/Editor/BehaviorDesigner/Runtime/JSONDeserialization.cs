using FrameWork.BehaviorDesigner.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace FrameWork.BehaviorDesigner.Runtime
{
    public class JSONDeserialization : Object
    {
        public struct TaskField
        {
            public Task task;

            public FieldInfo fieldInfo;

            public TaskField(Task t, FieldInfo f)
            {
                this.task = t;
                this.fieldInfo = f;
            }
        }

        private static Dictionary<JSONDeserialization.TaskField, List<int>> s_TaskIDs = null;

        private static Globa
    }
}
