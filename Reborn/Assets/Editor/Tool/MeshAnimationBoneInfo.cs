using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Editor.Tool
{
    public class MeshAnimationBoneInfo
    {
        private List<Vector3> m_Positions = new List<Vector3>();
        public List<Vector3> positions { get { return m_Positions; } set { m_Positions = value; } }

        private List<Quaternion> m_Rotations = new List<Quaternion>();
        public List<Quaternion> rotations { get { return m_Rotations; } set { m_Rotations = value; } }
    }
}
