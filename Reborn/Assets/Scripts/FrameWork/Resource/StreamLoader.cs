using FrameWork.Helper;
using UnityEngine;

namespace FrameWork.Resource
{
    public class StreamLoader : Loader
    {
        WWW m_request = null;

        public StreamLoader()
            : base(LoaderType.Stream)
        {

        }

        protected override void Reset()
        {
            base.Reset();

            if (m_request != null)
            {
                m_request.Dispose();
                m_request = null;
            }
        }

        public override void Load()
        {
            base.Load();

            if (m_Async)
            {
                string path = m_Path;

                bool hasHead = (bool)m_Param;
                if (!hasHead)
                {
                    bool addFileHead = true;

#if UNITY_ANDROID && !UNITY_EDITOR
                    // 如果是读取apk里的资源,不需要加file:///,其它情况都要加
                    if (path.Contains (Application.streamingAssetsPath)) {
                        addFileHead = false;
                    }
#endif
                    if (addFileHead)
                    {
                        path = string.Format("file:///{0}", path);
                    }
                }

                m_request = new WWW(path);
            }
            else
            {
                object data = null;
                try
                {
                    data = FileHelper.ReadByteFromFile(m_Path);
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.Log(e.Message);
                }
                finally
                {
                    OnLoadCompleted(data);
                    data = null;
                }
            }
        }

        public override void OnUpdate()
        {
            if (m_State == LoaderState.Loading)
            {
                if (m_request == null)
                {
                    OnLoadCompleted(null);
                }
                else if (!string.IsNullOrEmpty(m_request.error))
                {
                    UnityEngine.Debug.Log(m_request.error);
                    OnLoadCompleted(null);
                }
                else if (m_request.isDone)
                {
                    OnLoadCompleted(m_request.bytes);
                }
                else
                {
                    OnLoadProgress(m_request.progress);
                }
            }
        }
    }
}
