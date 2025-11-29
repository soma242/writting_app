using System;
using System.Collections.Generic;
using System.Text;

namespace writting_app;

public static class GlobalScreenIndex
{
    private static object gate = new object();

    internal static IndexList indexList = new IndexList();

    public static int GetScreenIndex()
    {
        return indexList.Add(); 
    } 

    public static void RemoveScreen(int index)
    {
        indexList.Remove(index);
    }

}
