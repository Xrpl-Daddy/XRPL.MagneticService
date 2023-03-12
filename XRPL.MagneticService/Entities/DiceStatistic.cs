using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRPL.MagneticService.Entities
{
    public class DiceStatistic
    {
        public string Wallet { get; set; }
        public string Code { get; set; }
        public string Issuer { get; set; }
        public uint TotalGames { get; set; }
        public uint TotalWins { get; set; }
        public uint TotalLoss { get; set; }
        public double LossBlanace { get; set; }
        public double WinBlanace { get; set; }
        public double TotalVolume { get; set; }
    }
}
