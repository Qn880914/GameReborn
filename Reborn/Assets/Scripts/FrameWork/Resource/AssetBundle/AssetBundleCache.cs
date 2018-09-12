using UnityEngine;

namespace FrameWork.Resource
{
    /// <summary>
    ///     <para>AssetBundleCache let you store AssetBundle with ref count. AssetBundle unload when ref cout is zero</para>
    /// </summary>
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

        /// <summary>
        /// 驻留时间到
        /// </summary>
        public bool isTimeOut { get { return Time.realtimeSinceStartup - m_StartTime >= ConstantData.assetBundleCacheTime; } }

        /// <summary>
        /// <para>Main asset that was supplied when building the asset bundle (Read Only).</para>
        /// 在构建资产时提供的主要资产
        /// </summary>
        public Object mainAsset { get { return assetBundle.mainAsset; } }

        /// <summary>
		///   <para>Return true if the AssetBundle is a streamed scene AssetBundle.</para>
        ///   如果是场景bundle则返回true
		/// </summary>
        public bool isStreamedSceneAssetBundle { get { return assetBundle.isStreamedSceneAssetBundle; } }

        public AssetBundleCache(string name, AssetBundle assetBunle, bool persistent, int refCount = 1)
        {
            this.m_Name = name;
            this.assetBundle = assetBundle;
            this.persistent = persistent;
            this.refCount = refCount;
        }

        /// <summary>
        ///     <para>Loads asset with name of type T from the bundle.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Object LoadAsset(string name)
        {
            return this.LoadAsset(name, typeof(Object));
        }

        public T LoadAsset<T>(string name) where T : Object
        {
            return (T)this.LoadAsset(name, typeof(T));
        }

        /// <summary>
        ///     <para>Loads asset with name of a given type from the bundle.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Object LoadAsset(string name, System.Type type)
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

        public AssetBundleRequest LoadAssetAsync<T>(string name) where T : Object
        {
            return this.LoadAssetAsync(name, typeof(T));
        }

        public AssetBundleRequest LoadAssetAsync(string name, System.Type type)
        {
            if (null == assetBundle)
            {
                UnityEngine.Debug.Log(string.Format("[AssetBundleCache.LoadAssetAsync] Error  Name : {0}  Type : {1}", name, type));
                //throw new NullReferenceException("The assetBundle cannot be null");
                return null;
            }

            return assetBundle.LoadAssetAsync(name, type);
        }

        /// <summary>
        ///     <para>Loads all assets contained in the asset bundle</para>
        /// </summary>
        /// <returns></returns>
        public Object[] LoadAllAssets()
        {
            return LoadAllAssets(typeof(Object));
        }

        public T[] LoadAllAssets<T>() where T : Object
        {
            return assetBundle.LoadAllAssets<T>();
        }

        /// <summary>
        ///     <para>Loads all assets contained in the asset bundle that inherit from type</para>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Object[] LoadAllAssets(System.Type type)
        {
            return assetBundle.LoadAllAssets(type);
        }

        /// <summary>
        ///     <para>Return all asset names in the AssetBundle.</para>
        /// </summary>
        /// <returns></returns>
        public string [] GetAllAssetNames()
        {
            return assetBundle.GetAllAssetNames();
        }

        /// <summary>
        ///     <para>Return all the scene asset paths (paths to *.unity assets) in the AssetBundle.</para>
        /// </summary>
        /// <returns></returns>
        public string []GetAllScenePaths()
        {
            return assetBundle.GetAllScenePaths();
        }

        /// <summary>
        ///     <para>Unlaods all assets in the bundle</para>
        /// 卸载 assetbundle
        /// </summary>
        /// <param name="unloadUnusedAsset"></param>
        public void Unload(bool unloadUnusedAsset = false)
        {
            if (null != assetBundle)
            {
                bool unloadAllLoadedObjects = false;
                if (m_Name.Contains("atlas"))
                {
                    // 图集总是卸载不掉,所以图集强制卸载
                    unloadAllLoadedObjects = true;
                }

                assetBundle.Unload(unloadAllLoadedObjects);
                assetBundle = null;

                if (unloadUnusedAsset)
                {
                    Resources.UnloadUnusedAssets();
                }
            }
        }
    }
}
