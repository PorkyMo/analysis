using DataAnalyst.Score;
using System.Collections.Generic;
using System.Linq;
using System;

namespace DataAnalyst.Base
{
    public class StockDataSet
    {
        public List<StockData> DataList { get; set; }
        public List<ChangedScoreSummary> SummaryList { get; set; }
        public List<DateTime> TurningList { get; set; }

        public StockDataSet()
        {
            DataList = new List<Base.StockData>();
            SummaryList = new List<ChangedScoreSummary>();
        }

        public void AddStockData(StockData data)
        {
            DataList.Add(data);
        }

        public void Clear()
        {
            DataList.Clear();
            SummaryList.Clear();
        }

        public List<CrossDataInfo> GetCrossedData(Period period)
        {
            return DataList.Select(d => d.GetCrossDataInfo(period)).Where(cd => cd.CrossDataList.Count > 0).ToList();
        }

        public List<DoublePoleDataInfo> GetDoublePoleData(Period period)
        {
            return DataList.Select(d => d.GetDoublePoleDataInfo(period)).Where(cd => cd.DoublePoles.Count > 0).ToList();
        }


        public List<TimelineInfo> GetTimelineData(Period period)
        {
            return DataList.Select(d => 
                d.GetTimelineInfo(period))
                .Where(tl => tl.Timeline.Count > 0)
                .Where(tl => tl.Timeline.Exists(e => e.EventType == EventType.UpCross))
                .Where(tl => tl.Timeline.Exists(e => e.EventType == EventType.TopPole))
                .ToList();
        }

        public StockDataSet GetSubSet(System.Func<StockData, bool> filter)
        {
            var subSet = new StockDataSet();
            subSet.DataList = DataList.Where(filter).ToList();
            return subSet;
        }

        public StockData GetStockDataByCode(string code)
        {
            return DataList.Find(d => d.Code == code);
        }

        public ChangedScoreSummary GetScoreSummaryByCode(string code)
        {
            return SummaryList.Find(s => s.Code == code);
        }

        public void FindDoublePoles(Period period, DateTime startDate, DateTime endDate, Direction trend)
        {
            DataList.ForEach(stockData => {
                stockData.FindDoublePole(period, startDate, endDate, trend);
            });
        }

        public void FindDoublePolesWithCross(Period period, int barCount, Direction trend)
        {
            DataList.ForEach(stockData => 
            {
                stockData.FindDoublePolesWithCross(period, barCount, trend);
            });
        }

        public void BuildTimeLine(Period period, DateTime startDate, DateTime endDate)
        {
            DataList.ForEach(stockData => stockData.BuildTimeline(period, startDate, endDate));
        }
    }
}
