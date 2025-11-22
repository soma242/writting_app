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

public partial class DialogForm : Form
{
    public int buttonHeight = 40;
    public string returnValue = "";

    

    public DialogForm(WorkNameSelecter uc)
    {
        InitializeComponent();
        CommonInit(uc as UserControl);

        this.Text = "作品名";

        FormClosing += WorkMessageClosing;
    }

    public DialogForm(InputString uc)
    {
        InitializeComponent();
        CommonInit(uc as UserControl);

        this.Text = "文字列入力フォーム";
        this.AcceptButton = this.okButton;

    }

    public void WorkMessageClosing(object sender, EventArgs e)
    {
        var pub = GlobalMessagePipe.GetPublisher<DisposeWorkNameSelecter>();
        pub.Publish(new DisposeWorkNameSelecter());
    }



    public void CommonInit(UserControl uc)
    {
        this.splitContainer1.Panel1.Controls.Add(uc);

        this.Size = new Size(Math.Max(uc.Width, buttonHeight), uc.Height + buttonHeight + 60);
        this.splitContainer1.SplitterDistance = uc.Height;
    }

    public void okButton_Click(object sender, EventArgs e)
    {
        this.DialogResult = DialogResult.OK;
        this.Close();
    }
    public void cancelButton_Click(object sender, EventArgs e)
    {
        this.DialogResult = DialogResult.Cancel;
        this.Close();
    }
}
