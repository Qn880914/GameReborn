using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using FrameWork.BehaviorDesigner.Tasks;

namespace FrameWork.BehaviorDesigner.Runtime
{
    [Serializable]
    public class NodeData
    {
        [SerializeField]
        private object m_NodeDesigner;
        public object nodeDesigner
        {
            get
            {
                return this.m_NodeDesigner;
            }
            set
            {
                this.m_NodeDesigner = value;
            }
        }

        [SerializeField]
        private Vector2 m_Offset;
        public Vector2 offset
        {
            get
            {
                return this.m_Offset;
            }
            set
            {
                this.m_Offset = value;
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
        private string m_Comment = string.Empty;
        public string comment
        {
            get
            {
                return this.m_Comment;
            }
            set
            {
                this.m_Comment = value;
            }
        }

        [SerializeField]
        private bool m_IsBreakpoint;
        public bool isBreakpoint
        {
            get
            {
                return this.m_IsBreakpoint;
            }
            set
            {
                this.m_IsBreakpoint = value;
            }
        }

        [SerializeField]
        private Texture m_Icon;
        public Texture icon
        {
            get
            {
                return this.m_Icon;
            }
            set
            {
                this.m_Icon = value;
            }
        }

        [SerializeField]
        private bool m_Collapsed;
        public bool collapsed
        {
            get
            {
                return this.m_Collapsed;
            }
            set
            {
                this.m_Collapsed = value;
            }
        }

        [SerializeField]
        private int m_ColorIndex;
        public int colorIndex
        {
            get
            {
                return this.m_ColorIndex;
            }
            set
            {
                this.m_ColorIndex = value;
            }
        }

        [SerializeField]
        private List<string> m_WatchedFieldNames;
        public List<string> watchedFieldNames
        {
            get
            {
                return this.m_WatchedFieldNames;
            }
            set
            {
                this.m_WatchedFieldNames = value;
            }
        }

        private List<FieldInfo> m_WatchedFields;
        public List<FieldInfo> watchedFields
        {
            get
            {
                return this.m_WatchedFields;
            }
            set
            {
                this.m_WatchedFields = value;
            }
        }

        private float m_PushTime = -1f;
        public float pushTime
        {
            get
            {
                return this.m_PushTime;
            }
            set
            {
                this.m_PushTime = value;
            }
        }

        private float m_PopTime = -1f;
        public float popTime
        {
            get
            {
                return this.m_PopTime;
            }
            set
            {
                this.m_PopTime = value;
            }
        }

        private float m_InterruptTime = -1f;
        public float interruptTime
        {
            get
            {
                return this.m_InterruptTime;
            }
            set
            {
                this.m_InterruptTime = value;
            }
        }

        private bool m_IsReevaluating;
        public bool isReevaluating
        {
            get
            {
                return this.m_IsReevaluating;
            }
            set
            {
                this.m_IsReevaluating = value;
            }
        }

        private TaskStatus m_ExecutionStatus;
        public TaskStatus executionStatus
        {
            get
            {
                return this.m_ExecutionStatus;
            }
            set
            {
                this.m_ExecutionStatus = value;
            }
        }

        public void InitWatchedFields(Task task)
        {
            if (this.m_WatchedFieldNames != null && this.m_WatchedFieldNames.Count > 0)
            {
                this.m_WatchedFields = new List<FieldInfo>();
                for (int i = 0; i < this.m_WatchedFieldNames.Count; i++)
                {
                    FieldInfo field = task.GetType().GetField(this.m_WatchedFieldNames[i], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (field != null)
                    {
                        this.m_WatchedFields.Add(field);
                    }
                }
            }
        }

        public void CopyFrom(NodeData nodeData, Task task)
        {
            this.m_NodeDesigner = nodeData.m_NodeDesigner;
            this.m_Offset = nodeData.m_Offset;
            this.m_Comment = nodeData.m_Comment;
            this.m_IsBreakpoint = nodeData.m_IsBreakpoint;
            this.m_Collapsed = nodeData.m_Collapsed;
            if (nodeData.m_WatchedFields != null && nodeData.m_WatchedFields.Count > 0)
            {
                this.m_WatchedFields = new List<FieldInfo>();
                this.m_WatchedFieldNames = new List<string>();
                for (int i = 0; i < nodeData.m_WatchedFields.Count; i++)
                {
                    FieldInfo field = task.GetType().GetField(nodeData.m_WatchedFields[i].Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (field != null)
                    {
                        this.m_WatchedFields.Add(field);
                        this.m_WatchedFieldNames.Add(field.Name);
                    }
                }
            }
        }

        public bool ContainsWatchedField(FieldInfo field)
        {
            return this.m_WatchedFields != null && this.m_WatchedFields.Contains(field);
        }

        public void AddWatchedField(FieldInfo field)
        {
            if (this.m_WatchedFields == null)
            {
                this.m_WatchedFields = new List<FieldInfo>();
                this.m_WatchedFieldNames = new List<string>();
            }
            this.m_WatchedFields.Add(field);
            this.m_WatchedFieldNames.Add(field.Name);
        }

        public void RemoveWatchedField(FieldInfo field)
        {
            if (this.m_WatchedFields != null)
            {
                this.m_WatchedFields.Remove(field);
                this.m_WatchedFieldNames.Remove(field.Name);
            }
        }

        private static Vector2 StringToVector2(string vector2String)
        {
            string[] array = vector2String.Substring(1, vector2String.Length - 2).Split(new char[]
            {
                ','
            });
            return new Vector3(float.Parse(array[0]), float.Parse(array[1]));
        }
    }
}
