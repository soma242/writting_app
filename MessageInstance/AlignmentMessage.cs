using System;
using System.Collections.Generic;
using System.Text;

namespace writting_app.MessageInstance;

//サイズ変更時に通知
public struct AlignmentWidth
{
    public AlignmentWidth(int width)
    {
        this.width = width;
    }
    public int width;
}

public struct ExpandMainTexts { }

public struct AlignCall {}