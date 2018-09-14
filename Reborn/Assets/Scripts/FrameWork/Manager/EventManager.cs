using FrameWork.Event;
using FrameWork.Utility;
using System;
using System.Collections.Generic;

namespace FrameWork.Manager
{
    public sealed class EventManager : Singleton<EventManager>
    {
        private Dictionary<Type, Delegate> delegates = new Dictionary<Type, Delegate>();

        private static readonly object lock_helper = new object();

        /// <summary>
        /// add event listen
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="del"></param>
        public void AddListener<T>(EventDelegate<T> del) where T : GameEvent
        {
            System.Delegate tempDel;
            if (delegates.TryGetValue(typeof(T), out tempDel))
                tempDel = System.Delegate.Combine(tempDel, del);
            else
                tempDel = del;

            delegates[typeof(T)] = tempDel;
        }

        public void RemoveListener<T>(EventDelegate<T> del) where T : GameEvent
        {
            System.Delegate currentDel;
            if(delegates.TryGetValue(typeof(T), out currentDel))
            {
                currentDel = System.Delegate.Remove(currentDel, del);
                if (null == currentDel)
                    delegates.Remove(typeof(T));
                else
                    delegates[typeof(T)] = currentDel;
            }
        }

        public void RemoveAllListener<T>() where T : GameEvent
        {
            delegates.Remove(typeof(T));
        }

        public void DispatchEvent<T>(T evt) where T : GameEvent
        {
            System.Delegate del;
            if(delegates.TryGetValue(typeof(T), out del))
            {
                evt.LogEvent();
                del.DynamicInvoke(evt);
            }
        }

        public void ClearListener()
        {
            delegates.Clear();
        }

        /*#region single instance module
        private EventManager() { }

        private static EventManager instance = null;

        public static EventManager GetInstance()
        {
            if (instance == null)
            {
                /// Thread safe
                lock (lock_helper)
                {
                    if (instance == null)
                        instance = new EventManager();
                }
            }
            return instance;
        }
        #endregion*/
    }
}
