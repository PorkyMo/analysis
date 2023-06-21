using System;
using System.Collections.Generic;
using System.IO;
using DataAnalyst.MarketData;

namespace DataAnalyst.Portfolio
{
    public class Portfolio
    {
        private const decimal StopLossPercentage = 0.06M; //from purchasePrice
        private const decimal StopProfitPercentage = 0.1M; //from highest price
        private const decimal ProfitDownToLossPercentage = 0.01M; //from purchasePrice
        private const string HoldingsFile = @"C:\git\analysis\ConsoleApplication1\Portfolio\Holdings.txt";

        public List<AlgoHolding> Holdings = new List<AlgoHolding>();

        public Portfolio()
        {
            Holdings = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AlgoHolding>>(File.ReadAllText(HoldingsFile));
        }

        public void StartWorking()
        {
            var timer = new System.Timers.Timer(10 * 60 * 1000); // 10 minutes
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void OnTimedEvent(Object o, System.Timers.ElapsedEventArgs e)
        {
            if (MarketTime())
            {
                Check();
            }
        }

        public void Check()
        {
            var data = MarketData.MarketData.GetMarketData(Holdings);
            foreach(var holding in Holdings)
            {
                Check(holding, data.Find(d => d.Code == holding.Code));
            }
            using (var sw = new StreamWriter(HoldingsFile, false))
            {
                sw.Write(Newtonsoft.Json.JsonConvert.SerializeObject(Holdings, Newtonsoft.Json.Formatting.Indented));
            }
        }

        private void Check(AlgoHolding holding, Quote quote)
        {
            if (holding == null || quote == null)
                return;

            var currentLatestPrice = holding.LatestPrice;
            holding.LatestPrice = quote.Close;

            if (holding.HighestPriceAfterPurchase < quote.Close)
            {
                holding.HighestPriceAfterPurchase = quote.Close;
                holding.HighestPriceFallAlerted = false;
            }

            if (!holding.StopLossAlerted)
            {
                var stopLossPrice = holding.PurchasePrice * (1 - StopLossPercentage);
                if (stopLossPrice >= quote.Close)
                {
                    Mail.MailService.SendStopLossAlert(holding.Code, holding.PurchasePrice, quote.Close);
                    holding.StopLossAlerted = true;
                    return;
                }
            }

            if (!holding.HighestPriceFallAlerted && holding.HighestPriceAfterPurchase != 0)
            {
                var alertPrice = holding.HighestPriceAfterPurchase * (1 - StopProfitPercentage);
                if (alertPrice >= quote.Close)
                {
                    Mail.MailService.SendStopProfitAlert(holding.Code, holding.PurchasePrice, quote.Close);
                    holding.HighestPriceFallAlerted = true;
                    return;
                }
            }

            if (!holding.ProfitDownToLossAlerted)
            {
                var alertPrice = holding.PurchasePrice * (1 + ProfitDownToLossPercentage);
                if (currentLatestPrice > alertPrice && quote.Close <= alertPrice)
                {
                    Mail.MailService.SendProfitDownToLossAlert(holding.Code, holding.PurchasePrice, quote.Close);
                    holding.ProfitDownToLossAlerted= true;
                    return;
                }
            }
        }

        private static bool MarketTime()
        {
            var currentHour = DateTime.Now.Hour;
            var currentMinute = DateTime.Now.Minute;
            return  (DateTime.Now.DayOfWeek != DayOfWeek.Saturday && DateTime.Now.DayOfWeek != DayOfWeek.Sunday)
                && ((currentHour == 12 && currentMinute >= 30) || currentHour == 13 || (currentHour == 14 && currentMinute <= 30)
                || currentHour == 16 || currentHour == 17);
        }
    }
}
