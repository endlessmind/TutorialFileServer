using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TutorialFileServer.Server
{
    class ClientConnectEventArgs : EventArgs
    {
        public Client ConnectedClient { get; private set; }

        public ClientConnectEventArgs(Client c)
        {
            ConnectedClient = c;
        }
    }
}
