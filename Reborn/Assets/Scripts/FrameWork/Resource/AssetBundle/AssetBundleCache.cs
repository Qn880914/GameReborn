using System;
using UnityEngine;

namespace FrameWork.Resource
{
    public sealed class AssetBundleCache
    {
        /// <summary>
        /// AssetBundle name
        /// </summary>
        private string m_Name;

        /// <summary>
        /// reference count
        /// </summary>
        private int m_ReferenceCount;
        public int refCount
        {
            get { return m_ReferenceCount; }
            set
            {
                m_ReferenceCount = value;
                if (canRemove)
                {
                    if (ConstantData.assetBundleCacheTime == 0)
                    {
                        Unload();
                    }
                    else
                    {
                        m_StartTime = Time.realtimeSinceStartup;
                    }
                }
                else
                {
                    m_StartTime = 0;
                }
            }
        }

        /// <summary>
        /// 开始使用时间
        /// </summary>
        private float m_StartTime;

        public AssetBundle assetBundle { get; private set; }

        /// <summary>
        /// 是否常驻
        /// </summary>
        public bool persistent { get; set; }
        
        public bool canRemove { get { return !persistent && m_ReferenceCount <= 0; } }

        public bool isTimeOut { get { return Time.realtimeSinceStartup - m_StartTime >= ConstantData.assetBundleCacheTime; } }

        public AssetBundleCache(string name, AssetBundle assetBunle, bool persistent, int refCount = 1)
        { }

        public object LoadAsset(string name)
        {
            return this.LoadAsset(name, typeof(object));
        }

        public T LoadAsset<T>(string name)
        {
            return (T)this.LoadAsset(name, typeof(T));
        }

        public object LoadAsset(string name, Type type)
        {
            if (null == assetBundle)
            {
                UnityEngine.Debug.Log(string.Format("[AssetBundleCache.LoadAsset] Error  Name : {0}  Type : {1}", name, type));
                //throw new NullReferenceException("The assetBundle cannot be null");
                return null;
            }

            return assetBundle.LoadAsset(name, type);
        }

        public AssetBundleRequest LoadAssetAsync(string name)
        {
            return this.LoadAssetAsync(name, typeof(object));
        }

        public AssetBundleRequest LoadAssetAsync<T>(string name)
        {
            return this.LoadAssetAsync(name, typeof(T));
        }

        public AssetBundleRequest LoadAssetAsync(string name, Type type)
        {
            if (null == assetBundle)
            {
                UnityEngine.Debug.Log(string.Format("[AssetBundleCache.LoadAssetAsync] Error  Name : {0}  Type : {1}", name, type));
                //throw new NullReferenceException("The assetBundle cannot be null");
                return null;
            }

            return assetBundle.LoadAssetAsync(name, type);
        }

        public void Unload(bool unloadUnusedAsset = false)
        {
            if (null != assetBundle)
            {
                bool unloadAllLoadedObject = false;
                if (m_Name.Contains("atlas"))
                {
                    // 图集总是卸载不掉,所以图集强制卸载
                    unloadAllLoadedObject = true;
                }

                assetBundle.Unload(unloadAllLoadedObject);
                assetBundle = null;

                if (unloadUnusedAsset)
                {
                    Resources.UnloadUnusedAssets();
                }
            }
        }
    }
}
