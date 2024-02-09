using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PSSports.Properties;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using HtmlAgilityPack;

namespace PSSports
{
    class Sport16
    {
        HttpClient client;
        ChromeDriver driver;
        string url = string.Empty;
        HttpResponseMessage response;
        string iniFile = AppDomain.CurrentDomain.BaseDirectory + "\\Settings.ini";
        public string Login()
        {
            try
            {
                var myIni = new Ini(iniFile);
                myIni.Load();
                //k024ob.ofje104.com
                var chromeDriverProcesses = System.Diagnostics.Process.GetProcessesByName("chromedriver");

                foreach (var chromeDriverProcess in chromeDriverProcesses)
                {
                    chromeDriverProcess.Kill();
                }

                ChromeDriverService cService = ChromeDriverService.CreateDefaultService();
                cService.HideCommandPromptWindow = true;
                ChromeOptions options = new ChromeOptions();
                options.AddArgument("--start-maximized");
                // options.AddArgument("--headless");
                driver = new ChromeDriver(cService, options, TimeSpan.FromSeconds(300));
                driver.Navigate().GoToUrl("https://www.168977.net/Default.aspx?IsSSL=1&hidSelLang=en&fromMode=3");
                Thread.Sleep(1000);
                var loginLink = driver.FindElement(By.LinkText("Login"));
                loginLink.Click();
                var loginFrom = driver.FindElement(By.Id("frmLogin"));
                loginFrom.FindElement(By.Id("txtID")).SendKeys(myIni.GetValue("User16"));
                loginFrom.FindElement(By.Id("txtPW")).SendKeys(myIni.GetValue("Pass16"));
                loginLink = loginFrom.FindElement(By.LinkText("Login"));
                loginLink.Click();

                var cokkies = driver.Manage().Cookies.AllCookies;
                var c_k = "";
                foreach (var item in cokkies)
                {
                    c_k += item.ToString() + ";";
                }
               // driver.Quit();
                Thread.Sleep(2000);
                //string html = "";
                //c_k = "LangKey=en; _gid=GA1.2.1167635836.1680693653; LoginName=CKSAE00T3O; ASP.NET_SessionId=rtkv0xzyxigftrrbbjoi2j3n; .ASPXAUTH=74311B8A76FE6DFEB19ED4470F9371FCBED11B801E4EC37D64F310D19322C14BFD806B5E49397B62F483F83FB484B2AB9BFE82BAE7BC0F5DCEB20C70537B228D86AF15531A9041592EAA69D20A42CD4F877845A45A6029E20F47C9AE5C155CB3; _culture=en-US; _v1promo=1; _ga=GA1.1.230945528.1680693639; _ga_DG55XYWTEJ=GS1.1.1680693673.1.0.1680693673.0.0.0; _clck=rwb180|1|fai|0; reese84=3:G+VyIpOZZ8k/8lmepR/4qQ==:0G5ZdPQlmunPM/NejR9jw+lvzdeGYtiB5oP2X+z9lXvuJvp0N3omAyRWjk8/xJ49elBmeo7j761FemroLInYPEr+uD5vr7zD1TowpEic6Gj/s9MhnePIbmkROMHjsii63I0aQScksFOEo1ldXL83VPCG8OtgAOT1vbNzl96OSDVg2a8nxs1ZldR4xv27LTx10WdF8EOMftb+kpCZZwiEUGHYB4DiZ6toR4uFrJ9K+UJ7hUGakgnZM/lU3We4LGVSz1iqrCly3GpH/bmPtw5/nOYTmrA5HekTXYwcuKRNwFUIDrTFHPuQXTR0OuUncqLeADrws6fs377RrlXmPIMXldphnnU8FueTEzTaYCP5pjhGDa7N9Lt0Z90KAzGZjgm49ed4aa1VFVTFDGkyDcNrs/BbQd8X0SrRXc+fVHImhILBj3kNbtNqbHgHIdakPu85iB0ahQAfjWVJovqMQOVklfwFVo8Rtglpz9m9VuRZzKw=:xmJ3BtviB7N7smG5t8q22zSBmr6iRfQpRjzcLGDM8Ok=; _clsk=oku7f5|1680693734985|1|1|y.clarity.ms/collect";
                url = "https://bth45.168977.net/Customer/Balance";
                client = new HttpClient(new HttpClientHandler { UseCookies = false });
                client.DefaultRequestHeaders.Add("Accept", "*/*");
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36");
                client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                client.DefaultRequestHeaders.Add("Cookie", c_k);
                //var content = new StringContent("", Encoding.UTF8);
                //Thread.Sleep(2000);
                //response = client.GetAsync(url).Result;

                //if (response.IsSuccessStatusCode)
                //{
                //    string JsonResponce = response.Content.ReadAsStringAsync().Result;
                //    var json = (JObject)JsonConvert.DeserializeObject(JsonResponce);
                //    var n = json["ErrorCode"].ToString();
                //    if (n == "0")
                //    {
                //        return json["Data"]["BCredit"].ToString() + " " + json["Data"]["Curr"].ToString();
                //    }
                //    return "";
                //}
                var account = driver.FindElement(By.CssSelector("div.c-side-account")).Text.Replace("\r\n"," ");
                if (account != "")
                {
                    string balance = account.Substring(account.IndexOf("Bet Credit"));
                    balance = balance.Remove(balance.IndexOf("Cash")).Replace("Bet Credit","");
                    if (balance !="")
                    {
                        return balance;
                    }
                }

                return "";
            }
            catch (Exception ex)
            {
                LogTools.HandleException(ex, "Sport16 Error");
                CloseBrowser();
                return "";
            }
        }


        public LiveAndToday getLeauge()
        {

            var live = GetMatchesLive();
           // var today = GetMatchesSoccer();
            return new LiveAndToday { Live = live, Today = null };
        }
        public List<Soccer> GetMatchesSoccer()
        {
         
   
            List<Soccer> soccers = new List<Soccer>();
            try
            {
             string url = driver.Url;
             var uri = new Uri(url);
            string liveUrl = $"https://{uri.Host}/Sports/?market=T";

           if (url != liveUrl)
            {
                driver.Navigate().GoToUrl(liveUrl);
                Thread.Sleep(5000);
            }
            var html = driver.PageSource;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
        
            var mainArea = doc.DocumentNode.SelectSingleNode("//div[@id='mainArea']");
            if (mainArea != null)
            {
                var tables = mainArea.SelectNodes(".//div[@class='c-odds-page']/div");
                if (tables != null)
                {
                    foreach (var table in tables)
                    {
                        var header = table.SelectSingleNode(".//div[@class = 'c-odds-table__header']");
                        if (header != null)
                        {
                            string headerText = header.InnerText;
                            if (headerText.Contains("Event"))
                            {
                                var c_leauges = table.SelectNodes(".//div[@class='c-league']");
                                if (c_leauges != null)
                                {
                                    foreach (var c_leauge in c_leauges)
                                    {
                                        var leauge_header = c_leauge.SelectSingleNode(".//div[@class = 'c-league__info']");
                                        if (leauge_header != null)
                                        {

                                            string l_header = leauge_header.InnerText.Replace("Soccer /", "").Trim();
                                            var oddsGroup = c_leauge.SelectNodes(".//div[@class = 'c-match__odds-group']");
                                            if (oddsGroup != null)
                                            {
                                                foreach (var oddG in oddsGroup)
                                                {

                                                    var odds = oddG.SelectNodes(".//div[@class = 'c-match__odds']");
                                                    if (odds == null)
                                                    {
                                                        continue;
                                                    }
                                                    var names = odds[0].SelectNodes(".//div[@class = 'c-match__team']");
                                                        if (names == null)
                                                        {
                                                            continue;
                                                        }
                                                        string team1 = names[0].InnerText;
                                                    string team2 = names[1].InnerText;
                                                    string oddHDP = "";
                                                    string oddOU = "";
                                                    string HDP = "";
                                                    string OU = "";
                                                    string home = "";
                                                    string away = "";
                                                    string draw = "";
                                                   foreach (var odd in odds)
                                                    {
                                                        var c_odds = odd.SelectNodes(".//span[starts-with(@class,'c-odds')]");
                                                        if (c_odds != null)
                                                        {
                                                            if (c_odds.Count >= 4)
                                                            {
                                                                HDP += c_odds[0].InnerText + "    " + c_odds[1].InnerText + "\n";
                                                                OU += c_odds[2].InnerText + "    " + c_odds[3].InnerText + "\n";

                                                                if (home == "")
                                                                    {
                                                                        if (c_odds.Count > 4)
                                                                            home = c_odds[4].InnerText;
                                                                        if (c_odds.Count > 5)
                                                                            away = c_odds[5].InnerText;
                                                                        if (c_odds.Count > 6)
                                                                            draw = c_odds[6].InnerText;
                                                                    }
                                                             }
                                                        }
                                                        var oddsButton = odd.SelectNodes(".//span[@class = 'c-text-goal']");
                                                        if (oddsButton != null)
                                                        {
                                                            if (oddsButton.Count == 1)
                                                            {
                                                                continue;
                                                            }
                                                            string oh = oddsButton[0].InnerText.Replace("/", "-").Trim();
                                                            string ou = oddsButton[1].InnerText.Replace("/", "-").Trim();
                                                            oddHDP += (oh == "0" ? "0.0" : oh) + "\n";
                                                            oddOU += (ou == "0" ? "0.0" : ou) + "\n";

                                                        }
                                                    }
                                                    soccers.Add(new Soccer()
                                                    {
                                                        Team1 = team1.Trim(),
                                                        Team2 = team2.Trim(),
                                                        League = l_header,
                                                        TeamPs = team1 + " -vs- " + team2,
                                                        HDPOddNova = oddHDP,
                                                        OUoddNova = oddOU,
                                                        HDPNova = HDP,
                                                        OUNova = OU,
                                                        HomeNova = home,
                                                        AwayNova = away,
                                                        DrawNova = draw,
                                                    });
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                        }

                    }
                }
                }
                else
                {
                    CloseBrowser();
                    Login();
                }
            }
            catch (Exception ex)
            {
                LogTools.HandleException(ex, "Sport16 Error");
                CloseBrowser();
                Login();
            }
            return soccers;
        }
        public List<Soccer> GetMatchesLive()
        {
      
            List<Soccer> soccers = new List<Soccer>();
            try
            {
            string url = driver.Url;
            var uri = new Uri(url);
            string liveUrl = $"https://{uri.Host}/Sports/1/?market=L";
           if (url != liveUrl)
            {
                driver.Navigate().GoToUrl(liveUrl);
                Thread.Sleep(5000);
            }

            var html = driver.PageSource;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
    
            var mainArea = doc.DocumentNode.SelectSingleNode("//div[@id='mainArea']");
            if (mainArea != null)
            {
                var tables = mainArea.SelectNodes(".//div[@class='c-odds-page']/div");
                if (tables != null)
                {
                    foreach (var table in tables)
                    {
                        var header = table.SelectSingleNode(".//div[@class = 'c-odds-table__header']");
                        if (header != null)
                        {
                            string headerText = header.InnerText;
                            if (headerText.Contains("Soccer"))
                            {
                                var c_leauges = table.SelectNodes(".//div[@class='c-league']");
                                if (c_leauges != null)
                                {
                                    foreach (var c_leauge in c_leauges)
                                    {
                                        var leauge_header = c_leauge.SelectSingleNode(".//div[@class = 'c-league__info']");
                                        if (leauge_header != null)
                                        {

                                            string l_header = leauge_header.InnerText.Replace("Soccer /", "").Trim();
                                            var oddsGroup = c_leauge.SelectNodes(".//div[@class = 'c-match__odds-group']");
                                            if (oddsGroup != null)
                                            {
                                                foreach (var oddG in oddsGroup)
                                                {

                                                    var odds = oddG.SelectNodes(".//div[@class = 'c-match__odds']");
                                                    if (odds == null)
                                                    {
                                                        continue;
                                                    }
                                                    var names = odds[0].SelectNodes(".//div[@class = 'c-match__team']");
                                                        if (names == null)
                                                        {
                                                            continue;
                                                        }
                                                    string team1 = names[0].InnerText;
                                                    string team2 = names[1].InnerText;
                                                    string oddHDP = "";
                                                    string oddOU = "";
                                                    string HDP = "";
                                                    string OU = "";
                                                    string home = "";
                                                    string away = "";
                                                    string draw = "";
                                                    foreach (var odd in odds)
                                                    {
                                                        var c_odds = odd.SelectNodes(".//span[starts-with(@class,'c-odds')]");
                                                        if (c_odds != null)
                                                        {
                                                            if (c_odds.Count >= 4)
                                                            {
                                                                HDP += c_odds[0].InnerText + "    " + c_odds[1].InnerText + "\n";
                                                                OU += c_odds[2].InnerText + "    " + c_odds[3].InnerText + "\n";
                                                                if (home == "")
                                                                {
                                                                      if (c_odds.Count > 4)
                                                                             home = c_odds[4].InnerText;
                                                                      if (c_odds.Count > 5)
                                                                            away = c_odds[5].InnerText;
                                                                      if (c_odds.Count > 6)
                                                                            draw = c_odds[6].InnerText;
                                                                }
                                                            }
                                                        }
                                                        var oddsButton = odd.SelectNodes(".//span[@class = 'c-text-goal']");
                                                        if (oddsButton != null)
                                                        {
                                                            if (oddsButton.Count == 1)
                                                            {
                                                                continue;
                                                            }
                                                            string oh = oddsButton[0].InnerText.Replace("/", "-").Trim();
                                                            string ou = oddsButton[1].InnerText.Replace("/", "-").Trim();
                                                            oddHDP += (oh == "0"? "0.0" : oh) + "\n";
                                                            oddOU += (ou == "0" ? "0.0" : ou) + "\n";

                                                        }
                                                    }
                                                    soccers.Add(new Soccer()
                                                    {
                                                        Team1 = team1.Trim(),
                                                        Team2 = team2.Trim(),
                                                        League = l_header,
                                                        TeamPs = team1 + " -vs- " + team2,
                                                        HDPOddNova = oddHDP,
                                                        OUoddNova = oddOU,
                                                        HDPNova = HDP,
                                                        OUNova = OU,
                                                        HomeNova = home,
                                                        AwayNova =away,
                                                        DrawNova =draw,
                                                    });
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                        }

                    }
                }
                }
                else
                {
                    CloseBrowser();
                    Login();
                }
            }
            catch (Exception ex)
            {
                LogTools.HandleException(ex, "Sport16 Error");
                CloseBrowser();
                Login();
            }
            return soccers;
        }

        public void CloseBrowser()
        {
            if (driver !=null)
            {
                driver.Quit();
            }
        }
        public string getBalnace()
        {
            url = "https://bth45.168977.net/Customer/Balance";
            response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                string JsonResponce = response.Content.ReadAsStringAsync().Result;
                var json = (JObject)JsonConvert.DeserializeObject(JsonResponce);
                var n = json["ErrorCode"].ToString();
                if (n == "0")
                {
                    return json["Data"]["BCredit"].ToString() + " " + json["Data"]["Curr"].ToString();
                }
                return "";
            }
            return "";
        }
    }
}
