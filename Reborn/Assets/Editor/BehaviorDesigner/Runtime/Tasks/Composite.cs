using UnityEngine;

namespace FrameWork.BehaviorDesigner.Tasks
{
    public abstract class Composite : ParentTask
    {
        [Tooltip("Specifies the type of conditional abort. More information is located at http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=89."), SerializeField]
        protected AbortType m_AbortType;

        public AbortType abortType
        {
            get
            {
                return this.m_AbortType;
            }
        }
    }
}
