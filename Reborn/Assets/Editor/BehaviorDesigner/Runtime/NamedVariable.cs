using System;
using UnityEngine;

namespace FrameWork.BehaviorDesigner.Runtime
{
    [Serializable]
    public class NamedVariable : GenericVariable
    {
        [SerializeField]
        public string name = string.Empty;
    }
}
