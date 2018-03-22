using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;

namespace TopServer2k18
{
    class PeresvetServerNetwork
    {
        TcpListener serverTcp;
        RichTextBox console;
        PictureBox consoleImg;
        ImageProcessing imageProcessing = new ImageProcessing();

        const int MAX_SIZE_BUFFER = 921608;
        delegate void del(string str);
        
        public PeresvetServerNetwork(RichTextBox _rtb = null, PictureBox _px = null)
        {
            console = _rtb;
            consoleImg = _px;
            serverTcp = new TcpListener(IPAddress.Any, 8228);
            serverTcp.Start();
            if (console != null)
                console.AppendText("Сервер запущен. Порт: 8228\n");

            Thread th = new Thread(START_FUCKING_SERVER);
            th.IsBackground = true;
            th.Start();
        }


        public void START_FUCKING_SERVER()
        {
            TcpClient client = serverTcp.AcceptTcpClient(); // <--- блокировка
            client.ReceiveBufferSize = 921608;
            DataProcessing(client.GetStream());
        }

        private void DataProcessing(NetworkStream stream)
        {
            byte[] buffer = new byte[MAX_SIZE_BUFFER];

            while (stream.CanRead)
            {
                try
                {
                    byte[] response = new byte[1];
                    int count = 0;
                    int size = 0;
                    int it = 0;

                    /*do
                    {
                        count = stream.Read(buffer, it, buffer.Length - it);
                        it += count;
                        if (size == 0)
                        {
                            size = BitConverter.ToInt32(buffer, 0);
                            console.Invoke(new del((s) => console.Text += s), "size: " + size + "\n");
                        }
                            
                        console.Invoke(new del((s) => console.Text += s), "it: " + it + " count: " + count + "\n");

                    } while (it < size);*/

                    // Работает и ладно..
                    count = stream.Read(buffer, 0, buffer.Length);
                    it += count;
                    if (size == 0)
                    {
                        size = BitConverter.ToInt32(buffer, 0);
                        console.Invoke(new del((s) => console.Text += s), "size: " + size + "\n");
                    }
                    else
                        continue;

                    while (it < size)
                    {
                        count = stream.Read(buffer, it, buffer.Length - it);
                        it += count;
                        console.Invoke(new del((s) => console.Text += s), "it: " + it + " count: " + count + "\n");
                    }

                    consoleImg.Image = imageProcessing.ByteArrayToImage(buffer, it);
                    console.Invoke(new del((s) => console.Text += s), "Изображение полученно и обработанно!\n");
                    /*
                    stream.Write(, it, buffer.Length - it);
                    stream*/
                    stream.Write(response, 0 , response.Length);
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            }

        }
    }
}
