using DataAnalyst.Base;
using System;

namespace DataAnalyst.Score
{
    public class ChangedScore
    {
        public DateTime DateForChange { get; set; }
        public int Score { get; set; }  //score samller means its positive change rate is larger
        public PriceItem PriceItem { get; set; }
    }
}
