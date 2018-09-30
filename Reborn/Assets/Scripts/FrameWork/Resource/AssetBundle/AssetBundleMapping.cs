using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Resource
{
    public sealed class AssetBundleMapping : ScriptableObject
    {
        private List<AssetBundleInfo> m_AssetBundleInfos;
        public List<AssetBundleInfo> assetBundleInfos { get { return m_AssetBundleInfos; } set { m_AssetBundleInfos = value; } }

        /// <summary>
        /// 资源文件名   map  assetbundle名
        /// </summary>
        private Dictionary<string, string> m_FileMatchAssetBundleName;

        public void Init(Dictionary<string, HashSet<string>> assetbundleMapFilePaths)
        {
            m_AssetBundleInfos = new List<AssetBundleInfo>(assetbundleMapFilePaths.Count); 
            foreach(var map in assetbundleMapFilePaths)
            {
                AssetBundleInfo info = new AssetBundleInfo();
                info.name = map.Key.ToLower();
                info.files = new string[map.Value.Count];

                int index = 0;
                foreach(var filePath in map.Value)
                {
                    info.files[index++] = filePath.Replace("\\", "/").Replace("Assets/Data", "").Replace("Assets/", "").ToLower();

                    try
                    {
                        m_FileMatchAssetBundleName.Add(filePath, map.Key);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.Log(string.Format("[AssetBundleMapting.Init] Init Error  FileName : {0}   AssetBundleName : [1]  ",
                            filePath, map.Key));
                    }
                }

                m_AssetBundleInfos.Add(info);
            }
        }

        public void Init()
        {
            m_FileMatchAssetBundleName = new Dictionary<string, string>();
            foreach(var info in m_AssetBundleInfos)
            {
                foreach(var file in info.files)
                {
                    m_FileMatchAssetBundleName.Add(file, info.name);
                }
            }
        }

        /*public void Init()
        {
            m_FileMatchAssetBundles = new Dictionary<string, string>();

            AssetBundleInfo info;
            for(int i = 0; i < assetBundleInfos.Length; ++ i)
            {
                info = assetBundleInfos[i];
                for(int j = 0; j < info.files.Length; ++ j)
                {
                    try
                    {
                        m_FileMatchAssetBundles.Add(info.files[i], info.name);
                    }
                    catch(Exception e)
                    {
                        UnityEngine.Debug.Log(string.Format("[AssetBundleMapting.Init] Init Error  FileName : {0}   AssetBundleName : [1],  Already  AssetBundleName : [2] ",
                            info.files[i], info.name, m_FileMatchAssetBundles[info.files[i]]));
                    }
                }
            }
        }*/
    }
}

