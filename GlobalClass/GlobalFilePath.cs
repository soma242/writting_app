using MemoryPack;
using PublishStructure;
using PublishStructure.Internal;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using writting_app.MessageInstance;



namespace writting_app;

public static class GlobalFilePath
{
    //workNameのgate。　alignmentIndexについてはIndexListの実装でgateがついている。
    public static Object gate = new Object();
    public static GlobalFilePathInstance instance { get; private set; } = new GlobalFilePathInstance();

    public static string docPath { get { return instance.docPath; } }

    public static string workDocPath { get { return instance.workDocPath; } }

    public static string mainTextsCashePath = "mainTexts.bin";
    public static string flagsCashePath = "flags.bin";

    //mainTextsDirectory内の個別binファイルやディレクトリ作成で扱う
    public static string mainTexts = "mainTexts";
    public static string flags = "flags";
    public static string binIdentifier = ".bin";
    public static string manager = "manager";

    public static int alignmentIndex {  get { return instance.alignmentIndex; } }

    public static string preWorkDocPath
    {
        get
        {
            lock (gate)
            {
                return instance.preWorkDocPath;
            }
        }
    }
    public static string workName
    {
        get
        {
            lock (gate)
            {
                return instance.workName;
            }
        }
    }

    /*
    public static void CreateNodeCache(string tempName)
    {
        lock (gate) {
            instance.CreateNodeCache(tempName);
        }
    }
    */

    public static void SetWorkName(string workName)
    {
        lock (gate)
        {
            instance.SetWorkName(workName);
        }
    }

    public static void RemoveAlignment(int index)
    {
        instance.RemoveAlignment(index);
    }

}

/// <summary>
/// ///////////////////////////////////
/// </summary>
public class GlobalFilePathInstance
{
    public string docPath;
    public string workName;
    public string workDocPath;

    public string preWorkDocPath;


    public int alignmentIndex { get => indexList.Add(); }

    //alignmentPanelに対して空いているインデックスを割り当て、その値をkeyにして子Controlに対して操作を行う。
    internal IndexList indexList;




    public void RemoveAlignment(int index) {
        indexList.Remove(index);
    }

    public GlobalFilePathInstance()
    {
        indexList = new IndexList();

        string temp = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        //Path.Combineは結合する文字列の間に自動で/を挿入する /があると前が消える
        docPath = Path.Combine(temp, "writting_app");
        workName = "";
    }

    

    public void SetWorkName(string workName)
    {
        //null対応、完全一致の確認
        if(string.Equals(this.workName, workName, StringComparison.Ordinal))
        {
            return;
        }

        //現在のworkDocPath
        preWorkDocPath = this.workDocPath;
        //名前の変更
        this.workName = workName;
        //preWorkDocPathとはちゃんと分離されている。
        workDocPath = Path.Combine(docPath, workName);

        var pub = GlobalMessagePipe.GetPublisher<WorkChangedMessage>();
        pub.Publish(new WorkChangedMessage());


        //workが変更されたと通知

        //テスト用
        /*
        var pub = GlobalMessagePipe.GetPublisher<string>();
        pub.Publish(this.preWorkDocPath);
        pub.Publish(this.workName);
        */
    }

    public void CheckDeletedWork(string workName)
    {
        if (string.Equals(this.workName, workName, StringComparison.Ordinal))
        {
            workName = "";
        }
    }
}