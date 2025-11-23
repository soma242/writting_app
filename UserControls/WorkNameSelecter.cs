using PublishStructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using writting_app.MessageInstance;
using static System.Net.WebRequestMethods;

namespace writting_app;

public partial class WorkNameSelecter : UserControl
{

    private int buttonHeight = 40;
    //Desiner.csのDisposeでDispose
    private IDisposable disposable;

    //listBoxに変更があったか
    //dragdrop, createButton
    public bool changed { private set; get; }

    //ucが生き残らないようにindexのみを返して反映後のglobalWorkNamesのリストを参照する。
    public int returnValue { get { return listBox1.SelectedIndex; } }

    //public string? returnValue;

    public WorkNameSelecter()
    {
        InitializeComponent();

        changed = false;

        listBox1.MouseDown += ListBox1_MouseDown;
        listBox1.DragOver += ListBox1_DragOver;
        listBox1.DragDrop += ListBox1_DragDrop;

        /*
        var sub = GlobalMessagePipe.GetSubscriber<DisposeWorkNameSelecter>();
        disposable = sub.Subscribe(get =>
        {
            if(changed)
                ReflectListBoxItems();
        });
        */
    }

    private void WorkNameSelecter_Load(object sender, EventArgs e)
    {
        listBox1.BeginUpdate();
        SetListBoxItems();
        listBox1.EndUpdate();
    }

    private void WorkNameSelecter_Resize(object sender, EventArgs e)
    {
        splitContainer1.SplitterDistance = buttonHeight;
    }

    /*
    private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
    */





    private void ListBox1_MouseDown(object? sender, MouseEventArgs e)
    {
        if (listBox1.SelectedItem != null)
        {
            listBox1.DoDragDrop(listBox1.SelectedItem, DragDropEffects.Move);
        }
    }

    private void ListBox1_DragOver(object? sender, DragEventArgs e)
    {
        e.Effect = DragDropEffects.Move;
    }

    private void ListBox1_DragDrop(object? sender, DragEventArgs e)
    {
        Point point = listBox1.PointToClient(new Point(e.X, e.Y));
        int index = listBox1.IndexFromPoint(point);

        if (index < 0) index = listBox1.Items.Count - 1;

        if (e.Data.GetData(typeof(string)) is string data)
        {
            // data は string 型として null ではない状態で使える
            listBox1.Items.Remove(data);
            listBox1.Items.Insert(index, data);

            changed = true;
        }
    }


    private void SetListBoxItems()
    {
        foreach (var workName in GlobalWorkNames.workNamesAsSpan())
        {
            listBox1.Items.Add(workName);
        }
    }

    //順序をずらさないためにucを読み込んだ側で能動的に実行
    public void ReflectListBoxItems()
    {
        if (changed)
        {
            GlobalWorkNames.ReflectionList(this.listBox1);
        }

    }




    private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
    {

    }


    //
    private void createButton_Click(object sender, EventArgs e)
    {
        //var p = GlobalMessagePipe.GetPublisher<int, int>();
        //p.Publish(0, 0);
        using (var uc = new InputString())
        {
            using (var form = new DialogForm(uc))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {

                    if (!string.IsNullOrWhiteSpace(uc.returnValue))
                    {
                        listBox1.Items.Add(uc.returnValue);


                        string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        string directoryPath = Path.Combine(docPath, uc.returnValue);

                        //作品名のDirectoryを作成
                        Directory.CreateDirectory(directoryPath);

                        //未完成
                        //作品内のcasheを保存するファイルを作成。
                       // GlobalFilePath.CreateNodeCache(uc.returnValue);


                        //初期のディレクトリを作成。
                        //MainText, Character,

                        changed = true;

                        /*
                        //これは終了時にglobalWorkNamesから保存
                        using (StreamWriter sw = new StreamWriter(GlobalWorkNames.cashePath))
                        {
                            sw.WriteLine(uc.returnValue);
                        }
                        */

                    }

                }
            }
        }
    }

    private void DeleteButton_Click(Object sender, EventArgs e)
    {
        if (listBox1.SelectedIndex == -1)
        {
            return;
        }

        string workName = listBox1.SelectedItem as string;

        if (MessageBox.Show($"{workName}を削除しようとしています。\n本当に削除しますか？",
                    "確認",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
        {
            var temp = Path.Combine(GlobalFilePath.docPath, workName);


            listBox1.Items.Remove(listBox1.SelectedItem);
            changed = true;
            if (Directory.Exists(temp))
            {
                try
                {
                    Directory.Delete(temp, recursive: true);
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"ファイルの削除に失敗しました: {ex.Message}");
                }

                var pub = GlobalMessagePipe.GetPublisher<string>();
                pub.Publish(temp);
            }
            
            
        }
    }
}
