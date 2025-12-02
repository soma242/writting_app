using System;
using System.Collections.Generic;
using System.Text;

namespace writting_app;

public static class GlobalCreateFiles
{
    public static void CreateWorkFile(string directoryPath)
    {
        var managerDoc = Path.Combine(directoryPath, GlobalFilePath.manager);

        Directory.CreateDirectory(Path.Combine(directoryPath, GlobalFilePath.mainTexts));
        Directory.CreateDirectory(Path.Combine(directoryPath, GlobalFilePath.flags));
        Directory.CreateDirectory(managerDoc);

        var tempPath = Path.Combine(directoryPath, GlobalFilePath.mainTextsCashePath);
        using (var fs = System.IO.File.Create(tempPath))
        {
            //空のファイルを作成
        }

        tempPath = Path.Combine(managerDoc, GlobalFilePath.mainTextsCashePath);
        using (var fs = System.IO.File.Create(tempPath))
        {
            //空のファイルを作成
        }
        tempPath = Path.Combine(directoryPath, GlobalFilePath.flagsCashePath);
        using (var fs = System.IO.File.Create(tempPath))
        {
            //空のファイルを作成
        }
        tempPath = Path.Combine(managerDoc, GlobalFilePath.flagsCashePath);
        using (var fs = System.IO.File.Create(tempPath))
        {
            //空のファイルを作成
        }
    }
}
