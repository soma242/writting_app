using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace writting_app;


//TreeView
public class SingleClickTreeView : TreeView
{
    protected override void WndProc(ref Message m)
    {
        const int WM_LBUTTONDBLCLK = 0x0203;

        if (m.Msg == WM_LBUTTONDBLCLK)
        {
            // ダブルクリックを無視
            return;
        }

        base.WndProc(ref m);
    }
}
