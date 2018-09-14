using UnityEditor;

namespace FrameWork.Editor.Tool
{
    [CustomEditor(typeof(MeshAnimationExportParameter),true)]
    [CanEditMultipleObjects]
    public class MeshAnimationExportParamEditor : UnityEditor.Editor
    {
        //SerializedProperty m_Rotation;
        SerializedProperty m_Framerate;

        SerializedProperty m_AnimationClips;
        
        SerializedProperty m_AnimationNames;

        SerializedProperty m_BoneTransforms;

        SerializedProperty m_QuaternionOffset;

        SerializedProperty m_BoneNames;

        SerializedProperty m_OutputFolderPath;

        SerializedProperty m_OutputFilePath;

        protected virtual void OnEnable()
        {
            m_Framerate = serializedObject.FindProperty("m_Framerate");
            m_AnimationClips = serializedObject.FindProperty("m_AnimationClips");
            m_AnimationNames = serializedObject.FindProperty("m_AnimationNames");
            m_BoneTransforms = serializedObject.FindProperty("m_BoneTransforms");
            m_QuaternionOffset = serializedObject.FindProperty("m_QuaternionOffset");
            m_BoneNames = serializedObject.FindProperty("m_BoneNames");
            m_OutputFolderPath = serializedObject.FindProperty("m_OutputFolderPath");
            m_OutputFilePath = serializedObject.FindProperty("m_OutputFilePath");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Framerate);
            EditorGUILayout.PropertyField(m_AnimationClips);
            EditorGUILayout.PropertyField(m_AnimationNames);
            EditorGUILayout.PropertyField(m_BoneTransforms);
            EditorGUILayout.PropertyField(m_QuaternionOffset);
            EditorGUILayout.PropertyField(m_BoneNames);
            EditorGUILayout.PropertyField(m_OutputFolderPath);
            EditorGUILayout.PropertyField(m_OutputFilePath);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
