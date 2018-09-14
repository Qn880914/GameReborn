using System.IO;
using UnityEditor;
using UnityEngine;

namespace FrameWork.Editor.Tool
{
    public class MeshAnimationEditorWindow : EditorWindow
    {       
        [MenuItem("辅助工具/Animation/Mesh Export")]
        private static void CreateWizard()
        {
            MeshAnimationEditorWindow window = EditorWindow.GetWindow<MeshAnimationEditorWindow>();
            window.ShowUtility();
        }
        
        public void OnGUI()
        {
            windowWidth = position.width;

            eulerAngle = EditorGUILayout.Vector3Field("Rotate Angle(除了特殊需求，一般按默认值导出)", eulerAngle);
            EditorGUILayout.Space();

            /*Rect rect = new Rect(0f, 40f, 200f, 0f);
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(rect, "Export Parameter", EditorStyles.boldLabel);
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;*/
            EditorGUILayout.LabelField("Output File");

            EditorGUILayout.BeginHorizontal();
            {
                m_MeshAnimationExportParameter.outputFilePath = "Assets/" + "Data/Units/Troop/MeshAnimation/";
                m_MeshAnimationExportParameter.outputFolderPath = Path.GetDirectoryName(m_MeshAnimationExportParameter.outputFilePath);
                string projectRelativeFilePath = GetAssetPath(m_MeshAnimationExportParameter.outputFilePath);

                EditorGUILayout.SelectableLabel(projectRelativeFilePath,
                                                 EditorStyles.textField,
                                                 GUILayout.Height(EditorGUIUtility.singleLineHeight));

                GUI.SetNextControlName("BrowseButton");
                if (GUILayout.Button("Browse"))
                {
                    BrowseSaveFile();
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Base FBX:", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                GameObject newPrefab = EditorGUILayout.ObjectField(m_Prefab, typeof(GameObject), true) as GameObject;

                if (newPrefab != null && newPrefab != m_Prefab)
                {
                    // error if they drag the prefab itself, since it won't have any transform data
                    if (PrefabUtility.GetPrefabParent(newPrefab) != null)
                    {
                        m_Prefab = newPrefab;
                        m_MeshAnimationExportParameter.ParsePrefab(newPrefab);

                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            #region Framerate Setting
            m_MeshAnimationExportParameter.framerate = EditorGUILayout.FloatField("Capture Framerate:", m_MeshAnimationExportParameter.framerate);
            m_MeshAnimationExportParameter.framerate = Mathf.Max(m_MeshAnimationExportParameter.framerate, 1.0f);
            #endregion

            EditorGUILayout.Space();

            #region Clip Count Setting
            m_MeshAnimationExportParameter.clipCount = EditorGUILayout.IntField("Number of Clips:", m_MeshAnimationExportParameter.clipCount);
            #endregion

            EditorGUILayout.Space();

            labelWidth = GUILayout.Width(windowWidth * 0.3f);

            #region Animation Clip Setting
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Animation Name:", labelWidth);
                EditorGUILayout.LabelField("Animation File:", labelWidth);
                EditorGUILayout.LabelField("Frames:", GUILayout.Width(windowWidth * 0.2f));
            }
            EditorGUILayout.EndHorizontal();

            DrawAnimationArrayGui();
            #endregion

            EditorGUILayout.Space();

            #region Bone Count Setting
            m_MeshAnimationExportParameter.boneCount = EditorGUILayout.IntField("Number of Bones:", m_MeshAnimationExportParameter.boneCount);
            #endregion

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Bone Name:", labelWidth);
                EditorGUILayout.LabelField("Bone Transform:", labelWidth);
            }
            EditorGUILayout.EndHorizontal();

            DrawBoneArrayGui();

            #region Clear and Save Buttons
            EditorGUILayout.BeginHorizontal();
            {

                GUI.SetNextControlName("ClearButton");
                if (GUILayout.Button("Clear"))
                {
                    m_MeshAnimationExportParameter = new MeshAnimationExportParameter();
                    GUI.FocusControl("ClearButton");
                }

                if (GUILayout.Button("Export"))
                {
                    //Save 
                    if (eulerAngle != Vector3.zero)
                    {
                        m_MeshAnimationExportParameter.quaternionOffset = Quaternion.Euler(eulerAngle);
                    }

                    //MeshAnimationExporter.Export(fbx, exportSettings);
                    MeshAnimationExporter.ExportCombinedTexture(fbx, exportSettings);
                }
            }

            EditorGUILayout.EndHorizontal();
            #endregion            
        }

        private void DrawAnimationArrayGui()
        {
            float interval = 1.0f / m_MeshAnimationExportParameter.framerate;

            if (m_MeshAnimationExportParameter.animationNames != null && m_MeshAnimationExportParameter.animationNames.Length > 0)
            {
                for (int i = 0; i < m_MeshAnimationExportParameter.animationNames.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    m_MeshAnimationExportParameter.animationNames[i] = EditorGUILayout.TextField(m_MeshAnimationExportParameter.animationNames[i], labelWidth);

                    AnimationClip clip = EditorGUILayout.ObjectField(m_MeshAnimationExportParameter.animationClips[i], typeof(AnimationClip), true, labelWidth) as AnimationClip;

                    if (clip != m_MeshAnimationExportParameter.animationClips[i] && clip != null)
                    {
                        m_MeshAnimationExportParameter.animationNames[i] = clip.name;
                    }

                    m_MeshAnimationExportParameter.animationClips[i] = clip;

                    float frameCount = 0;

                    if (clip != null)
                    {
                        frameCount = clip.length / interval;
                    }

                    EditorGUILayout.LabelField(frameCount.ToString(), GUILayout.Width(windowWidth * 0.15f));

                    string result = "命名错误";
                    if (CheckAnimationName(clip.name))
                    {
                        result = "";
                    }
                    EditorGUILayout.LabelField(result, GUILayout.Width(windowWidth * 0.2f));

                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private void DrawBoneArrayGui()
        {

            if (m_MeshAnimationExportParameter.boneNames != null && m_MeshAnimationExportParameter.boneNames.Length > 0)
            {
                for (int i = 0; i < m_MeshAnimationExportParameter.boneNames.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    m_MeshAnimationExportParameter.boneNames[i] = EditorGUILayout.TextField(m_MeshAnimationExportParameter.boneNames[i], labelWidth);

                    Transform bone = EditorGUILayout.ObjectField(m_MeshAnimationExportParameter.boneTransforms[i], typeof(Transform), true, labelWidth) as Transform;

                    if (bone != m_MeshAnimationExportParameter.boneTransforms[i] && bone != null)
                    {
                        m_MeshAnimationExportParameter.boneNames[i] = bone.name;
                    }

                    m_MeshAnimationExportParameter.boneTransforms[i] = bone;

                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        /// <summary>
        /// 检查动画名称是否有错.
        /// attack1\attack2\dead\hit\run\wait1\wait2\skill1
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool CheckAnimationName(string name)
        {
            string[] strs = new string[] { "attack1", "attack2", "dead", "hit", "run", "wait1", "wait2", "skill1" };
            for (int i = 0; i < strs.Length; i++)
            {
                if (name.Equals(strs[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public string GetAssetPath(string pAbsolutePath)
        {
            string projectPath = GetProjectPath();
            if (pAbsolutePath.StartsWith(projectPath))
            {
                string relativePath = pAbsolutePath.Substring(projectPath.Length, pAbsolutePath.Length - projectPath.Length);

                if (relativePath.StartsWith("/") || relativePath.StartsWith("\\"))
                {
                    relativePath = relativePath.Substring(1, relativePath.Length - 1);
                }

                return relativePath;
            }

            return null;
        }

        private string GetProjectPath()
        {
            return Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/"));
        }

        private void BrowseSaveFile()
        {
            string output = EditorUtility.SaveFilePanel(
                "Save binary outpout",
                m_MeshAnimationExportParameter.outputFilePath,
                "",
                "bytes"
            );

            if (!string.IsNullOrEmpty(output.Trim()))
            {
                m_MeshAnimationExportParameter.outputFilePath = output;
                m_MeshAnimationExportParameter.outputFolderPath =  Path.GetDirectoryName(output);
            }

            GUI.FocusControl("");
        }

        private MeshAnimationExportParameter m_MeshAnimationExportParameter = new MeshAnimationExportParameter();

        public Vector3 eulerAngle = new Vector3(-90, 0, 0);

        private GameObject m_Prefab;

        private GUILayoutOption labelWidth;

        private float windowWidth;
    }
}
