using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PSSports.Properties;

namespace PSSports
{
    class Sport
    {
        HttpClient client;
        string url = string.Empty;
     //   HttpRequestMessage request;
        HttpResponseMessage response;
        string iniFile = AppDomain.CurrentDomain.BaseDirectory + "\\Settings.ini";
        public bool Login()
        {
            try
            {
                var myIni = new Ini(iniFile);
                myIni.Load();
                if (client !=null)
                {
                    client.Dispose();
                    client = null;
                }

                client = new HttpClient();
                url = "https://www.ps3838.com/en/";
                client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
                client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36");
                client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");

                response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var headers = response.Content.ReadAsStringAsync().Result;
                }
                client.DefaultRequestHeaders.Add("sec-ch-ua", "'Google Chrome';v='111', 'Not(A:Brand';v='8', 'Chromium';v='111'");
                client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "Windows");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
                url = "https://www.ps3838.com/member-service/v1/login?locale=en_US";

                var stringContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("loginId", myIni.GetValue("UserPs")),
                    new KeyValuePair<string, string>("password", myIni.GetValue("PassPs")),
                });
                response = client.PostAsync(url, stringContent).Result;
                var result = response.Content.ReadAsStringAsync().Result;
                if (result == "1")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                LogTools.HandleException(ex, "Ps3838 Login Error");
                return false;
            }
        }
        public string getBalance()
        {

            string result ="";
            try
            {
                url = "https://www.ps3838.com/member-service/v1/account-balance?locale=en_US&withCredentials=true ";
                response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var headers = response.Content.ReadAsStringAsync().Result;
                    var json = (JObject)JsonConvert.DeserializeObject(headers);
                    result = json["cashBalance"].ToString() + " " + json["currency"].ToString();
                }
            }
            catch (Exception ex)
            {
                LogTools.HandleException(ex, "Ps3838 Error");
                Login();
            }
      
            return result;
        }
        public LiveAndToday getLeauge()
        {
            var utcTime = DateTime.UtcNow;
            if (utcTime.Hour == 21 && utcTime.Minute <= 10)
            {
                LogTools.WriteToLog("Execting Utc check " + utcTime.ToString());
               // return new LiveAndToday { Live = new List<Soccer>(), Today = new List<Soccer>() };
            }
            string result = "";
            try
            {
                url = "https://www.ps3838.com/sports-service/sv/compact/events?l=3&locale=en_US";
                response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                }
                return new LiveAndToday { Live = getLeaugeLive(result), Today = getSccorData(result) };
            }
            catch (Exception ex)
            {
                LogTools.HandleException(ex, "Ps3838 Error");
                Login();
                return new LiveAndToday { Live = new List<Soccer>(), Today = new List<Soccer>() };
            }
        }

        public List<Soccer> getLeaugeLive(string jsonString)
        {
            List<Soccer> socs = new List<Soccer>();
            try
            {
                var json = (JObject)JsonConvert.DeserializeObject(jsonString);
                var n = json["l"][0][2].Children();

                foreach (var nn in n)
                {
                    string leagu = nn[1].ToString();
                    string leaugeNo = nn[0].ToString();
                    var nn2 = nn[2].Children();
                    foreach (var n3 in nn2)
                    {
                        Soccer sc = new Soccer();
                        sc.League = leagu;
                        sc.LeagueNo = leaugeNo;
                        string team1 = n3[1].ToString();
                        string team2 = n3[2].ToString();
                        sc.TeamPs = team1 + " -vs- " + team2;
                        sc.Team1 = team1.Trim();
                        sc.Team2 = team2.Trim();
                        sc.TimePs = UnixTimeStampToDateTime(Convert.ToDouble(n3[4]));
                        var n8 = n3[8].First();
                        var tt2 = n8.Children();

                        foreach (var tt3 in tt2)
                        {
                            var coutn = tt3[0].Count();
                            if (coutn == 0)
                            {
                                continue;
                            }
                            var temp = tt3[0][0];
                            coutn = tt3[0].Count();
                            if (coutn > 1)
                            {
                                temp = tt3[0][0];
                                sc.HDPoddPs = temp == null ? "" : temp[2].ToString();
                                sc.HDPPs = temp == null ? "" : temp[3].ToString() + " - " + temp[4].ToString();
                                temp = tt3[0][1];
                                sc.HDPoddPs += temp == null ? "" : "\n" + temp[2].ToString();
                                sc.HDPPs += temp == null ? "" : "\n" + temp[3].ToString() + " - " + temp[4].ToString();
                                if (coutn > 2)
                                {
                                    temp = tt3[0][2];
                                    sc.HDPoddPs += temp == null ? "" : "\n" + temp[2].ToString();
                                    sc.HDPPs += temp == null ? "" : "\n" + temp[3].ToString() + " - " + temp[4].ToString();
                                }
                            }

                            coutn = tt3[1].Count();
                            if (coutn > 1)
                            {
                                temp = tt3[1][0];
                                sc.OUoddPs = temp == null ? "" : temp[0].ToString();
                                sc.OUPs = temp == null ? "" : temp[2].ToString() + " - " + temp[3].ToString();
                                temp = tt3[1][1];
                                sc.OUoddPs += temp == null ? "" : "\n" + temp[0].ToString();
                                sc.OUPs += temp == null ? "" : "\n" + temp[2].ToString() + " - " + temp[3].ToString();
                                if (coutn > 2)
                                {
                                    temp = tt3[1][2];
                                    sc.OUoddPs += temp == null ? "" : "\n" + temp[0].ToString();
                                    sc.OUPs += temp == null ? "" : "\n" + temp[2].ToString() + " - " + temp[3].ToString();
                                }

                            }
                            var value = tt3[2].ToString().Trim();

                            sc.HomePs = string.IsNullOrEmpty(value) ? "" : tt3[2][1].ToString();
                            sc.AwayPs = string.IsNullOrEmpty(value) ? "" : tt3[2][0].ToString();
                            sc.DrawPs = string.IsNullOrEmpty(value) ? "" : tt3[2][2].ToString();
                        }
                        socs.Add(sc);
                    }
                }
            }
            catch (Exception ex)
            {
                LogTools.HandleException(ex, "Ps3838 Error");
                Login();
            }
            return socs;
        }

        private string GetOddsFormat(string v)
        {
            string slash = "\n";
            if (v.Contains("-"))
            {
                var ss = v.Split('-')[0];
                v += slash + ss;
            }
            else if(v == "0.0")
            {
                v += slash + "0-0.5";
            }
            switch (v)
            {
                case "0.5": v += slash + "0-0.5";
                    break;
                case "1.0":  v += slash + "0.5-1";
                     break;    
                case "1.5":  v += slash + "1-1.5";
                    break;     
                case "2.0":  v += slash + "1.5-2";
                     break;     
                case "2.5": v += slash+ "2-2.5";
                    break;        
                case "3.0":  v += slash + "2.5-3";
                    break;     
                case "3.5": v += slash + "3-3.5";
                    break;    
                case "4.0": v += slash + "3.5-4";
                    break;     
                case "4.5": v += slash + "4-4.5";
                    break;     
                case "5.0": v += slash + "4.5-5";
                    break;     
                case "5.5":v += slash + "5-5.5";
                    break;     
                case "6.0": v += slash + "5.5-6";
                    break;     
                case "6.5": v += slash + "6-6.5";
                    break;      
                case "7.0": v += slash + "6.5-7";
                    break;     
                case "7.5":v += slash + "7-7.5";
                    break;  
                case "8.0": v += slash + "7.5-8";
                    break;    
                case "8.5": v += slash + "8-8.5";
                    break;      
            }

            return slash + v;
        }

        private List<Soccer> getSccorData(string jsonString)
        {
            List<Soccer> socs = new List<Soccer>();
            try
            {
            var json = (JObject)JsonConvert.DeserializeObject(jsonString);
            var n = json["n"][0][2].Children();
            //if (n.Count() == 0)
            //{
            //    throw new Exception();
            //}
            foreach (var nn in n)
            {
                string leagu = nn[1].ToString();
                string leaugeNo = nn[0].ToString();
                var nn2 = nn[2].Children();
                foreach (var n3 in nn2)
                {
                    Soccer sc = new Soccer();
                    sc.League = leagu;
                    sc.LeagueNo = leaugeNo;
                    string team1 = n3[1].ToString();
                    string team2 = n3[2].ToString();
                    sc.TeamPs = team1 + " -vs- " + team2;
                    sc.Team1 = team1.Trim();
                    sc.Team2 = team2.Trim();
                    sc.TimePs = UnixTimeStampToDateTime(Convert.ToDouble(n3[4]));
                    var n8 = n3[8].First();
                    var tt2 = n8.Children();

                    foreach (var tt3 in tt2)
                    {
                        var coutn = tt3[0].Count();
                        if (coutn == 0)
                        {
                            continue;
                        }
                        var temp = tt3[0][0];
                        coutn = tt3[0].Count();
                        if (coutn > 1)
                        {
                            temp = tt3[0][0];
                            sc.HDPoddPs = temp == null ? "" : temp[2].ToString();
                            sc.HDPPs = temp == null ? "" : temp[3].ToString() + " - " + temp[4].ToString();
                            temp = tt3[0][1];
                            sc.HDPoddPs += temp == null ? "" : "\n" + temp[2].ToString();
                            sc.HDPPs += temp == null ? "" : "\n" + temp[3].ToString() + " - " + temp[4].ToString();
                            temp = tt3[0][2];
                            sc.HDPoddPs += temp == null ? "" : "\n" + temp[2].ToString();
                            sc.HDPPs += temp == null ? "" : "\n" + temp[3].ToString() + " - " + temp[4].ToString();
                        }

                        coutn = tt3[1].Count();
                        if (coutn > 1)
                        {
                            temp = tt3[1][0];
                            sc.OUoddPs = temp == null ? "" : temp[0].ToString();
                            sc.OUPs = temp == null ? "" : temp[2].ToString() + " - " + temp[3].ToString();
                            temp = tt3[1][1];
                            sc.OUoddPs += temp == null ? "" : "\n" + temp[0].ToString();
                            sc.OUPs += temp == null ? "" : "\n" + temp[2].ToString() + " - " + temp[3].ToString();
                            temp = tt3[1][2];
                            sc.OUoddPs += temp == null ? "" : "\n" + temp[0].ToString();
                            sc.OUPs += temp == null ? "" : "\n" + temp[2].ToString() + " - " + temp[3].ToString();
                        }
                        var value = tt3[2].ToString().Trim();

                        sc.HomePs = string.IsNullOrEmpty(value) ? "" : tt3[2][1].ToString();
                        sc.AwayPs = string.IsNullOrEmpty(value) ? "" : tt3[2][0].ToString();
                        sc.DrawPs = string.IsNullOrEmpty(value) ? "" : tt3[2][2].ToString();
                    }
                    socs.Add(sc);
                }
            }
            }
            catch (Exception ex)
            {
                LogTools.HandleException(ex, "Ps3838 Error");
                Login();
            }
            return socs;
        }
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }

    }
}
