using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.BehaviorDesigner.Runtime
{
    public interface IBehavior
    {
        string GetOwnerName();

        int GetInstanceID();

        BehaviorSource GetBehaviorSource();

        void SetBehaviorSource(BehaviorSource behaviorSource);

        Object GetObject();

        SharedVariable GetVariable(string name);

        void SetVariable(string name, SharedVariable item);

        void SetVariable(string name, object value);
    }
}
