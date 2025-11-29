using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MessagePack;
using PublishStructure;

namespace writting_app;

public static class GlobalWorkNames
{
    public static Object gate = new Object();
    public static WorkNameList instance { get; } = new WorkNameList();

    //public static string cashePath { get { return instance.cashePath; } }




    public static Span<string> workNamesAsSpan()
    {
        lock (gate)
            return CollectionsMarshal.AsSpan<string>(instance.workNames);
    }

    public static void ReflectionList(ListBox listBox)
    {
        lock (gate)
        {
            instance.ReflectionList(listBox);
        }
    }

    public static void SaveInstance()
    {
        lock (gate)
        {
            instance.SaveList();
        }
    }

    public static string ReturnValue(int index)
    {
        lock (gate)
        {
            return instance.workNames[index];
        }
    }

}

//アプリ中はこのリストに記録し、終了時に移す。
//新規作成時もちゃんと操作する。
public class WorkNameList
{
    public List<string> workNames { get; private set; }

    public string cashePath;

    public bool changed;

    //public object gate = new object();

    //GlobalWorkNamesのgateを受け取っている。
    public WorkNameList()
    {
        workNames = new List<string>();


        ListInit();
        changed = false;
        

    }


    //Listの初期化
    public void ListInit()
    {
        //string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        //一度生成したものを保持
        cashePath = Path.Combine(GlobalFilePath.docPath, "workNames.bin");

        if (!File.Exists(cashePath))
        {
            //ファイルがなければ作成する。
            using (var fs = File.Create(cashePath))
            {
                //空のファイルを作成

            }

            return;


        }else if(new FileInfo(cashePath).Length < 1)
        {
            var pub = GlobalMessagePipe.GetPublisher<string>();
            pub.Publish("Error, ファイル内にデータが存在しない");
            return;
        }

        //Read(Write)Streamは文字列の読み書きに最適化されているが、MessagePackはバイナリ形式なのでFileStreamを直接使う。
        using (var fs = File.OpenRead(cashePath))
        {
            var list = MessagePackSerializer.Deserialize<List<string>>(fs);
            if (list != null)
                workNames.AddRange(list);
        }
    }


    public void SaveList()
    {
        //初期化でfalse
        //reflectionでtrue
        if (!changed)
            return;


        using(var fs = File.OpenWrite(cashePath))
        {
            MessagePackSerializer.Serialize(fs, workNames);
        }
    }

    //ListBoxの内容を反映させる。
    //workNameSelecterが閉じられるときに呼ばれる。
    public void ReflectionList(ListBox listBox)
    {
        workNames.Clear();
        foreach (var obj in listBox.Items)
        {
            if (obj is string item)
                workNames.Add(item);
        }
        changed = true;

        //var pub = GlobalMessagePipe.GetPublisher<string>();
        //pub.Publish("reflect");
    }

}


