﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork.Utility;
using FrameWork.Resource;
using UnityEngine.Events;

namespace FrameWork.Manager
{
    public sealed class ResourceManager : Singleton<ResourceManager>
    {
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private static void LoadAsset(ResourceType resType, string name, System.Type type, UnityAction<object> callback, bool async = true, bool persistent = false, bool unload = true)
        {
            string path = ResourcePath.GetFullPath(resType, name);
            //LoadManager.instance
        }
    }
}
