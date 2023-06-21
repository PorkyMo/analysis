using System;
using System.Collections.Generic;
using System.Net;
using DataAnalyst.Portfolio;

namespace DataAnalyst.MarketData
{
    public class MarketData
    {
        //private const string PriceUrl = "http://hq.sinajs.cn/rn=1413630633912&";
        private const string PriceUrl = "https://qt.gtimg.cn/q=";
        private const string InfoUrl = "http://hq.sinajs.cn/rn=1413630633912&";
        private static List<char> Deli;

        static MarketData()
        {
            Deli = new List<char>();
            foreach (char c in Environment.NewLine)
                Deli.Add(c);
        }

        public static Quote GetMarketData(string stockCode, string exchange)
        {
            RequestManager rm = new RequestManager();
            string uri = $"{PriceUrl}list={exchange}{stockCode}";
            HttpWebResponse r = rm.SendGETRequest(uri, null, null, true);
            string info = rm.GetResponseContent(r);

            string[] lines = info.Split(Deli.ToArray());
            return lines.Length > 0 ? ParseLine(lines[0]) : null;
        }

        public static List<Quote> GetMarketData(List<AlgoHolding> holdings)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var holding in holdings)
            {
                sb.Append($"{holding.Exchange}{holding.Code},");
            }

            if (sb.Length > 0)
            {
                sb.Length = sb.Length - 1;
            }
            string uri = $"{PriceUrl}{sb.ToString()}";

            var quotes = new List<Quote>();

            RequestManager rm = new RequestManager();
            HttpWebResponse r = rm.SendGETRequest(uri, null, null, true);
            string info = rm.GetResponseContent(r);
            string[] lines = info.Split(Deli.ToArray());
            foreach(var line in lines)
            {
                quotes.Add(ParseLine(line));
            }

            return quotes;
        }

        public static Quote ParseLine(string line)
        {
            try
            {
                if (line.StartsWith("v_sh") || line.StartsWith("v_sz"))
                {
                    // strip beginning v_sh600000=" and ending ";
                    string infoString = line.Substring("v_sh600000=\"".Length);
                    infoString = infoString.Substring(0, infoString.Length - 2);

                    string[] fields = line.Split('~');

                    Quote quote = new Quote();
                    quote.Exchange = line.Substring(2, 2);

                    //1 name, 2 code, 3 close, 4 previous, 5 open, 32 high, 33 low, 36 volume, 
                    quote.Name = fields[1];
                    quote.Code = fields[2];
                    quote.PreviousClose = decimal.Parse(fields[4]);
                    quote.Open = decimal.Parse(fields[5]);
                    quote.Close = decimal.Parse(fields[3]);
                    quote.High = decimal.Parse(fields[33]);
                    quote.Low = decimal.Parse(fields[34]);
                    quote.Volume = int.Parse(fields[36]);
                    return quote;
                }
            }
            catch
            {
            }
            return null;
        }
    }
}