using FrameWork.BehaviorDesigner.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace FrameWork.BehaviorDesigner.Runtime
{
    [Serializable]
    public abstract class Behavior : MonoBehaviour, IBehavior
    {
        public enum EventTypes
        {
            OnCollisionEnter,
            OnCollisionExit,
            OnTriggerEnter,
            OnTriggerExit,
            OnCollisionEnter2D,
            OnCollisionExit2D,
            OnTriggerEnter2D,
            OnTriggerExit2D,
            OnControllerColliderHit,
            OnLateUpdate,
            OnFixedUpdate,
            OnAnimatorIK,
            None
        }

        public enum GizmoViewMode
        {
            Running,
            Always,
            Selected,
            Never
        }

        public delegate void BehaviorHandler(Behavior behavior);

        [SerializeField]
        private bool m_StartWhenEnabled = true;

        [SerializeField]
        private bool m_PauseWhenDisabled;

        [SerializeField]
        private bool m_RestartWhenComplete;

        [SerializeField]
        private bool m_LogTaskChanges;

        [SerializeField]
        private int m_Group;

        [SerializeField]
        private bool m_ResetValuesOnRestart;

        [SerializeField]
        private ExternalBehavior m_ExternalBehavior;

        private bool m_HasInheritedVariables;

        [SerializeField]
        private BehaviorSource m_BehaviorSource;

        private bool isPaused;

        private TaskStatus m_ExecutionStatus;

        private bool m_Initialized;

        private Dictionary<Task, Dictionary<string, object>> defaultValues;

        private Dictionary<string, object> defaultVariableValues;

        private bool[] hasEvent = new bool[12];

        private Dictionary<string, List<TaskCoroutine>> activeTaskCoroutines;

        private Dictionary<Type, Dictionary<string, Delegate>> eventTable;

        [NonSerialized]
        public Behavior.GizmoViewMode gizmoViewMode;

        [NonSerialized]
        public bool showBehaviorDesignerGizmo = true;

        public event Behavior.BehaviorHandler onBehaviorStart;

        public event Behavior.BehaviorHandler onBehaviorRestart;

        public event Behavior.BehaviorHandler onBehaviorEnd;

        public bool startWhenEnabled
        {
            get
            {
                return this.m_StartWhenEnabled;
            }
            set
            {
                this.m_StartWhenEnabled = value;
            }
        }

        public bool pauseWhenDisabled
        {
            get
            {
                return this.m_PauseWhenDisabled;
            }
            set
            {
                this.m_PauseWhenDisabled = value;
            }
        }

        public bool restartWhenComplete
        {
            get
            {
                return this.m_RestartWhenComplete;
            }
            set
            {
                this.m_RestartWhenComplete = value;
            }
        }

        public bool logTaskChanges
        {
            get
            {
                return this.m_LogTaskChanges;
            }
            set
            {
                this.m_LogTaskChanges = value;
            }
        }

        public int group
        {
            get
            {
                return this.m_Group;
            }
            set
            {
                this.m_Group = value;
            }
        }

        public bool resetValuesOnRestart
        {
            get
            {
                return this.m_ResetValuesOnRestart;
            }
            set
            {
                this.m_ResetValuesOnRestart = value;
            }
        }

        public ExternalBehavior externalBehavior
        {
            get
            {
                return this.m_ExternalBehavior;
            }
            set
            {
                if (BehaviorManager.instance != null)
                {
                    BehaviorManager.instance.DisableBehavior(this);
                }
                this.m_BehaviorSource.hasSerialized = false;
                this.m_Initialized = false;
                this.m_ExternalBehavior = value;
                if (this.startWhenEnabled)
                {
                    this.EnableBehavior();
                }
            }
        }

        public bool hasInheritedVariables
        {
            get
            {
                return this.m_HasInheritedVariables;
            }
            set
            {
                this.m_HasInheritedVariables = value;
            }
        }

        public string behaviorName
        {
            get
            {
                return this.m_BehaviorSource.behaviorName;
            }
            set
            {
                this.m_BehaviorSource.behaviorName = value;
            }
        }

        public string behaviorDescription
        {
            get
            {
                return this.m_BehaviorSource.behaviorDescription;
            }
            set
            {
                this.m_BehaviorSource.behaviorDescription = value;
            }
        }

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

        public bool[] HasEvent
        {
            get
            {
                return this.hasEvent;
            }
        }

        public Behavior()
        {
            this.m_BehaviorSource = new BehaviorSource(this);
        }

        public BehaviorSource GetBehaviorSource()
        {
            return this.m_BehaviorSource;
        }

        public void SetBehaviorSource(BehaviorSource behaviorSource)
        {
            this.m_BehaviorSource = behaviorSource;
        }

        public UnityEngine.Object GetObject()
        {
            return this;
        }

        public string GetOwnerName()
        {
            return base.gameObject.name;
        }

        public void Start()
        {
            if (this.startWhenEnabled)
            {
                this.EnableBehavior();
            }
        }

        private bool TaskContainsMethod(string methodName, Task task)
        {
            if (task == null)
            {
                return false;
            }
            MethodInfo method = task.GetType().GetMethod(methodName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method != null && method.DeclaringType.IsAssignableFrom(task.GetType()))
            {
                return true;
            }
            if (task is ParentTask)
            {
                ParentTask parentTask = task as ParentTask;
                if (parentTask.Children != null)
                {
                    for (int i = 0; i < parentTask.Children.Count; i++)
                    {
                        if (this.TaskContainsMethod(methodName, parentTask.Children[i]))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void EnableBehavior()
        {
            Behavior.CreateBehaviorManager();
            if (BehaviorManager.instance != null)
            {
                BehaviorManager.instance.EnableBehavior(this);
            }
            if (!this.m_Initialized)
            {
                for (int i = 0; i < 12; i++)
                {
                    this.hasEvent[i] = this.TaskContainsMethod(((Behavior.EventTypes)i).ToString(), this.m_BehaviorSource.rootTask);
                }
                this.m_Initialized = true;
            }
        }

        public void DisableBehavior()
        {
            if (BehaviorManager.instance != null)
            {
                BehaviorManager.instance.DisableBehavior(this, this.m_PauseWhenDisabled);
                this.isPaused = this.m_PauseWhenDisabled;
            }
        }

        public void DisableBehavior(bool pause)
        {
            if (BehaviorManager.instance != null)
            {
                BehaviorManager.instance.DisableBehavior(this, pause);
                this.isPaused = pause;
            }
        }

        public void OnEnable()
        {
            if (BehaviorManager.instance != null && this.isPaused)
            {
                BehaviorManager.instance.EnableBehavior(this);
                this.isPaused = false;
            }
            else if (this.startWhenEnabled && this.m_Initialized)
            {
                this.EnableBehavior();
            }
        }

        public void OnDisable()
        {
            this.DisableBehavior();
        }

        public void OnDestroy()
        {
            if (BehaviorManager.instance != null)
            {
                BehaviorManager.instance.DestroyBehavior(this);
            }
        }

        public SharedVariable GetVariable(string name)
        {
            this.CheckForSerialization();
            return this.m_BehaviorSource.GetVariable(name);
        }

        public void SetVariable(string name, SharedVariable item)
        {
            this.CheckForSerialization();
            this.m_BehaviorSource.SetVariable(name, item);
        }

        public void SetVariableValue(string name, object value)
        {
            SharedVariable variable = this.GetVariable(name);
            if (variable != null)
            {
                if (value is SharedVariable)
                {
                    SharedVariable sharedVariable = value as SharedVariable;
                    if (!string.IsNullOrEmpty(sharedVariable.propertyMapping))
                    {
                        variable.propertyMapping = sharedVariable.propertyMapping;
                        variable.propertyMappingOwner = sharedVariable.propertyMappingOwner;
                        variable.InitializePropertyMapping(this.m_BehaviorSource);
                    }
                    else
                    {
                        variable.SetValue(sharedVariable.GetValue());
                    }
                }
                else
                {
                    variable.SetValue(value);
                }
                variable.ValueChanged();
            }
            else if (value is SharedVariable)
            {
                SharedVariable sharedVariable2 = value as SharedVariable;
                SharedVariable sharedVariable3 = TaskUtility.CreateInstance(sharedVariable2.GetType()) as SharedVariable;
                sharedVariable3.name = sharedVariable2.name;
                sharedVariable3.isShared = sharedVariable2.isShared;
                sharedVariable3.isGlobal = sharedVariable2.isGlobal;
                if (!string.IsNullOrEmpty(sharedVariable2.propertyMapping))
                {
                    sharedVariable3.propertyMapping = sharedVariable2.propertyMapping;
                    sharedVariable3.propertyMappingOwner = sharedVariable2.propertyMappingOwner;
                    sharedVariable3.InitializePropertyMapping(this.m_BehaviorSource);
                }
                else
                {
                    sharedVariable3.SetValue(sharedVariable2.GetValue());
                }
                this.m_BehaviorSource.SetVariable(name, sharedVariable3);
            }
            else
            {
                Debug.LogError("Error: No variable exists with name " + name);
            }
        }

        public List<SharedVariable> GetAllVariables()
        {
            this.CheckForSerialization();
            return this.m_BehaviorSource.GetAllVariables();
        }

        public void CheckForSerialization()
        {
            if (this.externalBehavior != null)
            {
                List<SharedVariable> list = null;
                bool force = false;
                if (!this.hasInheritedVariables)
                {
                    this.m_BehaviorSource.CheckForSerialization(false, null);
                    list = this.m_BehaviorSource.GetAllVariables();
                    this.hasInheritedVariables = true;
                    force = true;
                }
                this.externalBehavior.BehaviorSource.owner = this.externalBehavior;
                this.externalBehavior.BehaviorSource.CheckForSerialization(force, this.GetBehaviorSource());
                this.externalBehavior.BehaviorSource.entryTask = this.m_BehaviorSource.entryTask;
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i] != null)
                        {
                            this.m_BehaviorSource.SetVariable(list[i].name, list[i]);
                        }
                    }
                }
            }
            else
            {
                this.m_BehaviorSource.CheckForSerialization(false, null);
            }
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (this.hasEvent[0] && BehaviorManager.instance != null)
            {
                BehaviorManager.instance.BehaviorOnCollisionEnter(collision, this);
            }
        }

        public void OnCollisionExit(Collision collision)
        {
            if (this.hasEvent[1] && BehaviorManager.instance != null)
            {
                BehaviorManager.instance.BehaviorOnCollisionExit(collision, this);
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (this.hasEvent[2] && BehaviorManager.instance != null)
            {
                BehaviorManager.instance.BehaviorOnTriggerEnter(other, this);
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (this.hasEvent[3] && BehaviorManager.instance != null)
            {
                BehaviorManager.instance.BehaviorOnTriggerExit(other, this);
            }
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (this.hasEvent[4] && BehaviorManager.instance != null)
            {
                BehaviorManager.instance.BehaviorOnCollisionEnter2D(collision, this);
            }
        }

        public void OnCollisionExit2D(Collision2D collision)
        {
            if (this.hasEvent[5] && BehaviorManager.instance != null)
            {
                BehaviorManager.instance.BehaviorOnCollisionExit2D(collision, this);
            }
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (this.hasEvent[6] && BehaviorManager.instance != null)
            {
                BehaviorManager.instance.BehaviorOnTriggerEnter2D(other, this);
            }
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            if (this.hasEvent[7] && BehaviorManager.instance != null)
            {
                BehaviorManager.instance.BehaviorOnTriggerExit2D(other, this);
            }
        }

        public void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (this.hasEvent[8] && BehaviorManager.instance != null)
            {
                BehaviorManager.instance.BehaviorOnControllerColliderHit(hit, this);
            }
        }

        public void OnAnimatorIK()
        {
            if (this.hasEvent[11] && BehaviorManager.instance != null)
            {
                BehaviorManager.instance.BehaviorOnAnimatorIK(this);
            }
        }

        public void OnDrawGizmos()
        {
            this.DrawTaskGizmos(false);
        }

        public void OnDrawGizmosSelected()
        {
            if (this.showBehaviorDesignerGizmo)
            {
                Gizmos.DrawIcon(base.transform.position, "Behavior Designer Scene Icon.png");
            }
            this.DrawTaskGizmos(true);
        }

        private void DrawTaskGizmos(bool selected)
        {
            if (this.gizmoViewMode == Behavior.GizmoViewMode.Never || (this.gizmoViewMode == Behavior.GizmoViewMode.Selected && !selected))
            {
                return;
            }
            if (this.gizmoViewMode == Behavior.GizmoViewMode.Running || this.gizmoViewMode == Behavior.GizmoViewMode.Always || (Application.isPlaying && this.executionStatus == TaskStatus.Running) || !Application.isPlaying)
            {
                this.CheckForSerialization();
                this.DrawTaskGizmos(this.m_BehaviorSource.rootTask);
                List<Task> detachedTasks = this.m_BehaviorSource.detachedTasks;
                if (detachedTasks != null)
                {
                    for (int i = 0; i < detachedTasks.Count; i++)
                    {
                        this.DrawTaskGizmos(detachedTasks[i]);
                    }
                }
            }
        }

        private void DrawTaskGizmos(Task task)
        {
            if (task == null)
            {
                return;
            }
            if (this.gizmoViewMode == Behavior.GizmoViewMode.Running && !task.NodeData.IsReevaluating &&
                (task.NodeData.IsReevaluating || task.NodeData.ExecutionStatus != TaskStatus.Running))
            {
                return;
            }
            task.OnDrawGizmos();
            if (task is ParentTask)
            {
                ParentTask parentTask = task as ParentTask;
                if (parentTask.Children != null)
                {
                    for (int i = 0; i < parentTask.Children.Count; i++)
                    {
                        this.DrawTaskGizmos(parentTask.Children[i]);
                    }
                }
            }
        }

        public T FindTask<T>() where T : Task
        {
            return this.FindTask<T>(this.m_BehaviorSource.rootTask);
        }

        private T FindTask<T>(Task task) where T : Task
        {
            if (task.GetType().Equals(typeof(T)))
            {
                return (T)((object)task);
            }
            ParentTask parentTask;
            if ((parentTask = (task as ParentTask)) != null && parentTask.Children != null)
            {
                for (int i = 0; i < parentTask.Children.Count; i++)
                {
                    T result = (T)((object)null);
                    if ((result = this.FindTask<T>(parentTask.Children[i])) != null)
                    {
                        return result;
                    }
                }
            }
            return (T)((object)null);
        }

        public List<T> FindTasks<T>() where T : Task
        {
            this.CheckForSerialization();
            List<T> result = new List<T>();
            this.FindTasks<T>(this.m_BehaviorSource.rootTask, ref result);
            return result;
        }

        private void FindTasks<T>(Task task, ref List<T> taskList) where T : Task
        {
            if (typeof(T).IsAssignableFrom(task.GetType()))
            {
                taskList.Add((T)((object)task));
            }
            ParentTask parentTask;
            if ((parentTask = (task as ParentTask)) != null && parentTask.Children != null)
            {
                for (int i = 0; i < parentTask.Children.Count; i++)
                {
                    this.FindTasks<T>(parentTask.Children[i], ref taskList);
                }
            }
        }

        public Task FindTaskWithName(string taskName)
        {
            this.CheckForSerialization();
            return this.FindTaskWithName(taskName, this.m_BehaviorSource.rootTask);
        }

        private Task FindTaskWithName(string taskName, Task task)
        {
            if (task.FriendlyName.Equals(taskName))
            {
                return task;
            }
            ParentTask parentTask;
            if ((parentTask = (task as ParentTask)) != null && parentTask.Children != null)
            {
                for (int i = 0; i < parentTask.Children.Count; i++)
                {
                    Task result;
                    if ((result = this.FindTaskWithName(taskName, parentTask.Children[i])) != null)
                    {
                        return result;
                    }
                }
            }
            return null;
        }

        public List<Task> FindTasksWithName(string taskName)
        {
            this.CheckForSerialization();
            List<Task> result = new List<Task>();
            this.FindTasksWithName(taskName, this.m_BehaviorSource.rootTask, ref result);
            return result;
        }

        private void FindTasksWithName(string taskName, Task task, ref List<Task> taskList)
        {
            if (task.FriendlyName.Equals(taskName))
            {
                taskList.Add(task);
            }
            ParentTask parentTask;
            if ((parentTask = (task as ParentTask)) != null && parentTask.Children != null)
            {
                for (int i = 0; i < parentTask.Children.Count; i++)
                {
                    this.FindTasksWithName(taskName, parentTask.Children[i], ref taskList);
                }
            }
        }

        public List<Task> GetActiveTasks()
        {
            if (BehaviorManager.instance == null)
            {
                return null;
            }
            return BehaviorManager.instance.GetActiveTasks(this);
        }

        public Coroutine StartTaskCoroutine(Task task, string methodName)
        {
            MethodInfo method = task.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method == null)
            {
                Debug.LogError("Unable to start coroutine " + methodName + ": method not found");
                return null;
            }
            if (this.activeTaskCoroutines == null)
            {
                this.activeTaskCoroutines = new Dictionary<string, List<TaskCoroutine>>();
            }
            TaskCoroutine taskCoroutine = new TaskCoroutine(this, (IEnumerator)method.Invoke(task, new object[0]), methodName);
            if (this.activeTaskCoroutines.ContainsKey(methodName))
            {
                List<TaskCoroutine> list = this.activeTaskCoroutines[methodName];
                list.Add(taskCoroutine);
                this.activeTaskCoroutines[methodName] = list;
            }
            else
            {
                List<TaskCoroutine> list2 = new List<TaskCoroutine>();
                list2.Add(taskCoroutine);
                this.activeTaskCoroutines.Add(methodName, list2);
            }
            return taskCoroutine.coroutine;
        }

        public Coroutine StartTaskCoroutine(Task task, string methodName, object value)
        {
            MethodInfo method = task.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method == null)
            {
                Debug.LogError("Unable to start coroutine " + methodName + ": method not found");
                return null;
            }
            if (this.activeTaskCoroutines == null)
            {
                this.activeTaskCoroutines = new Dictionary<string, List<TaskCoroutine>>();
            }
            TaskCoroutine taskCoroutine = new TaskCoroutine(this, (IEnumerator)method.Invoke(task, new object[]
            {
                value
            }), methodName);
            if (this.activeTaskCoroutines.ContainsKey(methodName))
            {
                List<TaskCoroutine> list = this.activeTaskCoroutines[methodName];
                list.Add(taskCoroutine);
                this.activeTaskCoroutines[methodName] = list;
            }
            else
            {
                List<TaskCoroutine> list2 = new List<TaskCoroutine>();
                list2.Add(taskCoroutine);
                this.activeTaskCoroutines.Add(methodName, list2);
            }
            return taskCoroutine.coroutine;
        }

        public void StopTaskCoroutine(string methodName)
        {
            if (!this.activeTaskCoroutines.ContainsKey(methodName))
            {
                return;
            }
            List<TaskCoroutine> list = this.activeTaskCoroutines[methodName];
            for (int i = 0; i < list.Count; i++)
            {
                list[i].Stop();
            }
        }

        public void StopAllTaskCoroutines()
        {
            base.StopAllCoroutines();
            foreach (KeyValuePair<string, List<TaskCoroutine>> current in this.activeTaskCoroutines)
            {
                List<TaskCoroutine> value = current.Value;
                for (int i = 0; i < value.Count; i++)
                {
                    value[i].Stop();
                }
            }
        }

        public void TaskCoroutineEnded(TaskCoroutine taskCoroutine, string coroutineName)
        {
            if (this.activeTaskCoroutines.ContainsKey(coroutineName))
            {
                List<TaskCoroutine> list = this.activeTaskCoroutines[coroutineName];
                if (list.Count == 1)
                {
                    this.activeTaskCoroutines.Remove(coroutineName);
                }
                else
                {
                    list.Remove(taskCoroutine);
                    this.activeTaskCoroutines[coroutineName] = list;
                }
            }
        }

        public void OnBehaviorStarted()
        {
            if (this.onBehaviorStart != null)
            {
                this.onBehaviorStart(this);
            }
        }

        public void OnBehaviorRestarted()
        {
            if (this.onBehaviorRestart != null)
            {
                this.onBehaviorRestart(this);
            }
        }

        public void OnBehaviorEnded()
        {
            if (this.onBehaviorEnd != null)
            {
                this.onBehaviorEnd(this);
            }
        }

        private void RegisterEvent(string name, Delegate handler)
        {
            if (this.eventTable == null)
            {
                this.eventTable = new Dictionary<Type, Dictionary<string, Delegate>>();
            }
            Dictionary<string, Delegate> dictionary;
            if (!this.eventTable.TryGetValue(handler.GetType(), out dictionary))
            {
                dictionary = new Dictionary<string, Delegate>();
                this.eventTable.Add(handler.GetType(), dictionary);
            }
            Delegate a;
            if (dictionary.TryGetValue(name, out a))
            {
                dictionary[name] = Delegate.Combine(a, handler);
            }
            else
            {
                dictionary.Add(name, handler);
            }
        }

        public void RegisterEvent(string name, System.Action handler)
        {
            this.RegisterEvent(name, handler);
        }

        public void RegisterEvent<T>(string name, Action<T> handler)
        {
            this.RegisterEvent(name, handler);
        }

        public void RegisterEvent<T, U>(string name, Action<T, U> handler)
        {
            this.RegisterEvent(name, handler);
        }

        public void RegisterEvent<T, U, V>(string name, Action<T, U, V> handler)
        {
            this.RegisterEvent(name, handler);
        }

        private Delegate GetDelegate(string name, Type type)
        {
            Dictionary<string, Delegate> dictionary;
            Delegate result;
            if (this.eventTable != null && this.eventTable.TryGetValue(type, out dictionary) && dictionary.TryGetValue(name, out result))
            {
                return result;
            }
            return null;
        }

        public void SendEvent(string name)
        {
            System.Action action = this.GetDelegate(name, typeof(System.Action)) as System.Action;
            if (action != null)
            {
                action();
            }
        }

        public void SendEvent<T>(string name, T arg1)
        {
            Action<T> action = this.GetDelegate(name, typeof(Action<T>)) as Action<T>;
            if (action != null)
            {
                action(arg1);
            }
        }

        public void SendEvent<T, U>(string name, T arg1, U arg2)
        {
            Action<T, U> action = this.GetDelegate(name, typeof(Action<T, U>)) as Action<T, U>;
            if (action != null)
            {
                action(arg1, arg2);
            }
        }

        public void SendEvent<T, U, V>(string name, T arg1, U arg2, V arg3)
        {
            Action<T, U, V> action = this.GetDelegate(name, typeof(Action<T, U, V>)) as Action<T, U, V>;
            if (action != null)
            {
                action(arg1, arg2, arg3);
            }
        }

        private void UnregisterEvent(string name, Delegate handler)
        {
            if (this.eventTable == null)
            {
                return;
            }
            Dictionary<string, Delegate> dictionary;
            Delegate source;
            if (this.eventTable.TryGetValue(handler.GetType(), out dictionary) && dictionary.TryGetValue(name, out source))
            {
                dictionary[name] = Delegate.Remove(source, handler);
            }
        }

        public void UnregisterEvent(string name, System.Action handler)
        {
            this.UnregisterEvent(name, handler);
        }

        public void UnregisterEvent<T>(string name, Action<T> handler)
        {
            this.UnregisterEvent(name, handler);
        }

        public void UnregisterEvent<T, U>(string name, Action<T, U> handler)
        {
            this.UnregisterEvent(name, handler);
        }

        public void UnregisterEvent<T, U, V>(string name, Action<T, U, V> handler)
        {
            this.UnregisterEvent(name, handler);
        }

        public void SaveResetValues()
        {
            if (this.defaultValues == null)
            {
                this.CheckForSerialization();
                this.defaultValues = new Dictionary<Task, Dictionary<string, object>>();
                this.defaultVariableValues = new Dictionary<string, object>();
                this.SaveValues();
            }
            else
            {
                this.ResetValues();
            }
        }

        private void SaveValues()
        {
            List<SharedVariable> allVariables = this.m_BehaviorSource.GetAllVariables();
            if (allVariables != null)
            {
                for (int i = 0; i < allVariables.Count; i++)
                {
                    this.defaultVariableValues.Add(allVariables[i].name, allVariables[i].GetValue());
                }
            }
            this.SaveValue(this.m_BehaviorSource.rootTask);
        }

        private void SaveValue(Task task)
        {
            if (task == null)
            {
                return;
            }
            FieldInfo[] publicFields = TaskUtility.GetPublicFields(task.GetType());
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            int i = 0;
            while (i < publicFields.Length)
            {
                object value = publicFields[i].GetValue(task);
                if (!(value is SharedVariable))
                {
                    goto IL_5A;
                }
                SharedVariable sharedVariable = value as SharedVariable;
                if (!sharedVariable.isGlobal && !sharedVariable.isShared)
                {
                    goto IL_5A;
                }
                IL_71:
                i++;
                continue;
                IL_5A:
                dictionary.Add(publicFields[i].Name, publicFields[i].GetValue(task));
                goto IL_71;
            }
            this.defaultValues.Add(task, dictionary);
            if (task is ParentTask)
            {
                ParentTask parentTask = task as ParentTask;
                if (parentTask.Children != null)
                {
                    for (int j = 0; j < parentTask.Children.Count; j++)
                    {
                        this.SaveValue(parentTask.Children[j]);
                    }
                }
            }
        }

        private void ResetValues()
        {
            foreach (KeyValuePair<string, object> current in this.defaultVariableValues)
            {
                this.SetVariableValue(current.Key, current.Value);
            }
            this.ResetValue(this.m_BehaviorSource.rootTask);
        }

        private void ResetValue(Task task)
        {
            if (task == null)
            {
                return;
            }
            Dictionary<string, object> dictionary;
            if (!this.defaultValues.TryGetValue(task, out dictionary))
            {
                return;
            }
            foreach (KeyValuePair<string, object> current in dictionary)
            {
                FieldInfo field = task.GetType().GetField(current.Key);
                if (field != null)
                {
                    field.SetValue(task, current.Value);
                }
            }
            if (task is ParentTask)
            {
                ParentTask parentTask = task as ParentTask;
                if (parentTask.Children != null)
                {
                    for (int i = 0; i < parentTask.Children.Count; i++)
                    {
                        this.ResetValue(parentTask.Children[i]);
                    }
                }
            }
        }

        public override string ToString()
        {
            return this.m_BehaviorSource.ToString();
        }

        public static BehaviorManager CreateBehaviorManager()
        {
            if (BehaviorManager.instance == null && Application.isPlaying)
            {
                return new GameObject
                {
                    name = "Behavior Manager"
                }.AddComponent<BehaviorManager>();
            }
            return null;
        }

        public virtual int GetInstanceID()
        {
            return base.GetInstanceID();
        }
    }
}
