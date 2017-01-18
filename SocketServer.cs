using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AServer
{
    class SocketServer
    {
        static TcpListener listener;

        public SocketServer(string ip, int port)
        {
            try
            {
                listener = new TcpListener(IPAddress.Parse(ip), port);
                listener.Start();
                Console.WriteLine("Ожидание подключений...");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    ClientObject clientObject = new ClientObject(client);
                    // создаем новый поток для обслуживания нового клиента
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception _Exception)
            {
                Console.WriteLine("Во время подключения возникла ошибка: " + _Exception);
            }
            /*finally
            {
                if (listener != null)
                    listener.Stop();
            }*/
        }
    }

    public class ClientObject
    {
        public TcpClient client;
        public ClientObject(TcpClient tcpClient)
        {
            client = tcpClient;
        }

        public void Process()
        {
            NetworkStream stream = null;
            try
            {
                stream = client.GetStream();
                byte[] data = new byte[64]; // буфер для получаемых данных
                while (true)
                {
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();
                    //Console.WriteLine(message);

                    Console.WriteLine("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ \n");

                    OraSes Ora = new OraSes(Program.ora_username, Program.ora_password, Program.ora_datasource);

                    JSON _JSON = new JSON(Ora.RQuery("select * from smgoods"));
                    message = _JSON.output;

                    



                    Console.WriteLine(message);

                    Console.WriteLine("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ \n");

                    data = Encoding.UTF8.GetBytes(message);
                    stream.Write(data, 0, data.Length);

                    //stream.Close();
                }
            }
            catch(Exception _Exception)
            {
                Console.WriteLine("Ошибка [SocketServer]: " + _Exception.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }
    }
}
