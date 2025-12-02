using MemoryPack;
using PublishStructure;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using writting_app;

namespace writting_app;





public static class LogChecker
{
    public static System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox();

    public static void Init(System.Windows.Forms.TextBox textbox)
    {
        textBox = textbox;
    }

    public static void WriteLog(string message)
    {
        textBox.Text += $"Log: {message}\n";
    }

    public static void WriteLog(int message) 
    {
        textBox.Text = $"Log: {message}";
    }

    /*
     LogChecker.WriteLog("");
    */
}

/*
//Genericとして使いまわしたいのでList<INodeData>になった。(List<TNode>として扱うとTNodeEnumから生成するタイミングでTNodeとしてうけとる必要がある)
//Unionは250未満で扱うべき(git hub)、ノードの種類なので大きく増えないはず
[MemoryPackable]
[MemoryPackUnion(0, typeof(MainTextNode))]
public partial interface INodeData
{
    public int id { get; }
    public string text { get; }

}
*/
[MemoryPackable]
public partial class NodeData
{
    public int id { get; private set; }
    public string text { get; set; }

    public NodeData(int id, string text) { 
        this.id = id;
        this.text = text;
    }


}

[MemoryPackable]
public partial class IdIndexManager
{
    public int lastAddedId;
    public Dictionary<int, int> idIndexPair;
    public List<int> deletedId;

    public IdIndexManager()
    {
        //最初に0を渡す
        lastAddedId = -1;
        idIndexPair = new Dictionary<int, int>();
        deletedId = new List<int>();

    }

    public int GetNewId()
    {
        //以前に消されたIDを割り当てる場合はlastAddedIdは触らない
        if(deletedId.Count > 0)
        {
            int temp = deletedId.Count - 1;
            int id = deletedId[temp];
            deletedId.RemoveAt(temp);
            return id;
        }
        else
        {
            lastAddedId++;
            return lastAddedId;
        }
    }
    public void SetNewPair(int id, int index)
    {
        idIndexPair.Add(id, index);
    }
}

/*
//interface
public interface IClickableNode
{
    public void IClick(TreeNodeMouseClickEventArgs e);
}

//開閉だけのノード
public class RootNode : TreeNode, IClickableNode
{
    //public string Text { get; private set; }

    public RootNode()
    {
        Text = "Root";
    }

    public void IClick(TreeNodeMouseClickEventArgs e)
    {
        LogChecker.WriteLog("RootNode clicked");
        if (e.Button == MouseButtons.Left)
        {
            if (e.Node.IsExpanded)
                e.Node.Collapse();
            else
                e.Node.Expand();
        }

    }
}

//選択したメインテキストを開くノード
public class MainTextNode : TreeNode, IClickableNode
{
    public MainTextNode()
    {
        Text = "MainText";
    }
    public void IClick(TreeNodeMouseClickEventArgs e)
    {
        //LogChecker.WriteLog("MainTextNode clicked");
        var p = GlobalMessagePipe.GetPublisher<int, int>();
        p.Publish(1,0);
    }
}


public class CharacterNode : TreeNode, IClickableNode
{
    public CharacterNode()
    {
        Text = "Character";
    }

    public void IClick(TreeNodeMouseClickEventArgs e)
    {
        LogChecker.WriteLog("CharacterNode clicked");
    }
}
*/