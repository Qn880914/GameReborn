using FrameWork.Utility;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FrameWork.Resource;

namespace FrameWork.Manager
{
    public class LoadManager : Singleton<LoadManager>
    {
        private AssetBundleManifest m_Manifest;
        private AssetBundleMapping m_mapping;
        private List<string> m_BundleNames = new List<string>();

        private Dictionary<string, AssetBundleCache> m_AssetBundleCaches = new Dictionary<string, AssetBundleCache>(); // 缓存队列

        private JSONClass m_RootNode;
        public JSONClass rootNode { get { return m_RootNode; } set { m_RootNode = value; } }

        private float m_LastClearTime = 0;

        private void LoadManifest()
        {
            if(null != m_Manifest)
            {
                Object.DestroyImmediate(m_Manifest, true);
                m_Manifest = null;
            }

            m_BundleNames.Clear();

            LoadAssetBundle(ConstantData.ASSETBUNDLE_MANIFEST, (data)=> 
            {
                AssetBundleCache abCache = data as AssetBundleCache;
                if(null != abCache)
                {
                    m_Manifest = abCache.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                }



            }, false, false, false);
        }

        public void LoadAsset(string path, System.Type type, UnityAction<object> callback, bool async = true, bool persistent = false, bool unload = false, bool inData = true)
        {
            if(ConstantData.enableAssetBundle)
            {

            }
        }

        private void LoadAssetBundle(string path, UnityAction<object> actionLoaded, bool async = true, bool persistent = false, bool manifest = true)
        {
            path = path.ToLower();

            if(manifest)
            {
                if(!HasBundle(path))
                {
                    Debug.Log(string.Format("AssetBundle is Not Exist : {0}", path));
                    if(null != actionLoaded)
                    {
                        if(!async)
                        {
                            actionLoaded(null);
                        }
                        else
                        {
                            //m_
                        }
                    }
                }
            }
        }

        private void UnLoadAssetBundle(string path, bool immediate = true)
        {
            path = GetRealAssetBundleName(path);
        }

        /// <summary>
        /// 添加 assetbundle 缓存
        /// </summary>
        /// <param name="name"></param>
        /// <param name="assetBundle"></param>
        /// <param name="persistent"></param>
        /// <param name="refCount"></param>
        /// <returns></returns>
        private AssetBundleCache AddAssetBundleCache(string name, AssetBundle assetBundle, bool persistent, int refCount)
        {
            AssetBundleCache assetBundleCache;
            if(!m_AssetBundleCaches.TryGetValue(name, out assetBundleCache))
            {
                assetBundleCache = new AssetBundleCache(name, assetBundle, persistent, refCount);
                m_AssetBundleCaches.Add(name, assetBundleCache);
            }

            return assetBundleCache;
        }

        /// <summary>
        /// 移除 assetbundle 缓存
        /// </summary>
        /// <param name="name"></param>
        /// <param name="immediate"></param>
        /// <returns></returns>
        private bool RemoveAssetBundleCache(string name, bool immediate = false)
        {
            AssetBundleCache assetBundleCache;
            if(!m_AssetBundleCaches.TryGetValue(name, out assetBundleCache))
            {
                return false;
            }

            --assetBundleCache.refCount;

            if((ConstantData.assetBundleCacheTime == 0 || immediate))
            {
                assetBundleCache.Unload();
                m_AssetBundleCaches.Remove(name);
            }

            return true;
        }

        /// <summary>
        /// 获取 assetbundle 缓存
        /// </summary>
        /// <param name="name"></param>
        /// <param name="persistent"></param>
        /// <returns></returns>
        private AssetBundleCache GetAssetBundleCache(string name, bool persistent)
        {
            AssetBundleCache assetBundleCache;
            if(!m_AssetBundleCaches.TryGetValue(name, out assetBundleCache))
            {
                return null;
            }

            ++assetBundleCache.refCount;
            if(persistent)
            {
                assetBundleCache.persistent = true;
            }

            return assetBundleCache;
        }

        /// <summary>
        /// 定时清楚 Assetbundle 缓存
        /// </summary>
        private void UpdateAssetBundleCache()
        {
            if(ConstantData.assetBundleCacheTime == 0 || Time.realtimeSinceStartup - m_LastClearTime < ConstantData.assetBundleCacheTime)
            {
                return;
            }

            m_LastClearTime = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// 清空 assetbundle 缓存
        /// </summary>
        /// <param name="onlyRefZero"></param>
        /// <param name="onlyTimeOut"></param>
        /// <param name="includePersistent"></param>
        private void ClearAssetBundleCache(bool onlyRefZero = true, bool onlyTimeOut = true, bool includePersistent = false)
        {
            AssetBundleCache assetBundleCache;
            string[] paths = new string[m_AssetBundleCaches.Count];
            m_AssetBundleCaches.Keys.CopyTo(paths, 0);

            string path;
            for(int i = 0; i < paths.Length; ++ i)
            {
                path = paths[i];
                assetBundleCache = m_AssetBundleCaches[path];
                if(onlyRefZero)
                {
                    if (assetBundleCache.canRemove && (!onlyTimeOut || assetBundleCache.isTimeOut))
                    {
                        assetBundleCache.Unload();
                        m_AssetBundleCaches.Remove(path);
                    }
                }
                else if (includePersistent || assetBundleCache.canRemove)
                {
                    assetBundleCache.Unload();
                    m_AssetBundleCaches.Remove(path);
                }
            }
        }

        /// <summary>
        /// 从缓存中 获取 assetbundle
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private AssetBundle GetAssetBundle(string path)
        {
            string fullPath = path.EndsWith(ConstantData.abExtend) ? path : string.Format("{0}{1}", path, ConstantData.abExtend);
            fullPath = fullPath.ToLower();

            AssetBundleCache assetBundleCache;
            if (m_AssetBundleCaches.TryGetValue(path, out assetBundleCache))
            {
                return assetBundleCache.assetBundle;
            }

            return null;
        }

        private string GetRealAssetBundleName(string path)
        {
            path = path.ToLower();
            if(ConstantData.enableMd5Name)
            {
                JSONClass node = rootNode[path] as JSONClass;
                path = string.Format("{0}{1}", node["md5"].Value, ConstantData.abExtend);
            }

            return path;
        }

        private bool HasBundle(string path)
        {
            return m_BundleNames.Count == 0 || m_BundleNames.Contains(path) || string.Equals(path, ConstantData.ASSETBUNDLE_MANIFEST);
        }
    }
}

