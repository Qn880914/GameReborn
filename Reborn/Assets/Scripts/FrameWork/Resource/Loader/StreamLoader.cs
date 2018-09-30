using FrameWork.Helper;
using UnityEngine;

namespace FrameWork.Resource
{
    public class StreamLoader : Loader
    {
        WWW m_Request = null;

        public StreamLoader()
            : base(LoaderType.Stream)
        {}

        public override void Reset()
        {
            base.Reset();

            if (m_Request != null)
            {
                m_Request.Dispose();
                m_Request = null;
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

                m_Request = new WWW(path);
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
                if (m_Request == null)
                {
                    OnLoadCompleted(null);
                }
                else if (!string.IsNullOrEmpty(m_Request.error))
                {
                    UnityEngine.Debug.Log(m_Request.error);
                    OnLoadCompleted(null);
                }
                else if (m_Request.isDone)
                {
                    OnLoadCompleted(m_Request.bytes);
                }
                else
                {
                    OnLoadProgress(m_Request.progress);
                }
            }
        }
    }
}
