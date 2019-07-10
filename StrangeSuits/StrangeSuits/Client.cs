using System;
using System.Net;
using System.Collections.Generic;
using System.Windows.Forms;

using Lidgren.Network;
using NetworkCommon;

namespace StrangeSuits
{
    static class Client
    {
        private static Lobby lobby;
        private static Dictionary<long, IPEndPoint[]> hostList;
        private static long host;
        private static float lastRefreshed;
        private static int count;

        [STAThread]
        public static void MainClient()
        {
            Application.EnableVisualStyles();
            lobby = new Lobby();
            
            hostList = new Dictionary<long, IPEndPoint[]>();
            SSEngine.MasterServerEndpoint = new IPEndPoint(NetUtility.Resolve("localhost"), SSEngine.MasterServerPort);
            count = 0;
            lastRefreshed = 0.0f;
            SSEngine.IsHost = null;

            NetPeerConfiguration config = new NetPeerConfiguration("strange suits");
            config.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            config.EnableMessageType(NetIncomingMessageType.NatIntroductionSuccess);
            SSEngine.Peer = new NetClient(config);
            SSEngine.Peer.Start();

            lobby.StartUpdate();
            lobby.ShowDialog();
        }

        public static void ClientUpdate(object sender, EventArgs e)
        {
            if (NetTime.Now > lastRefreshed + 5.0)
            {
                GetServerList();
                lastRefreshed = (float)NetTime.Now;
            }
            NetIncomingMessage inc;
            while ((inc = SSEngine.Peer.ReadMessage()) != null)
            {
                switch (inc.MessageType)
                {
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        NativeMethods.AppendText(lobby.richTextBox1, inc.ReadString());
                        break;
                    case NetIncomingMessageType.UnconnectedData:
                        if (inc.SenderEndPoint.Equals(SSEngine.MasterServerEndpoint))
                        {
                            // it's from the master server - must be a host
                            if (inc.ReadBoolean())
                            {
                                hostList.Clear();
                                lobby.comboBox1.Items.Clear();
                                lobby.comboBox1.Text = "";
                                break;
                            }
                            var id = inc.ReadInt64();
                            var hostInternal = inc.ReadIPEndPoint();
                            var hostExternal = inc.ReadIPEndPoint();

                            hostList[id] = new IPEndPoint[] { hostInternal, hostExternal };

                            // update combo box
                            lobby.comboBox1.Items.Clear();
                            foreach (var kvp in hostList)
                                lobby.comboBox1.Items.Add(kvp.Key.ToString() + " (" + kvp.Value[1] + ")");
                        }
                        break;
                    case NetIncomingMessageType.NatIntroductionSuccess:
                        count += 1;
                        if (count == 2 && SSEngine.Peer.ConnectionsCount == 0)
                        {
                            SSEngine.Peer.Connect(hostList[host][1]);
                            count = 0;
                        }
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        switch (inc.SenderConnection.Status)
                        {
                            case NetConnectionStatus.Connected:
                                NetOutgoingMessage conMsg = SSEngine.Peer.CreateMessage();
                                conMsg.Write((byte)ConnectionMessageType.Name);
                                conMsg.Write(SSEngine.Peer.UniqueIdentifier);
                                SSEngine.Peer.SendMessage(conMsg, SSEngine.Peer.Connections[0], NetDeliveryMethod.ReliableOrdered);
                                SSEngine.IsHost = false;
                                Server.MainServer();
                                lobby.Close();
                                break;
                            case NetConnectionStatus.Disconnected:
                                lobby.Close();
                                break;
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        switch ((ConnectionMessageType)inc.ReadByte())
                        {
                            case ConnectionMessageType.Name:
                                Server.ConForm.tbxHost.Text = inc.ReadInt64().ToString();
                                break;
                            case ConnectionMessageType.Checkbox:
                                Server.ConForm.cbHost.Checked = inc.ReadBoolean();
                                break;
                        }
                        break;
                }
            }
        }

        public static void GetServerList()
        {
            //
            // Send request for server list to master server
            //
            NetOutgoingMessage listRequest = SSEngine.Peer.CreateMessage();
            listRequest.Write((byte)MasterServerMessageType.RequestHostList);
            SSEngine.Peer.SendUnconnectedMessage(listRequest, SSEngine.MasterServerEndpoint);
        }

        public static void RequestConnection(long hostid)
        {
            if (hostid == 0)
            {
                MessageBox.Show("Select a host in the list first");
                return;
            }

            if (SSEngine.MasterServerEndpoint == null)
                throw new Exception("Must connect to master server first!");

            host = hostid;
            NetOutgoingMessage outMsg = SSEngine.Peer.CreateMessage();
            outMsg.Write((byte)MasterServerMessageType.RequestIntroduction);

            // write my internal ipendpoint
            IPAddress mask;
            outMsg.Write(new IPEndPoint(NetUtility.GetMyAddress(out mask), SSEngine.Peer.Port));

            // write requested host id
            outMsg.Write(hostid);

            // write token
            outMsg.Write("card");

            SSEngine.Peer.SendUnconnectedMessage(outMsg, SSEngine.MasterServerEndpoint);
        }

        public static void Host()
        {
            SSEngine.IsHost = true;
            SSEngine.Peer = null;
            Server.MainServer();
            lobby.Close();
        }

        public static void ClientChecked()
        {
            NetOutgoingMessage msg = SSEngine.Peer.CreateMessage();
            msg.Write((byte)ConnectionMessageType.Checkbox);
            msg.Write(Server.ConForm.cbClient.Checked);
            SSEngine.Peer.SendMessage(msg, SSEngine.Peer.Connections[0], NetDeliveryMethod.ReliableOrdered);
        }

        public static void Disconnect()
        {
            SSEngine.Peer.Shutdown("disconnect");
        }
    }
}