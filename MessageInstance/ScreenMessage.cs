using System;
using System.Collections.Generic;
using System.Text;
using writting_app.CustomUI;

namespace writting_app.MessageInstance;

public struct ChangeScreenFont
{
    public float size;
    public ChangeScreenFont(float size)
    {
        this.size = size; 
    }
}


public struct ScreenMainTextMessage 
{
    public MainTextBox instance;
    public ScreenMainTextMessage(MainTextBox instance)
    {
        this.instance = instance;
    }
}