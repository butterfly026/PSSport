using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSSports
{
    class LogTools
    {

        public static void HandleException(Exception ex, string message)
        {
            if (ex != null)
                WriteToLog(message + " Exception: " + ex.ToString());
            else
                WriteToLog(message);
        }

        public static void WriteToLog(string message)
        {

                try
                {
                    string filenameWithPath = Environment.CurrentDirectory + "\\log.txt";
                    if (File.Exists(filenameWithPath))
                    {
                        FileInfo fi = new FileInfo(filenameWithPath);

                        if (fi.Length > 50000000)
                        {
                            File.Delete(filenameWithPath);
                        }
                    }

                    using (TextWriter writer = new StreamWriter(filenameWithPath, true))
                    {
                        writer.WriteLine(DateTime.Now.ToString("dd HH:mm:ss.fff") + "- " + message);
                    }
                }
                catch (Exception)
                {
                }
            
        }

        public static void WriteToSoccer(List<Soccer> socs , bool isPS)
        {

            try
            {
                string message = "";
                foreach (var item in socs)
                {
                    message += item.TeamPs + "\n";
                }
                string fname = "\\Ps3838.txt";
                if (!isPS)
                {
                    fname = "\\168977.txt";
                }
                string filenameWithPath = Environment.CurrentDirectory + fname;
                if (File.Exists(filenameWithPath))
                {
                    FileInfo fi = new FileInfo(filenameWithPath);

                    if (fi.Length > 50000000)
                    {
                        File.Delete(filenameWithPath);
                    }
                }

                using (TextWriter writer = new StreamWriter(filenameWithPath, false))
                {
                    writer.WriteLine(DateTime.Now.ToString("dd HH:mm:ss.fff") + "\n" + message);
                }
            }
            catch (Exception)
            {
            }

        }

        public static void WriteToMatching(System.ComponentModel.BindingList<Soccer> socs)
        {

            try
            {
                string message = "";
                foreach (var item in socs)
                {
                    message += item.TeamPs + "\n";
                }
                string fname = "\\Matching.txt";
                string filenameWithPath = Environment.CurrentDirectory + fname;
                if (File.Exists(filenameWithPath))
                {
                    FileInfo fi = new FileInfo(filenameWithPath);

                    if (fi.Length > 50000000)
                    {
                        File.Delete(filenameWithPath);
                    }
                }

                using (TextWriter writer = new StreamWriter(filenameWithPath, false))
                {
                    writer.WriteLine(DateTime.Now.ToString("dd HH:mm:ss.fff") + "\n" + message);
                }
            }
            catch (Exception)
            {
            }

        }
    }
}
