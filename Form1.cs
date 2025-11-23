using PublishStructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using writting_app.MessageInstance;



namespace writting_app;

public partial class Form1 : Form
{
    private readonly int expandWidth = 200;
    private readonly int mainMenuSplit = 36;


    int leftMenuWidth;
    int splitPanelHeight = 20;

    private bool expandMenu = true;


    IDisposable disposable;

    public Form1()
    {
        InitializeComponent();

        SetMessageContainer();

        leftMenuWidth = expandWidth;

        disposable = GlobalMessagePipe.GetSubscriber<string>().Subscribe(get =>
        {

            LogChecker.WriteLog(get);
        });


        Application.ApplicationExit += OnAppExit;

        LogChecker.Init(this.textBox1);

        //test
        /*
        var pub = GlobalMessagePipe.GetPublisher<string>();
        pub.Publish(Path.Combine(GlobalFilePath.docPath, "aaaa"));
        */

        //mev.Test();

        var uc = new SelectPanel();
        uc.Dock = DockStyle.Fill; // 忘れずに

        splitContainer3.Panel2.Controls.Add(uc);


        this.Resize += new System.EventHandler(this.Form1_Resize);

        CompSizeIni();

    }

    ~Form1()
    {
        disposable?.Dispose();
    }

    private void Form1_Resize(object? sender, EventArgs e)
    {


        splitContainer1.SplitterDistance = leftMenuWidth;
        splitContainer3.SplitterDistance = splitPanelHeight;
        splitContainer4.SplitterDistance = splitPanelHeight;
        splitContainer_mainMenu.SplitterDistance = mainMenuSplit;


    }

    private void OnAppExit(object? sender, EventArgs e)
    {
        GlobalWorkNames.SaveInstance();
    }


    private void toolStripMenuItem1_Click(object sender, EventArgs e)
    {
        // richTextBox1.Text = "aaa";

    }

    private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {

    }

    private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
    {

    }







    private void CompSizeIni()
    {

        splitContainer1.SplitterDistance = leftMenuWidth;
    }



    private void button1_Click(object sender, EventArgs e)
    {

        SetWorkName();

    }

    private void expand_button_Click_1(object sender, EventArgs e)
    {
        if (expandMenu)
        {
            expandMenu = false;
            leftMenuWidth = 36;
            splitContainer1.SplitterDistance = leftMenuWidth;
            flowLayoutPanel1.Visible = false;

        }
        else
        {
            expandMenu = true;
            leftMenuWidth = expandWidth;
            splitContainer1.SplitterDistance = leftMenuWidth;
            flowLayoutPanel1.Visible = true;
        }
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        //
    }


    private void SetMessageContainer()
    {
        var builder = new BuiltinContainerBuilder();

        builder.AddMessagePipe(/* configure option */);

        // AddMessageBroker: Register for IPublisher<T>/ISubscriber<T>, includes async and buffered.
        builder.AddMessageBroker<int, int>();
        //builder.AddMessageBroker<DisposeWorkNameSelecter>(); 
        builder.AddMessageBroker<string>();

        var provider = builder.BuildServiceProvider();
        GlobalMessagePipe.SetProvider(provider);

    }

    private void SetWorkName()
    {
        using (var uc = new WorkNameSelecter())
        {
            using (var form = new DialogForm(uc))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    uc.ReflectListBoxItems();
                    int index = uc.returnValue;
                    if (index == -1)
                        return;

                    string value = GlobalWorkNames.ReturnValue(index);


                    //list内のデータを読み込む前にデータの反映が保証されている必要がある。

                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        GlobalFilePath.SetWorkName(value);
                    }


                }
            }
        }
    }


}
