using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RDP_Client
{
    public partial class Form2 : Form
    {
        private readonly int port;
        private TcpClient TC;
        private TcpListener TL;
        private NetworkStream NS;

        private readonly Thread listen;
        private readonly Thread getImage;


        public Form2(int Port)
        {
            port = Port;
            TC = new TcpClient();
            listen = new Thread(StartListening);
            getImage = new Thread(ReceiveImage);


            InitializeComponent();
        }

        private void StartListening()
        {
            while(!TC.Connected)
            {
                TL.Start();
                TC = TL.AcceptTcpClient();
            }
            getImage.Start();
        }

        private void StopListening()
        {
            TL.Stop();
            TC = null;
            if (listen.IsAlive) listen.Abort();
            if (getImage.IsAlive) getImage.Abort();
        }

        private void ReceiveImage()
        {
            BinaryFormatter BF = new BinaryFormatter();
            while (TC.Connected)
            {
                NS = TC.GetStream();
                pictureBox1.Image =  (Image )BF.Deserialize(NS);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            TL = new TcpListener(IPAddress.Any, port);
            listen.Start();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            StopListening();
        }
    }
}
