using MySql.Data.MySqlClient;
using System;

namespace AServer
{
    class MySQL
    {
        private MySqlConnection myConnection;

        public MySQL(string Host, string Database, string User, string Pass)
        {
            string ConStr = "Database="+Database+";Data Source="+Host+";User Id="+User+";Password="+Pass;

            myConnection = new MySqlConnection(ConStr);
            try
            {
                myConnection.Open();
                Console.WriteLine("Соединение с [mysql] установлено!");
            }
            catch(Exception _Exception)
            {
                Console.WriteLine("Ошибка установки соединения с [mysql]: "+_Exception.Message);
            }
        }

        ~MySQL()
        {
            myConnection.Close();
        }
    }
}
