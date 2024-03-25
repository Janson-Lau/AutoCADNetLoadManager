using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;


namespace AutoCADNetLoadManager
{
    /// <summary>
    /// WindowReLoad.xaml 的交互逻辑
    /// </summary>
    public partial class WindowReLoad : Window
    {
        #region ClassTree
        //Node TreeRoot { get; set; }
        //public void ShowTree()
        //{
        //    classTree.Items.Add(TreeRoot);
        //}
        public void Init()
        {
            for (int i = 0; i < classTree.Items.Count; i++)
            {
                classTree.Items.RemoveAt(i);
                i--;
            }
        }

        private void Button_Click_Load(object sender, RoutedEventArgs e)
        {
            //string fileName = GetFileName();
            //Assembly assembly = Assembly.LoadFrom(fileName);
            //GetClassTree(assembly);

            //Assembly assembly = Assembly.LoadFrom(fileName);
            //Assembly assembly = Assembly.Load(fileName);
            //GetClassTree(typeof(Autodesk.AutoCAD.DatabaseServices.Polyline));
            //GetClassTree(typeof(RibbonPanel));
            //typeof(BlockReference);

            //TreeRoot = new Node() { Title = "ClassTree" };
            //GetClassTree(typeof(Autodesk.AutoCAD.DatabaseServices.Polyline));
            //GetClassTree(typeof(RibbonPanel));

            Init();
            AppDomain currentDomain = AppDomain.CurrentDomain;
            var assemblies = currentDomain.GetAssemblies().ToList();
            List<Assembly> assemblies1 = new List<Assembly>();
            foreach (var assembly in assemblies)
            {
                try
                {
                    if (!string.IsNullOrEmpty(assembly.Location))
                    {
                        assemblies1.Add(assembly);
                    }
                }
                catch
                {
                }
            }
            assemblies1 = assemblies1.OrderBy(a => a.Location).ToList();

            Process p = Process.GetCurrentProcess();
            var path = Path.GetDirectoryName(p.MainModule.FileName);
            foreach (var assembly in assemblies1)
            {
                try
                {
                    if (assembly.Location.Contains(path) && (bool)OnlyAutoCADCheck.IsChecked || !(bool)OnlyAutoCADCheck.IsChecked)
                    {
                        GetClassTree(assembly);
                    }
                }
                catch
                {
                }
            }
        }

        List<string> searchs = new List<string>();
        private void Button_Click_FuzzyFind(object sender, RoutedEventArgs e)
        {
            string search = searchTB.Text;
            bool isFind = ExpandAndSelectItem(classTree, search, true);
            if (!isFind)
            {
                searchs.Clear();
            }
        }

        private void Button_Click_Find(object sender, RoutedEventArgs e)
        {
            string search = searchTB.Text;
            bool isFind = ExpandAndSelectItem(classTree, search, false);
            if (!isFind)
            {
                searchs.Clear();
            }
        }

        private bool IsEqual(string s1, string s2, bool isFuzzy)
        {
            if (s1 == s2)
            {
                return true;
            }
            else
            {
                if (isFuzzy && (s1.ToLower().Contains(s2.ToLower()) || s2.ToLower().Contains(s1.ToLower())))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// C#获取程序集中的所有类
        /// </summary>
        /// <param name="parentType">给定序集中的类型</param>
        /// <returns>所有类的名称</returns>
        public void GetClassTree(/*Type parentType*/Assembly assembly)
        {
            //TreeRoot = new Node() { Title = "ClassTree" };
            //System.Reflection.Assembly assembly = parentType.Assembly;//获取当前父类所在的程序集``
            //

            try
            {
                var TreeRootN = new Node() { Title = assembly.Location };
                //TreeRoot.Children.Add(TreeRootN);
                Type[] assemblyAllTypes = assembly.GetTypes();//获取该程序集中的所有类型
                foreach (Type itemType in assemblyAllTypes)//遍历所有类型进行查找
                {
                    var baseType = itemType.BaseType;//获取元素类型的基类
                    if (baseType != null)//如果有基类
                    {
                        Node baseNode = GetNodeFather(TreeRootN, baseType.Name);
                        if (baseNode == null)
                        {
                            baseNode = new Node() { Title = baseType.Name };
                            baseNode.Children.Add(new Node() { Title = itemType.Name });
                            TreeRootN.Children.Add(baseNode);
                        }
                        else
                        {
                            baseNode.Children.Add(new Node() { Title = itemType.Name });
                        }
                    }
                    else
                    {
                        Node baseNode = new Node() { Title = itemType.Name };
                        TreeRootN.Children.Add(baseNode);
                    }
                }
                if (TreeRootN.Children.Count > 0)
                {
                    SortNodes(TreeRootN);
                    classTree.Items.Add(TreeRootN);
                }
            }
            catch (System.Exception)
            {
                return;
            }
        }

        public Node GetNodeFather(Node root, string name)
        {
            if (root.Title == name)
            {
                return root;
            }
            foreach (Node node in root.Children)
            {
                root = GetNodeFather(node, name);
                if (root != null)
                {
                    return root;
                }
            }
            return null;
        }

        public void SortNodes(Node root)
        {
            List<Node> childNodes = root.Children.ToList();
            childNodes.Sort((a, b) => a.Title.CompareTo(b.Title));
            root.Children = new ObservableCollection<Node>(childNodes);
            foreach (Node node in root.Children)
            {
                SortNodes(node);
            }
        }

        public List<string> GetNodeTitles(Node root)
        {
            List<string> titles = new List<string>();
            titles.Add(root.Title);
            foreach (Node node in root.Children)
            {
                titles.AddRange(GetNodeTitles(node));
            }
            return titles;
        }

        /// <summary>
        /// C#获取一个类在其所在的程序集中的所有子类
        /// </summary>
        /// <param name="parentType">给定的类型</param>
        /// <returns>所有子类的名称</returns>
        public static List<string> GetSubClassNames(Type parentType)
        {
            var subTypeList = new List<Type>();
            var assembly = parentType.Assembly;//获取当前父类所在的程序集``
            var assemblyAllTypes = assembly.GetTypes();//获取该程序集中的所有类型
            foreach (var itemType in assemblyAllTypes)//遍历所有类型进行查找
            {
                var baseType = itemType.BaseType;//获取元素类型的基类
                if (baseType != null)//如果有基类
                {
                    if (baseType.Name == parentType.Name)//如果基类就是给定的父类
                    {
                        subTypeList.Add(itemType);//加入子类表中
                    }
                }
            }
            return subTypeList.Select(item => item.Name).ToList();//获取所有子类类型的名称
        }

        private bool ExpandAndSelectItem(ItemsControl parentContainer, string itemToSelect, bool isFuzzy)
        {
            //检查当前级别的所有项目
            foreach (object item in parentContainer.Items)
            {
                Node node = item as Node;
                TreeViewItem currentContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                //如果数据项与我们要选择的项匹配，则设置相应的项
                //TreeViewItem 是被选为真的
                if (IsEqual(node.Title, itemToSelect, isFuzzy) && currentContainer != null)
                {
                    string searched = node.Title;
                    //bool isSearched=false;
                    //foreach (var s in searchs)
                    //{
                    //    if (s==searched)
                    //    {
                    //        isSearched = true;
                    //    }
                    //}                 
                    if (!searchs.Contains(searched))
                    {
                        searchs.Add(searched);
                        currentContainer.IsSelected = true;
                        currentContainer.BringIntoView();
                        currentContainer.Focus();
                        //item被找到
                        return true;
                    }
                }
            }
            //如果我们到达这一点，在当前级别上找不到所选项目，所以我们必须检查 children
            foreach (Object item in parentContainer.Items)
            {
                TreeViewItem currentContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                //如果 children 存在
                if (currentContainer != null && currentContainer.Items.Count > 0)
                {
                    //跟踪树视图项是否被扩展
                    bool wasExpanded = currentContainer.IsExpanded;
                    //展开当前的树视图项，以便我们可以检查它的子树视图项
                    currentContainer.IsExpanded = true;
                    //如果尚未生成树视图项子容器，则必须侦听
                    //状态更改事件，直到它们是
                    if (currentContainer.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                    {
                        //将事件处理程序存储在变量中，以便我们可以删除它（在处理程序本身中）
                        EventHandler eh = null;
                        eh = new EventHandler(delegate
                        {
                            if (currentContainer.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                            {
                                if (ExpandAndSelectItem(currentContainer, itemToSelect, isFuzzy) == false)
                                {
                                    //假设在此事件处理程序中执行的代码是父进程的结果
                                    //由于没有生成容器而被扩展。
                                    //由于在孩子们中找不到“选择”项，所以折叠父项，因为它以前是折叠的
                                    currentContainer.IsExpanded = false;
                                }
                                //删除状态更改事件处理程序，因为我们刚刚处理了它（我们只需要它一次）
                                currentContainer.ItemContainerGenerator.StatusChanged -= eh;
                            }
                        });
                        currentContainer.ItemContainerGenerator.StatusChanged += eh;
                    }
                    else //否则容器已经生成，所以在子目录中查找要选择的项
                    {
                        if (ExpandAndSelectItem(currentContainer, itemToSelect, isFuzzy) == false)
                        {
                            //恢复当前树视图项的扩展状态
                            currentContainer.IsExpanded = wasExpanded;
                            //currentContainer.IsExpanded = false;
                        }
                        else //否则找到并选择节点，因此返回true
                        {
                            return true;
                        }
                    }
                }
            }
            //找不到item           
            return false;
        }

        private void Button_Click_Unload(object sender, RoutedEventArgs e)
        {
            classTree.Items.Remove(classTree.SelectedItem);
            //MessageBox.Show(classTree.Items.Count.ToString());
        }
        #endregion

        #region MethodTree

        private string _dataPath = null;
        public string DataPath
        {
            get
            {
                if (_dataPath == null)
                {
                    string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    string path = Path.Combine(folderPath, "AutoCADNetLoadManager");
                    _dataPath = Path.Combine(path, "AutoCADNetLoadManager.xml");
                }
                return _dataPath;
            }
        }

        public static string FilePath = null;
        public static string ClassName = null;
        public static string MethodName = null;

        public WindowReLoad()
        {
            InitializeComponent();
            AutoLoadDll();
        }

        private void AutoLoadDll()
        {
            if (File.Exists(DataPath))
            {
                try
                {
                    Datas datas = XmlUtils.DeserializeFromXml<Datas>(DataPath);
                    foreach (var dllPath in datas.DllPaths)
                    {
                        LoadDll(dllPath);
                    }
                }
                catch
                {
                }
            }
        }

        private void SaveDatas()
        {
            List<string> paths = new List<string>();
            foreach (var item in methodTree.Items)
            {
                Node node = item as Node;
                paths.Add(node.Title);
            }
            Datas datas = new Datas
            {
                DllPaths = paths.ToArray()
            };
            XmlUtils.SerializeToXml(datas, DataPath);
        }

        private void LoadDll(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            foreach (var item in methodTree.Items)
            {
                Node tree = item as Node;
                if (tree.Title == filePath)
                {
                    return;
                }
            }

            Assembly assembly = null;
            AssemLoader assemLoader = new AssemLoader();
            try
            {
                assemLoader.HookAssemblyResolve();
                assembly = assemLoader.LoadAddinsToTempFolder(filePath, true);
                if (null == assembly)
                {
                    return;
                }
                List<CmdInfo> cmdInfos = GetCommandMethod(assembly);
                GetTree(filePath, cmdInfos);
            }
            catch
            {
            }
            finally
            {
                assemLoader.UnhookAssemblyResolve();
            }
        }

        public string GetFileName()
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "dll文件|*.dll";
            if (openFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return null;
            }
            string filePath = openFileDialog.FileName;
            return filePath;
        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = GetFileName();
            if (!string.IsNullOrEmpty(filePath))
            {
                LoadDll(filePath);
            }
        }

        public List<CmdInfo> GetCommandMethod(Assembly assembly)
        {
            List<CmdInfo> cmds = new List<CmdInfo>();
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                MethodInfo[] methods = type.GetMethods();
                foreach (MethodInfo method in methods)
                {
                    var methodInfo = method.GetCustomAttribute<CommandMethodAttribute>();
                    if (methodInfo != null)
                    {
                        cmds.Add(new CmdInfo()
                        {
                            DllPath = assembly.Location,
                            ClassName = type.Namespace + "." + type.Name,
                            MethodName = methodInfo.GlobalName
                        });
                    }
                }
            }
            return cmds;
        }

        public void GetTree(string sourceDllPath, List<CmdInfo> children)
        {
            Node tree = new Node() { Title = sourceDllPath };
            foreach (var child in children)
            {
                tree.Children.Add(new Node()
                {
                    Title = sourceDllPath,
                    ClassName = child.ClassName,
                    MethodName = child.MethodName,
                });
            }
            for (int i = 0; i < methodTree.Items.Count; i++)
            {
                Node node = methodTree.Items[i] as Node;
                if (node.Title == sourceDllPath)
                {
                    methodTree.Items.RemoveAt(i);
                    i--;
                }
            }
            methodTree.Items.Add(tree);
            foreach (var item in methodTree.Items)
            {
                Node node = item as Node;
                DependencyObject dependencyObject = methodTree.ItemContainerGenerator.ContainerFromItem(item);
                TreeViewItem treeItem = dependencyObject as TreeViewItem;
                if (treeItem == null) continue;
                if (node.Title == sourceDllPath)
                {
                    treeItem?.ExpandSubtree();
                }
                else
                {
                    treeItem.IsExpanded = false;
                }
            }
        }

        private void OnItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //防止双击触发父TreeView项
            if (sender is TreeViewItem)
            {
                if (!((TreeViewItem)sender).IsSelected)
                {
                    return;
                }
            }

            Node tree = methodTree.SelectedItem as Node;
            if (tree.Children.Count != 0)
            {
                return;
            }

            Node selectedItem = methodTree.SelectedValue as Node;
            FilePath = tree.Title;
            ClassName = selectedItem.ClassName;
            MethodName = selectedItem.MethodName;
            this.Close();
            Commander.RunActiveCommand(FilePath, ClassName, MethodName);
        }


        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            if (methodTree.SelectedItem != null)
            {
                methodTree.Items.Remove(methodTree.SelectedItem);
            }
            else
            {
                MessageBox.Show("请选择要卸载的dll");
            }            
        }

        private void reloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (methodTree.SelectedItem != null)
            {
                Node node = methodTree.SelectedItem as Node;
                if (node != null)
                {
                    string fileName = node.Title;
                    methodTree.Items.Remove(methodTree.SelectedItem);                  
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        LoadDll(fileName);
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择要重载的dll");
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SaveDatas();
        }
        #endregion        
    }
}
