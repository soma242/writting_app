using MemoryPack;
using PublishStructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using writting_app.MessageInstance;

namespace writting_app;

public static class GlobalEdgeControler
{
    public static EdgeControlerInstance Instance = new EdgeControlerInstance();

    //From__Main
    public static EdgePair[] fromMainEdgeList { get {  return fromMainEdgeList; }  }
    public static EdgeInstance fromMainToFlag {  get { return fromMainToFlag; } }


    //From__Flag
    public static EdgePair[] fromFlagEdgeList { get {  return fromFlagEdgeList; }  }
    public static EdgeInstance fromFlagToMain { get { return fromFlagToMain; } }


    public static void EdgeSave()
    {

    }
}

//EdgeInstanceの追加
 //EdgePairにも相互に追加
 //Global側にも追加
public class EdgeControlerInstance
{
    //Dictionaryをラップして(保存先ファイル名)stringを渡し、Lazyに。一度初期化されたなら、workの変更をSubしてそのタイミングで変更
    //変更があったならboolを変更して保存可能にd

    //private bool 
    public EdgePair[] fromMainEdgeList;
    public EdgeInstance fromMainToFlag;

    public EdgePair[] fromFlagEdgeList;
    public EdgeInstance fromFlagToMain; 

    public EdgeControlerInstance() 
    {
        //Mainから
        fromMainToFlag = new EdgeInstance("FromMain_ToFlag");

        //Flagから
        fromFlagToMain = new EdgeInstance("FromFlag_ToMain");



        //////配列
        //削除にも使うのでEdgeInstanceの組として扱う
        ////Mainから
        //ToFlag
        fromMainEdgeList = new[] { new EdgePair(fromMainToFlag, fromFlagToMain) };
        
        ////Flagから
        //ToMain
        fromMainEdgeList = new[] { new EdgePair(fromFlagToMain, fromMainToFlag) };

    }


    public void SaveAllEdge()
    {
        SaveEdge(fromMainEdgeList);
        SaveEdge(fromFlagEdgeList);

    }
    private void SaveEdge(EdgePair[] edgesList)
    {
        foreach (var edgePair in edgesList)
        {
            edgePair.main.SaveEdges();
        }
    }
}

public class EdgePair
{
    public EdgeInstance main;
    public EdgeInstance pair;

    public EdgePair(EdgeInstance main, EdgeInstance pair)
    {
        this.main = main;
        this.pair = pair;
    }
}

public class EdgeInstance
{
    //<id, 繋がっているノードのid>
    public Dictionary<int, List<int>> edges;

    private object gate = new object();

    private bool initialized = false;

    private string fileName;
    private bool changed = false;

    private IDisposable disposableSave;

    public EdgeInstance(string fileName)
    {
        this.fileName = fileName;
        initialized = false;
        changed = false;
    }

    ~EdgeInstance()
    {
        disposableSave?.Dispose();
    }

    public void Initialize()
    {
        if (initialized)
            return;

        lock (gate)
        {


            initialized = true;
            //デシリアライズ
            byte[] bytes = File.ReadAllBytes(Path.Combine(GlobalFilePath.workDocPath, fileName));
            MemoryPackSerializer.Deserialize<Dictionary<int, List<int>>>(bytes, ref edges);

            var sub = GlobalMessagePipe.GetSubscriber<WorkChangedMessage>();
            disposableSave = sub.Subscribe(get =>
            {
                //ひとつ前をアップデート、更新
                EdgeUpdate();
            });
        }
    }

    public void SaveEdges()
    {
        if(!changed) return;

        lock (gate) {
            
            var path = Path.Combine(GlobalFilePath.workDocPath, fileName);
            using (var fs = File.OpenWrite(path))
            {
                byte[] bytes = MemoryPackSerializer.Serialize(edges);
                fs.Write(bytes, 0, bytes.Length);
            }
            
        }
    }



    public void EdgeUpdate()
    {
        lock (gate)
        {
            if (changed)
            {
                //変更前のpathを持ってくる
                var path = Path.Combine(GlobalFilePath.preWorkDocPath, fileName);
                using (var fs = File.OpenWrite(path))
                {
                    byte[] savebytes = MemoryPackSerializer.Serialize(edges);
                    fs.Write(savebytes, 0, savebytes.Length);
                }


                changed = false;
            }

            //デシリアライズ
            byte[] bytes = File.ReadAllBytes(Path.Combine(GlobalFilePath.workDocPath, fileName));
            MemoryPackSerializer.Deserialize<Dictionary<int, List<int>>>(bytes, ref edges);
        }
    }

    public Span<int> GetValue(int id)
    {
        Initialize();

        lock (gate)
        {
            return CollectionsMarshal.AsSpan<int>(edges[id]);
        }
    }

    public void AddValue(int id, int value)
    {
        Initialize();
        lock (gate)
        {
            //idのedgeがないなら追加に成功
            if (!edges.TryAdd(id, new List<int>()))
            {
                changed = true;
            }
            //idのedgeがあってedge内にvalueがなければ
            //List.Containsはパフォーマンスが重いが、Spanとして獲得したいのとHashSetなどと比べてSerializeサイズは小さいはず
            //また、一つのListのサイズはそれほど大きくならないはず
            else if (!edges[id].Contains(value))
            {
                edges[id].Add(value);
                changed = true;
            }
        }
    }

    //idからedgeを特定、edge内のvalueを削除する。
    public void RemoveValue(int id, int value) { 
        Initialize();
        lock (gate)
        {
            //edges内にedgeが存在しないならreturn
            if (!edges.ContainsKey(id))
                return;
            //edgeに存在しない(remove失敗)ならfalse
            if(edges[id].Remove(value))
                changed = true;
        }
    }

    //idからEdgeごと削除する。
    public void RemoveEdge(int id) 
    {
        Initialize();
        lock (gate)
        {
            //Dictionaryにidがキーの要素があればtrue
            if (edges.Remove(id))
            {
                changed = true;
            }
        }
    }

}