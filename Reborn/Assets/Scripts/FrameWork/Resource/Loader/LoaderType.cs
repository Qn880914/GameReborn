namespace FrameWork.Resource
{
    public enum LoaderType
    {
        None,
        Stream,         // 流(原则上可以是任何文件，包括远程服务器上的)
        Asset,          // Asset 目录下的资源
        Resources,       // Resource 目录下的资源
        Bundle,         // AssetBundle
        Scene,          // 场景
        BundleAsset,    // AB 中的资源加载
    }
}
