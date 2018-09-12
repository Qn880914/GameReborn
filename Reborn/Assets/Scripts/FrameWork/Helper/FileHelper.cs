using System;
using System.Collections.Generic;
using System.IO;

namespace FrameWork.Helper
{
    public static class FileHelper
    {
        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool Exists(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static DirectoryInfo CreateDirectory(string path)
        {
            if (Directory.Exists(path))
                return null;

            return Directory.CreateDirectory(path);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path"></param>
        public static void Delete(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch(Exception e)
            {
                UnityEngine.Debug.Log(string.Format("FileHelper Delete Exception : ", e.Message));
            }
        }

        // <summary>
        // 清空文件夹
        // </summary>
        public static void ClearFileDirectory(string path)
        {
            try
            {
                List<string> lfile = new List<string>();

                DirectoryInfo rootDirInfo = new DirectoryInfo(path);
                foreach (FileInfo file in rootDirInfo.GetFiles())
                {
                    File.Delete(file.FullName);
                }
                
                foreach (DirectoryInfo fileDic in rootDirInfo.GetDirectories())
                {
                    DeleteFileDirectory(fileDic.FullName);
                }

            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(string.Format("FileHelper ClearFileDirectory Exception : ", e.Message));
            }
        }

        // 拷贝文件夹
        public delegate bool CopyFilter(string file);
        public static void CopyDirectory(string sourcePath, string destinationPath, string suffix = "", CopyFilter onFilter = null)
        {
            if (onFilter != null && onFilter(sourcePath))
            {
                return;
            }
            
            Directory.CreateDirectory(destinationPath);
            DirectoryInfo info = new DirectoryInfo(sourcePath);
            foreach(FileSystemInfo fsi in info.GetFileSystemInfos())
            {
                string destName = Path.Combine(destinationPath, fsi.Name);
                if (fsi is System.IO.FileInfo)
                    File.Copy(fsi.FullName, destName);
                else
                {
                    Directory.CreateDirectory(destName);
                    CopyDirectory(fsi.FullName, destName);
                }
            }            
        }

        // <summary>
        // 删除文件夹
        // </summary>
        public static void DeleteFileDirectory(string path)
        {
            DirectoryInfo rootInfo = new DirectoryInfo(path);
            try
            {
                rootInfo.Delete(true);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(string.Format("FileHelper ClearFileDirectory Exception : ", e.Message));
            }
        }

        // 拷贝文件
        public static void CopyFile(string sourcePath, string destinationPath)
        {
            if (!File.Exists(sourcePath))
            {
                return;
            }

            Delete(destinationPath);

            CreateDirectoryFromFile(destinationPath);
            File.Copy(sourcePath, destinationPath);
        }

        // 根据文件名创建文件所在的目录
        public static void CreateDirectoryFromFile(string path)
        {
            path = path.Replace("\\", "/");
            int index = path.LastIndexOf("/");

            string dir = path.Substring(0, index);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        // 获取指定文件下的所有文件
        public static List<string> GetAllChildFiles(string path, string suffix = null, List<string> files = null)
        {
            if (files == null)
            {
                files = new List<string>();
            }

            if (!Directory.Exists(path))
            {
                return files;
            }

            AddFiles(path, suffix, files);

            string[] temps = Directory.GetDirectories(path);
            for (int i = 0; i < temps.Length; ++i)
            {
                string dir = temps[i];
                GetAllChildFiles(dir, suffix, files);
            }

            return files;
        }

        private static void AddFiles(string path, string suffix, List<string> files)
        {
            string[] temps = Directory.GetFiles(path);
            for (int i = 0; i < temps.Length; ++i)
            {
                string file = temps[i];
                if (string.IsNullOrEmpty(suffix) || file.ToLower().EndsWith(suffix.ToLower()))
                {
                    files.Add(file);
                }
            }
        }

        public static bool IsFileInPackage(string fullpath)
        {

#if UNITY_ANDROID && !UNITY_EDITOR
            if (fullpath.Contains(Application.streamingAssetsPath))
            {
                fullpath = fullpath.Replace(Application.streamingAssetsPath + "/", "");
                return AndroidHelper.FileHelper.CallStatic<bool> ("IsAssetExist", fullpath);;
            }
#endif

            return File.Exists(fullpath);
        }

        // 拷贝文件(Android)
        public static bool CopyAndroidAssetFile(string pathSrc, string pathDst)
        {
            bool ret = true;
#if UNITY_ANDROID && !UNITY_EDITOR
		    pathSrc = pathSrc.Replace (Application.streamingAssetsPath + "/", "");
		    ret = AndroidHelper.FileHelper.CallStatic<bool>("CopyFileTo", pathSrc, pathDst);
#endif

            return ret;
        }

        // 保存字节流到文件
        public static void SaveBytesToFile(byte[] bytes, string path)
        {
            CreateDirectoryFromFile(path);

            try
            {
                Stream stream = File.Open(path, FileMode.Create);
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e.Message);
            }
        }

        // 保存文件到文件
        public static void SaveTextToFile(string text, string path)
        {
            CreateDirectoryFromFile(path);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);

            SaveBytesToFile(bytes, path);
        }

        // 以字节流方式读取整个文件
        public static byte[] ReadByteFromFile(string path)
        {
            byte[] bytes = null;

            bool useFileReader = true;
#if UNITY_ANDROID && !UNITY_EDITOR
            // 如果是读apk包里的资源,使用Android帮助库加载(目前还没有用到,如果需要,要实现一下java代码,暂时保留)
            if (path.Contains(Application.streamingAssetsPath))
            {
                useFileReader = false;

                path = path.Replace(Application.streamingAssetsPath + "/", "");
                bytes = AndroidHelper.FileHelper.CallStatic<byte[]> ("LoadFile", path);
            }
#endif
            if (useFileReader && File.Exists(path))
            {
                bytes = File.ReadAllBytes(path);
            }

            return bytes;
        }

        // 以文本方式读取整个文件
        public static string ReadTextFromFile(string path)
        {
            string text = "";

            byte[] bytes = ReadByteFromFile(path);
            if (bytes != null)
            {
                text = System.Text.Encoding.UTF8.GetString(bytes);
            }

            return text;
        }

        static string CalcMd5StringFromHash(byte[] bytes)
        {
            string ret = "";
            foreach (byte b in bytes)
            {
                ret += Convert.ToString(b, 16);
            }

            return ret;
        }

        public static string CalcFileMd5(string path)
        {
            if (!File.Exists(path))
            {
                return "";
            }

            FileStream stream = File.OpenRead(path);

            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(stream);
            stream.Close();

            return CalcMd5StringFromHash(result);
        }
    }
}
