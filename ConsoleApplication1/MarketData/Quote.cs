namespace DataAnalyst.MarketData
{
    public class Quote
    {
        public string Code;
        public string Exchange;
        public string Name;
        public decimal Open;
        public decimal Close;
        public decimal PreviousClose;
        public decimal High;
        public decimal Low;
        public int Volume;
        public decimal Change
        {
            get { return Close - PreviousClose; }
        }
        public decimal Percentage
        {
            get { return (Close - PreviousClose) / PreviousClose * 100; }
        }
    }
}
