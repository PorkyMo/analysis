using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAnalyst.Base
{
    public class StockData
    {
        private List<int> Intervals = new List<int>() { 5, 10, 20, 30, 60};
        public string Code { get; set; }
        public string Name { get; set; }
        public Exchange StockExchange { get; set; }
        public PriceList RawData = new PriceList();
        public List<AnalysedData> StockAnalysedData = new List<AnalysedData>();

        public void FindCross(Period period, int range, bool crossUp, DateTime startDate)
        {
            var analysedData = StockAnalysedData.Find(ad => ad.Period == period);
            if (analysedData != null)
            {
                analysedData.FindCross(range, crossUp, startDate);
            }
        }

        public void FindPoles(Period period, DateTime startDate, DateTime endDate)
        {
            var analysedData = StockAnalysedData.Find(ad => ad.Period == period);
            if (analysedData != null)
            {
                analysedData.FindPoles(startDate, endDate);
            }
        }

        public void FindDoublePole(Period period, DateTime startDate, DateTime endDate, Direction trend)
        {
            var analysedData = StockAnalysedData.Find(ad => ad.Period == period);
            if (analysedData != null)
            {
                analysedData.FindDoublePole(Code, startDate, endDate, trend);
            }
        }

        public void FindDoublePolesWithCross(Period period, int barCount, Direction trend)
        {
            var analysedData = StockAnalysedData.Find(ad => ad.Period == period);
            if (analysedData != null)
            {
                analysedData.FindDoublePolesWithCross(Code, barCount, trend);
            }
        }

        public CrossDataInfo GetCrossDataInfo(Period period)
        {
            return new CrossDataInfo
            {
                Code = Code,
                Name = Name,
                StockExchange = StockExchange,
                Period = period,
                CrossDataList = StockAnalysedData.Find(ad => ad.Period == period).StockCrossData
            };
        }

        public DoublePoleDataInfo GetDoublePoleDataInfo(Period period)
        {
            return new DoublePoleDataInfo
            {
                Code = Code,
                Name = Name,
                StockExchange = StockExchange,
                Period = period,
                DoublePoles = StockAnalysedData.Find(ad => ad.Period == period).DoublePoles
            };
        }

        public TimelineInfo GetTimelineInfo(Period period)
        {
            return new TimelineInfo
            {
                Code = Code,
                Name = Name,
                StockExchange = StockExchange,
                Period = period,
                Timeline = StockAnalysedData.Find(ad => ad.Period == period).Timeline
            };
        }

        public void BuildTimeline(Period period, DateTime startDate, DateTime endDate)
        {
            var analysedData = StockAnalysedData.Find(ad => ad.Period == period);
            if (analysedData != null)
            {
                this.StockAnalysedData.ForEach(d => d.BuildTimeLine(startDate, endDate));
            }
        }

        public static async Task<StockData> ReadData(string filePath, DateTime startDay, DateTime lastDay)
        {
            var stockData = new StockData();
            var sr = new StreamReader(filePath, System.Text.Encoding.GetEncoding(936));
            var line = await sr.ReadLineAsync().ConfigureAwait(false);
            try
            {
                stockData.StockExchange = filePath.IndexOf("SZ#") > -1 ? Exchange.SZ : Exchange.SH;
                stockData.Code = line.Split(' ')[0];
                stockData.Name = line.Split(' ')[1];
                stockData.RawData.Period = Period.Day;
                stockData.RawData.interval = 1;
            }
            catch(Exception ex)
            {
            }

            await sr.ReadLineAsync().ConfigureAwait(false);
            line = await sr.ReadLineAsync().ConfigureAwait(false);
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
                    stockData.RawData.Items.Add(item);
                }

                line = await sr.ReadLineAsync().ConfigureAwait(false);
            }

            sr.Close();

            foreach (var v in Enum.GetValues(typeof(Period)))
            {
                //redundant
                //stockData.PeriodData.Add(MathLib.ConvertPeriod(stockData.RawData, (Period)v));
                stockData.StockAnalysedData.Add(new AnalysedData((Period)v, stockData.RawData));
            }

            return stockData;
        }
    }
}
