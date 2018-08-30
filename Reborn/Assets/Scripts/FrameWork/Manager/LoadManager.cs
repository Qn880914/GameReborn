using FrameWork.Utility;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FrameWork.Manager
{
    public class LoadManager : Singleton<LoadManager>
    {
        private AssetBundleManifest m_Manifest;
        private List<string> m_BundleNames = new List<string>();

        private void LoadManifest()
        {
            if(null != m_Manifest)
            {
                Object.DestroyImmediate(m_Manifest, true);
                m_Manifest = null;
            }

            m_BundleNames.Clear();
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

        private bool HasBundle(string path)
        {
            return m_BundleNames.Count == 0 || m_BundleNames.Contains(path) || string.Equals(path, ConstantData.ASSETBUNDLE_MANIFEST);
        }
    }
}

