using System;
using System.Collections.Generic;
using System.Text;

namespace writting_app.CustomUI;

public class MainTextBox : TextBox
{
    public MainTextBox()
    {
        Multiline = true;
        ScrollBars = ScrollBars.Vertical;
        WordWrap = true;
    }
}
