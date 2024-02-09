using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using FuzzySharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using PSSports.Properties;
using Timer = System.Windows.Forms.Timer;

namespace PSSports
{
    public partial class MainForm : Form
    {
        string iniFile = AppDomain.CurrentDomain.BaseDirectory + "\\Settings.ini";
        Ini myIni;
        private Thread _workThread = null;
        public MainForm()
        {
            InitializeComponent();
        }
        AccordionControl acControl;
        AccordionControlElement acItemActivity;
        AccordionControlElement acItemNews;
        // AccordionControl acControlRightside;
        // LabelControl labelControl1;
        object __lockObj = new object();
        BindingList<Soccer> bb = new BindingList<Soccer>();
        GridControl gridControl1;
        GridView gridView1;
        Timer t1 = new Timer();
        Sport sp;
        Sport16 sp16;
        LiveAndToday sp16Records =new LiveAndToday();
        List <Soccer> socs = new List<Soccer>();
        private void InitAccordionControl()
        {
            acControl.BeginUpdate();
            AccordionControlElement acRootGroupHome = new AccordionControlElement();
            acItemActivity = new AccordionControlElement();
            acItemNews = new AccordionControlElement();
            AccordionControlElement acRootItemSettings = new AccordionControlElement();

            acControl.Cursor = Cursors.Hand;
            acRootGroupHome.Elements.AddRange(new AccordionControlElement[] {
            acItemActivity,
            acItemNews});
            acRootGroupHome.Expanded = true;
            //acRootGroupHome.ImageOptions.ImageUri.Uri = "Home;Office2013";
            acRootGroupHome.Name = "acRootGroupHome";
            acRootGroupHome.Text = "Home";
            // 
            
            // Child Item 'Activity'
            // 
            acItemActivity.Name = "acItemActivity";
            acItemActivity.Style = ElementStyle.Item;
            acItemActivity.Tag = "idActivity";
            acItemActivity.Text = "Live";
            acItemActivity.Click += LiveActivity_Click;
            acItemActivity.Enabled = false;
           
            // 
            // Child Item 'News'
            // 
            acItemNews.Name = "acItemNews";
            acItemNews.Style = ElementStyle.Item;
            acItemNews.Tag = "idNews";
            acItemNews.Text = "Today";
            acItemNews.Click += AcItemNews_Click;
            // 
            // Root Item 'Settings' with ContentContainer
            // 
            //acRootItemSettings.ImageOptions.ImageUri.Uri = "Customization;Office2013";
            acRootItemSettings.Name = "acRootItemSettings";
            acRootItemSettings.Style = ElementStyle.Item;
            acRootItemSettings.Text = "Settings";
            // 


            //toggleSwitch1.Toggled += new System.EventHandler(this.toggleSwitch1_Toggled);

            acControl.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[] {
                acRootGroupHome
               // ,acRootItemSettings
            });

           // acRootItemSettings.Expanded = true;

            acControl.EndUpdate();
        }

        private void AcItemNews_Click(object sender, EventArgs e)
        {
            acItemActivity.Enabled = true;
            acItemNews.Enabled = false;
            GetLeaugeAndLoad();
        }

        private void LiveActivity_Click(object sender, EventArgs e)
        {
            acItemActivity.Enabled = false;
            acItemNews.Enabled = true;
            GetLeaugeAndLoad();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            myIni = new Ini(iniFile);
            myIni.Load();
            gb1.Text = myIni.GetValue("UserPs") + "/" + myIni.GetValue("User16");

            LoadGridControl();

            t1.Interval = 4000;
            t1.Tick += T1_Tick;
            t1.Enabled = true;
          //  LogTools.WriteToLog("t1 timer started");
        }

        private void LoadGridControl()
        {
            acControl = new AccordionControl();
            acControl.Dock = DockStyle.Left;
            acControl.Parent = this;
            acControl.Width = 200;
            InitAccordionControl();

            panel1.Controls.Add(acControl);

            gridControl1 = new GridControl()
            {
                Name = "gridControl1",
                Parent = this,
                Dock = DockStyle.Fill
            };
          //  BindingList<Soccer> bb = new BindingList<Soccer>();
            foreach (var sc in socs)
            {
                bb.Add(sc);
            }
            gridControl1.DataSource = bb;
            gridView1 = gridControl1.MainView as GridView;

            // Obtain created columns.
            GridColumn colCompany = gridView1.Columns["League"];
            GridColumn colID = gridView1.Columns["LeagueNo"];
            GridColumn colDate = gridView1.Columns["TimePs"];
            GridColumn colTeam = gridView1.Columns["TeamPs"];
            GridColumn hdpOdd = gridView1.Columns["HDPoddPs"];
            GridColumn ouOdd = gridView1.Columns["OUoddPs"];
            GridColumn colFirst = gridView1.Columns["HDPPs"];
            GridColumn colFirst2 = gridView1.Columns["OUPs"];
            GridColumn team1 = gridView1.Columns["Team1"];
            GridColumn tesm2 = gridView1.Columns["Team2"];
            GridColumn home = gridView1.Columns["HomePs"];
            GridColumn away = gridView1.Columns["AwayPs"];
            GridColumn draw = gridView1.Columns["DrawPs"];
            GridColumn hdp16 = gridView1.Columns["HDPOddNova"];
            GridColumn ou16 = gridView1.Columns["OUoddNova"];
            GridColumn hd16 = gridView1.Columns["HDPNova"];
            GridColumn o16 = gridView1.Columns["OUNova"];

            GridColumn homeN = gridView1.Columns["HomeNova"];
            GridColumn awayN = gridView1.Columns["AwayNova"];
            GridColumn drawN = gridView1.Columns["DrawNova"];

            RepositoryItemMemoEdit memoEdit = new RepositoryItemMemoEdit();
            team1.Visible = false;
            tesm2.Visible = false;
            hdpOdd.ColumnEdit = memoEdit;
            hdp16.ColumnEdit = memoEdit;
            ou16.ColumnEdit = memoEdit;
            hd16.ColumnEdit = memoEdit;
            o16.ColumnEdit = memoEdit;
            ouOdd.ColumnEdit = memoEdit;
            colFirst.ColumnEdit = memoEdit;
            colFirst2.ColumnEdit = memoEdit;
            // Hide a column.
            colID.Visible = false;
            colCompany.GroupIndex = 0;
            colDate.SortIndex = 0;


            GridGroupSummaryItem item = new GridGroupSummaryItem();

            gridView1.GroupSummary.Add(item);

            GridGroupSummaryItem item1 = new GridGroupSummaryItem();
            item1.FieldName = colFirst.FieldName;

            gridView1.GroupSummary.Add(item1);

            gridView1.FocusedRowHandle = 1;
            gridView1.OptionsBehavior.Editable = false;
            gridView1.OptionsBehavior.AutoExpandAllGroups = true;
            gridView1.FocusedColumn = colCompany;

            gridView1.OptionsView.RowAutoHeight = true;
            //gridView1.CustomRowCellEdit += (sender, e) =>
            //{
            //    //  e.RepositoryItem.Appearance.BackColor
            //};
            //gridView1.CalcRowHeight += (sender, e) => {
            //  ///  if (e.RowHandle % 2 == 0)
            //  //  {
            //        e.RowHeight = 50;
            //   // }
            //};
            // colTeam.BestFit();
     
            colTeam.Width = 180;
            home.Width = 40;
            away.Width = 40;
            draw.Width = 40;
            homeN.Width = 40;
            awayN.Width = 40;
            drawN.Width = 40;
            //hdpOdd.Width = 40;
            //ouOdd.Width = 40;
            hdpOdd.BestFit();
            ouOdd.BestFit();
            colDate.BestFit();
            colFirst2.BestFit();
            panel2.Controls.Add(gridControl1);
        }
        private void T1_Tick(object sender, EventArgs e)
        {
            Monitor.Enter(__lockObj);
            if (_workThread == null)
            {
                _workThread = new Thread(() =>
                {
                    try
                    {
                        GetLeaugeAndLoad();
                    }
                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                        _workThread = null;
                    }
                });
                _workThread.Start();
            }
            Monitor.Exit(__lockObj);
        }

        private void GetLeaugeAndLoad()
        {
            try
            {
                lblScanTime.Invoke(new Action(()=> { lblScanTime.Text = "Scan : "+ DateTime.Now.ToString(); })); 
                if (sp == null || sp16 == null)
                {
                    return;
                }
                if (lblBal16.Text == "" || lblBalps.Text == "")
                {
                    return;
                }
                socs.Clear();
                //  List<Soccer> test = new List<Soccer>();
                if (acItemActivity.Enabled)
                {
                    socs = sp.getLeauge().Today;
                    LogTools.WriteToSoccer(socs, true);
                    if (sp16 != null)
                    {
                        sp16Records.Today = sp16.GetMatchesSoccer();
                        LogTools.WriteToSoccer(sp16Records.Today, false);
                        foreach (var t1 in socs)
                        {
                            foreach (var t2 in sp16Records.Today)
                            {
                                var perTeam1 = Fuzz.Ratio(t1.Team1, t2.Team1);
                                var perTeam2 = Fuzz.Ratio(t1.Team2, t2.Team2);
                                if (perTeam1 > 55 && perTeam2 > 55)
                                {
                                    var ou = MatchMake(t1.OUoddPs, t2.OUoddNova, t2.OUNova);
                                    var hdp = MatchMake(t1.HDPoddPs, t2.HDPOddNova, t2.HDPNova);
                                    t1.OUoddNova = ou[0];
                                    t1.HDPOddNova = hdp[0];
                                    t1.OUNova = ou[1];
                                    t1.HDPNova = hdp[1];
                                  //  if (!string.IsNullOrEmpty(t1.HomePs))
                                   // {
                                        t1.HomeNova = t2.HomeNova;
                                        t1.AwayNova = t2.AwayNova;
                                        t1.DrawNova = t2.DrawNova;
                                  //  }
                    
                                    break;
                                    //test.Add(new Soccer()
                                    //{
                                    //    Team = t2.Team,
                                    //    League = t1.League,
                                    //    OUodd168977 = t2.OUodd,
                                    //    HDPOdd168977 = t2.HDPodd
                                    //});
                                }
                            }
                        }
                    }
                }
                else
                {
                    socs = sp.getLeauge().Live;
                    LogTools.WriteToSoccer(socs, true);
                    if (sp16 != null)
                    {
                        sp16Records.Live = sp16.GetMatchesLive();
                        LogTools.WriteToSoccer(sp16Records.Live, false);
                        foreach (var t1 in socs)
                        {
                            foreach (var t2 in sp16Records.Live)
                            {
                                var perTeam1 = Fuzz.Ratio(t2.Team1, t1.Team1);
                                var perTeam2 = Fuzz.Ratio(t2.Team2, t1.Team2);
                                //if (t2.Team2.Contains("Adamstown Rosebud"))
                                //{

                                //}
                                if (perTeam1 > 55 && perTeam2 > 55)
                                {
                                    var ou = MatchMake(t1.OUoddPs, t2.OUoddNova, t2.OUNova);
                                    var hdp = MatchMake(t1.HDPoddPs, t2.HDPOddNova, t2.HDPNova);
                                    t1.OUoddNova = ou[0];
                                    t1.HDPOddNova = hdp[0];
                                    t1.OUNova = ou[1];
                                    t1.HDPNova = hdp[1];
                                  // if (!string.IsNullOrEmpty(t1.HomePs))
                                 //   {
                                        t1.HomeNova = t2.HomeNova;
                                        t1.AwayNova = t2.AwayNova;
                                        t1.DrawNova = t2.DrawNova;
                                 //   }
                                    break;
                                    //test.Add(new Soccer()
                                    //{
                                    //    Team = t2.Team,
                                    //    League = t1.League,
                                    //    OUodd168977 = t2.OUodd,
                                    //    HDPOdd168977 = t2.HDPodd
                                    //});
                                }
                            }
                        }
                    }
                }
                //  socs.AddRange(test);

                BindingList<Soccer> bbc = new BindingList<Soccer>();
                foreach (var sc in socs)
                {
                    if (sc.OUNova==null && sc.HDPNova == null 
                        && sc.HDPOddNova == null && sc.OUoddNova == null)
                    {
                        continue;
                    }

                    bbc.Add(sc);
                }
                LogTools.WriteToMatching(bbc);
                // gridControl1.Invoke().DataSource = bbc;
                // gridView1.RefreshData();
                gridControl1.Invoke(new Action(() => { gridControl1.DataSource = bbc;  }));
            }
            catch (Exception ex)
            {

            }
            finally
            {
               
            }
        }

        private string SetupSameLine(string oddNova, string Nova)
        {
            if (Nova == null)
            {
                return "";
            }
            if (oddNova == null)
            {
                return "";
            }
            string result = "";
            var nova = Nova.Split(new string[] { "\n" }, StringSplitOptions.None);
            var novas = oddNova.Split(new string[] { "\n" }, StringSplitOptions.None);
            int i = 0;
            foreach (var ps in novas)
            {
                if (ps.Trim() == "")
                {
                    result += "  \n";
                }
                else
                {
                    result += nova[i] + "\n";
                    i++;
                }
            }
            return result;
        }

        private string[] MatchMake(string oddPs,string oddNova,string nova)
        {
            if (oddPs == null)
            {
                return new string[] {"",nova };
            }
            if (oddNova == null)
            {
                return new string[] { "", nova };
            }
            string result="";
            string resultNova = "";
            var pss = oddPs.Split(new string[] { "\n" }, StringSplitOptions.None);
            var novas = oddNova.Split(new string[] { "\n" }, StringSplitOptions.None);
            var navs = nova.Split(new string[] { "\n" }, StringSplitOptions.None);
            foreach (var ps in pss)
            {
                var exist = novas.Where(i => i.Trim() == ps.Trim()).FirstOrDefault();
                if (exist == null)
                {
                    result +=  "  \n";
                    resultNova += "  \n";
                }
                else
                {
                    int index = novas.ToList().IndexOf(exist);
                    if (resultNova.Contains(navs[index]))
                    {
                        if (novas.ToList().Where(i=>i.Contains(exist)).Count() > 1)
                        {
                            resultNova += navs[index + 1] + "\n";
                        }
                    }
                    else {
                        resultNova += navs[index] + "\n";
                    }
                    result += ps + "\n";
                }
            }
            
            return new string[] {result, resultNova };
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm s = new SettingsForm();
            s.ShowDialog();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (_workThread == null)
            {
                btnStart.Enabled = false;
                _workThread = new Thread(() =>
                {
                    try
                    {
                        StartButtonWork();
                    }
                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                       _workThread = null;
                    }
                });
                _workThread.Start();
            }
        }

        private void StartButtonWork()
        {
            if (String.IsNullOrEmpty(myIni.GetValue("User16")) && String.IsNullOrEmpty(myIni.GetValue("Pass16")))
            {
                MessageBox.Show("Please Enter UserName/Password For 168977.", "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            if (String.IsNullOrEmpty(myIni.GetValue("UserPs")) && String.IsNullOrEmpty(myIni.GetValue("PassPs")))
            {
                MessageBox.Show("Please Enter UserName/Password For PS3838.", "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            //   btnStart.Enabled = false;
            try
            {
                sp16 = new Sport16();
                int loginCount = 0;
                while (loginCount < 3)
                {
                    sp16 = null;
                    sp16 = new Sport16();
                    string Bal16 = sp16.Login();
                    lblBal16.Invoke(new Action(() => { lblBal16.Text = Bal16; }));
                    if (!string.IsNullOrEmpty(Bal16))
                    {
                        break;
                    }

                    loginCount++;
                }
                sp16Records = sp16.getLeauge();

                // lblBal16.Text = sp16.getBalnace();
                sp = new Sport();
                if (sp.Login())
                {
                    string psbal = sp.getBalance();
                    lblBalps.Invoke(new Action(() => { lblBalps.Text = psbal; }));
                    GetLeaugeAndLoad();

                    t1.Interval = 4000;
                    t1.Tick += T1_Tick;
                    t1.Enabled = true;
                   // LogTools.WriteToLog("t1 timer started");
                }
            }
            catch (Exception ex)
            {
                LogTools.HandleException(ex, "btnStart Error");
            }
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sp16 !=null)
            {
                sp16.CloseBrowser();
            }
        }
    }

}
