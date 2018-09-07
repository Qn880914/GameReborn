using UnityEngine;
using UnityEngine.SceneManagement;

namespace FrameWork.Resource
{
    public class SceneLoader : Loader
    {
        AsyncOperation m_request = null;

        public SceneLoader()
            : base(LoaderType.Scene)
        { }

        public override void Load()
        {
            base.Load();

            if (m_Async)
            {
                m_request = SceneManager.LoadSceneAsync(m_Path);
            }
            else
            {
                SceneManager.LoadScene(m_Path);
                OnLoadCompleted(true);
            }
        }

        public override void OnUpdate()
        {
            if (m_State == LoaderState.Loading)
            {
                if (m_request == null)
                {
                    OnLoadCompleted(false);
                }
                else if (m_request.isDone)
                {
                    OnLoadCompleted(true);
                    m_request = null;
                }
                else
                {
                    OnLoadProgress(m_request.progress);
                }
            }
        }
    }
}
