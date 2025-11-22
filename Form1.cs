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

    int leftMenuWidth = 200;
    int splitPanelHeight = 20;


    IDisposable disposable;

    public Form1()
    {
        InitializeComponent();

        SetMessageContainer();

        disposable = GlobalMessagePipe.GetSubscriber<int, int>().Subscribe(0, get =>
        {

            LogChecker.WriteLog("Key Message Sub");
        });


        Application.ApplicationExit += OnAppExit;

        LogChecker.Init(this.textBox1);


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

    private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
    {

    }

    private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
    {

    }

    private void toolStripContainer1_ContentPanel_Load(object sender, EventArgs e)
    {

    }

    private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
    {

    }






    private void CompSizeIni()
    {

        splitContainer1.SplitterDistance = leftMenuWidth;
    }

    private void splitContainer2_Panel1_Paint(object sender, PaintEventArgs e)
    {

    }

    private void button1_Click(object sender, EventArgs e)
    {

        using(var uc = new WorkNameSelecter())
        {
            using (var form = new DialogForm(uc))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {

                    /*
                    if (!string.IsNullOrWhiteSpace(form.InputText))
                    {
                        //listBox1.Items.Add(form.InputText);
                    }
                    */
                }
            }
        }
        
    }

    private void Form1_Load(object sender, EventArgs e)
    {

    }


    private void SetMessageContainer()
    {
        var builder = new BuiltinContainerBuilder();

        builder.AddMessagePipe(/* configure option */);

        // AddMessageBroker: Register for IPublisher<T>/ISubscriber<T>, includes async and buffered.
        builder.AddMessageBroker<int, int>(); 
        builder.AddMessageBroker<DisposeWorkNameSelecter>(); 

        var provider = builder.BuildServiceProvider();
        GlobalMessagePipe.SetProvider(provider);

    }
}
