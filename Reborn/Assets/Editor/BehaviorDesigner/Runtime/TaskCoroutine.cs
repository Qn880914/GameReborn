using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace FrameWork.BehaviorDesigner.Runtime
{
    public class TaskCoroutine
    {
        private IEnumerator m_CoroutineEnumerator;

        private Coroutine m_Coroutine;

        private Behavior m_Parent;

        private string m_CoroutineName;

        private bool m_Stop;

        public Coroutine coroutine
        {
            get
            {
                return this.m_Coroutine;
            }
        }

        public TaskCoroutine(Behavior parent, IEnumerator coroutine, string coroutineName)
        {
            this.m_Parent = parent;
            this.m_CoroutineEnumerator = coroutine;
            this.m_CoroutineName = coroutineName;
            this.m_Coroutine = parent.StartCoroutine(this.RunCoroutine());
        }

        public void Stop()
        {
            this.m_Stop = true;
        }

        [DebuggerHidden]
        public IEnumerator RunCoroutine()
        {
            /*TaskCoroutine.< RunCoroutine > c__Iterator1 < RunCoroutine > c__Iterator = new TaskCoroutine.< RunCoroutine > c__Iterator1();

            < RunCoroutine > c__Iterator.<> f__this = this;
            return < RunCoroutine > c__Iterator;*/

            return this.m_CoroutineEnumerator;
        }
    }
}
