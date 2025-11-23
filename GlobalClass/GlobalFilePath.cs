using System;
using System.Collections.Generic;
using System.Text;

using MessagePack;
using PublishStructure;

namespace writting_app;

public static class GlobalFilePath
{
    public static Object gate = new Object();
    public static GlobalFilePathInstance instance { get; private set; } = new GlobalFilePathInstance();

    public static string docPath { get { return instance.docPath; } }

    public static void CreateNodeCache(string tempName)
    {
        lock (gate) {
            instance.CreateNodeCache(tempName);
        }
    }

    public static void SetWorkName(string workName)
    {
        lock (gate)
        {
            instance.SetWorkName(workName);
        }
    }


}


public class GlobalFilePathInstance
    {
    public string docPath;
    public string workName;


    public string mainTextCashePath;
    public Lazy<List<NodeData>> mainTextPathCashe;
    //インライン化されるはず(ラップ)
    public List<NodeData> mainTexts => mainTextPathCashe.Value;

    

    public GlobalFilePathInstance()
    {

        string temp = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        //Path.Combineは結合する文字列の間に自動で/を挿入する
        docPath = Path.Combine(temp, "writting_app");
        workName = "";
        mainTextCashePath = "/mainTextNode.bin";
        mainTextPathCashe = new Lazy<List<NodeData>>();

    }

    public List<NodeData> ReadMainTextsCashe()
    {
        if(mainTexts.Count != 0)
            mainTexts.Clear();

        using (var fs = File.OpenRead(Path.Combine(docPath, workName, mainTextCashePath)))
        {
            var list = MessagePackSerializer.Deserialize<List<NodeData>>(fs);
            //もともとここで行っていた、リストがない場合の処理は参照時に移植する
            return list;

        }
    }



    public void CreateNodeCache(string tempName)
    {
        string tempPath = Path.Combine(docPath, tempName, mainTextCashePath);
        if (!File.Exists(tempPath))
        {
            //ファイルがなければ作成する。
            using (var fs = File.Create(tempPath))
            {
                //空のファイルを作成

            }
            //
            List<NodeData> tempList = new List<NodeData>();

            //tempList.Add(new NodeData());

            using (var fs = File.OpenWrite(tempPath))
            {
                MessagePackSerializer.Serialize(fs, tempList);
            }

        }
    }

    public void SetWorkName(string workName)
    {
        //null対応、完全一致の確認
        if(string.Equals(this.workName, workName, StringComparison.Ordinal))
        {
            return;
        }

        //名前の変更、Lazyの再生成
        this.workName = workName;



        //テスト用
        var pub = GlobalMessagePipe.GetPublisher<string>();
        pub.Publish(this.workName);
    }
}