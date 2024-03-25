using System.Collections.ObjectModel;

namespace AutoCADNetLoadManager
{
    public class Node
    {
        public bool IsGrouping { get { return Children != null && Children.Count > 0; } }

        public Node Parent { get; set; }

        public string Title { get; set; }

        public string ClassName { get; set; }

        public string MethodName { get; set; }

        public ObservableCollection<Node> Children { get; set; }

        public Node()
        {
            this.Children = new ObservableCollection<Node>();
        }
    }
}
