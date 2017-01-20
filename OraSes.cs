using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OracleClient;
using System.Data;

namespace AServer
{
    public class OraSes
    {
        private OracleConnection con;

        public OraSes(string username, string password, string datasource)
        {
            try
            {
                con = new OracleConnection();
                con.ConnectionString = "User Id=" + username + ";Password=" + password + ";Data Source=" + datasource;
                con.Open();
                Console.WriteLine("Успешное соединение с: " + con.ServerVersion);
            }
            catch(Exception _Exception)
            {
                Console.WriteLine("Ошибка соединения [oracle]: "+_Exception.Message);
            }
        }

        public List<List<string>> RQuery(string SQL)
        {
            List<List<string>> Result = new List<List<string>>();
            try
            {
                OracleCommand cmd = new OracleCommand(SQL, con);
                cmd.CommandType = CommandType.Text;
                OracleDataReader dr = cmd.ExecuteReader();
                int i = 0;
                while (dr.Read())
                {
                    Result.Add(new List<string>());

                    int j = 0;
                    while (j != dr.FieldCount)
                    {
                        Result[i].Add(dr.GetValue(j).ToString());
                        j += 1;
                    }

                    i += 1;
                }

                return Result;
            }
            catch (Exception _Exception)
            {
                Console.WriteLine("Ошибка выполнения запроса в [oracle]: " + _Exception.Message);
                return Result;
            }
        }
    }
}
