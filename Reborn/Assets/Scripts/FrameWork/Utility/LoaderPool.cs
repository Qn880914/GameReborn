using FrameWork.Resource;

namespace FrameWork.Utility
{
    public class LoaderPool
    {
        private static readonly ObjectPool<StreamLoader> s_StreamLoaderPool = new ObjectPool<StreamLoader>(null, loader => loader.Reset());
        private static readonly ObjectPool<AssetLoader> s_AssetLoaderPool = new ObjectPool<AssetLoader>(null, loader=>loader.Reset());
        private static readonly ObjectPool<BundleLoader> s_BundleLoaderPool = new ObjectPool<BundleLoader>(null, loader => loader.Reset());
        private static readonly ObjectPool<ResourceLoader> s_ResourceLoaderPool = new ObjectPool<ResourceLoader>(null, loader => loader.Reset());
        private static readonly ObjectPool<SceneLoader> s_SceneLoaderPool = new ObjectPool<SceneLoader>(null, loader => loader.Reset());
        
        public static Loader Get(LoaderType type)
        {
            switch(type)
            {
                case LoaderType.Stream:
                    return s_StreamLoaderPool.Get();
                case LoaderType.Asset:
                    return s_AssetLoaderPool.Get();
                case LoaderType.Resources:
                    return s_ResourceLoaderPool.Get();
                case LoaderType.Bundle:
                    return s_BundleLoaderPool.Get();
                case LoaderType.Scene:
                    return s_SceneLoaderPool.Get();
            }

            return null;
        }

        public static void Release(Loader toRelease)
        {
            switch(toRelease.type)
            {
                case LoaderType.Stream:
                    s_StreamLoaderPool.Release(toRelease as StreamLoader);
                    break;
                case LoaderType.Asset:
                    s_AssetLoaderPool.Release(toRelease as AssetLoader);
                    break;
                case LoaderType.Resources:
                    s_ResourceLoaderPool.Release(toRelease as ResourceLoader);
                    break;
                case LoaderType.Bundle:
                    s_BundleLoaderPool.Release(toRelease as BundleLoader);
                    break;
                case LoaderType.Scene:
                    s_SceneLoaderPool.Release(toRelease as SceneLoader);
                    break;
            }
        }
    }
}

