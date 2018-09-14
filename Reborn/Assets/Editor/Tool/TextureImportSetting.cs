using UnityEditor;

namespace Editor.Tool
{
    public sealed class TextureImportSetting
    {

        [MenuItem("辅助工具/Texture/Clear all texture mipmap")]
        private static void RemoveAllTextureMipmap()
        {
            string[] guids = AssetDatabase.FindAssets("t:Texture");
            int count = guids.Length;

            for (int i = 0; i < count; ++i)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (path.StartsWith("Assets/Scene/"))
                {
                    continue;
                }

                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer && importer.mipmapEnabled)
                {
                    //Debug.Log(path);

                    importer.mipmapEnabled = false;
                    importer.SaveAndReimport();
                }

                ShowProgress(i * 1f / count, count, i);
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        public static void ShowProgress(float val, int total, int cur)
        {
            EditorUtility.DisplayProgressBar("设置图片中...", string.Format("请稍等({0}/{1}) ", cur, total), val);
        }
    }
}
