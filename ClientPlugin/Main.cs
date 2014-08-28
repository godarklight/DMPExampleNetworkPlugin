using System;
using UnityEngine;
using DarkMultiPlayer;
using MessageStream;

namespace ClientPlugin
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class Main : MonoBehaviour
    {
        private float lastPingTime;

        public void Awake()
        {
            GameObject.DontDestroyOnLoad(this);
            DMPModInterface.fetch.RegisterRawModHandler("ExamplePingMod", HandlePingModMessage);
        }

        public void Update()
        {
            if (Client.fetch.gameRunning)
            {
                if ((UnityEngine.Time.realtimeSinceStartup - lastPingTime) > 5f)
                {
                    DarkLog.Debug("Sending ping message");
                    lastPingTime = UnityEngine.Time.realtimeSinceStartup;

                    long currentTime = DateTime.UtcNow.Ticks;
                    //Make high priority payload
                    byte[] highData = new byte[9];
                    BitConverter.GetBytes(true).CopyTo(highData, 0);
                    BitConverter.GetBytes(currentTime).CopyTo(highData, 1);
                    //Make low priority payload
                    byte[] lowData = new byte[9];
                    BitConverter.GetBytes(false).CopyTo(lowData, 0);
                    BitConverter.GetBytes(currentTime).CopyTo(lowData, 1);
                    //Send the messages
                    DMPModInterface.fetch.SendDMPModMessage("ExamplePingMod", highData, false, true);
                    DMPModInterface.fetch.SendDMPModMessage("ExamplePingMod", lowData, false, false);
                }
            }
        }

        private void HandlePingModMessage(byte[] messageData)
        {
            using (MessageReader mr = new MessageReader(messageData, false))
            {
                bool highPriority = mr.Read<bool>();
                long clientSendTime = mr.Read<long>();
                long clientReceiveTime = DateTime.UtcNow.Ticks;
                //Dividing by 10,000 converts ticks to milliseconds
                double latency = (clientReceiveTime - clientSendTime) / 10000f;
                DarkLog.Debug("Successfully received ping, high priority: " + highPriority + ", latency: " + Math.Round(latency, 3) + "ms.");
            }
        }
    }
}

