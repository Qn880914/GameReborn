using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace FrameWork.BehaviorDesigner.Runtime
{
    [Serializable]
    public class GenericVariable
    {
        [SerializeField]
        public string type = "SharedString";

        [SerializeField]
        public SharedVariable value;

        public GenericVariable()
        {
            this.value = (Activator.CreateInstance(TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.SharedString"))) as SharedVariable;
        }
    }
}
