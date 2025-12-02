using PublishStructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using System.Windows.Forms;
using writting_app.MessageInstance;
using static System.Net.Mime.MediaTypeNames;

namespace writting_app.CustomUI;

public class MainTextsControlButton : IButtonInstance
{
    private readonly int ratioString = 8;

    private int indexKey;
    private AlignmentData alignData;

    private Size buttonSize;
    private int miniButtonX;

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
        
        //button
        g.DrawRectangle(Pens.Black, new Rectangle(new Point(miniButtonX, 0), new Size( alignData.fontHeight, alignData.fontHeight)));
        TextRenderer.DrawText(g,  ButtonString.miniCreateTexts, alignData.font, new Point(miniButtonX, 0), Color.Black);

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
        if(point.X < miniButtonX)
        {
            var pub = GlobalMessagePipe.GetPublisher<int, ExpandMainTexts>();
            pub.Publish(indexKey, new ExpandMainTexts());
        }
        else
        {
            using (var uc = new InputString())
            {
                using (var form = new DialogForm(uc))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {

                        if (!string.IsNullOrWhiteSpace(uc.returnValue))
                        {
                            

                            GlobalMainTextsCashes.AddNode(uc.returnValue);
                        }

                    }
                }
            }
        }

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
        // / (ボタン全体のサイズ * メインの比率)/ (fontHeight/2 *10) + 1(切り上げ)
        limitString = (buttonSize.Width * ratioString) / (alignData.fontHeight * 5) + 1;

        //(ボタン全体のサイズ) * (1-メインの比率)  (10-8)/10
        //stringWidth = buttonSize.Width * (10 - ratioString) / 10;
        miniButtonX = buttonSize.Width - alignData.fontHeight;
    }
}



