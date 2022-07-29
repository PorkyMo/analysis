using System;

namespace DataAnalyst.Base
{
    public class PriceItem
    {
        public DateTime Date;
        public decimal Open;
        public decimal High;
        public decimal Low;
        public decimal Close;
        public decimal Volumn;
        public decimal Amount;
        public decimal PreviousClose;
        public Period ItemPeriod;
        public DateTime EndDate; // like K bar

        public PriceItem()
        {
        }

        public PriceItem(DateTime dateTime, decimal open, decimal high, decimal low, decimal close, decimal volumn, decimal amount, Period period)
        {
            Date = dateTime;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volumn = volumn;
            Amount = amount;
            ItemPeriod = period;
            EndDate = Date;
        }

        public decimal ChangePercentage
        {
            get
            {
                return PreviousClose != 0 ? (Close - PreviousClose) / PreviousClose : (Open != 0 ? (Close - Open) / Open : 0);
            }
        }
        public PriceItem Clone()
        {
            return new PriceItem()
            {
                Date = this.Date,
                Open = this.Open,
                High = this.High,
                Low = this.Low,
                Close = this.Close,
                Volumn = this.Volumn,
                Amount = this.Amount,
                ItemPeriod = this.ItemPeriod
            };
        }
    }
}
