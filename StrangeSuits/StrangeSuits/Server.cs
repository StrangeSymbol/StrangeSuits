using System;
using System.Diagnostics;
using System.Net;
using System.Collections.Generic;
using System.Windows.Forms;

using Lidgren.Network;
using NetworkCommon;

namespace StrangeSuits
{
    static class Server
    {
        public static NetworkConnector ConForm;
        [STAThread]
        public static void MainServer()
        {
            Application.EnableVisualStyles();
            ConForm = new NetworkConnector();
            // The player pressed host.
            if (SSEngine.Peer == null)
            {
                NetPeerConfiguration config = new NetPeerConfiguration("strange suits");
                config.SetMessageTypeEnabled(NetIncomingMessageType.NatIntroductionSuccess, true);
                SSEngine.Peer = new NetServer(config);
                SSEngine.Peer.Start();
                Debug.WriteLine("Server started; waiting 2 seconds...");
                System.Threading.Thread.Sleep(2000);

                // register with master server
                NetOutgoingMessage regMsg = SSEngine.Peer.CreateMessage();
                regMsg.Write((byte)MasterServerMessageType.RegisterHost);
                IPAddress mask;
                IPAddress adr = NetUtility.GetMyAddress(out mask);
                regMsg.Write(SSEngine.Peer.UniqueIdentifier);
                regMsg.Write(new IPEndPoint(adr, SSEngine.Peer.Port));
                Debug.WriteLine("Sending registration to master server");
                SSEngine.Peer.SendUnconnectedMessage(regMsg, SSEngine.MasterServerEndpoint);
                ConForm.tbxHost.Text = SSEngine.Peer.UniqueIdentifier.ToString();
                ConForm.StartUpdate();
            }
            else
            {
                ConForm.tbxClient.Text = SSEngine.Peer.UniqueIdentifier.ToString();
            }
            ConForm.ShowDialog();
        }

        public static void ServerUpdate(object sender, EventArgs e)
        {
            NetIncomingMessage inc;
            while ((inc = SSEngine.Peer.ReadMessage()) != null)
            {
                switch (inc.MessageType)
                {
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.UnconnectedData:
                        Debug.WriteLine(inc.ReadString());
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        switch (inc.SenderConnection.Status)
                        {
                            case NetConnectionStatus.Connected:
                                NetOutgoingMessage conMsg = SSEngine.Peer.CreateMessage();
                                conMsg.Write((byte)ConnectionMessageType.Name);
                                conMsg.Write(SSEngine.Peer.UniqueIdentifier);
                                SSEngine.Peer.SendMessage(conMsg, SSEngine.Peer.Connections[0], NetDeliveryMethod.ReliableOrdered);
                                conMsg = SSEngine.Peer.CreateMessage();
                                conMsg.Write((byte)ConnectionMessageType.Checkbox);
                                conMsg.Write(ConForm.cbHost.Checked);
                                SSEngine.Peer.SendMessage(conMsg, SSEngine.Peer.Connections[0], NetDeliveryMethod.ReliableOrdered);
                                conMsg = SSEngine.Peer.CreateMessage();
                                conMsg.Write((byte)MasterServerMessageType.HostConnected);
                                conMsg.Write(SSEngine.Peer.UniqueIdentifier);
                                SSEngine.Peer.SendUnconnectedMessage(conMsg, SSEngine.MasterServerEndpoint);
                                break;
                            case NetConnectionStatus.Disconnected:
                                // Action after client disconnects.
                                ConForm.Close();
                                break;
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        switch ((ConnectionMessageType)inc.ReadByte())
                        {
                            case ConnectionMessageType.Name:
                                ConForm.tbxClient.Text = inc.ReadInt64().ToString();
                                break;
                            case ConnectionMessageType.Checkbox:
                                ConForm.cbClient.Checked = inc.ReadBoolean();
                                break;
                        }
                        break;
                }
            }
        }

        public static void HostChecked()
        {
            NetOutgoingMessage msg = SSEngine.Peer.CreateMessage();
            msg.Write((byte)ConnectionMessageType.Checkbox);
            msg.Write(ConForm.cbHost.Checked);
            SSEngine.Peer.SendMessage(msg, SSEngine.Peer.Connections[0], NetDeliveryMethod.ReliableOrdered);
        }

        public static void Disconnect()
        {
            if (SSEngine.Peer is NetServer && !(ConForm.cbClient.Checked && ConForm.cbHost.Checked))
            {
                NetOutgoingMessage exitMsg = SSEngine.Peer.CreateMessage();
                exitMsg.Write((byte)MasterServerMessageType.RemoveHost);
                exitMsg.Write(SSEngine.Peer.UniqueIdentifier);
                SSEngine.Peer.SendUnconnectedMessage(exitMsg, SSEngine.MasterServerEndpoint);
                System.Threading.Thread.Sleep(1000);
            }
            if (!(ConForm.cbClient.Checked && ConForm.cbHost.Checked))
                SSEngine.Peer.Shutdown("shutting down");
        }
    }
}