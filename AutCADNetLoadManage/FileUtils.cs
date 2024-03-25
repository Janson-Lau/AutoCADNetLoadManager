using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace AutoCADNetLoadManager
{
	public static class FileUtils
	{
		private const string TempFolderName = "AutoCADAddins";

		public static DateTime GetModifyTime(string filePath)
		{
			return File.GetLastWriteTime(filePath);
		}

		/// <summary>
		/// 创建临时文件夹
		/// </summary>
		/// <param name="prefix">文件夹名称前缀</param>
		/// <returns></returns>
		public static string CreateTempFolder(string prefix)
		{
			string tempPath = Path.GetTempPath();
			DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(tempPath, "AutoCADAddins"));
			if (!directoryInfo.Exists)
			{
				directoryInfo.Create();
			}
			//当前AutoCADAddins文件夹下的全部文件->删除
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			foreach (DirectoryInfo directoryInfo2 in directories)
			{
				try
				{
					Directory.Delete(directoryInfo2.FullName, recursive: true);
				}
				catch (Exception)
				{
				}
			}
			string str = $"{DateTime.Now:yyyyMMdd_HHmmss_ffff}";
			string path = Path.Combine(directoryInfo.FullName, prefix + str);
			DirectoryInfo directoryInfo3 = new DirectoryInfo(path);
			directoryInfo3.Create();
			return directoryInfo3.FullName;
		}

		public static void SetWriteable(string fileName)
		{
			if (File.Exists(fileName))
			{
				FileAttributes fileAttributes = File.GetAttributes(fileName) & ~FileAttributes.ReadOnly;
				File.SetAttributes(fileName, fileAttributes);
			}
		}

		public static bool SameFile(string file1, string file2)
		{
			return 0 == string.Compare(file1.Trim(), file2.Trim(), ignoreCase: true);
		}

		public static bool CreateFile(string filePath)
		{
			if (File.Exists(filePath))
			{
				return true;
			}
			try
			{
				string directoryName = Path.GetDirectoryName(filePath);
				if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				using (new FileInfo(filePath).Create())
				{
					SetWriteable(filePath);
				}
			}
			catch (Exception)
			{
				return false;
			}
			return File.Exists(filePath);
		}

		public static void DeleteFile(string fileName)
		{
			if (File.Exists(fileName))
			{
				FileAttributes fileAttributes = File.GetAttributes(fileName) & ~FileAttributes.ReadOnly;
				File.SetAttributes(fileName, fileAttributes);
				try
				{
					File.Delete(fileName);
				}
				catch (Exception)
				{
				}
			}
		}

		/// <summary>
		/// 目标文件夹下是否存在文件
		/// </summary>
		/// <param name="filePath">文件全路径名称</param>
		/// <param name="destFolder">目标文件夹</param>
		/// <returns></returns>
		public static bool FileExistsInFolder(string filePath, string destFolder)
		{
			string path = Path.Combine(destFolder, Path.GetFileName(filePath));
			return File.Exists(path);
		}

		/// <summary>
		/// 复制文件
		/// </summary>
		/// <param name="sourceFilePath">源文件dll全路径名称</param>
		/// <param name="destFolder">目标文件夹</param>
		/// <param name="onlyCopyRelated">是否只复制相关文件</param>
		/// <param name="allCopiedFiles">所有复制的文件</param>
		/// <returns></returns>
		public static string CopyFileToFolder(string sourceFilePath, string destFolder, bool onlyCopyRelated, List<FileInfo> allCopiedFiles)
		{
			if (!File.Exists(sourceFilePath))
			{
				return null;
			}
			string directoryName = Path.GetDirectoryName(sourceFilePath);
			if (onlyCopyRelated)
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(sourceFilePath);
				string searchPattern = fileNameWithoutExtension + ".*";
				string[] files = Directory.GetFiles(directoryName, searchPattern, SearchOption.TopDirectoryOnly);
				string[] array = files;
				foreach (string text in array)
				{
					string fileName = Path.GetFileName(text);
					string text2 = Path.Combine(destFolder, fileName);
					if (CopyFile(text, text2))
					{
						FileInfo item = new FileInfo(text2);
						allCopiedFiles.Add(item);
					}
				}
			}
			else
			{
				long folderSize = GetFolderSize(directoryName);
				if (folderSize > 50)
				{
                    switch (FolderTooBigDialog.Show(directoryName, folderSize))
                    {
                        case DialogResult.Yes:
                            CopyDirectory(directoryName, destFolder, allCopiedFiles);
                            break;
                        case DialogResult.No:
                            CopyFileToFolder(sourceFilePath, destFolder, onlyCopyRelated: true, allCopiedFiles);
                            break;
                        default:
                            return null;
                    }
                }
				else
				{
					CopyDirectory(directoryName, destFolder, allCopiedFiles);
				}
			}
			string text3 = Path.Combine(destFolder, Path.GetFileName(sourceFilePath));
			if (File.Exists(text3))
			{
				return text3;
			}
			return null;
		}

		public static bool CopyFile(string sourceFilename, string destinationFilename)
		{
			if (!File.Exists(sourceFilename))
			{
				return false;
			}
			FileAttributes fileAttributes = File.GetAttributes(sourceFilename) & ~FileAttributes.ReadOnly;
			File.SetAttributes(sourceFilename, fileAttributes);
			if (File.Exists(destinationFilename))
			{
				FileAttributes fileAttributes2 = File.GetAttributes(destinationFilename) & ~FileAttributes.ReadOnly;
				File.SetAttributes(destinationFilename, fileAttributes2);
				File.Delete(destinationFilename);
			}
			try
			{
				if (!Directory.Exists(Path.GetDirectoryName(destinationFilename)))
				{
					Directory.CreateDirectory(Path.GetDirectoryName(destinationFilename));
				}
				File.Copy(sourceFilename, destinationFilename, overwrite: true);
			}
			catch (Exception)
			{
				return false;
			}
			return File.Exists(destinationFilename);
		}

		/// <summary>
		/// 复制文件
		/// </summary>
		/// <param name="sourceDir">源文件夹</param>
		/// <param name="desDir">目标文件夹</param>
		/// <param name="allCopiedFiles">复制的文件</param>
		public static void CopyDirectory(string sourceDir, string desDir, List<FileInfo> allCopiedFiles)
		{
			try
			{
				//拿到到源文件夹下的所有文件夹->在目标文件夹下创建源文件夹下的所有文件夹（创建空文件夹，用于复制源文件夹下的文件）
				string[] directories = Directory.GetDirectories(sourceDir, "*.*", SearchOption.AllDirectories);
				string[] array = directories;
				foreach (string text in array)
				{
					string str = text.Replace(sourceDir, "");
					string path = desDir + str;
					if (!Directory.Exists(path))
					{
						Directory.CreateDirectory(path);
					}
				}
				//拿到到源文件夹下的所有文件，并复制到目标文件夹下
				string[] files = Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories);
				string[] array2 = files;
				foreach (string text2 in array2)
				{
					string str2 = text2.Replace(sourceDir, "");
					string text3 = desDir + str2;
					if (!Directory.Exists(Path.GetDirectoryName(text3)))
					{
						Directory.CreateDirectory(Path.GetDirectoryName(text3));
					}
					if (CopyFile(text2, text3))
					{
						allCopiedFiles.Add(new FileInfo(text3));
					}
				}
			}
			catch (Exception)
			{
			}
		}

		/// <summary>
		/// 计算文件夹大小
		/// </summary>
		/// <param name="folderPath"></param>
		/// <returns></returns>
		public static long GetFolderSize(string folderPath)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
			long num = 0L;
			FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos();
			foreach (FileSystemInfo fileSystemInfo in fileSystemInfos)
			{
				num = ((!(fileSystemInfo is FileInfo)) ? (num + GetFolderSize(fileSystemInfo.FullName)) : (num + ((FileInfo)fileSystemInfo).Length));
			}
			return num / 1024 / 1024;
		}
	}
}
