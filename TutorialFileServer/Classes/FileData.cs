using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TutorialFileServer.Classes
{
    class FileData
    {

        public FileData(string json)
        {
            JObject obj = JObject.Parse(json);
            Path = (string)obj["Path"];
            Size = (long)obj["Size"];
            FileName = (string)obj["FileName"];
            Type = (int)obj["Type"];
        }

        public FileData(string fPath, long fSize, string name)
        {
            Size = fSize;
            Path = fPath;
            FileName = name;
            Type = 1; //File
        }

        public FileData(string fPath, long fSize, string name, int ft)
        {
            Size = fSize;
            Path = fPath;
            FileName = name;
            Type = ft;
        }

        public long Size
        {
            get; set;
        }

        public string Path
        {
            get; set;
        }

        public string FileName
        {
            get; set;
        }

        public int Type //1 = file, 2 = folder
        {
            get; set;
        }

    }
}
