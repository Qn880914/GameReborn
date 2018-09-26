using UnityEngine;

namespace FrameWork.BehaviorDesigner.Runtime
{
    public abstract class SharedVariable
    {
        [SerializeField]
        private bool m_IsShared;

        [SerializeField]
        private bool m_IsGlobal;

        [SerializeField]
        private string m_Name;

        [SerializeField]
        private string m_PropertyMapping;

        [SerializeField]
        private GameObject m_PropertyMappingOwner;

        [SerializeField]
        private bool m_NetworkSync;

        public bool isShared
        {
            get { return this.m_IsShared; }
            set { this.m_IsShared = value; }
        }

        public bool isGlobal
        {
            get { return this.m_IsGlobal; }
            set { this.m_IsGlobal = value; }
        }

        public string name
        {
            get { return this.m_Name; }
            set { this.m_Name = value; }
        }

        public string propertyMapping
        {
            get { return this.m_PropertyMapping; }
            set { this.m_PropertyMapping = value; }
        }

        public GameObject propertyMappingOwner
        {
            get { return this.m_PropertyMappingOwner; }
            set { this.m_PropertyMappingOwner = value; }
        }

        public bool networkSync
        {
            get { return this.m_NetworkSync; }
            set { this.m_NetworkSync = value; }
        }

        public bool isNone
        {
            get { return this.m_IsShared && string.IsNullOrEmpty(this.m_Name); }
        }

        public void ValueChanged()
        { }

        public virtual void InitializePropertyMapping(BehaviorSource behaviorSource)
        { }

        public abstract object GetValue();

        public abstract void SetValue(object value);
    }
}
