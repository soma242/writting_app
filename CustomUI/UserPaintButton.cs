using PublishStructure;
using System;
using System.Collections.Generic;
using System.Text;
using writting_app.MessageInstance;

namespace writting_app.CustomUI;

public class UserPaintButton: Control, IAlignable
{

    public bool drawing => true;

    private int indexKey;

    private bool mouseDown;

    private Point mousePoint;


    private IButtonInstance instance;


    public UserPaintButton(int indexKey, ControlButtonKind kind, AlignmentData alignData) 
    {
        this.SetStyle(ControlStyles.UserPaint, true);
        this.SetStyle(ControlStyles.UserMouse, true);

        this.indexKey = indexKey;
        //this.font = font;

        mouseDown = false;

        //test
        //this.Height = 40;

        switch (kind)
        {
            case ControlButtonKind.maintext:
                instance = new MainTextsControlButton(indexKey, alignData);
                break;

            default:
                break;
        }
    }



    public int IAligne(int posY)
    {
        
        this.Location = new Point(Location.X, posY);
        //instance側で調整
        this.Height = instance.IResizeHeight();

        //var pub = GlobalMessagePipe.GetPublisher<string>();
       // pub.Publish("" + Location.Y);

        return this.Height;
    }

    public void IResizeWidth(int width)
    {
        this.Width = width;
        instance.IResizeWidth(width -5);
    }

    protected override void OnPaint(PaintEventArgs e)
    { 
        //string text = "test";

        instance.IDrawButton(e.Graphics);

        //var pub = GlobalMessagePipe.GetPublisher<string>();
        //pub.Publish("paint");

    }

    private void PanelClick(Point panelPos)
    {
        instance.IButtonClick(panelPos);
    }

    //Control内のWndProcからの呼び出し
    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
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
            mouseDown = false;
            this.Capture = false;
            if (CheckMouseClick(e.Location))
            {
                PanelClick(e.Location);
            }
        }
    }

    private bool CheckMouseClick(Point location)
    {
        int tempX = mousePoint.X - location.X;
        int tempY = mousePoint.Y - location.Y;
        int temp = tempX * tempX + tempY * tempY;

        return temp < 9;
    }




}
