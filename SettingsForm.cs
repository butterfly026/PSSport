using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PSSports
{
    public partial class SettingsForm : Form
    {
        string iniFile = AppDomain.CurrentDomain.BaseDirectory + "\\Settings.ini";
        Ini myIni;
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //Settings.Default.UserPs = txtUserPs.Text;
            //Settings.Default.PassPs = txtPassPs.Text;
            //Settings.Default.User16 = txtUser16.Text;
            //Settings.Default.Pass16 = txtPass16.Text;
            //Settings.Default.Save();
            SaveIniSettings();
            MessageBox.Show("Settings Saved.");
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            myIni = new Ini(iniFile);
            myIni.Load();
            txtUserPs.Text = myIni.GetValue("UserPs");
            txtPassPs.Text = myIni.GetValue("PassPs");
            txtUser16.Text = myIni.GetValue("User16"); 
            txtPass16.Text = myIni.GetValue("Pass16"); 
        }

        private void SaveIniSettings()
        {
            try
            {
                myIni.WriteValue("UserPs", txtUserPs.Text.Trim());
                myIni.WriteValue("PassPs", txtPassPs.Text.Trim());
                myIni.WriteValue("User16", txtUser16.Text.Trim());
                myIni.WriteValue("Pass16", txtPass16.Text.Trim());
    
                myIni.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
