using FrameWork.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Helper
{
    public class LZMACompressRequest : Disposable
    {
        private byte[] m_Data;
        public byte[] datas { get { return m_Data; } }

        private float m_Progress;
        public float progress { get { return m_Progress; } }

        private bool m_IsDone;
        public bool isDone { get { return m_IsDone; } }

        private string m_Error;
        public string error { get { return m_Error; } }

        public LZMACompressRequest()
        { }


    }
}
