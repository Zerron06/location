using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace location
{
    public partial class ClientForm : Form
    {
        public string serverAddress;
        public string serverPort;
        public string protocol;
        public string username;
        public string location;
        public string debug;
        public string timeout;
        string portComm;
        string hostComm;
        string timeComm;
        public ClientForm()
        {
            InitializeComponent();
        }

        private void serverText_TextChanged(object sender, EventArgs e)
        {
            if (serverText.Text == "")
            {
                hostComm = null;
                serverAddress =null;
            }
            else
            {
                hostComm = "-h";
                serverAddress =  serverText.Text;
            }
        }

        private void SendBox_Click(object sender, EventArgs e)
        {
            //When the button is clicked we create an array that we send to the main
            string[] args = {timeComm, timeout, hostComm, serverAddress, portComm, serverPort, protocol, debug, username, location };
            Whois.Main(args);
            textBox6.Text = Whois.serverResponse;//We get the serever and debug resonse and add them the the box
            textBox5.Text = Whois.debugResponse;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                portComm = null;
                serverPort = null;
            }
            else
            {
                portComm = "-p";
                serverPort = textBox2.Text;
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (textBox4.Text == "")
            {
                username = null;
            }
            else
            {
                username = textBox4.Text;
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            if (textBox7.Text == "")
            {
                location = null;
            }
            else
            {
                location = textBox7.Text;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string pSelection = comboBox1.SelectedItem.ToString();

            switch (pSelection.Trim())
            {
                case ("HTTP 0.9"):
                    protocol = "-h9";
                    break;
                case ("HTTP 1.0"):
                    protocol = "-h0";
                    break;
                case ("HTTP 1.1"):
                    protocol = "-h1";
                    break;
                case ("Whois"):
                    protocol = "whois";
                    break;
                default:
                    protocol = "whois";
                    break;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                debug = "-d";
            }
            else
            {
                debug = null;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox3.Text == "")
            {
                timeComm = "-t";
                timeout = "1000";
            }
            else
            {
                timeComm = "-t ";
                timeout = textBox3.Text;
            }
        }
    }
}
