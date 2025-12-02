using MemoryPack;
using PublishStructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Pipelines;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using writting_app.CustomUI;
using writting_app.MessageInstance;


namespace writting_app;

public partial class Form1 : Form
{
    private readonly int expandWidth = 200;
    private readonly int mainMenuSplit = 36;

    //setting
    private AppSetting setting;


    int leftMenuWidth;
    int splitPanelHeight = 20;

    private bool expandMenu = true;

    private TextScreen mainScreen;
    private TextScreen subScreen;

    IDisposable disposable;

    public Form1()
    {
        InitializeComponent();

        SetMessageContainer();

        //test
        InitializeSettings();
        //setting = new AppSetting();
        //SaveSettings();

        leftMenuWidth = expandWidth;

        disposable = GlobalMessagePipe.GetSubscriber<string>().Subscribe(get =>
        {

            LogChecker.WriteLog(get);
        });


        Application.ApplicationExit += OnAppExit;

        LogChecker.Init(this.textBox1);



        CompSizeIni();

        mainScreen = new TextScreen();
        subScreen = new TextScreen();

        
        mainScreen.Dock = DockStyle.Fill;
        splitContainer3.Panel2.Controls.Add(mainScreen);

        


        //flow.Dispose();

        //work選択用の画面を開く

        if (setting.openWithLastWork)
        {
            GlobalFilePath.SetWorkName(setting.lastWorkName);
        }
        else
        {
            SetWorkName();

        }

        //workNameが空のままだと開けなくなる
        var flow = new AlignmentPanel(GlobalFilePath.alignmentIndex, mainScreen.screenIndex, DrawKind.all);
        flow.Dock = DockStyle.Fill;
        flow.AutoScroll = true;
        //flow.
        mainScreen.Panel2.Controls.Add(flow);

        /*
        int size = Unsafe.SizeOf<>();
        int testSize = Unsafe.SizeOf<TestString>();
        var pub = GlobalMessagePipe.GetPublisher<string>();
        pub.Publish("size:" + size + "\n");
        pub.Publish("size:" + testSize + "\n");
        */

        //LogChecker.WriteLog(GlobalFilePath.workName);
        //GlobalFilePath.SetWorkName("");
    }

    ~Form1()
    {
        disposable?.Dispose();
    }

    private void SetMessageContainer()
    {
        var builder = new BuiltinContainerBuilder();

        builder.AddMessagePipe(/* configure option */);

        // AddMessageBroker: Register for IPublisher<T>/ISubscriber<T>, includes async and buffered.
        builder.AddMessageBroker<int, int>();
        //builder.AddMessageBroker<DisposeWorkNameSelecter>(); 
        builder.AddMessageBroker<string>();

        builder.AddMessageBroker<int, string>();

        builder.AddMessageBroker<WorkChangedMessage>();

        builder.AddMessageBroker<AligneListChanged>();


        builder.AddMessageBroker<int, ChangeScreenFont>();
        builder.AddMessageBroker<int, ScreenMainTextMessage>();

        builder.AddMessageBroker<int, AlignmentWidth>();
        builder.AddMessageBroker<int, ExpandMainTexts>();
        builder.AddMessageBroker<int, AlignCall>();

        var provider = builder.BuildServiceProvider();
        GlobalMessagePipe.SetProvider(provider);

    }

    /*
    private void Form1_Resize(object? sender, EventArgs e)
    {


        splitContainer1.SplitterDistance = leftMenuWidth;
        splitContainer3.SplitterDistance = splitPanelHeight;
        splitContainer4.SplitterDistance = splitPanelHeight;
        splitContainer_mainMenu.SplitterDistance = mainMenuSplit;


    }
    */

    private void OnAppExit(object? sender, EventArgs e)
    {
        //setting.openWithLastWork = true;
        GlobalWorkNames.SaveInstance();
        SaveSettings();
        //開かれている作品のcasheをセーブ。作品切り替え時と同一
        var pub = GlobalMessagePipe.GetPublisher<WorkChangedMessage>();
        pub.Publish(new WorkChangedMessage());
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

        splitContainer1.Panel1MinSize = leftMenuWidth;
        splitContainer3.Panel1MinSize = splitPanelHeight;
        //splitContainer4.Panel1MinSize = splitPanelHeight;
        splitContainer_mainMenu.Panel1MinSize = mainMenuSplit;
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
            splitContainer1.Panel1MinSize = leftMenuWidth;
            splitContainer1.SplitterDistance = leftMenuWidth;
            flowLayoutPanel1.Visible = false;

        }
        else
        {
            expandMenu = true;
            leftMenuWidth = expandWidth;
            splitContainer1.Panel1MinSize = leftMenuWidth;
            splitContainer1.SplitterDistance = leftMenuWidth;
            flowLayoutPanel1.Visible = true;
        }
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        //
    }



    private void SetWorkName()
    {
        //余裕出来たら考え直す
        using (var uc = new WorkNameSelecter())
        {
            //workNameが空の場合呼ばれ続ける
            //uc自体はusing抜けるまでDisposeせずに使いまわす
            do
            {
                using (var form = new DialogForm(uc))
                {
                    //ShowDialogを待ってからreflectionしないといけない
                    //Dialogの結果にかかわらずreflectionする
                    var result = form.ShowDialog();
                    uc.ReflectListBoxItems();


                    if (result == DialogResult.OK)
                    {
                        int index = uc.returnValue;
                        //次のWhileへ
                        if (index == -1)
                            continue;

                        string value = GlobalWorkNames.ReturnValue(index);


                        //list内のデータを読み込む前にデータの反映が保証されている必要がある。

                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            GlobalFilePath.SetWorkName(value);
                        }


                    }
                }
            } while (string.IsNullOrWhiteSpace(GlobalFilePath.workName));
           
        }
    }

    private void InitializeSettings()
    {
        string tempPath = Path.Combine(GlobalFilePath.docPath, "settings.bin");
        if (!File.Exists(tempPath))
        {
            //ファイルがなければ作成する。
            using (var fs = File.Create(tempPath))
            {
                //空のファイルを作成

            }

        }else if (new FileInfo(tempPath).Length < 1)
        {
            var pub = GlobalMessagePipe.GetPublisher<string>();
            pub.Publish("Error, ファイル内にデータが存在しない");
            return;
        }
        
        byte[] bytes = File.ReadAllBytes(tempPath);
        setting = MemoryPackSerializer.Deserialize<AppSetting>(bytes);
    }

    private void SaveSettings()
    {
        setting.lastWorkName = GlobalFilePath.workName;

        string tempPath = Path.Combine(GlobalFilePath.docPath, "settings.bin");
        using(var fs = File.OpenWrite(tempPath))
        {
            byte[] bytes = MemoryPackSerializer.Serialize<AppSetting>(setting);
            fs.Write(bytes, 0, bytes.Length);
        }



    }
}



//追加は可能、削除は別class作ってデータを掬った後このクラスに整えてSerialize
[MemoryPackable]
public partial class AppSetting 
{
    //最後に開かれたWorkで開くようにする
    [SuppressDefaultInitialization]
    public bool openWithLastWork = true;
    public string lastWorkName = "";

    //public 
}

