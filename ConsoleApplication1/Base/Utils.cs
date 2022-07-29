using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAnalyst.Base
{
    public static class Utils
    {
        public static bool Between(this DateTime dt, DateTime startDate, DateTime endDate)
        {
            return dt >= startDate && dt <= endDate;
        }

        private static bool HasFeatureWithThese(this string s, Func<string, bool> func, List<string> searchList)
        {
            foreach (var search in searchList)
                if (s.HasFeature(func, search))
                    return true;

            return false;
        }

        private static bool HasFeature(this string s, Func<string, bool> func, string param)
        {
            return func(param);
        }

        public static bool StartsWithThese(this string s, List<string> searchList)
        {
            return s.HasFeatureWithThese(s.StartsWith, searchList);
        }

        public static bool ContainsThese(this string s, List<string> searchList)
        {
            return s.HasFeatureWithThese(s.Contains, searchList);
        }
    }
}
