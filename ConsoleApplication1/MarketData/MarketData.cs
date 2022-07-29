using System;
using System.Collections.Generic;
using System.Net;
using DataAnalyst.Portfolio;

namespace DataAnalyst.MarketData
{
    public class MarketData
    {
        private const string PriceUrl = "http://hq.sinajs.cn/rn=1413630633912&";
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
            string uri = $"{PriceUrl}list={sb.ToString()}";

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
                if (line.StartsWith("var hq_str_sh") || line.StartsWith("var hq_str_sz"))
                {
                    string infoString = line.Substring("var hq_str_".Length);

                    string[] fields = line.Substring("var hq_str_sz002396=\"".Length).Split(',');

                    Quote quote = new Quote();
                    quote.Exchange = infoString.Substring(0, 2);
                    quote.Code = infoString.Substring(2, 6);

                    //0 name, 1 open, 2 previous, close, 3 close, 4 high, 5 low, 8 volume, 
                    quote.Name = fields[0];
                    quote.Open = decimal.Parse(fields[1]);
                    quote.PreviousClose = decimal.Parse(fields[2]);
                    quote.Close = decimal.Parse(fields[3]);
                    quote.High = decimal.Parse(fields[4]);
                    quote.Low = decimal.Parse(fields[5]);
                    quote.Volume = int.Parse(fields[8]);
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