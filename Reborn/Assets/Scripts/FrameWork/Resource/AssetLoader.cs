using UnityEngine;

namespace FrameWork.Resource
{
    public class AssetLoader : Loader
    {
        object m_Data;

        public AssetLoader()
            : base(LoaderType.Asset)
        { }

        public override void Load()
        {
            base.Load();

#if UNITY_EDITOR
            System.Type type = m_Param as System.Type;
            if(null == type)
            {
                type = typeof(Object);
            }

            if(m_Async)
            {
                m_Data = UnityEditor.AssetDatabase.LoadAssetAtPath(m_Path, type);
            }
            else
            {
                object data = UnityEditor.AssetDatabase.LoadAssetAtPath(m_Path, type);
            }
#else
            if(!m_Async)
            {
                OnLoadCompleted(null);
            }
#endif
        }

        public override void OnUpdate()
        {
            if (m_State == LoaderState.Loading)
            {
                OnLoadCompleted(m_Data);
                m_Data = null;
            }
        }
    }
}
