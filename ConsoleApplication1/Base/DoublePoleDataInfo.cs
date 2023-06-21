using System.Collections.Generic;
using DataAnalyst.Pole;

namespace DataAnalyst.Base
{
    public class DoublePoleDataInfo
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public Exchange StockExchange { get; set; }
        public Period Period { get; set; }

        public List<DoublePole> DoublePoles = new List<DoublePole>();
    }
}
