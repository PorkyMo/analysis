using System;
using DataAnalyst.Base;
using System.Collections.Generic;
using System.IO;

namespace UnitTestProject1
{
    public class Utils
    {
        public static List<PriceItem> ReadData(string filePath, DateTime startDay, DateTime lastDay)
        {
            var items = new List<PriceItem>();

            var sr = new StreamReader(filePath);

            sr.ReadLine();
            sr.ReadLine();
            var line = sr.ReadLine();
            decimal previousClose = 0;

            while (line != null)
            {
                var fields = line.Split(',');
                if (fields.Length < 2)
                {
                    break;
                }

                var item = new PriceItem()
                {
                    Date = Convert.ToDateTime(fields[0]),
                    Open = decimal.Parse(fields[1]),
                    High = decimal.Parse(fields[2]),
                    Low = decimal.Parse(fields[3]),
                    Close = decimal.Parse(fields[4]),
                    Volumn = decimal.Parse(fields[5]),
                    Amount = decimal.Parse(fields[6]),
                    PreviousClose = previousClose,
                    ItemPeriod = Period.Day
                };
                previousClose = item.Close;

                if (item.Date > lastDay)
                {
                    break;
                }
                if (item.Date >= startDay)
                {
                    items.Add(item);
                }

                line = sr.ReadLine();
            }

            sr.Close();

            return items;
        }
    }
}
