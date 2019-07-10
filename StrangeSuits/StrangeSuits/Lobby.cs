using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StrangeSuits
{
    public partial class Lobby : Form
    {
        private Timer timer;
        private static EventHandler clientHandle;

        public Lobby()
        {
            InitializeComponent();
            timer = new Timer();
            timer.Interval = 16;  //  delay in milliseconds.
            clientHandle = new EventHandler(Client.ClientUpdate);
            timer.Tick += clientHandle;
        }

        public void StartUpdate()
        {
            timer.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Client.GetServerList();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
                return;

            string[] splits = comboBox1.SelectedItem.ToString().Split(' ');
            long host = Int64.Parse(splits[0]);
            Client.RequestConnection(host);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer.Tick -= clientHandle;
            Client.Host();
        }

        private void Lobby_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer.Stop();
            if (!(SSEngine.Peer is Lidgren.Network.NetServer && 
                Server.ConForm.cbHost.Checked && Server.ConForm.cbClient.Checked) &&
                (SSEngine.IsHost == null || SSEngine.IsHost.Value))
                Client.Disconnect();
            
        }
    }
}