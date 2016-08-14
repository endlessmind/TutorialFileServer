using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TutorialFileServer.Server
{
    class Client
    {

        public delegate void NewClientHandler(object sender, ClientMsgReceivedEventArgs e);
        public event NewClientHandler OnClientReceive;
        public event EventHandler OnSendError;
        public event EventHandler OnSendSuccess;
        //Outgoing
        byte[] dataToSend;
        int dataTypeToSend;
        //Incoming
        int dataReciveLength;

        public class StateObject
        {
            public Socket workSocket = null;
            public const int BufferSize = 1024;
            public byte[] buffer = new byte[BufferSize];
            public int dataTypeRecived;
        }

        private Socket mSock;
        public Client()
        {

        }

        public void SendCMD(String json)
        {
            dataTypeToSend = 1;
            byte[] byteData = Encoding.UTF8.GetBytes(json);
            dataToSend = byteData;
            startSending();
        }

        public void SendData(byte[] data)
        {
            dataTypeToSend = 2;
            dataToSend = data;
            startSending();
        }

        private void startSending()
        {
            Console.WriteLine("Sending length");
            Console.WriteLine("Val:" + dataToSend.Length);
            byte[] data = BitConverter.GetBytes(dataToSend.Length);

            SocketAsyncEventArgs sae = new SocketAsyncEventArgs();
            sae.SetBuffer(data, 0, data.Length);
            sae.Completed += new EventHandler<SocketAsyncEventArgs>(sendDataType);

            bool completedAsync = false;

            try
            {
                completedAsync = mSock.SendAsync(sae);
            }
            catch (SocketException se)
            {
                Console.WriteLine("Socket Exception: " + se.ErrorCode + " Message: " + se.Message);
                CallErrorEvent();
            }


        }

        private void sendDataType(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                byte[] data = BitConverter.GetBytes(dataTypeToSend);

                SocketAsyncEventArgs sae = new SocketAsyncEventArgs();
                sae.SetBuffer(data, 0, data.Length);
                sae.Completed += new EventHandler<SocketAsyncEventArgs>(sendData);

                bool completedAsync = false;

                try
                {
                    completedAsync = mSock.SendAsync(sae);
                }
                catch (SocketException se)
                {
                    Console.WriteLine("Socket Exception: " + se.ErrorCode + " Message: " + se.Message);
                }

            } else
            {
                CallErrorEvent();
            }
        }

        private void sendData(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                try
                {
                    StateObject state = new StateObject();
                    state.workSocket = mSock;
                    state.buffer = new byte[dataToSend.Length];

                    mSock.BeginSend(dataToSend, 0, dataToSend.Length, 0, new AsyncCallback(SendCallback), state);
                }
                catch (SocketException se)
                {
                    Console.WriteLine("Socket Exception: " + se.ErrorCode + " Message: " + se.Message);
                }

            } else
            {
                CallErrorEvent();
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            if (ar.IsCompleted)
            {
                if (this.OnSendSuccess != null)
                {
                    EventArgs ea = new EventArgs();
                    this.OnSendSuccess(this, ea);
                    
                    handler.EndSend(ar);
                }
            } else
            {
                CallErrorEvent();
            }
        }

        private void CallErrorEvent()
        {
            if (this.OnSendError != null)
            {
                EventArgs ea = new EventArgs();
                this.OnSendError(this, ea);
            }
        }


        private void ReadLength(IAsyncResult ar)
        {
            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            // Read data from the client socket. 
            int bytesRead = handler.EndReceive(ar);
            if (bytesRead > 0)
            {
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(state.buffer);

                dataReciveLength = BitConverter.ToInt32(state.buffer, 0);
                state.buffer = new byte[sizeof(int)];
                handler.BeginReceive(state.buffer, 0, sizeof(int), 0, new AsyncCallback(ReadDataType), state);
            }
        }

        private void ReadDataType(IAsyncResult ar)
        {
            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            // Read data from the client socket. 
            int bytesRead = handler.EndReceive(ar);
            if (bytesRead > 0)
            {
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(state.buffer);

                state.dataTypeRecived = BitConverter.ToInt32(state.buffer,0);
                state.buffer = new byte[dataReciveLength];
                handler.BeginReceive(state.buffer, 0, dataReciveLength, 0, new AsyncCallback(ReadData), state);
            }
        }

        private void ReadData(IAsyncResult ar)
        {
            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            // Read data from the client socket. 
            int bytesRead = handler.EndReceive(ar);
            //We've read more than a int
            if (bytesRead > sizeof(int))
            {
                //There is more than an int left
                if (handler.Available > sizeof(int))
                {
                    state.buffer = new byte[dataReciveLength];
                    handler.BeginReceive(state.buffer, 0, dataReciveLength, 0, new AsyncCallback(ReadData), state);
                } else
                {
                    //All done, call event and start over
                    ClientMsgReceivedEventArgs args = new ClientMsgReceivedEventArgs(this, state);
                    this.OnClientReceive(this, args);

                    state.buffer = new byte[sizeof(int)];
                    handler.BeginReceive(state.buffer, 0, sizeof(int), 0, new AsyncCallback(ReadLength), state);
                }
            }
        }

        private void StartReceive()
        {
            StateObject state = new StateObject();
            state.workSocket = mSock;
            state.buffer = new byte[sizeof(int)];
            mSock.BeginReceive(state.buffer, 0, sizeof(int), 0, new AsyncCallback(ReadLength), state);
        }

        public Socket ClientSocket
        {
            get
            {
                return mSock;
            }
            set
            {
                mSock = value;
                StartReceive();
            }
        }

    }
}
