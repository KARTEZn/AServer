using System;

namespace AServer
{
    class Program
    {
        public static string ip;
        public static int port;

        static void Main(string[] args)
        {
            Console.Title = "AServer";
            try
            {
                int i = 0;
                while (i != args.Length)
                {
                    if (args[i].ToString().Contains("ip:"))
                    {
                        ip = args[i].ToString().Substring(3, args[i].ToString().Length - 3);
                        Console.WriteLine("IP: "+ip.ToString());
                    }
                    if (args[i].ToString().Contains("port:"))
                    {
                        port = Convert.ToInt32(args[i].ToString().Substring(5, args[i].ToString().Length - 5));
                        Console.WriteLine("PORT: " + port.ToString());
                    }
                    i += 1;
                }
                Console.WriteLine("Загрузка параметров завершена!");
            }
            catch(Exception _Exception)
            {
                Console.WriteLine("Возникло исключение при загрузке параметров: "+_Exception.Message);
            }

            AData.DataUsers[] _List = new AData.DataUsers[5];

            JSON _JSON = new JSON(_List);

            SocketServer _SocketServer = new SocketServer();

            Console.Read();
        }
    }
}
