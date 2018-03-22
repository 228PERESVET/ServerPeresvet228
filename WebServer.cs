using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TopServer2k18
{
    class WebServer
    {
        TcpListener listener; // Объект, принимающий TCP-клиентов

        // Запуск сервера
        public WebServer(int port)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(ThreadServise));
            thread.IsBackground = true;
            thread.Start(port);
        }

        private void ThreadServise(Object _port)
        {
            int port = (int)_port;

            listener = new TcpListener(IPAddress.Any, port); // Создаем "слушателя" для указанного порта
            listener.Start(); // Запускаем его

            // Сдохнет при закрытии программы
            while (true)
            {
                // Принимаем нового клиента
                TcpClient Client = listener.AcceptTcpClient();
                // Создаем поток
                Thread Thread = new Thread(new ParameterizedThreadStart(ClientThread));
                // И запускаем этот поток, передавая ему принятого клиента
                Thread.Start(Client);
            }
        }



        static void ClientThread(Object StateInfo)
        {
            // Просто создаем новый экземпляр класса Client и передаем ему приведенный к классу TcpClient объект StateInfo
            new WebClient((TcpClient)StateInfo);
        }

        // Остановка сервера
        ~WebServer()
        {
            // Если "слушатель" был создан
            if (listener != null)
            {
                // Остановим его
                listener.Stop();
            }
        }
    }
}