using PublishStructure;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using writting_app;

namespace writting_app.CustomUI;

public class DrawMainTextTree: IDrawPanel
{
    private int indexKey;

    public AlignmentData alignData { get; private set;  }

    //test
    List<string> list = new List<string>();

    public DrawMainTextTree(int indexKey, AlignmentData alignData)
    {
        this.indexKey = indexKey;
        this.alignData = alignData;

        //test
        list.Add("first");
        list.Add("second");
    }
    public void IDrawPanel(Graphics g)
    {
        //TreePaintPanelからの座標(ここでも考慮すると二倍動く)
        var span = CollectionsMarshal.AsSpan<string>(list);

        //TextRenderer.DrawText(g, "ok", alignData.font, new Point(0, 0, Color.Black);
        Point location = new Point(GlobalFontSetting.indentSize, 0);
        foreach (var item in span)
        {

            TextRenderer.DrawText(g, item, alignData.font, location, Color.Black);
            location = new Point(location.X, location.Y+alignData.fontHeight);
        }
        //var pub = GlobalMessagePipe.GetPublisher<string>();
        //pub.Publish("" + location);
    }

    public int IResizeHeight()
    {
        int temp = alignData.fontHeight * list.Count;
        return temp;
    }

    public void IDisposeSubscribes() 
    {
    
    }
}
