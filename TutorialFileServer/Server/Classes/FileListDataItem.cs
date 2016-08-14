using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TutorialFileServer.Classes;

namespace TutorialFileServer.Server.Classes
{
    class FileListDataItem
    {
        List<FileData> files = new List<FileData>();


        public List<FileData> allFiles
        {
            get
            {
                return files;
            }
            set
            {
                files = value;
            }
        }
        
    }
}
