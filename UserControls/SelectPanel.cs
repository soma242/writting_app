using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace writting_app
{

    public partial class SelectPanel : UserControl
    {
        private RootNode mainTextRoot;
        private RootNode characterRoot;

        public SelectPanel()
        {
            InitializeComponent();

            //LogChecker.WriteLog("");

            // イベントハンドラーの登録
            singleClickTreeView1.NodeMouseClick += TreeView1_NodeMouseClick;


        }

        private void SelectPanel_Load(object sender, EventArgs e)
        {
            var node = new RootNode();
            singleClickTreeView1.Nodes.Add(node);

            node = new RootNode();
            var chara = new CharacterNode();
            node.Nodes.Add(chara);
            var main = new MainTextNode();
            node.Nodes.Add(main);

            singleClickTreeView1.Nodes.Add(node);

        }



        private void TreeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node is IClickableNode)
            {
                ((IClickableNode)e.Node).IClick(e);
            }
            else
            {
                LogChecker.WriteLog("is not clickable");
            }

        }

        private void singleClickTreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
    }

}





