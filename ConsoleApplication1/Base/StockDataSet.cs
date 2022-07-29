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

        public List<StockData> GetCrossedData()
        {
            return DataList.Where(d => d.StockCrossData.Count > 0).ToList();
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
    }
}
