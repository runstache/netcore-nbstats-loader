using System;
using System.Collections.Generic;
using System.Text;

namespace NbaStats.Loader.Helpers
{
    public static class DataTypeHelper
    {
        public static double ConverToDouble(string value)
        {
            double.TryParse(value, out double result);
            return result;
        }

        public static int ConvertToInteger(string value)
        {
            int.TryParse(value, out int result);
            return result;
        }

        public static long ConvertToLong(string value)
        {
            long.TryParse(value, out long result);
            return result;
        }

        public static DateTime ConvertDateTime(string value)
        {
            DateTime.TryParse(value, out DateTime result);            
            return result;
        }
    }
}
