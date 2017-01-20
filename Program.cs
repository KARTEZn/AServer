using System;

namespace AServer
{
    class Program
    {
        public static string ip;
        public static int port;
        public static string ora_username;
        public static string ora_password;
        public static string ora_datasource;

        public static string sDomain;
        public static string sDefaultOU;
        public static string sDefaultRootOU;
        public static string sServiceUser;
        public static string sServicePassword;

        public static SocketServer _SocketServer;


        static void Main(string[] args)
        {
            Console.Title = "AServer";
            if (args.Length == 0)
            {
                Console.WriteLine("Ошибка: Отсутствуют параметры для запуска ....");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ \n");
                try
                {
                    int i = 0;
                    while (i != args.Length)
                    {
                        if (args[i].ToString().Contains("ip:"))
                        {
                            ip = args[i].ToString().Substring(3, args[i].ToString().Length - 3);
                            Console.WriteLine("IP: " + ip.ToString());
                        }
                        if (args[i].ToString().Contains("port:"))
                        {
                            port = Convert.ToInt32(args[i].ToString().Substring(5, args[i].ToString().Length - 5));
                            Console.WriteLine("PORT: " + port.ToString());
                        }
                        if (args[i].ToString().Contains("ora_u:"))
                        {
                            ora_username = args[i].ToString().Substring(6, args[i].ToString().Length - 6);
                            Console.WriteLine("oracle username: " + ora_username);
                        }
                        if (args[i].ToString().Contains("ora_p:"))
                        {
                            ora_password = args[i].ToString().Substring(6, args[i].ToString().Length - 6);
                            Console.WriteLine("oracle password: " + charpas(ora_password.Length));
                        }
                        if (args[i].ToString().Contains("ora_d:"))
                        {
                            ora_datasource = args[i].ToString().Substring(6, args[i].ToString().Length - 6);
                            Console.WriteLine("oracle datasource: " + ora_datasource);
                        }
                        if (args[i].ToString().Contains("sDomain:"))
                        {
                            sDomain = args[i].ToString().Substring(8, args[i].ToString().Length - 8);
                            Console.WriteLine("sDomain: " + sDomain);
                        }
                        if (args[i].ToString().Contains("sDefaultOU:"))
                        {
                            sDefaultOU = args[i].ToString().Substring(11, args[i].ToString().Length - 11);
                            Console.WriteLine("sDefaultOU: " + sDefaultOU);
                        }
                        if (args[i].ToString().Contains("sDefaultRootOU:"))
                        {
                            sDefaultRootOU = args[i].ToString().Substring(15, args[i].ToString().Length - 15);
                            Console.WriteLine("sDefaultRootOU: " + sDefaultRootOU);
                        }
                        if (args[i].ToString().Contains("sServiceUser:"))
                        {
                            sServiceUser = args[i].ToString().Substring(13, args[i].ToString().Length - 13);
                            Console.WriteLine("sServiceUser: " + sServiceUser);
                        }
                        if (args[i].ToString().Contains("sServicePassword:"))
                        {
                            sServicePassword = args[i].ToString().Substring(17, args[i].ToString().Length - 17);
                            Console.WriteLine("sServicePassword: " + charpas(sServicePassword.Length));
                        }
                        i += 1;
                    }
                    Console.WriteLine("Загрузка параметров завершена!");
                }
                catch (Exception _Exception)
                {
                    Console.WriteLine("Возникло исключение при загрузке параметров: " + _Exception.Message);
                }

                Console.WriteLine("_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ \n");

                MySQL _MySQL = new MySQL("192.168.1.242", "info", "root", "kj87dyt");

                _SocketServer = new SocketServer(ip, port);

                Console.ReadLine();
            }
        }

        private static string charpas(int count)
        {
            string temp  = "";
            int i = 0;
            while (i != count)
            {
                temp +=  "*";
                i += 1;
            }
            return temp;
        }

    }
}
