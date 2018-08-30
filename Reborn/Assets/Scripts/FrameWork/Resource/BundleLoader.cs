using FrameWork.Helper;
using UnityEngine;

namespace FrameWork.Resource
{
    public class BundleLoader : Loader
    {

        AssetBundleCreateRequest m_abRequest = null;
        LZMACompressRequest m_decompressRequest = null;
        bool m_needUnpack = false;
        int m_stageCurrent = 0;
        int m_stageCount = 1;

        public BundleLoader()
            : base(LoaderType.Bundle)
        {

        }

        public override void Reset()
        {
            base.Reset();

            m_abRequest = null;
            m_decompressRequest = null;

            m_stageCurrent = 0;
            m_stageCount = 1;
        }

        public override void Load()
        {
            base.Load();
            string path = m_Path;

            m_needUnpack = ConstantData.EnableUnpack && path.Contains(ConstantData.StreamingAssetsPath);

            if (m_Async)
            {
                if (m_needUnpack)
                {
                    m_stageCount = 2;

                    byte[] bytes = FileHelper.ReadByteFromFile(path);
                    m_decompressRequest = LZMACompressRequest.CreateDecompress(bytes);
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
                    if (m_needUnpack)
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
                        ++m_stageCurrent;
                        OnLoaded(m_abRequest.assetBundle);
                    }
                    else
                    {
                        DoProgress(m_abRequest.progress);
                    }
                }
                else if (m_decompressRequest != null)
                {
                    if (m_decompressRequest.isDone)
                    {
                        ++m_stageCurrent;
                        m_abRequest = AssetBundle.LoadFromMemoryAsync(m_decompressRequest.datas);

                        m_decompressRequest.Dispose();
                        m_decompressRequest = null;
                    }
                    else
                    {
                        DoProgress(m_decompressRequest.progress);
                    }
                }
            }
        }

        void DoProgress(float rate)
        {
            OnLoadProgress((m_stageCurrent + rate) / m_stageCount);
        }

        void OnLoaded(AssetBundle ab)
        {
            //Logger.Log(string.Format("Load {0} - {1} use {2}ms", m_path, m_async, m_watch.Elapsed.Milliseconds));

            OnLoadCompleted(ab);
        }
    }
}
