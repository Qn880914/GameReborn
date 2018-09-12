using FrameWork.Utility;

namespace FrameWork.Helper
{
    public sealed class LZMACompressRequest : Disposable
    {
        private byte[] m_Data;
        public byte[] datas { get { return this.m_Data; } }

        private float m_Progress;
        public float progress { get { return this.m_Progress; } }

        private bool m_IsDone;
        public bool isDone { get { return this.m_IsDone; } }

        private string m_Error;
        public string error { get { return m_Error; } }

        public LZMACompressRequest()
        { }

        public void Compress(byte[] data)
        {
            Loom.RunAsync(new LoomBase(), delegate (LoomBase param)
            {
                try
                {
                    m_Data = new byte[1];
                    int size = LZMAHelper.Compress(data, ref m_Data);
                    if (size == 0)
                    {
                        m_Error = "Compress Failed";
                    }
                }
                catch (System.Exception e)
                {
                    m_Error = e.Message;
                }
                finally
                {
                    Loom.QueueOnMainThread(param, OnDone);
                }
            });
        }

        public void Decompress(byte[] data)
        {
            Loom.RunAsync(new LoomBase(), delegate (LoomBase param)
            {
                try
                {
                    m_Data = new byte[1];
                    int size = LZMAHelper.Uncompress(data, ref m_Data);
                    if (size == 0)
                    {
                        m_Error = "Compress Failed";
                    }
                }
                catch (System.Exception e)
                {
                    m_Error = e.Message;
                }
                finally
                {
                    Loom.QueueOnMainThread(param, OnDone);
                }
            });
        }

        public static LZMACompressRequest CreateCompress(byte[] data)
        {
            LZMACompressRequest request = new LZMACompressRequest();
            request.Compress(data);

            return request;
        }

        public static LZMACompressRequest CreateDecompress(byte[] data)
        {
            LZMACompressRequest request = new LZMACompressRequest();
            request.Decompress(data);

            return request;
        }

        void OnDone(LoomBase param)
        {
            m_IsDone = true;
        }
    }
}
