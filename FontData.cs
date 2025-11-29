using System;
using System.Collections.Generic;
using System.Text;

namespace writting_app;

internal class FontData
{

    public FontData(Font font)
    {
        this.font = font;
        fontHeight = font.Height;

    }

    public System.Drawing.Font font { get; private set; }

    public int fontHeight { get; private set; }

}
