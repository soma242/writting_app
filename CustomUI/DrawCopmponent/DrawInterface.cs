using System;
using System.Collections.Generic;
using System.Text;

namespace writting_app.CustomUI;

public interface IDrawPanel
{
    //プロパティとして実装可能
    //privateにしたいならinterfaceでは隠して実装側で行う

    //indexKey=>SubのKeyに用いるが外から要求されることはなく不変なので隠しておく
    //public int indexKey { get; }

    //Fontは画面ごとに変更できるように。Subするindexから
    //これも外から要求されないので隠しておく
    //public System.Drawing.Font font {get; }

    //public int panelHeight { get; }

    //メソッド
    public void IDrawPanel(Graphics g);

    public int IResizeHeight();

    public void IResizeWidth(int width);

    public void IButtonClick(MouseEventArgs e);

    //fontのSubを行うのでInterface標準としてDisposeも用意してTreePaintのSubから呼び出す。
    //Dispose自体は必要ないので隠す
    public void IDisposeSubscribes();
}

/*
 
標準搭載パラメータ
int indexKey
Font font




*/



public interface IAlignable
{
    //public bool drawing { get; }

    //縦の再調整
    public int IAligne(int posY); 

    //横のfitting
    public void IResizeWidth(int width);
}



public interface IButtonInstance
{

    //OnPaint
    public void IDrawButton(Graphics g); 

    //Click
    public void IButtonClick(Point pos);

    public void IResizeWidth(int width);

    //Sizeだけ返せばいい。描写は上記関数で
    public int IResizeHeight();


    /*
    
    標準搭載パラメータ
    
    int indexKey
    Font font

     
    */
}