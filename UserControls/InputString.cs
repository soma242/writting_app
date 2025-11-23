using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace writting_app;

public partial class InputString : UserControl
{
    public string returnValue
    {
        get { return textBox1.Text; }
        //set { textBox1.Text = value; }
    }
    public InputString()
    {
        InitializeComponent();
    }

    private void InputString_Load(object sender, EventArgs e)
    {

    }

    private void textBox1_Enter(object sender, EventArgs e)
    {

    }
}
