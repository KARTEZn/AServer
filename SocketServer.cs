using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;

namespace AServer
{

    // Класс-обработчик клиента
    class Client
    {
        // Отправка страницы с ошибкой
        private void SendError(TcpClient Client, int Code)
        {
            string CodeStr;
            CodeStr = "<h1>Ошибочный запрос! Ошибка [" + Code.ToString() + "]</h1>";
            // Получаем строку вида "200 OK"
            // HttpStatusCode хранит в себе все статус-коды HTTP/1.1

            // Код простой HTML-странички
            string Html = "<html><body><h1>" + CodeStr + "</h1></body></html>";
            // Необходимые заголовки: ответ сервера, тип и длина содержимого. После двух пустых строк - само содержимое
            string Str = "HTTP/1.1 " + CodeStr + "\nContent-type: text/html\nContent-Length:" + Html.Length.ToString() + "\n\n" + Html;
            // Приведем строку к виду массива байт
            byte[] Buffer = Encoding.UTF8.GetBytes(Str);
            // Отправим его клиенту
            Client.GetStream().Write(Buffer, 0, Buffer.Length);
            // Закроем соединение
            Client.Close();
        }

        // Конструктор класса. Ему нужно передавать принятого клиента от TcpListener
        public Client(TcpClient Client)
        {
            // Объявим строку, в которой будет хранится запрос клиента
            string Request = "";
            // Буфер для хранения принятых от клиента данных
            byte[] Buffer = new byte[1024];
            // Переменная для хранения количества байт, принятых от клиента
            int Count;
            // Читаем из потока клиента до тех пор, пока от него поступают данные
            while ((Count = Client.GetStream().Read(Buffer, 0, Buffer.Length)) > 0)
            {
                // Преобразуем эти данные в строку и добавим ее к переменной Request
                Request += Encoding.UTF8.GetString(Buffer, 0, Count);
                // Запрос должен обрываться последовательностью \r\n\r\n
                // Либо обрываем прием данных сами, если длина строки Request превышает 4 килобайта
                // Нам не нужно получать данные из POST-запроса (и т. п.), а обычный запрос
                // по идее не должен быть больше 4 килобайт
                if (Request.IndexOf("\r\n\r\n") >= 0 || Request.Length > 4096)
                {
                    break;
                }
            }

            // Парсим строку запроса с использованием регулярных выражений
            // При этом отсекаем все переменные GET-запроса
            Match ReqMatch = Regex.Match(Request, @"^\w+\s+([^\s\?]+)[^\s]*\s+HTTP/.*|");

            // Если запрос не удался
            if (ReqMatch == Match.Empty)
            {
                // Передаем клиенту ошибку 400 - неверный запрос
                SendError(Client, 400);
                return;
            }

            // Получаем строку запроса
            string RequestUri = ReqMatch.Groups[1].Value;

            // Приводим ее к изначальному виду, преобразуя экранированные символы
            // Например, "%20" -> " "
            RequestUri = Uri.UnescapeDataString(RequestUri);

            // Если в строке содержится двоеточие, передадим ошибку 400
            // Это нужно для защиты от URL типа http://example.com/../../file.txt
            if (RequestUri.IndexOf("..") >= 0)
            {
                SendError(Client, 400);
                return;
            }

            // Если строка запроса оканчивается на "/", то добавим к ней index.html
            if (RequestUri.EndsWith("/"))
            {
                SendError(Client, 666);
                return;
            }

            if (RequestUri.IndexOf("admin") > -1)
            {
                try
                {
                    string aout = "";

                    aout += "<html>";
                    aout += "<head>";
                    aout += "<title>Панель администратора | AServer</title>";
                    aout += "</head>";
                    aout += "<body>";
                    aout += "<div style=\"position: absolute; width: 400px; height: 400px; left: 50%; top: 50%; margin-left:-200px; margin-top:-200px; background: #fc0;\">";
                    aout += "</div>";
                    aout += "</body>";
                    aout += "</html>";

                    string Headers = "HTTP/1.1 200 OK\nContent-Type: text/html;\nContent-Length: " + aout.Length + ";\ncharset=utf-8;\n\n";
                    byte[] HeadersBuffer = Encoding.UTF8.GetBytes(Headers);
                    Client.GetStream().Write(HeadersBuffer, 0, HeadersBuffer.Length);

                    string mymessage = aout;
                    byte[] HBuffer = Encoding.UTF8.GetBytes(mymessage);
                    Client.GetStream().Write(HBuffer, 0, HBuffer.Length);
                }
                catch { }
            }

            if (RequestUri.IndexOf("oracle") > -1)
            {
                Console.WriteLine("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ \n");

                string[] query;

                try
                {
                    query = RequestUri.Split(new char[] { '&' });
                }
                catch
                {
                    query = null;
                }

                if (query != null)
                {
                    Console.WriteLine("Запрос в базу [oracle]: " + query[1]);

                    OraSes Ora;
                    JSON _JSON;

                    try
                    {

                        Ora = new OraSes(Program.ora_username, Program.ora_password, Program.ora_datasource);

                        _JSON = new JSON(Ora.RQuery(query[1]));

                    }
                    catch (Exception _Exception)
                    {
                        Console.WriteLine("Ошибка [query]: " + _Exception.Message);

                        string error = "error: "+_Exception.Message;
                        byte[] EBuffer = Encoding.UTF8.GetBytes(error);
                        Client.GetStream().Write(EBuffer, 0, EBuffer.Length);

                        Ora = null;

                        _JSON = null;
                    }

                    string Headers = "HTTP/1.1 200 OK\nContent-Type: application/json;\nContent-Length: " + _JSON.output.Length + ";\ncharset=utf-8;\n\n";
                    byte[] HeadersBuffer = Encoding.UTF8.GetBytes(Headers);
                    Client.GetStream().Write(HeadersBuffer, 0, HeadersBuffer.Length);

                    string mymessage = _JSON.output;
                    byte[] HBuffer = Encoding.UTF8.GetBytes(mymessage);
                    Client.GetStream().Write(HBuffer, 0, HBuffer.Length);

                    Console.WriteLine("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ \n");
                }
                else SendError(Client, 404);
            }
            if (RequestUri.IndexOf("ad") > -1)
            {
                Console.WriteLine("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ \n");

                string[] query;

                try
                {
                    query = RequestUri.Split(new char[] { '&' });
                }
                catch
                {
                    query = null;
                }

                if (query != null)
                {
                    Console.WriteLine("Запрос в базу [active directory]: " + query[1]);

                    JSON _JSON;

                    try
                    {

                        _JSON = new JSON(AD.ValidateCredentials(query[1], query[2]));
                        Console.WriteLine("Результат [active directory]: "+_JSON.output);
                    }
                    catch (Exception _Exception)
                    {
                        Console.WriteLine("Ошибка [query]: " + _Exception.Message);

                        string error = "error: " + _Exception.Message;
                        byte[] EBuffer = Encoding.UTF8.GetBytes(error);
                        Client.GetStream().Write(EBuffer, 0, EBuffer.Length);

                        _JSON = null;
                    }

                    string Headers = "HTTP/1.1 200 OK\nContent-Type: application/json;\nContent-Length: " + _JSON.output.Length + ";\ncharset=utf-8;\n\n";
                    byte[] HeadersBuffer = Encoding.UTF8.GetBytes(Headers);
                    Client.GetStream().Write(HeadersBuffer, 0, HeadersBuffer.Length);

                    string mymessage = _JSON.output;
                    byte[] HBuffer = Encoding.UTF8.GetBytes(mymessage);
                    Client.GetStream().Write(HBuffer, 0, HBuffer.Length);

                    Console.WriteLine("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ \n");
                }
                else SendError(Client, 404);
            }
            if (RequestUri.IndexOf("rmysql") > -1)
            {
                Console.WriteLine("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ \n");

                string[] query;

                try
                {
                    query = RequestUri.Split(new char[] { '&' });
                }
                catch
                {
                    query = null;
                }

                if (query != null)
                {
                    Console.WriteLine("Запрос в базу [mysql]: " + query[1]);

                    MySQL _MySQL;
                    JSON _JSON;

                    try
                    {
                        _MySQL = new MySQL(Program.MHost, Program.MSource, Program.MUser, Program.MPass);
                        _JSON = new JSON(_MySQL.RQuery(query[1]));
                    }
                    catch (Exception _Exception)
                    {
                        Console.WriteLine("Ошибка [query]: " + _Exception.Message);

                        string error = "error: " + _Exception.Message;
                        byte[] EBuffer = Encoding.UTF8.GetBytes(error);
                        Client.GetStream().Write(EBuffer, 0, EBuffer.Length);

                        _MySQL = null;
                        _JSON = null;
                    }

                    string Headers = "HTTP/1.1 200 OK\nContent-Type: application/json;\nContent-Length: " + _JSON.output.Length + ";\ncharset=utf-8;\n\n";
                    byte[] HeadersBuffer = Encoding.UTF8.GetBytes(Headers);
                    Client.GetStream().Write(HeadersBuffer, 0, HeadersBuffer.Length);

                    string mymessage = _JSON.output;
                    byte[] HBuffer = Encoding.UTF8.GetBytes(mymessage);
                    Client.GetStream().Write(HBuffer, 0, HBuffer.Length);

                    Console.WriteLine("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ \n");
                }
                else SendError(Client, 404);
            }

            if (RequestUri.IndexOf("wmysql") > -1)
            {
                Console.WriteLine("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ \n");

                string[] query;

                try
                {
                    query = RequestUri.Split(new char[] { '&' });
                }
                catch
                {
                    query = null;
                }

                if (query != null)
                {
                    Console.WriteLine("Запрос в базу [mysql]: " + query[1]);

                    MySQL _MySQL;
                    JSON _JSON;

                    try
                    {
                        _MySQL = new MySQL(Program.MHost, Program.MSource, Program.MUser, Program.MPass);
                        _JSON = new JSON(_MySQL.WQuery(query[1]));
                    }
                    catch (Exception _Exception)
                    {
                        Console.WriteLine("Ошибка [query]: " + _Exception.Message);

                        string error = "error: " + _Exception.Message;
                        byte[] EBuffer = Encoding.UTF8.GetBytes(error);
                        Client.GetStream().Write(EBuffer, 0, EBuffer.Length);

                        _MySQL = null;
                        _JSON = null;
                    }

                    string Headers = "HTTP/1.1 200 OK\nContent-Type: application/json;\nContent-Length: " + _JSON.output.Length + ";\ncharset=utf-8;\n\n";
                    byte[] HeadersBuffer = Encoding.UTF8.GetBytes(Headers);
                    Client.GetStream().Write(HeadersBuffer, 0, HeadersBuffer.Length);

                    string mymessage = _JSON.output;
                    byte[] HBuffer = Encoding.UTF8.GetBytes(mymessage);
                    Client.GetStream().Write(HBuffer, 0, HBuffer.Length);

                    Console.WriteLine("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ \n");
                }
                else SendError(Client, 404);
            }

            // Закроем файл и соединение
            Client.Close();
        }
    }

    class SocketServer
    {
        TcpListener Listener; // Объект, принимающий TCP-клиентов

        // Запуск сервера
        public SocketServer(string ip, int Port)
        {
            // Определим нужное максимальное количество потоков
            // Пусть будет по 4 на каждый процессор
            int MaxThreadsCount = Environment.ProcessorCount * 4;
            // Установим максимальное количество рабочих потоков
            ThreadPool.SetMaxThreads(MaxThreadsCount, MaxThreadsCount);
            // Установим минимальное количество рабочих потоков
            ThreadPool.SetMinThreads(2, 2);

            Listener = new TcpListener(IPAddress.Parse(ip), Port); // Создаем "слушателя" для указанного порта
            Listener.Start(); // Запускаем его

            // В бесконечном цикле
            while (true)
            {
                // Принимаем новых клиентов. После того, как клиент был принят, он передается в новый поток (ClientThread)
                // с использованием пула потоков.
                //ThreadPool.QueueUserWorkItem(new WaitCallback(ClientThread), Listener.AcceptTcpClient());

                
                // Принимаем нового клиента
                TcpClient Client = Listener.AcceptTcpClient();
                // Создаем поток
                Thread Thread = new Thread(new ParameterizedThreadStart(ClientThread));
                // И запускаем этот поток, передавая ему принятого клиента
                Thread.Start(Client);
                
            }
        }

        static void ClientThread(Object StateInfo)
        {
            // Просто создаем новый экземпляр класса Client и передаем ему приведенный к классу TcpClient объект StateInfo
            new Client((TcpClient)StateInfo);
        }

        // Остановка сервера
        ~SocketServer()
        {
            // Если "слушатель" был создан
            if (Listener != null)
            {
                // Остановим его
                Listener.Stop();
            }
        }
    }
}
