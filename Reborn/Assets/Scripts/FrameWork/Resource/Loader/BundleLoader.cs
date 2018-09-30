using FrameWork.Helper;
using UnityEngine;

namespace FrameWork.Resource
{
    public class BundleLoader : Loader
    {
        private AssetBundleCreateRequest m_abRequest = null;
        private LZMACompressRequest m_DecompressRequest = null;
        private bool m_NeedUnpack = false;
        private int m_StageCurrent = 0;
        private int m_StageCount = 1;

        public BundleLoader()
            : base(LoaderType.Bundle)
        { }

        public override void Reset()
        {
            base.Reset();

            m_abRequest = null;
            m_DecompressRequest = null;

            m_StageCurrent = 0;
            m_StageCount = 1;
        }

        public override void Load()
        {
            base.Load();
            string path = m_Path;

            m_NeedUnpack = ConstantData.enableUnpack && path.Contains(ConstantData.assetBundleAbsolutePath);

            if (m_Async)
            {
                if (m_NeedUnpack)
                {
                    m_StageCount = 2;

                    byte[] bytes = FileHelper.ReadByteFromFile(path);
                    m_DecompressRequest = LZMACompressRequest.CreateDecompress(bytes);
                }
                else
                {
                    m_abRequest = AssetBundle.LoadFromFileAsync(path);
                }
            }
            else
            {
                AssetBundle ab = null;
                try
                {
                    if (m_NeedUnpack)
                    {
                        byte[] bytes = FileHelper.ReadByteFromFile(path);
                        byte[] result = new byte[1];
                        int size = LZMAHelper.Uncompress(bytes, ref result);
                        if (size == 0)
                        {
                            UnityEngine.Debug.Log("Uncompress Failed");
                        }
                        else
                        {
                            ab = AssetBundle.LoadFromMemory(result);
                        }
                    }
                    else
                    {
                        ab = AssetBundle.LoadFromFile(path);
                    }
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.Log(e.Message);
                }
                finally
                {
                    OnLoaded(ab);
                }
            }
        }

        public override void OnUpdate()
        {
            if (m_State == LoaderState.Loading)
            {
                if (m_abRequest != null)
                {
                    if (m_abRequest.isDone)
                    {
                        ++m_StageCurrent;
                        OnLoaded(m_abRequest.assetBundle);
                    }
                    else
                    {
                        DoProgress(m_abRequest.progress);
                    }
                }
                else if (m_DecompressRequest != null)
                {
                    if (m_DecompressRequest.isDone)
                    {
                        ++m_StageCurrent;
                        m_abRequest = AssetBundle.LoadFromMemoryAsync(m_DecompressRequest.datas);

                        m_DecompressRequest.Dispose();
                        m_DecompressRequest = null;
                    }
                    else
                    {
                        DoProgress(m_DecompressRequest.progress);
                    }
                }
            }
        }

        void DoProgress(float rate)
        {
            OnLoadProgress((m_StageCurrent + rate) / m_StageCount);
        }

        void OnLoaded(AssetBundle ab)
        {
            //Logger.Log(string.Format("Load {0} - {1} use {2}ms", m_path, m_async, m_watch.Elapsed.Milliseconds));

            OnLoadCompleted(ab);
        }
    }
}
