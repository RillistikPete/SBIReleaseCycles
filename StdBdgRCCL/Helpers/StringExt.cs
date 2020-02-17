using System;
using System.Collections.Generic;
using System.Text;

namespace StdBdgRCCL.Helpers
{
    public static class StringExt
    {
        public static Tuple<string, string> CalculateQuarter(this DateTime date)
        {
            if (date.Month >= 8 && date.Month <= 10)
            {
                return new Tuple<string, string>("Q1", "S1");
            }
            else if (date.Month >= 10 && date.Month <= 12)
            {
                return new Tuple<string, string>("Q2", "S1");
            }
            else if (date.Month >= 1 && date.Month <= 3)
            {
                return new Tuple<string, string>("Q3", "S2");
            }
            else if (date.Month >= 3 && date.Month <= 5)
            {
                return new Tuple<string, string>("Q4", "S2");
            }
            else
            {
                return new Tuple<string, string>("Outside of school year", "N/A");
            }
        }

        public static int ParseInt32(string str)
        {
            int result;
            return Int32.TryParse(str, out result) ? result : 0;
        }
    }
}
