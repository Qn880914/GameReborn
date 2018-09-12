using FrameWork.Helper;
using FrameWork.Resource;
using System.Collections.Generic;
using UnityEditor;

namespace FrameWork.Editor.Tool
{
    public sealed class BuildAssetBundleHelper
    {
        /// <summary>
        ///     <para>Is a player currently being built?</para>
        /// </summary>
        public static bool isBuildingPlayer { get { return BuildPipeline.isBuildingPlayer; } }

        public static string assetBundlePath
        {
            get
            {
                return string.Format("{0}/ab/{1}", "output", ConstantData.version);
            }
        }

        /// <summary>
        ///  <para>assetbundle Name map files path which packed in assetbundle</para>
        ///     <para>assetbundle Name</para>
        ///     <para>assetbundle contains files path</para>
        /// </summary>
        private static Dictionary<string, HashSet<string>> s_AssetbundleNameMapFilePaths= new Dictionary<string, HashSet<string>>();

        public static void BuildAssetBundle()
        {
            ReimportAssets(false);

            AssetDatabase.RemoveUnusedAssetBundleNames();

            BuildAssetBundleOptions options = BuildAssetBundleOptions.DeterministicAssetBundle;
            if(ConstantData.enableAssetBundle)
            {
                options |= BuildAssetBundleOptions.UncompressedAssetBundle;
            }

            BuildPipeline.BuildAssetBundles(assetBundlePath, options, EditorUserBuildSettings.activeBuildTarget);
        }

        public static void ReimportAssets(bool clearAssetBundles = true, bool resetAllAsset = false)
        {
            if (clearAssetBundles)
                FileHelper.ClearFileDirectory(assetBundlePath);
            else
                FileHelper.CreateDirectory(assetBundlePath);

            if(resetAllAsset)
            {
                string[] names = AssetDatabase.GetAllAssetBundleNames();
                foreach(var name in names)
                {
                    AssetDatabase.RemoveAssetBundleName(name, true);
                }
            }

            AssetDatabase.Refresh();
            AssetDatabase.RemoveUnusedAssetBundleNames();

            ReimportScene();
        }

        private static void ReimportScene()
        {
            foreach(var scene in EditorBuildSettings.scenes)
            {
                string sceneName = scene.path.Substring(scene.path.LastIndexOf("/") + 1);
                sceneName = sceneName.Substring(0, sceneName.IndexOf(".")).ToLower();
                if (sceneName.Equals("lauch"))
                    continue;

                Reimport(scene.path, string.Format("scene/{0}", sceneName));
            }
        }

        private static void ReimportUI()
        { }

        private static void Reimport(string path, string assetbundleName)
        {
            AssetImporter importer = AssetImporter.GetAtPath(path);
            if(null == importer)
            {
                UnityEngine.Debug.LogErrorFormat("[BuildAssetBundleHelper.Reimport] failed : {0}", path);
                return;
            }

            assetbundleName = assetbundleName.ToLower().Replace("(", "").Replace(")", "");
            importer.assetBundleName = assetbundleName;
            importer.assetBundleVariant = "ab";
            importer.SaveAndReimport();

            AddMapping(path, assetbundleName);
        }

        private static void AddMapping(string path, string assetbundleName)
        {
            HashSet<string> files;
            if(!s_AssetbundleNameMapFilePaths.TryGetValue(assetbundleName, out files))
            {
                files = new HashSet<string>();
                s_AssetbundleNameMapFilePaths.Add(assetbundleName, files);
            }

            if(!files.Contains(path))
            {
                files.Add(path);
            }
        }

        private static void SaveMapping()
        {
            bool exist = true;
            AssetBundleMapping assetbundleMapping = AssetDatabase.LoadAssetAtPath<AssetBundleMapping>(ConstantData.assetBundleMappingPath);
            if(null == assetbundleMapping)
            {
                exist = false;
                assetbundleMapping = new AssetBundleMapping();
            }

            assetbundleMapping.Init(s_AssetbundleNameMapFilePaths);

            if (exist)
                EditorUtility.SetDirty(assetbundleMapping);
            else
                AssetDatabase.CreateAsset(assetbundleMapping, ConstantData.assetBundleMappingPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            s_AssetbundleNameMapFilePaths.Clear();

            Reimport(ConstantData.assetBundleMappingPath, ConstantData.assetBundleMappingName);
        }
    }
}
