using System;
using DataAnalyst.Base;

namespace DataAnalyst.Cross
{
    public class CrossData
    {
        public Period Period { get; set; }
        public DateTime CrossDate { get; set; }
        public CrossDirection Direction { get; set; }
    }
}
