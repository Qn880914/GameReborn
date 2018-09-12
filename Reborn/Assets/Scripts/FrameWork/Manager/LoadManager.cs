using FrameWork.Utility;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FrameWork.Resource;
using FrameWork.Helper;

namespace FrameWork.Manager
{
    public sealed class LoadManager : Singleton<LoadManager>
    {
        private AssetBundleManifest m_Manifest;
        private AssetBundleMapping m_Mapping;

        private Dictionary<string, AssetBundleCache> m_AssetBundleCaches = new Dictionary<string, AssetBundleCache>(); // 缓存队列

        private JSONClass m_RootNode;
        public JSONClass rootNode { get { return m_RootNode; } set { m_RootNode = value; } }

        private float m_LastClearTime = 0;

        private List<string> m_AssetBundleNames = new List<string>();
        public List<string> assetBundleNames { get { return m_AssetBundleNames; } }

        private LoadTask m_LoadTask;

        private List<string> m_SearchPaths = new List<string>();

        public LoadManager()
        {
            m_LoadTask = new LoadTask();

            if(ConstantData.enableAssetBundle)
            {

            }
        }

        private void LoadVersion()
        {
            if (ConstantData.enableMd5Name)
            {
            }
            else
                LoadAssetBundleManifest();
        }

        /// <summary>
        /// Load assetbundle manifest
        /// </summary>
        private void LoadAssetBundleManifest()
        {
            if(null != m_Manifest)
            {
                Object.DestroyImmediate(m_Manifest, true);
                m_Manifest = null;
            }

            m_AssetBundleNames.Clear();

            LoadAssetBundle(ConstantData.assetBundleManifest, (data)=> 
            {
                AssetBundleCache abCache = data as AssetBundleCache;
                if(null != abCache)
                {
                    m_Manifest = abCache.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                }

                UnLoadAssetBundle(ConstantData.assetBundleManifest, true);

                if(null != m_Manifest)
                {
                    string[] assetBundleNames = m_Manifest.GetAllAssetBundles();
                    m_AssetBundleNames.AddRange(assetBundleNames);
                }

            }, false, false, false);
        }

        /// <summary>
        /// 加载assetbundle 映射表
        /// file map assetbundle name
        /// </summary>
        private void LoadAssetBundleMapping()
        {
            LoadAssetFromBundle(ConstantData.assetBundleMappingName, ConstantData.assetBundleMappingName, typeof(AssetBundleMapping), (data)=> 
            {
                m_Mapping = data as AssetBundleMapping;
                if(null != m_Mapping)
                {
                    m_Mapping.Init();
                }
            }, false, false, true);
        }

        public void LoadAssetFromBundle(string path, string name, System.Type type, UnityAction<object> callback, bool async, bool persistent, bool unload = false)
        {
            string fullPath = path;
            if(!path.EndsWith(ConstantData.abExtend))
            {
                fullPath = string.Format("{0}{1}", path, ConstantData.abExtend);
            }

            fullPath = fullPath.ToLower();

            LoadAssetBundle(fullPath, (data)=> 
            {
                AssetBundleCache cache = data as AssetBundleCache;
                Object asset = null;
                if(null != cache && !string.IsNullOrEmpty(name))
                {
                    asset = cache.LoadAsset(name, type);
                }

                if(null == asset)
                {
                    UnityEngine.Debug.LogFormat("[LoadManager.LoadAssetFromBundle], path: {0}, name : {1}, asset is null", path, name);
                }

                if(null != callback)
                {
                    callback(asset);
                }

                if(unload)
                {
                    UnLoadAssetBundle(path);
                }
            }, async, persistent);
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
        /// 定时清除 Assetbundle 缓存
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
            return m_AssetBundleNames.Count == 0 || m_AssetBundleNames.Contains(path) || string.Equals(path, ConstantData.assetBundleManifest);
        }

        public void LoadStream(string path, UnityAction<object> actionLoaded, bool async = true, bool remote = false, bool isFullPath = false)
        {
            string fullPath = path;
            if(!remote)
            {
                if(!isFullPath)
                    fullPath = SearchPath(path, false, false, false);
            }
            else
            {
                // 从服务器加载，一定是异步的
                async = true;
            }

            m_LoadTask.AddLoadTask(LoaderType.Stream, fullPath, remote, actionLoaded, async);
        }

        public string SearchPath(string subPath, bool researchStreamAssetPath = false, bool addSuffix = false, bool isAssetBundle = true)
        {
            if(addSuffix)
                subPath = string.Format("{0}{1}", subPath, ConstantData.abExtend);

            string fullPath = string.Empty;

            // 优先从查找目录找
            foreach(var path in m_SearchPaths)
            {
                fullPath = string.Format("{0}/{1}", path, subPath);
                if(FileHelper.Exists(fullPath))
                {
                    return fullPath;
                }
            }

            // 不查找StreamAsset目录
            if(!researchStreamAssetPath)
            {
                return "";
            }

            if(isAssetBundle)
            {
                fullPath = string.Format("{0}/{1}", ConstantData.assetBundleAbsolutePath, subPath);
            }
            else
            {
                fullPath = string.Format("{0}/{1}", Application.streamingAssetsPath, subPath);
            }

            return fullPath;
        }
    }
}

