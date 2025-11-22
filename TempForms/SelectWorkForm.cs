using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace writting_app.TempForms;

public partial class SelectWorkForm : Form
{

    public string? workName = null;

    public SelectWorkForm()
    {
        InitializeComponent();
    }

    private void SelectWork_Load(object sender, EventArgs e)
    {
        WorkNameSelecter uc = new WorkNameSelecter();
        uc.Dock = DockStyle.Fill;

        this.Controls.Add(uc);
    }

    private void createButton_Click(object sender, EventArgs e)
    {
        using(var uc = new InputString())
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
}
