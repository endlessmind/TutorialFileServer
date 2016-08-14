using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Forms;
using TutorialFileServer.Classes;
using TutorialFileServer.Server;
using TutorialFileServer.Server.Classes;

namespace TutorialFileServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string selectedPath = "C:\\Windows\\System32";
        FolderBrowserDialog fbd = new FolderBrowserDialog();
        //List<FileData> fileList = new List<FileData>();
        IPAddress selectedIP;
        TCPServer server;

        public MainWindow()
        {
            InitializeComponent();
        }


        private void startServer()
        {
            server.Start(selectedIP);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                selectedPath = fbd.SelectedPath;
            }

            Console.WriteLine("Selected: " + selectedPath);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            comboBox.ItemsSource = TCPServer.getAvailableNetworkAdapters();
            server = new TCPServer();
            server.OnNewClientConnected += new TCPServer.NewClientHandler(Server_ClientConnected);
        }


        private void Client_OnError(object sender, EventArgs e)
        {
            System.Windows.MessageBox.Show("Something went wrong!");
        }

        private void Client_OnSuccess(object sender, EventArgs e)
        {
            Console.WriteLine("Data sent");
        }


        private void Server_ClientConnected(object sender, ClientConnectEventArgs e)
        {
            Client c = e.ConnectedClient;
            Console.WriteLine("New client");
            c.OnSendError += new EventHandler(Client_OnError);
            c.OnSendSuccess += new EventHandler(Client_OnSuccess);
            c.OnClientReceive += new Client.NewClientHandler(Client_OnClientReceived);
        }

        private void Client_OnClientReceived(object sender, ClientMsgReceivedEventArgs e)
        {
            Console.Write("Client sent:");
            if (e.State.dataTypeRecived == App.TYPE_CMD)
            {
                //CMD
                JavaScriptSerializer requestDeserializer = new JavaScriptSerializer();
                string requestString = Encoding.UTF8.GetString(e.State.buffer);
                BaseItem data = requestDeserializer.Deserialize<BaseItem>(requestString);
                Console.WriteLine(data.Request);
               


                if (data.Request == App.CMD_REQUEST_FILES)
                {
                    IDictionary<string, object> DirctionaryData = (dynamic)data.Data as IDictionary<string, object>;
                    string folderPath = selectedPath + (DirctionaryData["folder"] as string).Replace("root", ""); //exemple: root\\pictures\\May2k16\\ -> \\pictures\\May2k16\\

                    BaseItem bi = new BaseItem();
                    bi.Request = App.CMD_REQUEST_FILES_RESPONSE;
                    FileListDataItem fldi = new FileListDataItem();
                    fldi.allFiles = Utils.getFiles(folderPath);
                    bi.Data = fldi;

                    JavaScriptSerializer serial = new JavaScriptSerializer();
                    e.ConnectedClient.SendCMD(serial.Serialize(bi));
                    //Console.WriteLine(serial.Serialize(bi));
                } else if (data.Request == App.CMD_REQUEST_FILE_DOWNLOAD)
                {
                    //Send file to client
                    IDictionary<string, object> DirctionaryData = (dynamic)data.Data as IDictionary<string, object>;
                    string fileName = DirctionaryData["filePath"] as string;
                    byte[] fileByte = File.ReadAllBytes(fileName);
                    Console.WriteLine("Client requested file: " + fileName);
                    e.ConnectedClient.SendData(fileByte);
                }


            } else if (e.State.dataTypeRecived == App.TYPE_DATA) //This won't be used in this example, just to show how it should be done.
            {
                //Data, save to file
                Console.WriteLine(e.State.buffer.Length + " bytes");
                File.WriteAllBytes("snopp.jpg", e.State.buffer); //Saved the file in the same directory as the exe-file is in. Need full path to save it somewhere else
            }
        }

        private void comboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (selectedIP == null)
                button1.IsEnabled = true; //Was null, will be set

            selectedIP = (IPAddress)comboBox.SelectedItem;

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (server.isRunning)
            {
                new Thread(server.Stop).Start();
                button1.Content = "Start";
            } else
            {
                new Thread(startServer).Start();
                button1.Content = "Stop";
            }
        }
    }
}
