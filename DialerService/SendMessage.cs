using Newtonsoft.Json;
using Npgsql;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace ServiceIntegration {
    class SendMessage {
        public static string urlligacao = null;
        public static string urlservice = null;
        public static string token = null;

        public static void Message() {
            NpgsqlConnection conn = Connection.GetConnection();
            XElement configXml = XElement.Load(System.AppDomain.CurrentDomain.BaseDirectory + @"\config.xml");
            urlligacao = configXml.Element("UrlLigacao").Value.ToString();
            urlservice = configXml.Element("UrlService").Value.ToString();
            token = configXml.Element("Token").Value.ToString();

            CallModel call = new CallModel();
            string call_id = "";
            string result = "";
            string campanha = "";
            string extension = "";
            int total_time = 0;
            string reason = "";

            try {
                string query = "select callid,REPLACE(REPLACE(REPLACE(data::text,'[',''),']',''),'\"','') as data from cdr where processed = false group by callid";

                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader rd = command.ExecuteReader();

                while (rd.Read()) {
                    total_time = 0;
                    extension = "";
                    campanha = "";
                    reason = "";

                    string[] lineList = rd["data"].ToString().Split(',');

                    call.CallId = lineList[0].ToString().Replace("Call", "").Trim();
                    call.HistoryId = lineList[1].ToString();
                    
                    campanha = lineList[2].ToString().ToUpper();

                    call.DtInicioChamada = lineList[4].ToString();
                    call.DtFimChamada = lineList[5].ToString();

                    if (!lineList[6].Equals("")) {
                        string[] timeList = lineList[6].Split(':');

                        int hour = Int32.Parse(timeList[0]) * 3600;
                        int minute = Int32.Parse(timeList[1]) * 60;
                        int seconds = Int32.Parse(timeList[2]);
                        total_time = hour + minute + seconds;
                    }
                    
                    call.TempoConversacao = total_time.ToString();

                    call.ReasonTerminated = lineList[8].ToString();
                    reason = lineList[8].ToString(); ;
                    call.DestinoTel = lineList[11].ToString();                   
                    call.FinalDn = lineList[14].ToString();
                    extension = lineList[14].ToString();
                    call.Chain = lineList[18].ToString();

                    if (!lineList[18].Equals("")) {
                        string[] callList = lineList[18].Split(';');

                        if (callList.Length > 2) {
                            call.Campanha = callList[2].Replace("Ext.", "").Trim();
                        }
                    }

                    call.OrigemTel = extension;
                    call.DestinationType = lineList[20].ToString();
                    call.FinalDestinationType = lineList[21].ToString();
                    call.DestinationDisplayName = lineList[23].ToString();
                    call.SourceDisplayName = lineList[24].ToString();
                    call.MissedQueueCalls = lineList[25].ToString();

                    if (campanha.Contains("MAKECALL") && !reason.Equals("LicenseLimit")) {
                        result = "";
                        call.UrlLigacao = urlligacao + Util.FindDirectory(extension, rd["callid"].ToString()); ;
                        call.Token = token;
                        result = Send(JsonConvert.SerializeObject(call), rd["callid"].ToString());

                        if (result.Equals("OK")) {
                            call_id += rd["callid"] + " ";
                        }
                    } else {
                        call_id += rd["callid"] + " ";
                    }
                }

                rd.Close();

                if (call_id != "") {
                    call_id = call_id.Trim().Replace(" ", "','");
                    NpgsqlCommand cmd = new NpgsqlCommand("update cdr set processed = true where callid in ('" + call_id + "')", conn);
                    cmd.ExecuteReader();
                }
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

        private static string Send(string json, string call_id) {
            string code = "ERROR";

            try {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlservice);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                Util.Log(json);

                byte[] data = Encoding.UTF8.GetBytes(json);
                httpWebRequest.ContentLength = data.Length;

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream())) {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                HttpStatusCode respStatusCode = httpResponse.StatusCode;

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                    HttpStatusCode statusCode = ((HttpWebResponse)httpResponse).StatusCode;
                    code = statusCode.ToString();
                    Util.Log("ID: " + call_id + " code: " + code);
                }
            }
            catch (Exception ex) {
                Util.Log(ex.ToString());
            }

            return code;
        }
    }
}
