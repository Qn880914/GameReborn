using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

namespace FrameWork.Utility
{
    public class LoomBase
    {
        public UnityAction<LoomBase> action;
    }

    public class Loom : MonoBehaviour
    {
        public struct DelayedQueueItem
        {
            public float time;
            public LoomBase param;

            public DelayedQueueItem(float time, LoomBase param)
            {
                this.time = Time.time + time;
                this.param = param;
            }
        }

        public static int maxThreads = 10;
        public static int numThreads;
        private static bool s_Initialized;

        private static Loom s_Current;
        public static Loom current
        {
            get
            {
                Initialize();
                return s_Current;
            }
        }

        private List<LoomBase> m_Actions = new List<LoomBase>();
        private List<LoomBase> m_CurrentActions = new List<LoomBase>();

        private List<DelayedQueueItem> m_QueueItems = new List<DelayedQueueItem>();
        private List<DelayedQueueItem> m_CurrentQueueItems = new List<DelayedQueueItem>();

        private void Awake()
        {
            s_Current = this;
            s_Initialized = true;
        }

        private static void Initialize()
        {
            if(!s_Initialized)
            {
                if (!Application.isPlaying)
                    return;

                s_Initialized = true;
                GameObject g = new GameObject("Loom");
                DontDestroyOnLoad(g);
                s_Current = g.AddComponent<Loom>();
            }
        }

        public static void QueueOnMainThread(LoomBase param, UnityAction<LoomBase> action)
        {
            QueueOnMainThread(param, action, 0f);
        }

        public static void QueueOnMainThread(LoomBase param, UnityAction<LoomBase> action, float time)
        {
            param.action = action;

            if(0 != time)
            {
                lock(current.m_QueueItems)
                {
                    current.m_QueueItems.Add(new DelayedQueueItem(time, param));
                }
            }
            else
            {
                lock(current.m_Actions)
                {
                    current.m_Actions.Add(param);
                }
            }
        }

        public static Thread RunAsync(LoomBase param, UnityAction<LoomBase> action)
        {
            param.action = action;

            Initialize();
            while(numThreads >= maxThreads)
            {
                Thread.Sleep(1);
            }

            Interlocked.Increment(ref numThreads);
            ThreadPool.QueueUserWorkItem(RunAction, param);
            return null;
        }

        private static void RunAction(object param)
        {
            try
            {
                LoomBase loomObj = (LoomBase)param;
                loomObj.action(loomObj);
            }
            catch
            { }
            finally
            {
                Interlocked.Decrement(ref numThreads);
            }
        }

        private void OnDisable()
        {
            if(s_Current == this)
            {
                s_Current = null;
            }
        }

        private void Update()
        {
            lock (m_Actions)
            {
                m_CurrentActions.Clear();
                m_CurrentActions.AddRange(m_Actions);
                m_Actions.Clear();
            }
            foreach (var a in m_CurrentActions)
            {
                a.action(a);
            }
            lock (m_QueueItems)
            {
                m_CurrentQueueItems.Clear();
                m_CurrentQueueItems.AddRange(m_QueueItems.Where(d => d.time <= Time.time));
                foreach (var item in m_CurrentQueueItems)
                    m_QueueItems.Remove(item);
            }
            foreach (var delayed in m_CurrentQueueItems)
            {
                delayed.param.action(delayed.param);
            }
        }
    }
}
