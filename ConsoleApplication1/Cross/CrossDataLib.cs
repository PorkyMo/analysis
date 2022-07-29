using System;
using DataAnalyst.Base;
using System.Collections.Generic;

namespace DataAnalyst.Cross
{
    public class CrossDataLib
    {
        private static bool AddMockItem = false;

        public static void FindAllCross(StockDataSet data, DateTime crossDateTime)
        {

        }

        public static void FindCross(StockDataSet data, Period crossPeriod, DateTime crossDateTime)
        {
            if (AddMockItem)
            {
                AppendMockPriceItem(data);
            }

            var findCrossStartDate = crossDateTime;

            data.DataList.ForEach(d => d.FindCross(crossPeriod, 2, true, findCrossStartDate));

            //foreach (var cross in crosses)
            //{
            //    var crossDate = cross.StockCrossData[0].CrossDate;
            //    var crossPrice = cross.RawData.FindItem(crossDate).Close;
            //    if (cross.RawData.items
            //        .Any(i => i.Date > findCrossStartDate
            //                    && i.Date < crossDate.AddDays(10)
            //                    && ((i.Close - crossPrice) / crossPrice) > (decimal)0.10))
            //    {
            //        Console.WriteLine($"{cross.StockCrossData[0].Period} {cross.Code} crossed at {cross.StockCrossData[0].CrossDate}");
            //    }
            //}
            //crosses.ForEach(d => d.StockCrossData.ForEach(c => Console.WriteLine($"{c.Period} {d.Code} crossed at {c.CrossDate}")));
        }

        public static void AppendMockPriceItem(StockDataSet data)
        {
            data.DataList.ForEach(stockData => {
                var count = stockData.RawData.Items.Count;

                if (count > 0)
                {
                    var lastItem = stockData.RawData.Items[count - 1];
                    var mockItem = lastItem.Clone();

                    while (true)
                    {
                        mockItem.Date = mockItem.Date.AddDays(1);
                        if (mockItem.Date.DayOfWeek == DayOfWeek.Saturday || mockItem.Date.DayOfWeek == DayOfWeek.Sunday)
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                    stockData.RawData.Items.Add(mockItem);
                }
            });
        }

        public static bool InATrend(PriceList priceList, DateTime date, int range, bool up)
        {
            var index = priceList.FindIndex(date, DateNotFound.None);
            if (index < range-1)
                return false;

            for(var i = 0; i < range-1; i++)
            {
                if (up)
                {
                    if (priceList[index - i].Close < priceList[index - i - 1].Close)
                        return false;
                }
                else
                {
                    if (priceList[index - i].Close > priceList[index - i - 1].Close)
                        return false;
                }
            }

            return true;
        }

        private static bool InSameTrend(List<PriceList> priceLists, DateTime startDate, int range, bool up)
        {
            return !priceLists.Exists(pl => !InATrend(pl, startDate, range, up));
        }

        public static List<DateTime> FindTurningsForward(List<PriceList> priceLists, DateTime startDate, int range, bool up)
        {
            priceLists.Sort((pl1, pl2) => pl1.interval - pl2.interval);
            var shortIndex = priceLists[0].FindIndex(startDate, DateNotFound.None);

            var turnings = new List<DateTime>();
            for(var index = shortIndex + 1; index < priceLists[0].Count; index++)
            {
                var currentDate = priceLists[0][index].Date;
                var previousDate = priceLists[0][index - 1].Date;

                //the longest average line turn
                if (InSameTrend(priceLists, currentDate, range, up) && !InATrend(priceLists[priceLists.Count-1], previousDate, range, up)) //!InSameTrend(priceLists, previousDate, range, up))
                    turnings.Add(currentDate);
            }

            return turnings;
        }

        public static List<DateTime> FindTurningsBackward(List<PriceList> priceLists, DateTime startDate, DateTime endDate, int range, bool up)
        {
            priceLists.Sort((pl1, pl2) => pl1.interval - pl2.interval);
            var shortIndex = priceLists[0].FindIndex(startDate, DateNotFound.None);

            var turnings = new List<DateTime>();
            while (priceLists[0][shortIndex].Date > endDate)
            {
                var currentDate = priceLists[0][shortIndex].Date;
                var previousDate = priceLists[0][shortIndex - 1].Date;

                //the longest average line turn
                if (InSameTrend(priceLists, currentDate, range, up) && !InATrend(priceLists[priceLists.Count - 1], previousDate, range, up))
                    turnings.Add(currentDate);

                shortIndex--;
            }

            return turnings;
        }
    }
}
