using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Enums;

namespace WaccaMyPageScraper.Localization
{
    public static class RateTextParser
    {
        public static string[] ToArray() => new string[]
        {
            Localization.Data.Rate_No,
            Localization.Data.Rate_D,
            Localization.Data.Rate_C,
            Localization.Data.Rate_B,
            Localization.Data.Rate_A,
            Localization.Data.Rate_AA,
            Localization.Data.Rate_AAA,
            Localization.Data.Rate_S,
            Localization.Data.Rate_S_Plus,
            Localization.Data.Rate_SS,
            Localization.Data.Rate_SS_Plus,
            Localization.Data.Rate_SSS,
            Localization.Data.Rate_SSS_Plus,
            Localization.Data.Rate_Master,
        };

        public static string Parse(Rate rate) => rate switch
        {
            Rate.D => Localization.Data.Rate_D,
            Rate.C => Localization.Data.Rate_C,
            Rate.B => Localization.Data.Rate_B,
            Rate.A => Localization.Data.Rate_A,
            Rate.AA => Localization.Data.Rate_AA,
            Rate.AAA => Localization.Data.Rate_AAA,
            Rate.S => Localization.Data.Rate_S,
            Rate.S_Plus => Localization.Data.Rate_S_Plus,
            Rate.SS => Localization.Data.Rate_SS,
            Rate.SS_Plus => Localization.Data.Rate_S_Plus,
            Rate.SSS => Localization.Data.Rate_SSS,
            Rate.SSS_Plus => Localization.Data.Rate_SSS_Plus,
            Rate.Master => Localization.Data.Rate_Master,
            _ => Localization.Data.Rate_No
        };
    }
}
