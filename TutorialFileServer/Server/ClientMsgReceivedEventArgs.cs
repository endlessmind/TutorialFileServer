using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TutorialFileServer.Server
{
    class ClientMsgReceivedEventArgs
    {

        public Client ConnectedClient { get; private set; }
        public Client.StateObject State { get; private set; }

        public ClientMsgReceivedEventArgs(Client c, Client.StateObject state)
        {
            ConnectedClient = c;
            State = state;
        }

    }
}
