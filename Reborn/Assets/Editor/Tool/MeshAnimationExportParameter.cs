using FrameWork.Resource;
using FrameWork.Utility;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FrameWork.Editor.Tool
{
    public class MeshAnimationExportParameter
    {
        private float m_Framerate = 30;
        public float framerate { get { return m_Framerate; } set { m_Framerate = value; } }

        private AnimationClip[] m_AnimationClips;
        public AnimationClip[] animationClips { get { return m_AnimationClips; } set { m_AnimationClips = value; if(null != m_AnimationClips) m_ClipCount = m_AnimationClips.Length; } }

        private int m_ClipCount;
        public int clipCount { get { return m_ClipCount; } set { m_ClipCount = value; } }

        private string[] m_AnimationClipNames;
        public string[] animationClipNames { get { return m_AnimationClipNames; } set { m_AnimationClipNames = value; } }
        
        private Transform[] m_BoneTransforms;
        public Transform[] boneTransforms { get { return m_BoneTransforms; } set { m_BoneTransforms = value; if (null != m_BoneTransforms) m_BoneCount = m_BoneTransforms.Length; } }

        private int m_BoneCount;
        public int boneCount { get { return m_BoneCount; } set { m_BoneCount = value; } }

        private Quaternion m_QuaternionOffset;
        public Quaternion quaternionOffset { get { return m_QuaternionOffset; } set { m_QuaternionOffset = value; } }

        private string[] m_BoneNames;
        public string[] boneNames { get { return m_BoneNames; } set { m_BoneNames = value; } }

        private string m_OutputFolderPath;
        public string outputFolderPath { get { return m_OutputFolderPath; } set { m_OutputFolderPath = value; } }

        private string m_OutputFilePath;
        public string outputFilePath { get { return m_OutputFilePath; } set { m_OutputFilePath = value; } }

        private GameObject m_PrefabParse;

        private static readonly string MESH_ANIMATIONS_ABSOLUTE_FOLDER_PATH = Application.dataPath + "/" + ResourcePath.troopMeshAniPath;

        public void ParsePrefab(GameObject prefab)
        {
            if(null == prefab)
            {
                Debug.LogWarning("Export settings prediction Error : prefab is empty!");
                return;
            }

            if (PrefabUtility.GetPrefabParent(m_PrefabParse.transform.parent) != null)
            {
                Debug.LogWarning("Export settings prediction only works when given the root of the prefab instance!");
                return;
            }

            m_PrefabParse = prefab;
        }

        public void GenerateDefaultData()
        {
            ParseAnimationParameter();
        }

        private void ParseAnimationParameter()
        {
            GameObject go = GameObject.Instantiate(m_PrefabParse) as GameObject;
            Animation animation = go.GetComponentInChildren<Animation>();
            if (null == animation)
            {
                UnityEngine.Debug.LogErrorFormat("[MeshAnimationExporter.ParseAnimationParameter]  Error : Target prefab has no animation component!");
                return;
            }

            int clipCount = animation.GetClipCount();
            int index = 0;
            this.animationClips = new AnimationClip[clipCount];
            this.m_AnimationClipNames = new string[clipCount];
            foreach (AnimationState state in animation)
            {
                this.animationClips[index] = state.clip;
                this.m_AnimationClipNames[index] = state.clip.name.Capitalize();
                ++index;
            }
            GameObject.DestroyImmediate(go);
        }

        private void ParseOutputPath()
        {
            string prefabPath = null == m_PrefabParse ? string.Empty : AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabParent(m_PrefabParse));
            if (string.IsNullOrEmpty(prefabPath))
            {
                UnityEngine.Debug.LogError("Please use this tool with an instance of the prefab");
                return;
            }            

            string objectFileName = ConvertStringToFileFormat(m_PrefabParse.name);
            string subFolderNameBossOrSoldier = Directory.GetParent(Application.dataPath + prefabPath).Name;

            string folderPath = MESH_ANIMATIONS_ABSOLUTE_FOLDER_PATH;

            // is there some way to get the prefix number so that we can generate the filename without an existing version?
            string[] possibleMatch = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
                .Where(filepath => Path.GetFileNameWithoutExtension(filepath.ToLower())
                       .Contains(objectFileName) && !filepath.ToLower().Contains(".meta")).ToArray();

            if (possibleMatch.Length == 1)
            {
                this.outputFilePath = possibleMatch[0];
                this.outputFolderPath = Path.GetDirectoryName(this.outputFilePath);
            }
            else
            {
                this.outputFilePath = "";
                this.outputFolderPath = folderPath;
            }
        }

        private void ParseBoneInfo()
        {
            Transform[] transforms = m_PrefabParse.GetComponentsInChildren<Transform>();
            // the transforms in the object tree not in the prefab are the emitter anchors
            var emitterAnchorsQuery = transforms.Where(transform => PrefabUtility.GetPrefabParent(transform.gameObject) == null);
            Transform[] emitterAnchors = emitterAnchorsQuery.ToArray();
            string[] emitterAnchorNames = emitterAnchorsQuery.Select(transform => transform.gameObject.name).ToArray();
            int anchorCount = emitterAnchors.Length;

            //ResizeDataLists(ref this.boneTransforms, ref this.boneNames, pDesiredCapacity: anchorCount);

            // fill data
            this.boneTransforms = emitterAnchors;
            this.boneNames = emitterAnchorNames;
        }

        // multi word names are supposed to map like WarBird => war_bird
        private string ConvertStringToFileFormat(string pInput)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            foreach (char c in pInput)
            {
                if (System.Char.IsUpper(c) && result.Length > 0)
                {
                    result.Append('_');
                }
                result.Append(System.Char.ToLower(c));
            }

            return result.ToString();
        }
    }
}
