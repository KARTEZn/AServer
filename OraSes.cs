using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OracleClient;

namespace AServer
{
    public class OraSes
    {
        OracleConnection con;

        void Connect()
        {
            con = new OracleConnection();
            con.ConnectionString = "User Id=<username>;Password=<password>;Data Source=<datasource>";
            con.Open();
            Console.WriteLine("Connected to Oracle" + con.ServerVersion);
        }
    }
}
