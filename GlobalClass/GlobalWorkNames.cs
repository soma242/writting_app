using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace writting_app;

//アプリ中はこのリストに記録し、終了時に移す。
//新規作成時もちゃんと操作する。
public class WorkNameList
{
    public List<string> workNames { get; private set; }

    public string cashePath;

    //public object gate = new object();

    public WorkNameList(object gate)
    {
        workNames = new List<string>();

        lock (gate)
            ListInit();
    }


    public void ListInit()
    {
        string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        cashePath = Path.Combine(docPath, @"writting_app/workNames.txt");

        if (!File.Exists(cashePath))
        {
            //ファイルがなければ作成する。
            using (var fs = File.Create(cashePath))
            {
                //空のファイルを作成
            }
            return;
        }

        using (var reader = new StreamReader(cashePath, Encoding.UTF8))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    workNames.Add(line);
                }
            }
        }
    }

    public void RelflectionList(ListBox listBox)
    {
        workNames.Clear();
        foreach (var obj in listBox.Items)
        {
            if (obj is string item)
                workNames.Add(item);
        }
    }

}


public static class GlobalWorkNames
{
    public static Object gate = new Object();
    public static WorkNameList instance { get; } = new WorkNameList(gate);

    public static string cashePath { get { return instance.cashePath; } }




    public static Span<string> workNamesAsSpan()
    {
        lock (gate)
            return CollectionsMarshal.AsSpan<string>(instance.workNames);
    }

    public static void ReflectionList(ListBox listBox)
    {
        lock (gate)
        {
            instance.RelflectionList(listBox);
        }
    }

    public static void SaveInstance()
    {
        lock (gate)
        {
            using (StreamWriter sw = new StreamWriter(cashePath))
            {
                foreach (var item in workNamesAsSpan())
                {
                    sw.WriteLine(item);
                }
            }
        }
    }

}