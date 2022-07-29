using DataAnalyst.Base;
using System.Collections.Generic;
using System.IO;

namespace DataAnalyst.Watchlist
{
    public class WatchlistTrial
    {
        public static List<string> WatchlistCodes = new List<string>();
        public static StockDataSet DataSet { get; set; }

        public static void Init(StockDataSet fullData)
        {
            WatchlistCodes.Clear();
            ReadFile(@"c:\tt\ConsoleApplication1\ConsoleApplication1\watchlist\watchlist.txt");
            ReadFile(@"c:\tt\ConsoleApplication1\ConsoleApplication1\watchlist\topscore.txt");
            DataSet = fullData.GetSubSet(s => WatchlistCodes.Contains(s.Code));
        }

        private static void ReadFile(string file)
        {
            StreamReader sr = new StreamReader(file);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                var t = line.Split(',');
                if (!WatchlistCodes.Contains(t[0]))
                {
                    WatchlistCodes.Add(t[0]);
                }
            }
            sr.Close();
        }
    }
}
