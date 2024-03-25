using Autodesk.AutoCAD.Runtime;

namespace AutoCADNetLoadManager
{
    //方法
    public class Command
    {        
        [CommandMethod("Test")]        
        public void Test()
        {
            WindowReLoad windowReLoad = new WindowReLoad();
            windowReLoad.ShowDialog();
        }

        [CommandMethod("Test1")]
        public void Test1()
        {
            string filePath = WindowReLoad.FilePath;
            string className = WindowReLoad.ClassName;
            string methodName = WindowReLoad.MethodName;
            Commander.RunActiveCommand(filePath, className, methodName);
        }
    }
}
