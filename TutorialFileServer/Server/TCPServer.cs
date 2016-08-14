using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TutorialFileServer.Server
{
    class TCPServer
    {

        List<Client> connectedClients = new List<Client>();

        public delegate void NewClientHandler(object sender, ClientConnectEventArgs e);
        public event NewClientHandler OnNewClientConnected;

        Socket serverSock;
        bool listening = false;
        public static ManualResetEvent allDone = new ManualResetEvent(false);



        public TCPServer()
        {
        }


        public bool isRunning
        {
            get { return listening; }
        }

        public static IPAddress[] getAvailableNetworkAdapters()
        {
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            return ipHostInfo.AddressList;
        }

        public void Start(IPAddress ip)
        {
            IPEndPoint ipep = new IPEndPoint(ip, 7462);

            serverSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSock.Bind(ipep);

            Console.WriteLine("Server is started!");
            serverSock.Listen(100);
            listening = true;
            while (listening)
            {
                allDone.Reset();
                serverSock.BeginAccept(Accept, serverSock);
                allDone.WaitOne();
            }


        }

        public void Stop()
        {
            listening = false;
            allDone.Set();
            serverSock.Close();

            Console.WriteLine("Server is stoped!");
        }

        private void Accept(IAsyncResult ar)
        {
            if (!listening)
                return;

            allDone.Set();

            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            Client c = new Client();
            c.ClientSocket = handler;

            GC.Collect();



            if (OnNewClientConnected == null) return;
            ClientConnectEventArgs args = new ClientConnectEventArgs(c);
            OnNewClientConnected(this, args);

        }
    }
}
