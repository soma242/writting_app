using PublishStructure;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using writting_app;
using writting_app.MessageInstance;

namespace writting_app.CustomUI;

public class DrawMainTextTree: IDrawPanel
{
    private int indexKey;

    private Size buttonSize;

    public AlignmentData alignData { get; private set;  }



    public DrawMainTextTree(int indexKey, AlignmentData alignData)
    {
        this.indexKey = indexKey;
        this.alignData = alignData;

        buttonSize = new Size(0, alignData.fontHeight);
    }
    public void IDrawPanel(Graphics g)
    {
        //TreePaintPanelからの座標(ここでも考慮すると二倍動く)


        int pointY = 0;

        foreach( var mainText in GlobalMainTextsCashes.mainTextsSpan)
        {
            g.DrawRectangle(Pens.Black, new Rectangle(new Point(alignData.indent, pointY), buttonSize));
            TextRenderer.DrawText(g, mainText.text, alignData.font, new Point(alignData.indent, pointY), Color.Black);
            pointY += buttonSize.Height;

        }



        //var pub = GlobalMessagePipe.GetPublisher<string>();
        //pub.Publish("" + location);
    }

    public void IButtonClick(MouseEventArgs e)
    {
        int index = e.Y / alignData.fontHeight;
        if (index > GlobalMainTextsCashes.mainTexts.Count -1)
            return;
        var pub = GlobalMessagePipe.GetPublisher<int, ScreenMainTextMessage>();
        pub.Publish(alignData.screenIndex, new MainTextBox());
    }

    public int IResizeHeight()
    {
        buttonSize.Height = alignData.fontHeight;
        int temp = alignData.fontHeight * GlobalMainTextsCashes.mainTexts.Count;
        return temp +5;
    }

    public void IResizeWidth(int width)
    {

        buttonSize.Width = width - alignData.indent;
    }

    public void IDisposeSubscribes() 
    {
    
    }
}
