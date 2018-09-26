using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.BehaviorDesigner.Runtime
{
    public class GlobalVariables : ScriptableObject, IVariableSource
    {
        private static GlobalVariables instance;

        [SerializeField]
        private List<SharedVariable> m_Variables;

        private Dictionary<string, int> m_SharedVariableIndex;

        [SerializeField]
        private VariableSerializationData m_VariableData;

        [SerializeField]
        private string m_Version;

        public static GlobalVariables Instance
        {
            get
            {
                if (GlobalVariables.instance == null)
                {
                    GlobalVariables.instance = (Resources.Load("BehaviorDesignerGlobalVariables", typeof(GlobalVariables)) as GlobalVariables);
                    if (GlobalVariables.instance != null)
                    {
                        GlobalVariables.instance.CheckForSerialization(false);
                    }
                }
                return GlobalVariables.instance;
            }
        }

        public List<SharedVariable> Variables
        {
            get
            {
                return this.m_Variables;
            }
            set
            {
                this.m_Variables = value;
                this.UpdateVariablesIndex();
            }
        }

        public VariableSerializationData VariableData
        {
            get
            {
                return this.m_VariableData;
            }
            set
            {
                this.m_VariableData = value;
            }
        }

        public string Version
        {
            get
            {
                return this.m_Version;
            }
            set
            {
                this.m_Version = value;
            }
        }

        public void CheckForSerialization(bool force)
        {
            if (force || this.m_Variables == null || (this.m_Variables.Count > 0 && this.m_Variables[0] == null))
            {
                if (this.VariableData != null && !string.IsNullOrEmpty(this.VariableData.JSONSerialization))
                {
                    JSONDeserialization.Load(this.VariableData.JSONSerialization, this, this.m_Version);
                }
                else
                {
                    BinaryDeserialization.Load(this, this.m_Version);
                }
            }
        }

        public SharedVariable GetVariable(string name)
        {
            if (name == null)
            {
                return null;
            }
            this.CheckForSerialization(false);
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
            this.CheckForSerialization(false);
            return this.m_Variables;
        }

        public void SetVariable(string name, SharedVariable sharedVariable)
        {
            this.CheckForSerialization(false);
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

        public void SetVariableValue(string name, object value)
        {
            SharedVariable variable = this.GetVariable(name);
            if (variable != null)
            {
                variable.SetValue(value);
                variable.ValueChanged();
            }
        }

        public void UpdateVariableName(SharedVariable sharedVariable, string name)
        {
            this.CheckForSerialization(false);
            sharedVariable.name = name;
            this.UpdateVariablesIndex();
        }

        public void SetAllVariables(List<SharedVariable> variables)
        {
            this.m_Variables = variables;
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
    }
}
