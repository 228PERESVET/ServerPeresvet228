using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace TopServer2k18
{
    class WebClient
    {
        ConnectDataBase connectDataBase;

        // Отправка страницы с ошибкой
        private void SendError(TcpClient Client, int Code)
        {
            // Получаем строку вида "200 OK"
            // HttpStatusCode хранит в себе все статус-коды HTTP/1.1
            string codeStr = Code.ToString() + " " + ((HttpStatusCode)Code).ToString();
            // Код простой HTML-странички
            string Html = "<html><body><h1>" + codeStr + "</h1></body></html>";
            // Необходимые заголовки: ответ сервера, тип и длина содержимого. После двух пустых строк - само содержимое
            string Str = "HTTP/1.1 " + codeStr + "\nContent-type: text/html\nContent-Length:" + Html.Length.ToString() + "\n\n" + Html;
            // Приведем строку к виду массива байт
            byte[] buffer = Encoding.ASCII.GetBytes(Str);
            // Отправим его клиенту
            Client.GetStream().Write(buffer, 0, buffer.Length);
            // Закроем соединение
            Client.Close();
        }

        // Отправка ajax ответа. Формат передаваемых данных JSON
        private void SendAjax(TcpClient client, string data)
        {
            // Получаем строку вида "200 OK"
            // HttpStatusCode хранит в себе все статус-коды HTTP/1.1
            //string CodeStr = Code.ToString() + " " + ((HttpStatusCode)Code).ToString();
            // Код простой HTML-странички

            // 15.03.18 Запрос к бд
            connectDataBase = new ConnectDataBase();

            string body = connectDataBase.Select();

            // Необходимые заголовки: ответ сервера, тип и длина содержимого. После двух пустых строк - само содержимое
            string http = "HTTP/1.1 " + 200 + "\r\nContent-type: application/json; charset=utf-8\r\nContent-Length:" + body.Length.ToString() + "\r\n\r\n" + body;
            // Приведем строку к виду массива байт
            byte[] buffer = Encoding.ASCII.GetBytes(http);
            // Отправим его клиенту
            client.GetStream().Write(buffer, 0, buffer.Length);
            // Закроем соединение
            client.Close();
        }

        // Конструктор класса. Ему нужно передавать принятого клиента от TcpListener
        public WebClient(TcpClient client)
        {
            // Объявим строку, в которой будет хранится запрос клиента
            string Request = "";
            // Буфер для хранения принятых от клиента данных
            byte[] buffer = new byte[1024];
            // Переменная для хранения количества байт, принятых от клиента
            int count;
            // Читаем из потока клиента до тех пор, пока от него поступают данные
            count = client.GetStream().Read(buffer, 0, buffer.Length);
            Request += Encoding.ASCII.GetString(buffer, 0, count);
            /*while ((count = client.GetStream().Read(buffer, 0, buffer.Length)) > 0)
            {
                // Преобразуем эти данные в строку и добавим ее к переменной Request
                Request += Encoding.ASCII.GetString(buffer, 0, count);
                // Запрос должен обрываться последовательностью \r\n\r\n
                // Либо обрываем прием данных сами, если длина строки Request превышает 4 килобайта
                // Нам не нужно получать данные из POST-запроса (и т. п.), а обычный запрос
                // по идее не должен быть больше 4 килобайт
                if (Request.IndexOf("\r\n\r\n") >= 0 || Request.Length > 4096)
                {
                    break;
                }
            }*/



            // Запрос данных от системы пересвет 09.05.17 
            int pos;
            if ((pos = Request.LastIndexOf("type=get")) > 0)
            {
                string text = Request.Substring(pos);
                Console.WriteLine(text);
                SendAjax(client, text);
                return;
            }


            // Парсим строку запроса с использованием регулярных выражений
            // При этом отсекаем все переменные GET-запроса
            Match ReqMatch = Regex.Match(Request, @"^\w+\s+([^\s\?]+)[^\s]*\s+HTTP/.*|");


            // Если запрос не удался
            if (ReqMatch == Match.Empty)
            {
                // Передаем клиенту ошибку 400 - неверный запрос
                SendError(client, 400);
                return;
            }
            // Получаем строку запроса
            string requestUri = ReqMatch.Groups[1].Value;

            // Приводим ее к изначальному виду, преобразуя экранированные символы
            // Например, "%20" -> " "
            requestUri = Uri.UnescapeDataString(requestUri);
            // Если в строке содержится двоеточие, передадим ошибку 400
            // Это нужно для защиты от URL типа http://example.com/../../file.txt
            if (requestUri.IndexOf("..") >= 0)
            {
                SendError(client, 400);
                return;
            }

            // Если строка запроса оканчивается на "/", то добавим к ней index.html
            if (requestUri.EndsWith("/"))
                requestUri += "index.html";

            string FilePath = "www/" + requestUri;

            // Если в папке www не существует данного файла, посылаем ошибку 404
            if (!File.Exists(FilePath))
            {
                SendError(client, 404);
                return;
            }

            // Получаем расширение файла из строки запроса
            string extension = requestUri.Substring(requestUri.LastIndexOf('.'));

            // Тип содержимого
            string сontentType = "";

            // Пытаемся определить тип содержимого по расширению файла
            switch (extension)
            {
                case ".htm":
                case ".html":
                    сontentType = "text/html";
                    break;
                case ".css":
                    сontentType = "text/stylesheet";
                    break;
                case ".js":
                    сontentType = "text/javascript";
                    break;
                case ".jpg":
                    сontentType = "image/jpeg";
                    break;
                case ".jpeg":
                case ".png":
                case ".gif":
                    сontentType = "image/" + extension.Substring(1);
                    break;
                default:
                    if (extension.Length > 1)
                    {
                        сontentType = "application/" + extension.Substring(1);
                    }
                    else
                    {
                        сontentType = "application/unknown";
                    }
                    break;
            }

            // Открываем файл, страхуясь на случай ошибки
            FileStream fileStream;
            try
            {
                fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (Exception)
            {
                // Если случилась ошибка, посылаем клиенту ошибку 500
                SendError(client, 500);
                return;
            }

            // Посылаем заголовки
            string Headers = "HTTP/1.1 200 OK\nContent-Type: " + сontentType + "\nContent-Length: " + fileStream.Length + "\n\n";
            byte[] HeadersBuffer = Encoding.ASCII.GetBytes(Headers);
            client.GetStream().Write(HeadersBuffer, 0, HeadersBuffer.Length);

            // Пока не достигнут конец файла
            while (fileStream.Position < fileStream.Length)
            {
                // Читаем данные из файла
                count = fileStream.Read(buffer, 0, buffer.Length);
                // И передаем их клиенту
                client.GetStream().Write(buffer, 0, count);
            }

            // Закроем файл и соединение
            fileStream.Close();
            client.Close();


        }
    }
}