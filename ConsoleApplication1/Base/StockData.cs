using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAnalyst.Cross;
using DataAnalyst.Pole;

namespace DataAnalyst.Base
{
    public class StockData
    {
        private int CrossRange = 1;
        private List<int> Intervals = new List<int>() { 5, 10, 20, 30};
        public string Code { get; set; }
        public string Name { get; set; }
        public Exchange StockExchange { get; set; }
        public PriceList RawData = new PriceList();
        public List<PriceList> PeriodData = new List<PriceList>(); // contain day, week, month period data
        public List<PriceList> AveragedData = new List<PriceList>(); // contain average data (5/10/20/60) for each period
        public List<CrossData> StockCrossData = new List<CrossData>();
        public List<DoublePole> DoublePoles = new List<DoublePole>();
        public List<DateTime> Turnings = new List<DateTime>();

        private void AddAveragedData(Period period)
        {
            if (FindAverageDataList(period, Intervals[0]) == null)
            {
                Intervals.ForEach(i => AddAveragedData(period, i));
            }
        }

        private void ClearAveragedData()
        {
            AveragedData.Clear();
        }

        public PriceList GetPeriodPriceList(Period period)
        {
            if (period == Period.Day)
                return RawData;

            return PeriodData.Find(pl => pl.Period == period);
        }

        public void AddAveragedData(Period period, int interval)
        {
            AveragedData.Add(MathLib.GetAveragePrice(MathLib.ConvertPeriod(RawData, period), interval));
        }

        public PriceList FindAverageDataList(Period period, int interval)
        {
            return AveragedData.Find(l => l.Period == period && l.interval == interval);
        }

        //crossup: avgList1 crossup avgList2
        public bool AveragedPriceCrossed(PriceList avgList1, PriceList avgList2, DateTime date, bool crossUp)
        {
            if (avgList1 == null || avgList2 == null || avgList1.Period != avgList2.Period || avgList1.interval == avgList2.interval)
            {
                return false;
            }

            var adjustedDate = MathLib.GetDateForPeriod(date, avgList1.Period);
                        
            var index1 = avgList1.FindIndex(adjustedDate, DateNotFound.None);
            var index2 = avgList2.FindIndex(adjustedDate, DateNotFound.None);
            var previousIndex1 = index1 - 1;
            var previousIndex2 = index2 - 1;
            
            if (previousIndex1 < 0 || previousIndex2 < 0)
            {
                return false;
            }

            if (avgList1[index1].Date != avgList2[index2].Date || avgList1[previousIndex1].Date != avgList2[previousIndex2].Date)
            {
                return false;
            }

            if (crossUp)
            {
                return avgList1[previousIndex1].Close <= avgList2[previousIndex2].Close &&
                    avgList1[index1].Close >= avgList2[index2].Close;
            }
            else
            {
                return avgList1[previousIndex1].Close >= avgList2[previousIndex2].Close &&
                    avgList1[index1].Close <= avgList2[index2].Close;
            }
        }

        public bool AveragedPriceCrossed(PriceList avgList1, PriceList avgList2, DateTime date, int intervalRange, bool crossUp)
        {
            if (avgList1 == null || avgList2 == null || avgList1.Period != avgList2.Period || avgList1.interval == avgList2.interval)
            {
                return false;
            }

            var adjustedDate = MathLib.GetDateForPeriod(date, avgList1.Period);

            var index1 = avgList1.FindIndex(adjustedDate, DateNotFound.None);
            var index2 = avgList2.FindIndex(adjustedDate, DateNotFound.None);
            if (index1 < 0 || index2 < 0 || index1 < intervalRange || index2 < intervalRange)
            {
                return false;
            }

            if (crossUp)
            {
                if (avgList1[index1].Close >= avgList2[index2].Close)
                {
                    for(var i = 1; i <= intervalRange; i++)
                    {
                        if (avgList1[index1 - i].Date != avgList2[index2 - i].Date)
                        {
                            return false;
                        }
                        if (avgList1[index1 - i].Close <= avgList2[index2 - i].Close)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            else
            {
                if (avgList1[index1].Close <= avgList2[index2].Close)
                {
                    for (var i = 1; i < intervalRange; i++)
                    {
                        if (avgList1[index1 - i].Date != avgList2[index2 - i].Date)
                        {
                            return false;
                        }
                        if (avgList1[index1 - i].Close >= avgList2[index2 - i].Close)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public void FindCross(Period period, int range, bool crossUp, DateTime startDate)
        {
            CrossRange = range;

            StockCrossData.Clear();

            AddAveragedData(period);
            startDate = MathLib.GetDateForPeriod(startDate, period);
            var dataShort = FindAverageDataList(period, Intervals[0]);
            var dataMedium = FindAverageDataList(period, Intervals[1]);
            var dataLong = FindAverageDataList(period, Intervals[2]);
            var dataLonger = FindAverageDataList(period, Intervals[3]);

            var shortIndex = dataShort.FindIndex(startDate, DateNotFound.None);
            var mediumIndex = dataMedium.FindIndex(startDate, DateNotFound.None);
            var longIndex = dataLong.FindIndex(startDate, DateNotFound.None);
            var longerIndex = dataLonger.FindIndex(startDate, DateNotFound.None);

            if (shortIndex < 0 || mediumIndex < 0 || longIndex < 0 || longerIndex < 0)
            {
                return;
            }

            var count = dataShort.Count;
            if (longerIndex < CrossRange)
            {
                return;
            }
            
            for (var i = shortIndex; i < dataShort.Count; i++)
            {
                if (AveragedPriceCrossed(dataShort, dataMedium, dataShort[i].Date, CrossRange, crossUp)
                    && AveragedPriceCrossed(dataShort, dataLong, dataShort[i].Date, CrossRange, crossUp)
                    && AveragedPriceCrossed(dataShort, dataLonger, dataShort[i].Date, CrossRange, crossUp))
                {
                    //Console.WriteLine($"find {Code} in {dataShort[count - i - 1].Date}" 
                    //    + (dataShort[count - i - 1].Close < RawData[RawData.Count-1].Close ? "price up" : "price down"));
                    StockCrossData.Add(new CrossData {
                        Period = period, CrossDate = dataShort[i].Date,
                        Direction = crossUp ? CrossDirection.Up : CrossDirection.Down});
                }
            }
        }

        //this function find turning from startDate and forward
        public void FindTurningForward(Period period, DateTime startDate, int range)
        {
            AddAveragedData(period);
            Turnings.Clear();

            var dataShort = FindAverageDataList(period, Intervals[0]);
            var dataMedium = FindAverageDataList(period, Intervals[1]);
            var dataLong = FindAverageDataList(period, Intervals[2]);
            var dataLonger = FindAverageDataList(period, Intervals[3]);

            //first find an available start date
            while (startDate < DateTime.Today)
            {
                startDate = MathLib.GetDateForPeriod(startDate, period);

                var shortIndex = dataShort.FindIndex(startDate, DateNotFound.None);
                var mediumIndex = dataMedium.FindIndex(startDate, DateNotFound.None);
                var longIndex = dataLong.FindIndex(startDate, DateNotFound.None);
                var longerIndex = dataLonger.FindIndex(startDate, DateNotFound.None);

                if (shortIndex > 0 && mediumIndex > 0 && longIndex > 0 && longerIndex > 0)
                {
                    if (longerIndex > CrossRange)
                    {
                        break;
                    }
                }

                startDate = startDate.AddDays(1);
            }

            if (startDate < DateTime.Today)
            {
                var priceLists = new List<PriceList> { dataShort, dataMedium, dataLong, dataLonger };
                Turnings.AddRange(CrossDataLib.FindTurningsForward(priceLists, startDate, range, true));
            }

            ClearAveragedData();
        }

        //this function find turning from the latest date and backward till reaching endDate
        public void FindTurningBackward(Period period, DateTime endDate, int range)
        {
            AddAveragedData(period);
            Turnings.Clear();

            var dataShort = FindAverageDataList(period, Intervals[0]);
            var dataMedium = FindAverageDataList(period, Intervals[1]);
            var dataLong = FindAverageDataList(period, Intervals[2]);
            var dataLonger = FindAverageDataList(period, Intervals[3]);

            var currentDate = DateTime.Today;
            //first find an available start date
            while (endDate < currentDate)
            {
                currentDate = MathLib.GetDateForPeriod(currentDate, period);

                var shortIndex = dataShort.FindIndex(currentDate, DateNotFound.None);
                var mediumIndex = dataMedium.FindIndex(currentDate, DateNotFound.None);
                var longIndex = dataLong.FindIndex(currentDate, DateNotFound.None);
                var longerIndex = dataLonger.FindIndex(currentDate, DateNotFound.None);

                if (shortIndex > 0 && mediumIndex > 0 && longIndex > 0 && longerIndex > 0)
                {
                    if (longerIndex > range)
                    {
                        break;
                    }
                }

                currentDate = currentDate.AddDays(-1);
            }

            if (endDate < currentDate)
            {
                var priceLists = new List<PriceList> { dataShort, dataMedium, dataLong, dataLonger };
                Turnings.AddRange(CrossDataLib.FindTurningsBackward(priceLists, currentDate, endDate, range, true));
            }

            ClearAveragedData();
        }

        public Tuple<string, PriceItem> GetPrictItem(DateTime date, Period period)
        {
            var periodData = PeriodData.Find(pd => pd.Period == period);
            if (periodData == null)
                return null;
            var priceItem = periodData?.FindItemByDate(date);
            return priceItem == null ? null : new Tuple<string, PriceItem>(Code, priceItem);
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
                stockData.PeriodData.Add(MathLib.ConvertPeriod(stockData.RawData, (Period)v));
            }

            return stockData;
        }
    }
}
