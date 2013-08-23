using System;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace sender {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        Thread tRec = null;
        UdpClient udpclient = new UdpClient(15000);
        bool stReceive = false;

        private void buttonSend_Click(object sender, EventArgs e) {
            try
            {
                UdpClient   udpclient   = new UdpClient();

                IPAddress   ipaddress   = IPAddress.Parse(textBoxAddress.Text);
                IPEndPoint  ipi         = new IPEndPoint(ipaddress, 15000);

                byte[] message = Encoding.Default.GetBytes(textBoxSend.Text);
                int sended = udpclient.Send(message, message.Length, ipi);
                textBoxSend.Text = "";

                udpclient.Close();
            } catch (Exception es) {
                MessageBox.Show(es.Message, "Ошибка при отправке!");
            }
        }

        delegate void ShowMessageCallback(string message);
        void ShowMessage(string message) {
            if (textBoxReceive.InvokeRequired) {
                ShowMessageCallback dt = new ShowMessageCallback(ShowMessage);
                Invoke(dt, new object[] { message });
            } else {
                textBoxReceive.Text = message;
            }
        }

        private void Receive() {
            try {
                while (true) {
                    IPEndPoint ipi = null;
                    byte[] message = udpclient.Receive(ref ipi);
                    ShowMessage(Encoding.Default.GetString(message));
                    if (stReceive == true) break;
                }
            } catch (Exception e) {
                //MessageBox.Show(e.Message);
            }
        }

        private void StopReceive() {
            stReceive = true;
            if (udpclient != null) udpclient.Close();
            if (tRec != null) tRec.Join();
        }

        private void Form1_FormClosing(object sender, EventArgs e) {
            StopReceive();
        }

        private void buttonReceive_Click(object sender, EventArgs e) {
            stReceive = false;
            tRec = new Thread(new ThreadStart(Receive));
            tRec.Start();
        }
    }
}
                                              