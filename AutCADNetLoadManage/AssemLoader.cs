using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AutoCADNetLoadManager
{
    public class AssemLoader
    {
        private List<string> m_refedFolders;

        private Dictionary<string, DateTime> m_copiedFiles;

        private bool m_parsingOnly;

        private string m_originalFolder;

        private string m_tempFolder;

        private static string m_dotnetDir = Environment.GetEnvironmentVariable("windir") + "\\Microsoft.NET\\Framework\\v2.0.50727";

        public static string m_resolvedAssemPath = string.Empty;

        private string m_revitAPIAssemblyFullName;

        public string OriginalFolder
        {
            get
            {
                return m_originalFolder;
            }
            set
            {
                m_originalFolder = value;
            }
        }

        public string TempFolder
        {
            get
            {
                return m_tempFolder;
            }
            set
            {
                m_tempFolder = value;
            }
        }

        public AssemLoader()
        {
            m_tempFolder = string.Empty;
            m_refedFolders = new List<string>();
            m_copiedFiles = new Dictionary<string, DateTime>();
        }

        public void CopyGeneratedFilesBack()
        {
            string[] files = Directory.GetFiles(m_tempFolder, "*.*", SearchOption.AllDirectories);
            string[] array = files;
            foreach (string text in array)
            {
                if (m_copiedFiles.ContainsKey(text))
                {
                    DateTime t = m_copiedFiles[text];
                    FileInfo fileInfo = new FileInfo(text);
                    if (fileInfo.LastWriteTime > t)
                    {
                        string str = text.Remove(0, m_tempFolder.Length);
                        string destinationFilename = m_originalFolder + str;
                        FileUtils.CopyFile(text, destinationFilename);
                    }
                }
                else
                {
                    string str2 = text.Remove(0, m_tempFolder.Length);
                    string destinationFilename2 = m_originalFolder + str2;
                    FileUtils.CopyFile(text, destinationFilename2);
                }
            }
        }

        public void HookAssemblyResolve()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        public void UnhookAssemblyResolve()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
        }

        /// <summary>
        /// 复制到临时文件夹，并加载
        /// </summary>
        /// <param name="originalFilePath">原文件全路径</param>
        /// <param name="parsingOnly">是否只是分析</param>
        /// <returns></returns>
        public Assembly LoadAddinsToTempFolder(string originalFilePath, bool parsingOnly)
        {
            if (string.IsNullOrEmpty(originalFilePath) || originalFilePath.StartsWith("\\") || !File.Exists(originalFilePath))
            {
                return null;
            }
            m_parsingOnly = parsingOnly;
            m_originalFolder = Path.GetDirectoryName(originalFilePath);
            StringBuilder stringBuilder = new StringBuilder(Path.GetFileNameWithoutExtension(originalFilePath));
            if (parsingOnly)
            {
                stringBuilder.Append("-Parsing-");
            }
            else
            {
                stringBuilder.Append("-Executing-");
            }
            m_tempFolder = FileUtils.CreateTempFolder(stringBuilder.ToString());
            bool onlyCopyRelated = parsingOnly;
            Assembly assembly = CopyAndLoadAddin(originalFilePath, onlyCopyRelated);
            if (null == assembly || !IsAPIReferenced(assembly))
            {
                return null;
            }
            return assembly;
        }

        /// <summary>
        /// 复制并加载
        /// </summary>
        /// <param name="srcFilePath">原dll全路径名称</param>
        /// <param name="onlyCopyRelated">是否只复制相关文件</param>
        /// <returns></returns>
        private Assembly CopyAndLoadAddin(string srcFilePath, bool onlyCopyRelated)
        {
            string text = string.Empty;
            if (!FileUtils.FileExistsInFolder(srcFilePath, m_tempFolder))
            {
                //获取文件的文件夹路径
                string directoryName = Path.GetDirectoryName(srcFilePath);
                if (!m_refedFolders.Contains(directoryName))
                {
                    m_refedFolders.Add(directoryName);
                }
                List<FileInfo> list = new List<FileInfo>();
                text = FileUtils.CopyFileToFolder(srcFilePath, m_tempFolder, onlyCopyRelated, list);
                if (string.IsNullOrEmpty(text))
                {
                    return null;
                }
                foreach (FileInfo item in list)
                {
                    m_copiedFiles.Add(item.FullName, item.LastWriteTime);
                }
            }
            Assembly assembly = LoadAddin(text);
            
            //AssemblyName[] assemblyNames = assembly.GetReferencedAssemblies();
            //foreach (AssemblyName assemblyName in assemblyNames)
            //{
            //    string name = assemblyName.Name.Split(',')[0] + ".dll";
            //    string filePath = Path.GetDirectoryName(text) + "\\" + name;
            //    if (File.Exists(filePath))
            //    {
            //        LoadAddin(filePath);
            //    }
            //}

            return assembly;
        }

        private Assembly LoadAddin(string filePath)
        {
            Assembly assembly = null;
            try
            {
                Monitor.Enter(this);
                //assembly = Assembly.Load(File.ReadAllBytes(filePath));
                //assembly = Assembly.LoadFrom(filePath);
                assembly = Assembly.LoadFile(filePath);
            }
            finally
            {
                Monitor.Exit(this);
            }
            return assembly;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            lock (this)
            {               
                string text = SearchAssemblyFileInTempFolder(args.Name);
                if (File.Exists(text))
                {
                    return LoadAddin(text);
                }
                text = SearchAssemblyFileInOriginalFolders(args.Name);
                if (string.IsNullOrEmpty(text))
                {
                    string[] array = args.Name.Split(',');
                    string text2 = array[0];
                    if (array.Length > 1)
                    {
                        string text3 = array[2];
                        if (text2.EndsWith(".resources", StringComparison.CurrentCultureIgnoreCase) && !text3.EndsWith("neutral", StringComparison.CurrentCultureIgnoreCase))
                        {
                            text2 = text2.Substring(0, text2.Length - ".resources".Length);
                        }
                        text = SearchAssemblyFileInTempFolder(text2);
                        if (File.Exists(text))
                        {
                            return LoadAddin(text);
                        }
                        text = SearchAssemblyFileInOriginalFolders(text2);
                    }
                }
                if (string.IsNullOrEmpty(text))
                {
                    using (AssemblySelectorForm assemblySelectorForm = new AssemblySelectorForm(args.Name))
                    {
                        if (assemblySelectorForm.ShowDialog() != DialogResult.OK)
                        {
                            return null;
                        }
                        text = assemblySelectorForm.m_resultPath;
                    }
                }
                return CopyAndLoadAddin(text, onlyCopyRelated: true);
            }
        }

        /// <summary>
        /// 在临时文件夹下查找程序集
        /// </summary>
        /// <param name="assemName"></param>
        /// <returns></returns>
        private string SearchAssemblyFileInTempFolder(string assemName)
        {
            string[] array = new string[2]
            {
            ".dll",
            ".exe"
            };
            string empty = string.Empty;
            string str = assemName.Substring(0, assemName.IndexOf(','));
            string[] array2 = array;
            foreach (string str2 in array2)
            {
                empty = m_tempFolder + "\\" + str + str2;
                if (File.Exists(empty))
                {
                    return empty;
                }
            }
            return string.Empty;
        }

        private string SearchAssemblyFileInOriginalFolders(string assemName)
        {
            string[] array = new string[2]
            {
            ".dll",
            ".exe"
            };
            string empty = string.Empty;
            string text = assemName.Substring(0, assemName.IndexOf(','));
            string[] array2 = array;
            foreach (string str in array2)
            {
                empty = m_dotnetDir + "\\" + text + str;
                if (File.Exists(empty))
                {
                    return empty;
                }
            }
            string[] array3 = array;
            foreach (string str2 in array3)
            {
                foreach (string refedFolder in m_refedFolders)
                {
                    empty = refedFolder + "\\" + text + str2;
                    if (File.Exists(empty))
                    {
                        return empty;
                    }
                }
            }
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName));
                string path = directoryInfo.Parent.FullName + "\\Regression\\_RegressionTools\\";
                if (Directory.Exists(path))
                {
                    string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                    foreach (string text2 in files)
                    {
                        if (Path.GetFileNameWithoutExtension(text2).Equals(text, StringComparison.OrdinalIgnoreCase))
                        {
                            return text2;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            int num = assemName.IndexOf("XMLSerializers", StringComparison.OrdinalIgnoreCase);
            if (num != -1)
            {
                assemName = "System.XML" + assemName.Substring(num + "XMLSerializers".Length);
                return SearchAssemblyFileInOriginalFolders(assemName);
            }
            return null;
        }

        private bool IsAPIReferenced(Assembly assembly)
        {
            if (string.IsNullOrEmpty(m_revitAPIAssemblyFullName))
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly assembly2 in assemblies)
                {
                    if (string.Compare(assembly2.GetName().Name, "Acdbmgd", ignoreCase: true) == 0)
                    {
                        m_revitAPIAssemblyFullName = assembly2.GetName().Name;
                        break;
                    }
                }
            }
            AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
            foreach (AssemblyName assemblyName in referencedAssemblies)
            {
                if (m_revitAPIAssemblyFullName == assemblyName.Name)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
