using System;
using System.Collections.Generic;

namespace AServer
{
    class AdminMode
    {
        public enum DBTypes {Oracle, MySQL, SQLite, ActiveDirectory};
        public enum UserTypes {User, Admin};
        private SQLite AMData;

        public AdminMode()
        {
            #region AdminMode init

            AMData = new SQLite(Environment.CurrentDirectory + "\\Mods\\AMData.sqlite");
            try
            {
                string Query = @"CREATE TABLE IF NOT EXISTS [am_db] (
                            [id] integer PRIMARY KEY AUTOINCREMENT NOT NULL,
                            [name] char(12),
                            [type] byte,
                            [host] char(20),
                            [user] char(12),
                            [pass] char(12),
                            [rkey] char(32),
                            [wkey] char(32)
                            );";
                AMData.WQuery(Query);
            }
            catch { }
            try
            {
                string Query = @"CREATE INDEX IF NOT EXISTS _am_db ON am_db (
                                            id,
                                            name,
                                            type,
                                            host,
                                            user,
                                            pass,
                                            rkey,
                                            wkey
                                        );";
                AMData.WQuery(Query);
            }
            catch { }
            try
            {
                string Query = @"CREATE TABLE IF NOT EXISTS [am_user] (
                            [id] integer PRIMARY KEY AUTOINCREMENT NOT NULL,
                            [user] char(12),
                            [pass] char(12),
                            [type] byte
                            );";
                AMData.WQuery(Query);
            }
            catch { }
            try
            {
                string Query = @"CREATE INDEX IF NOT EXISTS _am_user ON am_user (
                                            id,
                                            user,
                                            pass,
                                            type
                                        );";
                AMData.WQuery(Query);
            }
            catch { }
            try
            {
                string Query = @"CREATE TABLE IF NOT EXISTS [am_sockserv] (
                            [id] integer PRIMARY KEY AUTOINCREMENT NOT NULL,
                            [name] char(20),
                            [ip] char(15),
                            [port] char(4)
                            );";
                AMData.WQuery(Query);
            }
            catch { }
            try
            {
                string Query = @"CREATE INDEX IF NOT EXISTS _am_sockserv ON am_sockserv (
                                            id,
                                            name,
                                            ip,
                                            port
                                        );";
                AMData.WQuery(Query);
            }
            catch { }

            #endregion

            #region AdminMode find Root
            try
            {
                if (!AMData.EQuery("root", "user", "am_user"))
                {
                    string Query = @"INSERT INTO am_user(user, pass, type) values('root', '123456', 1);";
                    AMData.WQuery(Query);
                }
            }
            catch { }
            #endregion

            #region AdminMode find Default SocketServer
            try
            {
                String host = System.Net.Dns.GetHostName();
                System.Net.IPAddress ip = System.Net.Dns.GetHostByName(host).AddressList[0];
                if (!AMData.EQuery("Default", "name", "am_sockserv"))
                {
                    string Query = @"INSERT INTO am_sockserv(name, ip, port) values('Default', '" + ip.ToString() + "', '888');";
                    AMData.WQuery(Query);
                }
            }
            catch { }
            #endregion

        }

        public List<List<string>> GetServers()
        {
            return AMData.RQuery("SELECT * from am_sockserv");
        }
    }
}
