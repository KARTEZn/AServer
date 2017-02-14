using System;
using System.IO;
using System.Data.Common;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;

namespace AServer
{
    class SQLite
    {
        public SQLiteConnection conn;

        public SQLite(string file)
        {
            try
            {
                if (File.Exists(file))
                {
                    conn = new SQLiteConnection("Data Source = " + file);
                }
                else
                {
                    SQLiteConnection.CreateFile(file);
                    conn = new SQLiteConnection("Data Source = " + file);
                }
                
            }
            catch { }
        }

        public bool EQuery(string What, string ColumnName, string Table)
        {
            try
            {
                string Query = @"SELECT " + ColumnName + " from " + Table + " where " + ColumnName + "='" + What + "'";
                conn.Open();
                SQLiteCommand myCommand = new SQLiteCommand(Query, conn);
                SQLiteDataReader MyDataReader;
                MyDataReader = myCommand.ExecuteReader();
                int k = 0;
                while (MyDataReader.Read()) { k++; }
                MyDataReader.Close();
                conn.Close();
                if (k > 0) return true; else return false;
            }
            catch { return false; }           
        }

        public List<List<string>> RQuery(string SQL)
        {
            List<List<string>> Result = new List<List<string>>();
            try
            {
                conn.Open();
                SQLiteCommand myCommand = new SQLiteCommand(SQL, conn);
                SQLiteDataReader MyDataReader;
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
                conn.Close();
                return Result;
            }
            catch { conn.Close(); return Result; }     
        }

        public bool WQuery(string SQL)
        {
            try
            {
                conn.Open();
                using (SQLiteCommand command = new SQLiteCommand(conn))
                {
                    command.CommandText = SQL;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
                conn.Close();
                return true;
            }
            catch { return false; }
            
        }

        ~SQLite()
        {
            try
            {
                conn.Close();
                conn.Dispose();
            }
            catch { }
        }
    }
}
