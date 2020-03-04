using System;
using System.Collections.Generic;
using System.Text;

namespace StdBdgRCCL.Helpers
{
    public static class EdFiGradeConverter
    {
        public static IDictionary<string, string> GradeLevels = new Dictionary<string, string>()
        {
            { "Preschool/Prekindergarten", "P4" },
            { "Kindergarten", "K" },
            { "First grade", "1" },
            { "Second grade", "2" },
            { "Third grade", "3" },
            { "Fourth grade", "4" },
            { "Fifth grade", "5" },
            { "Sixth grade", "6" },
            { "Seventh grade", "7" },
            { "Eighth grade", "8" },
            { "Ninth grade", "9" },
            { "Tenth grade", "10" },
            { "Eleventh grade", "11" },
            { "Twelfth grade", "12" },
        };

        public static string ConvertGrade(string grade)
        {
            if (GradeLevels.TryGetValue(grade, out string value))
            {
                return value;
            }
            else
            {
                return "Grade Conversion Failed";
            }
        }
    }
}
