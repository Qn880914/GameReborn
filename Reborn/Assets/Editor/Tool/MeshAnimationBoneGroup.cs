using System.Collections.Generic;

namespace FrameWork.Editor.Tool
{
    public sealed class MeshAnimationBoneGroup
    {
        private List<string> m_BoneNames;
        public List<string> boneNames { get { return m_BoneNames; } set { m_BoneNames = value; } }

        private Dictionary<string, MeshAnimationBoneInfo> m_BoneInfos;
        public Dictionary<string, MeshAnimationBoneInfo> boneInfos { get { return m_BoneInfos; } set { m_BoneInfos = value; } }

        public MeshAnimationBoneGroup(List<string> boneNames)
        {
            this.m_BoneNames = boneNames;
            this.m_BoneInfos = new Dictionary<string, MeshAnimationBoneInfo>();
            foreach(var name in boneNames)
            {
                this.m_BoneInfos[name] = new MeshAnimationBoneInfo();
            }
        }
    }
}

