using System;
using DarkMultiPlayerServer;

namespace ServerPlugin
{
    public class Main : DMPPlugin
    {
        public Main()
        {
            DMPModInterface.RegisterModHandler("ExamplePingMod", HandleMessage);
        }

        public void HandleMessage(ClientObject client, byte[] messageData)
        {
            //Relay it back to where we got the message from.
            bool highPriority = BitConverter.ToBoolean(messageData, 0);
            DMPModInterface.SendDMPModMessageToClient(client, "ExamplePingMod", messageData, highPriority);
        }
    }
}

