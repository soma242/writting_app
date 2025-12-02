using PublishStructure;
using System;
using System.Collections.Generic;
using System.Text;
using writting_app.MessageInstance;

namespace writting_app.CustomUI;

public class TreePaintPanel : Panel, IAlignable
{
    //単体での初期化ならtrue, 全体でならfalse
    //OnPaintや、Ialignableからの呼び出しをreturnする
    public bool drawing {  get; private set; }

    private int indexKey;

    private int indent = 0;
    private int fontHeight;

    private readonly int smallButtonWidth = 20;
    private int smallButtonX;

    //private Rectangle rect;

    private List<int> testList;

    private Point mousePoint;
    private bool mouseDown = false;


    //System.Drawing.Font font;

    //int paintCount = 0;

    private IDisposable disposableExpand;

    private IDrawPanel painter;



    public TreePaintPanel(int indexKey, DrawKind kind, AlignmentData alignData, bool drawing)
    {
        //autoScrollはこれを内部に持つflowLayoutで行う。
        //this.AutoScroll = true;
        //this.MouseClick += TreePaintPanel_MouseClick;

        //FormではなくControl単体としてマウス操作を受け付ける
        //
        this.SetStyle(ControlStyles.UserMouse, true);

        this.indexKey = indexKey;
        this.drawing = drawing;

        //painterで使うのでここでは使わない
        //this.font = font;

        //painterの選択
        switch (kind)
        {
            case DrawKind.mainText:
                painter = new DrawMainTextTree(indexKey, alignData);
                var sub = GlobalMessagePipe.GetSubscriber<int, ExpandMainTexts>();
                disposableExpand = sub.Subscribe(indexKey, get =>
                {
                    ChangeDrawing();
                    var pub = GlobalMessagePipe.GetPublisher<int, AlignCall>();
                    pub.Publish(indexKey, new AlignCall());
                    //this.Refresh();
                });
                break;

            default:
                break;
        }

        //

        //Test用
        testList = new List<int>();
        for (int i = 0; i < 40; i++)
        {
            testList.Add(i);
        }

    }

    private void ChangeDrawing()
    {
        if (drawing)
        {
            drawing = false;
            //visible変えないと透明なパネルが下の要素を隠してしまう
            Visible = false;
        }
        else
        {
            drawing = true;
            Visible = true;
        }
        //this.Refresh();
    }

    public int IAligne(int posY)
    {
        if (!drawing)
            return 0;
        Location = new Point(Location.X, posY);
        this.Height = painter.IResizeHeight();
        return this.Height;
    }

    public void IResizeWidth(int width)
    {
        this.Width = width;
        painter.IResizeWidth(width - 5);
    }




    private void DisposeSubscribes()
    {
        disposableExpand?.Dispose();

        painter.IDisposeSubscribes();

        //var pub = GlobalMessagePipe.GetPublisher<string>();
        //pub.Publish("dispose");
    }

    protected override void OnPaint(PaintEventArgs e)
    {

        //var pub = GlobalMessagePipe.GetPublisher<string>();
        //pub.Publish(""+this.Location);
        if (!drawing)
        {
            return;
        }

        //背景描画
        base.OnPaint(e);
        //Rectangle view = e.ClipRectangle;

        //実描画
        painter.IDrawPanel(e.Graphics);

        /*
        //描画テスト
        string text = "test";
        fontHeight = font.Height;
        int drawY = 0;
        Graphics g = e.Graphics;

        Point drawPoint = new Point(indent, drawY);

        //上に線を描写
        g.DrawLine(Pens.Gray, indent, drawY, this.Width, drawY);
        //g.DrawLine(Pens.Gray, indent, drawY + fontHeight - 1, this.Width, drawY + font.Height - 1);

        //文字列表示
        TextRenderer.DrawText(g, text, font, drawPoint, Color.Black);

        string plus = "+";

        Point plusPoint = new Point(drawPoint.X + 80, drawPoint.Y);

        int i = 0;
        for (i = 1; i < 40; i++)
        {
            drawY += fontHeight;

            drawPoint = new Point(indent, drawY);

            g.DrawLine(Pens.Gray, indent, drawY, this.Width, drawY);

            //文字列表示
            TextRenderer.DrawText(g, text + i, font, drawPoint, Color.Black);

            plusPoint = new Point(smallButtonX, drawPoint.Y);
            TextRenderer.DrawText(g, plus, font, plusPoint, Color.Black);
            g.DrawRectangle(Pens.Black, new Rectangle(plusPoint, new Size(smallButtonWidth, fontHeight)));


        }
        int totalHeight = (i + 1) * fontHeight;

        this.Height = totalHeight;
        */
    }

    private void PanelClick(MouseEventArgs e)
    {
        painter.IButtonClick(e);

    }

    //Control内のWndProcからの呼び出し
    protected override void OnMouseDown(MouseEventArgs e) 
    { 
        if(e.Button == MouseButtons.Left)
        {
            mouseDown = true;
            mousePoint = e.Location;
            this.Capture = true;
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        if (mouseDown)
        {
            mouseDown=false;
            this.Capture = false;
            if (CheckMouseClick(e.Location))
            {
                PanelClick(e);
            }
        }
    }

    private bool CheckMouseClick(Point location)
    {
        int tempX = mousePoint.X - location.X;
        int tempY = mousePoint.Y - location.Y;
        int temp = tempX * tempX + tempY * tempY;

        //var pub = GlobalMessagePipe.GetPublisher<string>();
        //pub.Publish("" + temp);

        return temp < 9;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            DisposeSubscribes();
        }
        base.Dispose(disposing);
    }
}