using System;
using System.Collections.Generic;
using DataAnalyst.Cross;
using DataAnalyst.Pole;

namespace DataAnalyst.Base
{
    public class AnalysedData
    {
        public AnalysedData(Period period, PriceList rawData)
        {
            Period = period;
            PeriodData = MathLib.ConvertPeriod(rawData, period);
        }

        private int CrossRange = 1;
        private List<int> Intervals = new List<int>() { 5, 10, 20, 30, 60 };
        public Period Period { get; }
        public PriceList PeriodData = new PriceList();
        public List<PriceList> AveragedData = new List<PriceList>(); // contain average data (5/10/20/60) for PeriodData
        public List<CrossData> StockCrossData = new List<CrossData>();
        public List<Pole.Pole> Poles = new List<Pole.Pole>();
        public List<DoublePole> DoublePoles = new List<DoublePole>();
        public List<DateTime> Turnings = new List<DateTime>();
        public List<Event> Timeline = new List<Event>();

        private void AddAveragedData()
        {
            if (FindAverageDataList(Intervals[0]) == null)
            {
                Intervals.ForEach(i => AddAveragedData(i));
            }
        }

        private void ClearAveragedData()
        {
            AveragedData.Clear();
        }

        public void AddAveragedData(int interval)
        {
            AveragedData.Add(MathLib.GetAveragePrice(PeriodData, interval));
        }

        public PriceList FindAverageDataList(int interval)
        {
            return AveragedData.Find(l => l.interval == interval);
        }

        public void FindCross(int range, bool crossUp, DateTime startDate)
        {
            CrossRange = range;

            StockCrossData.Clear();

            AddAveragedData();
            startDate = MathLib.GetDateForPeriod(startDate, Period);
            var dataShort = FindAverageDataList(Intervals[0]);
            var dataMedium = FindAverageDataList(Intervals[1]);
            var dataLong = FindAverageDataList(Intervals[2]);
            var dataLonger = FindAverageDataList(Intervals[3]);
            var dataLongest = FindAverageDataList(Intervals[4]);

            var shortIndex = dataShort.FindIndex(startDate, DateNotFound.None);
            var mediumIndex = dataMedium.FindIndex(startDate, DateNotFound.None);
            var longIndex = dataLong.FindIndex(startDate, DateNotFound.None);
            var longerIndex = dataLonger.FindIndex(startDate, DateNotFound.None);
            var longestIndex = dataLongest.FindIndex(startDate, DateNotFound.None);

            if (shortIndex < 0 || mediumIndex < 0 || longIndex < 0 || longerIndex < 0 || longestIndex < 0)
            {
                return;
            }

            var count = dataShort.Count;
            if (longerIndex < CrossRange || longestIndex < CrossRange)
            {
                return;
            }

            for (var i = shortIndex; i < dataShort.Count; i++)
            {
                if (CrossDataLib.AveragedPriceCrossed(dataShort, dataMedium, dataShort[i].Date, CrossRange, crossUp)
                    && CrossDataLib.AveragedPriceCrossed(dataShort, dataLong, dataShort[i].Date, CrossRange, crossUp)
                    && CrossDataLib.AveragedPriceCrossed(dataShort, dataLonger, dataShort[i].Date, CrossRange, crossUp)
                    && CrossDataLib.AveragedPriceCrossed(dataShort, dataLongest, dataShort[i].Date, CrossRange, crossUp)
                    && (CrossDataLib.AveragedPriceCrossed(dataMedium, dataLong, dataShort[i].Date, CrossRange, crossUp)
                        || CrossDataLib.AveragedPriceCrossed(dataMedium, dataLonger, dataShort[i].Date, CrossRange, crossUp)
                        || CrossDataLib.AveragedPriceCrossed(dataMedium, dataLongest, dataShort[i].Date, CrossRange, crossUp)))
                {
                    StockCrossData.Add(new CrossData
                    {
                        Period = Period,
                        CrossDate = dataShort[i].Date,
                        Direction = crossUp ? CrossDirection.Up : CrossDirection.Down
                    });
                }
            }

            ClearAveragedData();
        }

        //this function find turning from startDate and forward
        public void FindTurningForward(Period period, DateTime startDate, int range)
        {
            AddAveragedData();
            Turnings.Clear();

            var dataShort = FindAverageDataList(Intervals[0]);
            var dataMedium = FindAverageDataList(Intervals[1]);
            var dataLong = FindAverageDataList(Intervals[2]);
            var dataLonger = FindAverageDataList(Intervals[3]);

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
            AddAveragedData();
            Turnings.Clear();

            var dataShort = FindAverageDataList(Intervals[0]);
            var dataMedium = FindAverageDataList(Intervals[1]);
            var dataLong = FindAverageDataList(Intervals[2]);
            var dataLonger = FindAverageDataList(Intervals[3]);

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

        public List<CrossData> GetCrossData(Period period)
        {
            return StockCrossData.FindAll(d => d.Period == period);
        }

        public void FindPoles(DateTime startDate, DateTime endDate)
        {
            Poles.Clear();
            Poles = PoleLib.FindPoles(PeriodData, startDate, endDate);
        }

        public void FindDoublePole(string code, DateTime startDate, DateTime endDate, Direction trend)
        {
            FindPoles(startDate, endDate);
            var result = PoleLib.FindDoublePole(code, Poles, trend, Period);
            if (result != null)
                DoublePoles.Add(result);
        }

        public void FindDoublePolesWithCross(string code, int barCount, Direction trend)
        {
            var doubles = new List<DoublePole>();
            DoublePoles.Clear();
            if (StockCrossData.Count > 0)
            {
                var poles = PoleLib.FindPoles(PeriodData, StockCrossData[0].CrossDate, barCount);
                poles.Reverse();
                var result = PoleLib.FindDoublePole(code, poles, trend, Period);
                if (result != null)
                    DoublePoles.Add(result);
            }
        }

        public void BuildTimeLine(DateTime startDate, DateTime endDate)
        {
            this.FindCross(7, true, startDate);
            this.FindPoles(startDate, endDate);
            this.StockCrossData.ForEach(cd => this.Timeline.Add(new Event(cd.CrossDate, EventType.UpCross)));
            this.Poles.ForEach(p => this.Timeline.Add(new Event(p.Item.Date, p.Direction == Direction.Up ? EventType.TopPole : EventType.BottomPole)));
            this.Timeline.Sort((tl1, tl2) => tl1.EventDate.CompareTo(tl2.EventDate));
        }
    }
}
