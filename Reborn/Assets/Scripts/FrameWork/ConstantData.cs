using UnityEngine;

public class ConstantData
{
    public static string version = "1.0";

    public static float assetBundleCacheTime = 10;  // assetbundle 缓存时间

    public const string assetBundleManifest = "data";

    public const string assetBundleMappingPath = "Assets/Data/assetbundlemapping.asset";

    public const string assetBundleMappingName = "assetbundleMapping";

#if UNITY_EDITOR
    public static bool enableAssetBundle = false;
#else
    public static bool enableAssetBundle = true
#endif



    public static bool enableUnpack = false;

    public const string abPath = "ab";     // AssetBundle相对路径

    public static readonly string abExtend = ".ab";    // assetbundle 后缀

    static string m_AssetBundleAbsolutePath;    // 资源的ab包绝对路径
    public static string assetBundleAbsolutePath
    {
        get
        {
            if (string.IsNullOrEmpty(m_AssetBundleAbsolutePath))
            {
                m_AssetBundleAbsolutePath = string.Format("{0}/{1}", Application.streamingAssetsPath, abPath);
            }
            return m_AssetBundleAbsolutePath;
        }
    }

    private static bool m_EnableMd5Name = false;
    public static bool enableMd5Name { get { return m_EnableMd5Name; } set { m_EnableMd5Name = value; } }
}
