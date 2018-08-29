using UnityEngine.Events;

namespace FrameWork.Resource
{
    public class Loader
    {
        public enum LoaderType
        {
            Stream,         // 流(原则上可以是任何文件，包括远程服务器上的)
            Asset,          // Asset 目录下的资源
            Resources,       // Resource 目录下的资源
            Bundle,         // AssetBundle
            Scene,          // 场景
            BundleAsset,    // AB 中的资源加载
        }

        public enum LoaderState
        {
            None,
            Loading,    // 加载中
            Finish,     // 完成
        }        

        protected LoaderType m_Type;    // 加载器类型
        public LoaderType type { get { return m_Type; } }

        protected LoaderState m_State;  // 加载状态
        public LoaderState state { get { return m_State; } }

        protected string m_Path;        // 路径
        public string path { get { return m_Path; } }

        protected object m_Param;       // 附加参数

        protected bool m_Async;         // 是否异步加载
        public bool async { get { return m_Async; } }

        public bool isFinish { get { return m_State == LoaderState.Finish; } }

        protected UnityAction<Loader, float> m_ActionProgress; // 加载进度
        protected UnityAction<Loader, object> m_ActionLoaded;      // 完成回调

        protected Loader(LoaderType type)
        {
            m_Type = type;
        }

        public virtual void Init(string path, object param, UnityAction<Loader, float> actionProgress, UnityAction<Loader, object> actionLoaded, bool async = true)
        {
            m_State = LoaderState.None;
            m_Path = path;
            m_Param = param;
            m_Async = async;

            m_ActionProgress = actionProgress;
            m_ActionLoaded = actionLoaded;
        }

        public virtual void Load()
        {
            m_State = LoaderState.Loading;
            OnLoadProgress(0.0f);
        }

        public virtual void OnUpdate()
        { }

        protected virtual void OnLoadProgress(float progress)
        {
            if(null != m_ActionProgress)
            {
                m_ActionProgress(this, progress);
            }
        }

        protected void OnLoadCompleted(object data)
        {
            m_State = LoaderState.Finish;
            
            if(null != m_ActionLoaded)
            {
                m_ActionLoaded(this, data);
            }

            OnLoadProgress(1.0f);
            Reset();
        }

        protected virtual void Reset()
        {
            m_Path = "";
            m_ActionLoaded = null;
            m_Async = true;
        }

        public virtual void Stop()
        {
            Reset();
        }
    }
}
