using System;
using System.Collections.Generic;
using DataAnalyst.Base;

namespace DataAnalyst.Pole
{
    public class PoleLib
    {
        private static DateTime EndDate = new DateTime(2020, 03, 27);
        private static DateTime StartDate = new DateTime(2020, 03, 01);
        private static Direction Trend = Direction.Down;

        public static void FindDoublePoles(StockDataSet data, Period period, DateTime startDate, DateTime endDate)
        {
            var doubles = new List<DoublePole>();
            data.DataList.ForEach(stockData => {
                stockData.DoublePoles.Clear();
                FindDoublePole(stockData, period, startDate, endDate);
            });
        }

        public static void FindDoublePole(StockData stockData, Period period, DateTime startDate, DateTime endDate)
        {
            var poles = FindPoles(stockData.GetPeriodPriceList(period), startDate, endDate);
            var result = FindDoublePole(stockData.Code, poles, Trend, period);
            if (result != null)
                stockData.DoublePoles.Add(result);
        }

        private static DoublePole FindDoublePole(string code, List<Pole> poles, Direction direction, Period period)
        {
            if (poles == null || poles.Count == 0)
            {
                return null;
            }

            var polesInterested = poles.FindAll(p => p.Direction == direction);

            for (int i = polesInterested.Count - 1; i >= 0; i--)
            {
                var currentPole = polesInterested[i];
                for (var j = i-1; j >= 0; j--)
                {
                    if (direction == Direction.Down)
                    {
                        if (IsSimilar(currentPole.Item.Low, polesInterested[j].Item.Low))
                        {
                            return new DoublePole(code, period, currentPole, polesInterested[j]);
                        }

                        if (currentPole.Item.Low > polesInterested[j].Item.Low)
                        {
                            break;
                        }
                    }

                    if (direction == Direction.Up)
                    {
                        if (IsSimilar(currentPole.Item.High, polesInterested[j].Item.High))
                        {
                            return new DoublePole(code, period, currentPole, polesInterested[j]);
                        }

                        if (currentPole.Item.High < polesInterested[j].Item.High)
                        {
                            break;
                        }
                    }
                }
            }

            return null;
        }

        public static void FindDoublePolesWithCross(StockDataSet data, Period period, int barCount)
        {
            var doubles = new List<DoublePole>();
            data.DataList.ForEach(stockData => {
                stockData.DoublePoles.Clear();
                if (stockData.StockCrossData.Count > 0)
                {
                    var poles = FindPoles(stockData.GetPeriodPriceList(period), stockData.StockCrossData[0].CrossDate, barCount);
                    poles.Reverse();
                    var result = FindDoublePole(stockData.Code, poles, Trend, period);
                    if (result != null)
                        stockData.DoublePoles.Add(result);
                }
            });
        }

        public static List<Pole> FindPoles(PriceList pl, DateTime startDate, DateTime endDate)
        {
            var poles = new List<Pole>();
            var data = pl.Items.FindAll(i => i.Date.Between(startDate, endDate));

            if (data.Count == 0)
            {
                return null;
            }

            return FindPoles(data);
        }

        // reverse from startDate to find poles in the barCount time
        public static List<Pole> FindPoles(PriceList pl, DateTime startDate, int barCount)
        {
            var poles = new List<Pole>();
            var data = new List<PriceItem>();
            for (int i = pl.Count - 1; i >= 0; i--)
            {
                if (pl[i].Date > startDate)
                    continue;
                for(int j=0; j<barCount; j++)
                {
                    var index = i - j;
                    if (index >= 0)
                    {
                        data.Add(pl[index]);
                    }
                    else
                        break;
                }
                break;
            }
            return FindPoles(data);
        }

        private static List<Pole> FindPoles(List<PriceItem> data)
        {
            var poles = new List<Pole>();
            var currentItem = data[0];

            PriceItem nextItem;
            var currentDirection = Direction.Unknown;

            for (int i = 1; i < data.Count - 1; i++)
            {
                nextItem = data[i];

                switch (currentDirection)
                {
                    case Direction.Unknown:
                        if (currentItem.High >= nextItem.High && currentItem.Low <= nextItem.Low
                            || currentItem.High <= nextItem.High && currentItem.Low >= nextItem.Low)
                        {
                            break;
                        }
                        if (currentItem.High < nextItem.High)
                        {
                            currentDirection = Direction.Up;
                        }
                        else if (currentItem.Low > nextItem.Low)
                        {
                            currentDirection = Direction.Down;
                        }
                        break;

                    case Direction.Up:
                        if (currentItem.High > nextItem.High)
                        {
                            poles.Add(new Pole { Direction = Direction.Up, Item = currentItem });
                            currentDirection = Direction.Down;
                        }
                        break;
                    case Direction.Down:
                        if (currentItem.Low < nextItem.Low)
                        {
                            poles.Add(new Pole { Direction = Direction.Down, Item = currentItem });
                            currentDirection = Direction.Up;
                        }
                        break;
                }

                currentItem = nextItem;
            }

            poles.Add(new Pole { Direction = currentDirection, Item = currentItem });

            return poles;
        }

        private static bool IsSimilar(decimal value1, decimal value2)
        {
            if (value1 == 0)
                return false;

            return Math.Abs(value1 - value2) / value1 < 0.01m;
        }
    }
}
