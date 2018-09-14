namespace FrameWork.Utility
{
    /// <summary>
    ///     <para>Hieracher this class to make sure only have a instance</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> : Disposable where T : Singleton<T>
    {
        private static T s_Instance;

        private static object helper_lock;

        protected Singleton() { }

        public static T instance
        {
            get
            {
                if (null == s_Instance)
                {
                    lock (helper_lock)
                    {
                        if (null == s_Instance)
                        {
                            s_Instance = System.Activator.CreateInstance<T>();
                        }
                    }
                }

                return s_Instance;
            }
        }
    }
}
