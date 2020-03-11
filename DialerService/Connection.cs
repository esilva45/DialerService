using Npgsql;
using System;
using System.Xml.Linq;

namespace ServiceIntegration {
    class Connection {
        private static NpgsqlConnection conn = null;

        private Connection() { }

        public static NpgsqlConnection GetConnection() {
            try {
                XElement configXml = XElement.Load(System.AppDomain.CurrentDomain.BaseDirectory + @"\config.xml");

                NpgsqlConnectionStringBuilder sb = new NpgsqlConnectionStringBuilder();
                sb.Host = configXml.Element("DBHost").Value.ToString();
                sb.Port = Int32.Parse(configXml.Element("DBPort").Value.ToString());
                sb.UserName = configXml.Element("DBUser").Value.ToString();
                sb.Password = SecurePassword.Decrypt(configXml.Element("DBPass").Value.ToString());
                sb.Database = configXml.Element("DBBase").Value.ToString();
                sb.MinPoolSize = 1;
                sb.MaxPoolSize = 20;
                sb.Pooling = true;

                conn = new NpgsqlConnection(sb.ToString());
                conn.Open();
            }
            catch (Exception ex) {
                Util.Log(ex.ToString());
            }

            return conn;
        }
    }
}
