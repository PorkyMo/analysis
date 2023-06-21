using System.Collections.Generic;
using DataAnalyst.Cross;

namespace DataAnalyst.Base
{
    public class CrossDataInfo
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public Exchange StockExchange { get; set; }
        public Period Period { get; set; }

        public List<CrossData> CrossDataList = new List<CrossData>();
    }
}
