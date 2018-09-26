using FrameWork.BehaviorDesigner.Tasks;
using FrameWork.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FrameWork.BehaviorDesigner.Runtime
{
    public class TaskUtility
    {
        [NonSerialized]
        private static Dictionary<string, Type> s_TypeLookup = new Dictionary<string, Type>();

        private static List<string> s_LoadedAssemblies = null;

        private static Dictionary<Type, FieldInfo[]> s_AllFieldsLookup = new Dictionary<Type, FieldInfo[]>();

        private static Dictionary<Type, FieldInfo[]> s_PublicFieldsLookup = new Dictionary<Type, FieldInfo[]>();

        private static Dictionary<FieldInfo, Dictionary<Type, bool>> m_HasFieldLoopup = new Dictionary<FieldInfo, Dictionary<Type, bool>>();

        public static object CreateInstance(Type t)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                t = Nullable.GetUnderlyingType(t);
            }
            return Activator.CreateInstance(t, true);
        }

        public static FieldInfo[] GetAllFields(Type t)
        {
            FieldInfo[] array = null;
            if (!TaskUtility.s_AllFieldsLookup.TryGetValue(t, out array))
            {
                List<FieldInfo> list = ListPool<FieldInfo>.Get();
                list.Clear();
                BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                TaskUtility.GetFields(t, ref list, (int)flags);
                array = list.ToArray();
                ListPool<FieldInfo>.Release(list);
                TaskUtility.s_AllFieldsLookup.Add(t, array);
            }
            return array;
        }

        public static FieldInfo[] GetPublicFields(Type t)
        {
            FieldInfo[] array = null;
            if (!TaskUtility.s_PublicFieldsLookup.TryGetValue(t, out array))
            {
                List<FieldInfo> list = ListPool<FieldInfo>.Get();
                list.Clear();
                BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;
                TaskUtility.GetFields(t, ref list, (int)flags);
                array = list.ToArray();
                ListPool<FieldInfo>.Release(list);
                TaskUtility.s_PublicFieldsLookup.Add(t, array);
            }
            return array;
        }

        private static void GetFields(Type t, ref List<FieldInfo> fieldList, int flags)
        {
            if (t == null || t.Equals(typeof(ParentTask)) || t.Equals(typeof(Task)) || t.Equals(typeof(SharedVariable)))
            {
                return;
            }
            FieldInfo[] fields = t.GetFields((BindingFlags)flags);
            for (int i = 0; i < fields.Length; i++)
            {
                fieldList.Add(fields[i]);
            }
            TaskUtility.GetFields(t.BaseType, ref fieldList, flags);
        }

        public static Type GetTypeWithinAssembly(string typeName)
        {
            Type type;
            if (TaskUtility.s_TypeLookup.TryGetValue(typeName, out type))
            {
                return type;
            }
            type = Type.GetType(typeName);
            if (type == null)
            {
                if (TaskUtility.s_LoadedAssemblies == null)
                {
                    TaskUtility.s_LoadedAssemblies = new List<string>();
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    for (int i = 0; i < assemblies.Length; i++)
                    {
                        TaskUtility.s_LoadedAssemblies.Add(assemblies[i].FullName);
                    }
                }
                for (int j = 0; j < TaskUtility.s_LoadedAssemblies.Count; j++)
                {
                    type = Type.GetType(typeName + "," + TaskUtility.s_LoadedAssemblies[j]);
                    if (type != null)
                    {
                        break;
                    }
                }
            }
            if (type != null)
            {
                TaskUtility.s_TypeLookup.Add(typeName, type);
            }
            return type;
        }

        public static bool CompareType(Type t, string typeName)
        {
            Type type = Type.GetType(typeName + ", Assembly-CSharp");
            if (type == null)
            {
                type = Type.GetType(typeName + ", Assembly-CSharp-firstpass");
            }
            return t.Equals(type);
        }

        public static bool HasAttribute(FieldInfo field, Type attribute)
        {
            if (field == null)
            {
                return false;
            }
            Dictionary<Type, bool> dictionary;
            if (!TaskUtility.m_HasFieldLoopup.TryGetValue(field, out dictionary))
            {
                dictionary = new Dictionary<Type, bool>();
                TaskUtility.m_HasFieldLoopup.Add(field, dictionary);
            }
            bool flag;
            if (!dictionary.TryGetValue(attribute, out flag))
            {
                flag = (field.GetCustomAttributes(attribute, false).Length > 0);
                dictionary.Add(attribute, flag);
            }
            return flag;
        }
    }
}
