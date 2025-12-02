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
    //private IDisposable disposable;

    //listBoxに変更があったか
    //dragdrop, createButton
    public bool changed { private set; get; }

    //ucが生き残らないようにindexのみを返して反映後のglobalWorkNamesのリストを参照する。
    public int returnValue { get { return listBox1.SelectedIndex; } }

    //public string? returnValue;

    private int dragIndex = -1;      // ドラッグ開始位置
    private int hoverIndex = -1;     // 現在のプレビュー位置


    public WorkNameSelecter()
    {
        InitializeComponent();

        changed = false;



        listBox1.MouseDown += ListBox1_MouseDown;
        listBox1.MouseMove += ListBox1_MouseMove;
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
        if(listBox1.Items.Count > 0) 
            listBox1.SelectedIndex = 0;
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



    private void ListBox1_MouseDown(object sender, MouseEventArgs e)
    {
        dragIndex = listBox1.IndexFromPoint(e.Location);
    }

    private void ListBox1_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left && dragIndex >= 0)
        {
            listBox1.DoDragDrop(listBox1.Items[dragIndex], DragDropEffects.Move);
        }
    }

    private void ListBox1_DragOver(object sender, DragEventArgs e)
    {
        e.Effect = DragDropEffects.Move;

        Point p = listBox1.PointToClient(new Point(e.X, e.Y));

        int temp = listBox1.IndexFromPoint(p);
        if (hoverIndex != temp) {
            hoverIndex = temp;
            DrawPreviewLine();
        }

    }

    private void ListBox1_DragDrop(object sender, DragEventArgs e)
    {
        if (dragIndex < 0 || hoverIndex < 0 || dragIndex == hoverIndex) return;

        object item = listBox1.Items[dragIndex];
        listBox1.Items.RemoveAt(dragIndex);
        listBox1.Items.Insert(hoverIndex, item);

        changed = true;

        dragIndex = -1;
        hoverIndex = -1;
    }

    //MouseOverしている位置のindexが変わるたびに呼ばれて、プレビューラインを乗せる
    private void DrawPreviewLine()
    {
        listBox1.Refresh(); 
        if (hoverIndex < 0) return;

        Rectangle rect = listBox1.GetItemRectangle(hoverIndex);

        using (Graphics g = listBox1.CreateGraphics())
        {
            g.FillRectangle(Brushes.Red, 0, rect.Top - 1, listBox1.Width, 2); // 上側に線
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


                        
                        string directoryPath = Path.Combine(GlobalFilePath.docPath, uc.returnValue);

                        // 既に存在するなら
                        if (Directory.Exists(directoryPath)) 
                        {
                            MessageBox.Show("そのディレクトリはすでに存在しています", "エラー",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        listBox1.Items.Add(uc.returnValue);


                        //作品名のDirectoryを作成
                        Directory.CreateDirectory(directoryPath);


                        //Directoryとbinファイルは先に用意。増やしたらそれ用の処理を走らせる。
                        GlobalCreateFiles.CreateWorkFile(directoryPath);



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

                //var pub = GlobalMessagePipe.GetPublisher<string>();
                //pub.Publish(temp);
            }
            
            
        }
    }
}
