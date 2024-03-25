using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoCADNetLoadManager
{
    public static class Commander
    {
        public static void RunActiveCommand(string filePath, string className, string methodName)
        {
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(className) || string.IsNullOrEmpty(methodName))
            {
                MessageBox.Show("程序记录中没有上一次执行的命令！");
                return;
            }
            AssemLoader assemLoader = new AssemLoader();
            try
            {
                assemLoader.HookAssemblyResolve();
                Assembly assembly = assemLoader.LoadAddinsToTempFolder(filePath, false);
                if (null == assembly)
                {
                    return;
                }

                string m_activeTempFolder = assemLoader.TempFolder;
                Type targetType = assembly.GetType(className);
                MethodInfo targetMethod = targetType.GetMethod(methodName);
                object targetObject = Activator.CreateInstance(targetType);
                Action cmd = () => targetMethod.Invoke(targetObject, null);
                try
                {
                    cmd?.Invoke();
                }
                catch (System.Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "提示");
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            finally
            {
                //assemLoader.UnhookAssemblyResolve();//非模态窗口不能关闭
                assemLoader.CopyGeneratedFilesBack();
            }
        }

    }
}
