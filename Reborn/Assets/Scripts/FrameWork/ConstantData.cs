using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantData
{
    public static float assetBundleCacheTime = 10;  // assetbundle 缓存时间

    public const string ASSETBUNDLE_MANIFEST = "data";

#if UNITY_EDITOR
    public static bool enableAssetBundle = false;
#else
    public static bool enableAssetBundle = true
#endif



    public static bool enableUnpack = false;

    public const string abPath = "ab";     // AssetBundle相对路径

    static string m_StreamingAssetsPath;    // 资源的ab包绝对路径
    public static string streamingAssetsPath
    {
        get
        {
            if (string.IsNullOrEmpty(m_StreamingAssetsPath))
            {
                m_StreamingAssetsPath = string.Format("{0}/{1}", Application.streamingAssetsPath, abPath);
            }
            return m_StreamingAssetsPath;
        }
    }
}
