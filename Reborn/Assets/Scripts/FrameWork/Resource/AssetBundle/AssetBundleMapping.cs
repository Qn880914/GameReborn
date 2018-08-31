using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FrameWork.Resource
{

    public class AssetBundleMapping : ScriptableObject
    {
        [Serializable]
        public class AssetBundleInfo
        {
            public string name;
            public string[] files;
        }

        public AssetBundleInfo[] assetBundles;
        private Dictionary<string, string> m_FileMatchAssetBundles;

        public void Init()
        {
            m_FileMatchAssetBundles = new Dictionary<string, string>();

            AssetBundleInfo info;
            for(int i = 0; i < assetBundles.Length; ++ i)
            {
                info = assetBundles[i];
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
        }
    }
}

