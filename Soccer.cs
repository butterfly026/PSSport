using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSSports
{
    class Soccer
    {
        public string League { get; set; }
        public string LeagueNo { get; set; }
        public string Team1 { get; set; }
        public string Team2 { get; set; }
        public string TeamPs { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm tt}")]
        public DateTime TimePs { get; set; }
        public string HDPoddPs { get; set; }
        public string HDPOddNova { get; set; }
        public string HDPPs { get; set; }
        public string HDPNova { get; set; }
        public string OUoddPs { get; set; }
        public string OUoddNova { get; set; }
        public string OUPs { get; set; }
        public string OUNova { get; set; }
        public string HomePs { get; set; }
        public string AwayPs { get; set; }
        public string DrawPs { get; set; }
        public string HomeNova { get; set; }
        public string AwayNova { get; set; }
        public string DrawNova { get; set; }
    }

    class LiveAndToday
    {
        public List<Soccer> Today { get; set; }
        public List<Soccer> Live { get; set; }
    }
}


