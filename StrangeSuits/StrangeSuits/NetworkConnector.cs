using System;
using System.Windows.Forms;

namespace StrangeSuits
{
    public partial class NetworkConnector : Form
    {
        private Timer timer;
        public NetworkConnector()
        {
            InitializeComponent();
            timer = new Timer();
            timer.Interval = 16;  //  delay in milliseconds.
            timer.Tick += new EventHandler(Server.ServerUpdate);
            
        }

        public void StartUpdate()
        {
            timer.Start();
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer.Stop();
            Server.Disconnect();
        }

        private void cbHost_Click(object sender, EventArgs e)
        {
            if (SSEngine.IsHost.Value && SSEngine.Peer.ConnectionsCount == 1)
            {
                Server.HostChecked();
            }
            else if (!SSEngine.IsHost.Value)
            {
                cbHost.Checked = !cbHost.Checked;
            }
        }

        private void cbClient_Click(object sender, EventArgs e)
        {
            if (!SSEngine.IsHost.Value && SSEngine.Peer.ConnectionsCount == 1)
            {
                Client.ClientChecked();
            }
            else if (SSEngine.IsHost.Value)
            {
                cbClient.Checked = !cbClient.Checked;
            }
        }
    }
}
