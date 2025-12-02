using System;
using System.Collections.Generic;
using System.Text;

namespace writting_app;


//インターフェイスの静的抽象メンバー
public interface INodeEnum
{
    public abstract string GetEnumString();
    public string GetEnumCasheString();

}

public readonly struct MainTextsEnum : INodeEnum
{
    public string GetEnumString()
    {
        return GlobalFilePath.mainTexts; 
    }

    public string GetEnumCasheString()
    {
        return GlobalFilePath.mainTextsCashePath;
    }

}

//test

public struct TestString {
    public string test = GlobalFilePath.mainTexts;
    public TestString(string test)
    {
        this.test = test;
    }
}
