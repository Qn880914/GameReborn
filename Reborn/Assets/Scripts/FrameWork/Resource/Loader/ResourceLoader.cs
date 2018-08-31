using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Resource
{
    public class ResourceLoader : Loader
    {
        private ResourceRequest m_Request;

        public ResourceLoader()
            : base(LoaderType.Resources)
        { }

        public override void Load()
        {
            base.Load();

            if(m_Async)
            {
                m_Request = Resources.LoadAsync(m_Path);
            }
            else
            {
                object obj = Resources.Load(m_Path);
                OnLoadCompleted(obj);
            }
        }

        public override void OnUpdate()
        {
            if(m_State == LoaderState.Loading)
            {
                if(null == m_Request)
                {
                    OnLoadCompleted(null);
                }
                else if (m_Request.isDone)
                {
                    OnLoadCompleted(m_Request.asset);
                    m_Request = null;
                }
                else
                {
                    OnLoadProgress(m_Request.progress);
                }
            }
        }
    }
}
