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

            data.DataList.ForEach(d => d.FindCross(crossPeriod, 7, true, findCrossStartDate));

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

        //crossup: avgList1 crossup avgList2
        public static bool AveragedPriceCrossed(PriceList avgList1, PriceList avgList2, DateTime date, bool crossUp)
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

        public static bool AveragedPriceCrossed(PriceList avgList1, PriceList avgList2, DateTime date, int intervalRange, bool crossUp)
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
                    for (var i = 1; i <= intervalRange; i++)
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
    }
}
