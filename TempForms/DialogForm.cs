using PublishStructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using writting_app.MessageInstance;


namespace writting_app;

//実際の動作は渡されるucに任せてok,cancelを返す

public partial class DialogForm : Form
{
    public int buttonHeight = 40;
   // public string returnValue = "";

    

    public DialogForm(WorkNameSelecter uc)
    {
        InitializeComponent();
        CommonInit(uc as UserControl);

        this.Text = "作品名を選択してください";
        this.AcceptButton = this.okButton;

        //FormClosing += WorkMessageClosing;
    }

    public DialogForm(InputString uc)
    {
        InitializeComponent();
        CommonInit(uc as UserControl);

        this.Text = "文字列入力フォーム";
        this.AcceptButton = this.okButton;

    }

    /*
    public void WorkMessageClosing(object sender, EventArgs e)
    {
        //ucに終了を通知
        var pub = GlobalMessagePipe.GetPublisher<DisposeWorkNameSelecter>();
        pub.Publish(new DisposeWorkNameSelecter());
    }
    */



    public void CommonInit(UserControl uc)
    {
        this.splitContainer1.Panel1.Controls.Add(uc);

        this.Size = new Size(Math.Max(uc.Width, buttonHeight), uc.Height + buttonHeight + 60);
        this.splitContainer1.SplitterDistance = uc.Height;
    }

    public void okButton_Click(object sender, EventArgs e)
    {
        //呼び出しもとでDisposeを行う
        this.splitContainer1.Panel1.Controls.Clear();
        this.DialogResult = DialogResult.OK;
        this.Close();
    }
    public void cancelButton_Click(object sender, EventArgs e)
    {
        //呼び出しもとでDisposeを行う
        this.splitContainer1.Panel1.Controls.Clear();
        this.DialogResult = DialogResult.Cancel;
        this.Close();
    }
}
