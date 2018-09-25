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
            get { return m_IsShared; }
            set { m_IsShared = value; }
        }

        public bool isGlobal
        {
            get { return m_IsGlobal; }
            set { m_IsGlobal = value; }
        }

        public string name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public string propertyMapping
        {
            get { return m_PropertyMapping; }
            set { m_PropertyMapping = value; }
        }

        public GameObject propertyMappingOwner
        {
            get { return m_PropertyMappingOwner; }
            set { m_PropertyMappingOwner = value; }
        }

        public bool networkSync
        {
            get { return m_NetworkSync; }
            set { m_NetworkSync = value; }
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
