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
