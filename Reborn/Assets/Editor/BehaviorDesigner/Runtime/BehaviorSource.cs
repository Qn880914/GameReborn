using FrameWork.BehaviorDesigner.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.BehaviorDesigner.Runtime
{
    [Serializable]
    public class BehaviorSource : IVariableSource
    {
        public string behaviorName = "Behavior";

        public string behaviorDescription = string.Empty;

        private int m_BehaviorID = -1;

        private Task m_EntryTask;

        private Task m_RootTask;

        private List<Task> m_DetachedTasks;

        private List<SharedVariable> m_Variables;

        private Dictionary<string, int> m_SharedVariableIndex;

        [NonSerialized]
        private bool m_HasSerialized;

        [SerializeField]
        private TaskSerializationData m_TaskData;

        [SerializeField]
        private IBehavior m_Owner;

        public int behaviorID
        {
            get { return this.m_BehaviorID; }
            set { this.m_BehaviorID = value; }
        }

        public Task entryTask
        {
            get { return this.m_EntryTask; }
            set { this.m_EntryTask = value; }
        }

        public Task rootTask
        {
            get { return this.m_RootTask; }
            set { this.m_RootTask = value; }
        }

        public List<Task> detachedTasks
        {
            get { return this.m_DetachedTasks; }
            set { this.m_DetachedTasks = value; }
        }

        public List<SharedVariable> variables
        {
            get
            {
                this.CheckForSerialization(false, null);
                return this.m_Variables;
            }
            set
            {
                this.m_Variables = value;
                this.UpdateVariablesIndex();
            }
        }

        public bool hasSerialized
        {
            get { return this.m_HasSerialized; }
            set { this.m_HasSerialized = value; }
        }

        public TaskSerializationData taskData
        {
            get { return m_TaskData; }
            set { m_TaskData = value; }
        }

        public IBehavior owner
        {
            get { return m_Owner; }
            set { m_Owner = value; }
        }

        public BehaviorSource() { }

        public BehaviorSource(IBehavior owner)
        {
            Initialize(owner);
        }

        public void Initialize(IBehavior owner)
        {
            this.m_Owner = owner;
        }

        public void Save(Task entryTask, Task rootTask, List<Task> detachedTasks)
        {
            this.m_EntryTask = entryTask;
            this.m_RootTask = rootTask;
            this.m_DetachedTasks = detachedTasks;
        }

        public void Load(out Task entryTask, out Task rootTask, out List<Task> detachedTasks)
        {
            entryTask = this.m_EntryTask;
            rootTask = this.m_RootTask;
            detachedTasks = this.m_DetachedTasks;
        }

        public bool CheckForSerialization(bool force, BehaviorSource behaviorSource = null)
        {
            bool flag = (behaviorSource == null) ? this.m_HasSerialized : behaviorSource.m_HasSerialized;
            if (!flag || force)
            {
                if (behaviorSource != null)
                {
                    behaviorSource.m_HasSerialized = true;
                }
                else
                {
                    this.m_HasSerialized = true;
                }
                if (this.m_TaskData != null && !string.IsNullOrEmpty(this.m_TaskData.JSONSerialization))
                {
                    JSONDeserialization.Load(this.m_TaskData, (behaviorSource != null) ? behaviorSource : this);
                }
                else
                {
                    BinaryDeserialization.Load(this.m_TaskData, (behaviorSource != null) ? behaviorSource : this);
                }
                return true;
            }
            return false;
        }

        public SharedVariable GetVariable(string name)
        {
            if (name == null)
            {
                return null;
            }
            this.CheckForSerialization(false, null);
            if (this.m_Variables != null)
            {
                if (this.m_SharedVariableIndex == null || this.m_SharedVariableIndex.Count != this.m_Variables.Count)
                {
                    this.UpdateVariablesIndex();
                }
                int index;
                if (this.m_SharedVariableIndex.TryGetValue(name, out index))
                {
                    return this.m_Variables[index];
                }
            }
            return null;
        }

        public List<SharedVariable> GetAllVariables()
        {
            this.CheckForSerialization(false, null);
            return this.m_Variables;
        }

        public void SetVariable(string name, SharedVariable sharedVariable)
        {
            if (this.m_Variables == null)
            {
                this.m_Variables = new List<SharedVariable>();
            }
            else if (this.m_SharedVariableIndex == null)
            {
                this.UpdateVariablesIndex();
            }
            sharedVariable.name = name;
            int index;
            if (this.m_SharedVariableIndex != null && this.m_SharedVariableIndex.TryGetValue(name, out index))
            {
                SharedVariable sharedVariable2 = this.m_Variables[index];
                if (!sharedVariable2.GetType().Equals(typeof(SharedVariable)) && !sharedVariable2.GetType().Equals(sharedVariable.GetType()))
                {
                    Debug.LogError(string.Format("Error: Unable to set SharedVariable {0} - the variable type {1} does not match the existing type {2}", name, sharedVariable2.GetType(), sharedVariable.GetType()));
                }
                else if (!string.IsNullOrEmpty(sharedVariable.propertyMapping))
                {
                    sharedVariable2.propertyMappingOwner = sharedVariable.propertyMappingOwner;
                    sharedVariable2.propertyMapping = sharedVariable.propertyMapping;
                    sharedVariable2.InitializePropertyMapping(this);
                }
                else
                {
                    sharedVariable2.SetValue(sharedVariable.GetValue());
                }
            }
            else
            {
                this.m_Variables.Add(sharedVariable);
                this.UpdateVariablesIndex();
            }
        }

        public void UpdateVariableName(SharedVariable sharedVariable, string name)
        {
            this.CheckForSerialization(false, null);
            sharedVariable.name = name;
            this.UpdateVariablesIndex();
        }

        public void SetAllVariables(List<SharedVariable> variables)
        {
            this.m_Variables = variables;
            this.UpdateVariablesIndex();
        }

        private void UpdateVariablesIndex()
        {
            if (this.m_Variables == null)
            {
                if (this.m_SharedVariableIndex != null)
                {
                    this.m_SharedVariableIndex = null;
                }
                return;
            }
            if (this.m_SharedVariableIndex == null)
            {
                this.m_SharedVariableIndex = new Dictionary<string, int>(this.m_Variables.Count);
            }
            else
            {
                this.m_SharedVariableIndex.Clear();
            }
            for (int i = 0; i < this.m_Variables.Count; i++)
            {
                if (this.m_Variables[i] != null)
                {
                    this.m_SharedVariableIndex.Add(this.m_Variables[i].name, i);
                }
            }
        }

        public override string ToString()
        {
            if (this.m_Owner == null || this.m_Owner.GetObject() == null)
            {
                return this.behaviorName;
            }
            return string.Format("{0} - {1}", this.m_Owner.GetOwnerName(), this.behaviorName);
        }
    }
}
