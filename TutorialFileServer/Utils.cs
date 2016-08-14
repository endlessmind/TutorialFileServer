using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TutorialFileServer.Classes;

namespace TutorialFileServer
{
    class Utils
    {

        public static List<FileData> getFiles(String path)
        {
            List<FileData> fileList = new List<FileData>();
            string[] files = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);

            foreach (string s in dirs)
            {
                DirectoryInfo di = new DirectoryInfo(s);
                FileData fa = new FileData(di.FullName, -1, di.Name, 2);
                fileList.Add(fa);
            }

            foreach (string s in files)
            {
                FileInfo fi = new FileInfo(s);
                FileData fa = new FileData(fi.FullName, fi.Length, fi.Name);
                fileList.Add(fa);
            }

            return fileList;
        }
    }
}
