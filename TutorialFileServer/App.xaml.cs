using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace TutorialFileServer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static int TYPE_CMD = 1;
        public static int TYPE_DATA = 2;

        //Request accepted by the server from this type of client
        public static String CMD_REQUEST_FILES = "server_get_files";
        public static String CMD_REQUEST_FILES_RESPONSE = "server_get_files_response";
        public static String CMD_REQUEST_FILE_DOWNLOAD = "server_download_file";
        public static String CMD_PUT_DATA = "server_add_this_to_database";
    }
}
