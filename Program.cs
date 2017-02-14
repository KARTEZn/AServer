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

        public static string MHost;
        public static string MSource;
        public static string MUser;
        public static string MPass;

        public static SocketServer _SocketServer;


        static void Main(string[] args)
        {
            AdminMode AM = new AdminMode();
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
