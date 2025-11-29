using PublishStructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using writting_app.MessageInstance;
using static System.Net.Mime.MediaTypeNames;

namespace writting_app.CustomUI;

public class MainTextsControlButton : IButtonInstance
{
    private readonly int rationString = 8;

    private int indexKey;
    private AlignmentData alignData;

    private Size buttonSize;

    //文字数制限
    private int limitString;

    public MainTextsControlButton(int indexKey, AlignmentData alignData) 
    {
        this.indexKey = indexKey;
        this.alignData = alignData;

        

        buttonSize = new Size(0, alignData.fontHeight);
    }



    public void IDrawButton(Graphics g )
    {


        //int limit = buttonSize.Width / alignData.fontHeight;
        
        //var pub = GlobalMessagePipe.GetPublisher<string>();
        //pub.Publish("" + limitString);
        //pub.Publish("" + alignData.fontHeight);
        //pub.Publish("" + alignData.font.Size);
        //pub.Publish("" + buttonSize.Height);
        
        //g.DrawLine(Pens.Gray, 0, 0, 400, 0);
        g.DrawRectangle(Pens.Black, new Rectangle(new Point(0,0), buttonSize));
        
        if (limitString < ButtonString.mainTexts.Length)
        {
            ReadOnlySpan<char> span = ButtonString.mainTexts.AsSpan(0, limitString);
            TextRenderer.DrawText(g, span, alignData.font, new Point(0, 0), Color.Black);
            
        }
        else
        {
            TextRenderer.DrawText(g, ButtonString.mainTexts, alignData.font, new Point(0, 0), Color.Black);

        }
    }

    public void IButtonClick(Point point) 
    {
        var pub = GlobalMessagePipe.GetPublisher<int, ExpandMainTexts>();
        pub.Publish(indexKey, new ExpandMainTexts());

        //var pubSt = GlobalMessagePipe.GetPublisher<string>();
        //pubSt.Publish("click");
    }

    public void IResizeWidth(int width) {
        buttonSize.Width = width;
        CalculateLimitString();
    }

    public int IResizeHeight()
    {
        if (buttonSize.Height != alignData.fontHeight) {
            buttonSize.Height = alignData.fontHeight;
            CalculateLimitString();
        }
        return buttonSize.Height+5;
    }

    private void CalculateLimitString()
    {
        //(AlignmentPanel.Width-20) * 0.8(8/10) / (alignData.fontHeight/2) +1(切り上げ)
        limitString = (buttonSize.Width * rationString) / (alignData.fontHeight * 5) +1;
    }
}



