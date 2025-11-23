using MessagePack;
using System;

namespace writting_app;

[MessagePackObject]
public class NodeData
{
	[Key(0)]
	public string path { get; set; }

	//[Key(1)]
	//public NodeKind kind { get; set; }

	public NodeData(string path)
	{
		this.path = path;
		//this.kind = kind;
	}
}


//ファイルを分けて必要分だけLazyで読み込む形式にする。enumはひつようない
/*
//MessagePackを通すのでenumは必ず値を固定する。　= (int)
public enum NodeKind
{
	

}

//treeNodeの中で内部にNodeを持つものは別に用意してそこにforeachで追加していく。
*/