using Newtonsoft.Json;
using Npgsql;
using System;
using System.IO;
using System.Xml.Linq;

namespace ServiceIntegration {
    class LoadFile {
        public static void Load() {
            NpgsqlConnection conn = Connection.GetConnection();
            XElement configXml = XElement.Load(System.AppDomain.CurrentDomain.BaseDirectory + @"\config.xml");
            string patch = configXml.Element("LogFile").Value.ToString();

            string nextLine;
            string callid = "";
            string query = "";

            try {
                FileStream file = File.Open(patch, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader reader = new StreamReader(file);
                reader.ReadLine();

                while (!reader.EndOfStream) {
                    nextLine = reader.ReadLine();

                    if (!string.IsNullOrWhiteSpace(nextLine)) {
                        string[] lineList = nextLine.Split(',');
                        var json = JsonConvert.SerializeObject(lineList);
                        callid = lineList[0].Replace("Call", "").Trim();

                        query = "INSERT INTO cdr(callid, data) VALUES ('" + callid + "', '" + json + "')";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                        try {
                            cmd.ExecuteReader();
                        }
                        catch (Exception) { }
                    }
                }

                reader.Close();
                file.Close();
            }
            catch (Exception ex) {
                Util.Log(ex.ToString());
            }
            finally {
                if (conn != null) {
                    conn.Close();
                }
            }
        }
    }
}
