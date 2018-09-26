using System;
using System.Reflection;
using UnityEngine;

namespace FrameWork.BehaviorDesigner.Runtime
{
    public abstract class SharedVariable<T> : SharedVariable
    {
        private Func<T> m_Getter;

        private Action<T> m_Setter;

        [SerializeField]
        protected T m_Value;

        public T value
        {
            get
            {
                return (this.m_Getter == null) ? this.m_Value : this.m_Getter();
            }
            set
            {
                bool flag = base.networkSync && !this.m_Value.Equals(value);
                if (this.m_Setter != null)
                {
                    this.m_Setter(value);
                }
                else
                {
                    this.m_Value = value;
                }
                if (flag)
                {
                    base.ValueChanged();
                }
            }
        }

        public override void InitializePropertyMapping(BehaviorSource behaviorSource)
        {
            if (!Application.isPlaying || !(behaviorSource.owner.GetObject() is Behavior))
            {
                return;
            }
            if (!string.IsNullOrEmpty(base.propertyMapping))
            {
                string[] array = base.propertyMapping.Split(new char[]
                {
                    '/'
                });
                GameObject gameObject;
                if (!object.Equals(base.propertyMappingOwner, null))
                {
                    gameObject = base.propertyMappingOwner;
                }
                else
                {
                    gameObject = (behaviorSource.owner.GetObject() as Behavior).gameObject;
                }
                if (gameObject == null)
                {
                    Debug.LogError("Error: Unable to find GameObject on " + behaviorSource.behaviorName + " for property mapping with variable " + base.Name);
                    return;
                }
                Component component = gameObject.GetComponent(TaskUtility.GetTypeWithinAssembly(array[0]));
                if (component == null)
                {
                    Debug.LogError("Error: Unable to find component on " + behaviorSource.behaviorName + " for property mapping with variable " + base.Name);
                    return;
                }
                Type type = component.GetType();
                PropertyInfo property = type.GetProperty(array[1]);
                if (property != null)
                {
                    MethodInfo methodInfo = property.GetGetMethod();
                    if (methodInfo != null)
                    {
                        this.m_Getter = (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), component, methodInfo);
                    }
                    methodInfo = property.GetSetMethod();
                    if (methodInfo != null)
                    {
                        this.m_Setter = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), component, methodInfo);
                    }
                }
            }
        }

        public override object GetValue()
        {
            return this.value;
        }

        public override void SetValue(object value)
        {
            if (this.m_Setter != null)
            {
                this.m_Setter((T)((object)value));
            }
            else
            {
                this.m_Value = (T)((object)value);
            }
        }

        public override string ToString()
        {
            string arg_2E_0;
            if (this.value == null)
            {
                arg_2E_0 = "(null)";
            }
            else
            {
                T value = this.value;
                arg_2E_0 = value.ToString();
            }
            return arg_2E_0;
        }
    }
}
