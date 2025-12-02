using MemoryPack;
using PublishStructure;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using writting_app.MessageInstance;

namespace writting_app;

public static class GlobalMainTextsCashes
{
    public static object gate = new object();

    public static ScreenDataCashes<MainTextsEnum>instance = new ScreenDataCashes<MainTextsEnum>();

    public static Span<NodeData> mainTextsSpan
    {
        get
        {
            lock (gate)
            {
                return CollectionsMarshal.AsSpan(instance.nodes);
            }
        }
    }

    public static List<NodeData> mainTexts
    {
        get
        {
            lock (gate)
            {
                return instance.nodes;
            }
        }
    }

    public static void AddNode(string text)
    {
        lock (gate)
        {
            instance.AddNode(text);
        }
    }

    public static void RemoveNode(int index) 
    {
        lock (gate)
        {
            instance.RemoveNode(index);
        }
    }
    
}

/*
public class MainTextsCashe 
{


    //外からRemoveなどされたくない(自作メソッドからのみ操作したい)
    public List<MainTextNode> mainTexts;

    //from_NodeClass
    private IdIndexManager manager;

    private IDisposable disposableChanged;

    private bool changed;

    public MainTextsCashe() 
    {
        //manager = new IdIndexManager();
        ReadData();

        var sub = GlobalMessagePipe.GetSubscriber<WorkChangedMessage>();
        disposableChanged = sub.Subscribe(get =>
        {
            SaveData();
            ReadData();
            changed = false;
        });
    }

    ~MainTextsCashe()
    {
        disposableChanged?.Dispose();
    }

    private void ReadData()
    {
        lock (GlobalMainTextsCashes.gate)
        {
            ReadMainTextsCashe();
            ReadManagerCashe();
        }

    }

    private void SaveData()
    {
        lock (GlobalMainTextsCashes.gate)
        {
            SaveNodeCache();
            SaveManagerCache();
        }

    }

    public void ReadMainTextsCashe()
    {
        var temp = Path.Combine(GlobalFilePath.workDocPath, GlobalFilePath.mainTextsCashePath);
        if (new FileInfo(temp).Length < 1)
        {
            mainTexts = new List<MainTextNode>();

            var pub = GlobalMessagePipe.GetPublisher<string>();
            pub.Publish("Error, ファイル内にデータが存在しない");
            return;
        }
        //ファイルが巨大になる展望はいまのところないのでReadAllBytesを使う。必要になったらその部分だけpipeReaderなどで最適化するかも
        //byte[] bytes = File.ReadAllBytes(Path.Combine(docPath, workName, mainTextCashePath));
        byte[] bytes = File.ReadAllBytes(temp);
        MemoryPackSerializer.Deserialize<List<MainTextNode>>(bytes, ref mainTexts);




    }

    public void ReadManagerCashe()
    {
        //manager
        var temp = Path.Combine(GlobalFilePath.workDocPath, GlobalFilePath.manager, GlobalFilePath.mainTextsCashePath);
        if (new FileInfo(temp).Length < 1)
        {
            manager = new IdIndexManager();

            return;
        }
        byte[] bytes = File.ReadAllBytes(temp);
        MemoryPackSerializer.Deserialize<IdIndexManager>(bytes, ref manager);
    }

    //test




    public void SaveNodeCache()
    {
        string tempPath = Path.Combine(GlobalFilePath.workDocPath,GlobalFilePath.mainTextsCashePath);
        using (var fs = File.OpenWrite(tempPath))
        {

            byte[] bytes = MemoryPackSerializer.Serialize(mainTexts, MemoryPackSerializerOptions.Utf16);
            fs.Write(bytes, 0, bytes.Length);
        }
    }
    public void SaveManagerCache()
    {
        string tempPath = Path.Combine(GlobalFilePath.workDocPath, GlobalFilePath.manager, GlobalFilePath.mainTextsCashePath);
        using (var fs = File.OpenWrite(tempPath))
        {

            byte[] bytes = MemoryPackSerializer.Serialize(manager, MemoryPackSerializerOptions.Utf16);
            fs.Write(bytes, 0, bytes.Length);
        }
    }

    public void AddNode(string text)
    {
        //Combineだと自動的に/で区切られるので、binIdentifierを使うときは必ず + 
        string tempPath = Path.Combine(GlobalFilePath.workDocPath, GlobalFilePath.mainTexts, text + GlobalFilePath.binIdentifier);
        var pub = GlobalMessagePipe.GetPublisher<string>();
        pub.Publish($"{tempPath}");
        if (File.Exists(tempPath))
        {
            MessageBox.Show("そのファイルはすでに存在しています", "エラー",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        else
        {
            using (var fs = File.Create(tempPath))
            {
                //空のファイルを作成

            }
        }

        //indexを現在のindex+1として渡す(count)
        int count = mainTexts.Count;
        //その要素のidを受け取る(削除で空いたか新規)
        int id = manager.GetNewId();
        //Listに追加
        mainTexts.Add(new MainTextNode(manager.GetNewId(), text));
        //managerに追加
        manager.SetNewPair(id, count);

        var alignePub = GlobalMessagePipe.GetPublisher<AligneListChanged>();
        alignePub.Publish(new AligneListChanged());
        changed = true;
    }

    public void RemoveNode(int index) {
        //indexを受け取って削除する。重い
        //idを要素から受け取る、List自身から削除、Edge1(Dictionary<int, List<int>>)からつながりを持つ相手をidで検索（spanとして受け取る）
        // => foreachで対象からのつながりEdge1'を削除(相手のidで検索してListからRemove)、終了後にEdge1削除。自分の持つEdgeの数だけ繰り返す。
        // => =>indexを使ってRemoveAt、managerのdeletedIdに追加、indexを用いてSpanに切り出してforeachとidでDictionary<int, int>の対応する要素のindexを-1していく。

        int id = mainTexts[index].id;
        string text = mainTexts[index].text;

        mainTexts.RemoveAt(index);
        manager.deletedId.Add(id);

        int nodeIndex = index;

        var span = CollectionsMarshal.AsSpan(mainTexts);
        span.Slice(index, span.Length - 1);

        foreach (var afterNode in span)
        {
            manager.idIndexPair[nodeIndex] = afterNode.id;
            nodeIndex++;
        }

        File.Delete(Path.Combine(GlobalFilePath.workDocPath, GlobalFilePath.mainTexts, text + GlobalFilePath.binIdentifier));

        //mainからつながっているedgePairの一覧
        foreach(var edgePair in GlobalEdgeControler.fromMainEdgeList)
        {
            //対応するedge内で自分につながりを持つidを獲得
            var edgesSpan = edgePair.main.GetValue(id);
            //edge => int
            foreach(int edgeId in edgesSpan)
            {
                //相手側のedges(Dictionary<int, List<int>>)
                //edgeId(key: List<int>を検索)からid(int)を取り除く
                edgePair.pair.RemoveValue(edgeId, id);
            } 

            //相手側から取り除き終わったedgesからedgeごと取り除く
            edgePair.main.RemoveEdge(id);
        }

        var alignePub = GlobalMessagePipe.GetPublisher<AligneListChanged>();
        alignePub.Publish(new AligneListChanged());
        changed = true;
    }
}

*/