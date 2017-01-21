using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

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

        public List<List<string>> RQuery(string SQL)
        {
            List<List<string>> Result = new List<List<string>>();
            try
            {
                MySqlCommand myCommand = new MySqlCommand(SQL, myConnection);
                MySqlDataReader MyDataReader;
                MyDataReader = myCommand.ExecuteReader();
                int i = 0;
                while (MyDataReader.Read())
                {
                    Result.Add(new List<string>());

                    int j = 0;
                    while (j != MyDataReader.FieldCount)
                    {
                        Result[i].Add(MyDataReader.GetValue(j).ToString());
                        j += 1;
                    }

                    i += 1;
                }
                MyDataReader.Close();

                return Result;
            }
            catch (Exception _Exception)
            {
                Console.WriteLine("Ошибка выполнения запроса в [mysql]: " + _Exception.Message);
                return Result;
            }
        }

        public bool WQuery(string SQL)
        {
            List<List<string>> Result = new List<List<string>>();
            try
            {
                byte[] bytes = Encoding.Default.GetBytes(SQL);
                SQL = Encoding.UTF8.GetString(bytes);

                MySqlCommand myCommand = new MySqlCommand(SQL, myConnection);
                myCommand.ExecuteNonQuery();

                return true;
            }
            catch (Exception _Exception)
            {
                Console.WriteLine("Ошибка выполнения запроса в [mysql]: " + _Exception.Message);
                return false;
            }
        }

        ~MySQL()
        {
            myConnection.Close();
        }
    }
}
