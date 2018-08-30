using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Utility
{
    public class LoaderPool
    {
        private static readonly ObjectPool<> s_ListPool = new ObjectPool<List<T>>(null, l => l.Clear());
    }
}

