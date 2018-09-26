using FrameWork.BehaviorDesigner.Runtime;
using System;
using System.Collections;
using UnityEngine;

namespace FrameWork.BehaviorDesigner.Tasks
{
    public abstract class Task
    {
        protected GameObject m_GameObject;
        public GameObject gameObject
        {
            set
            {
                this.m_GameObject = value;
            }
        }

        protected Transform m_Transform;
        public Transform transform
        {
            set
            {
                this.m_Transform = value;
            }
        }

        [SerializeField]
        private NodeData m_NodeData;
        public NodeData nodeData
        {
            get
            {
                return this.m_NodeData;
            }
            set
            {
                this.m_NodeData = value;
            }
        }

        [SerializeField]
        private Behavior m_Owner;
        public Behavior owner
        {
            get
            {
                return this.m_Owner;
            }
            set
            {
                this.m_Owner = value;
            }
        }

        [SerializeField]
        private int m_ID = -1;
        public int id
        {
            get
            {
                return this.m_ID;
            }
            set
            {
                this.m_ID = value;
            }
        }

        [SerializeField]
        private string m_FriendlyName = string.Empty;
        public string friendlyName
        {
            get
            {
                return this.m_FriendlyName;
            }
            set
            {
                this.m_FriendlyName = value;
            }
        }

        [SerializeField]
        private bool m_IsInstant = true;
        public bool isInstant
        {
            get
            {
                return this.m_IsInstant;
            }
            set
            {
                this.m_IsInstant = value;
            }
        }

        private int m_ReferenceID = -1;
        public int referenceID
        {
            get
            {
                return this.m_ReferenceID;
            }
            set
            {
                this.m_ReferenceID = value;
            }
        }

        private bool m_Disabled;
        public bool disabled
        {
            get
            {
                return this.m_Disabled;
            }
            set
            {
                this.m_Disabled = value;
            }
        }

        public virtual void OnAwake()
        {
        }

        public virtual void OnStart()
        {
        }

        public virtual TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }

        public virtual void OnLateUpdate()
        {
        }

        public virtual void OnFixedUpdate()
        {
        }

        public virtual void OnEnd()
        {
        }

        public virtual void OnPause(bool paused)
        {
        }

        public virtual float GetPriority()
        {
            return 0f;
        }

        public virtual float GetUtility()
        {
            return 0f;
        }

        public virtual void OnBehaviorRestart()
        {
        }

        public virtual void OnBehaviorComplete()
        {
        }

        public virtual void OnReset()
        {
        }

        public virtual void OnDrawGizmos()
        {
        }

        protected void StartCoroutine(string methodName)
        {
            this.owner.StartTaskCoroutine(this, methodName);
        }

        protected Coroutine StartCoroutine(IEnumerator routine)
        {
            return this.owner.StartCoroutine(routine);
        }

        protected Coroutine StartCoroutine(string methodName, object value)
        {
            return this.owner.StartTaskCoroutine(this, methodName, value);
        }

        protected void StopCoroutine(string methodName)
        {
            this.owner.StopTaskCoroutine(methodName);
        }

        protected void StopCoroutine(IEnumerator routine)
        {
            this.owner.StopCoroutine(routine);
        }

        protected void StopAllCoroutines()
        {
            this.owner.StopAllTaskCoroutines();
        }

        public virtual void OnCollisionEnter(Collision collision)
        {
        }

        public virtual void OnCollisionExit(Collision collision)
        {
        }

        public virtual void OnTriggerEnter(Collider other)
        {
        }

        public virtual void OnTriggerExit(Collider other)
        {
        }

        public virtual void OnCollisionEnter2D(Collision2D collision)
        {
        }

        public virtual void OnCollisionExit2D(Collision2D collision)
        {
        }

        public virtual void OnTriggerEnter2D(Collider2D other)
        {
        }

        public virtual void OnTriggerExit2D(Collider2D other)
        {
        }

        public virtual void OnControllerColliderHit(ControllerColliderHit hit)
        {
        }

        public virtual void OnAnimatorIK()
        {
        }

        protected T GetComponent<T>() where T : Component
        {
            return this.m_GameObject.GetComponent<T>();
        }

        protected Component GetComponent(Type type)
        {
            return this.m_GameObject.GetComponent(type);
        }

        protected GameObject GetDefaultGameObject(GameObject go)
        {
            if (go == null)
            {
                return this.m_GameObject;
            }

            return go;
        }
    }
}
