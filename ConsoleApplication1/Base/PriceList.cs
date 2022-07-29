using System;
using System.Collections.Generic;

namespace DataAnalyst.Base
{
    public class PriceList
    {
        public Period Period;
        public int interval;
        public List<PriceItem> Items = new List<PriceItem>();

        public PriceItem this[int index]
        {
            get { return index >= 0 && index < this.Items.Count ? this.Items[index] : null; }
        }

        public int Count
        {
            get { return Items.Count; }
        }

        public int FindIndex(DateTime date, DateNotFound option)
        {
            while (Items.FindIndex(i => i.Date.Date == date.Date) == -1)
            {
                switch(option)
                {
                    case DateNotFound.None:
                        return -1;
                    case DateNotFound.MovingForward:
                        date = date.AddDays(1);
                        if (Items[Items.Count - 1].Date < date)
                        {
                            return -1; //reach the end
                        }
                        break;
                    case DateNotFound.MovingBackward:
                        date = date.AddDays(-1);
                        if (Items[0].Date > date)
                        {
                            return -1; //reach the end
                        }
                        break;
                }
            }
            return Items.FindIndex(i => i.Date.Date == date.Date);
        }

        // no reference in this moment
        public PriceItem FindItem(DateTime date)
        {
            if (Items.Count == 0)
                return null;

            var targetDate = date;
            var index = -1;
            while ((index = FindIndex(date, DateNotFound.None)) == -1 && date >= Items[0].Date)
            {
                date = date.AddDays(-1);
            }

            return index == -1 ? null : Items[index];
        }

        public PriceItem FindItemByDate(DateTime date)
        {
            if (Items.Count == 0)
                return null;
            var index = FindIndex(date, DateNotFound.None);
            return index == -1 ? null : Items[index];
        }

        public PriceItem GetItemByRange(DateTime startDate, DateTime endDate)
        {
            var start = MathLib.GetDateForPeriod(startDate, this.Period);
            var end = MathLib.GetDateForPeriod(endDate, this.Period);

            var startIndex = FindIndex(startDate, DateNotFound.MovingForward);
            var endIndex = FindIndex(endDate, DateNotFound.MovingBackward);

            if (startIndex == -1 || endIndex == -1 || startIndex > endIndex)
                return null;

            var startPriceItem = Items[startIndex];
            var endPriceItem = Items[endIndex];

            decimal high = 0;
            decimal low = 0;
            decimal amount = 0;
            decimal volumn = 0;

            for (int i=startIndex; i<=endIndex; i++)
            {
                amount += Items[i].Amount;
                volumn += Items[i].Volumn;
                high = high < Items[i].High ? Items[i].High : high;
                low = low > Items[i].Low ? Items[i].Low : low;
            }

            return new PriceItem
            {
                Date = startDate,
                EndDate = endDate,
                Open = startPriceItem.Open,
                Close = endPriceItem.Close,
                PreviousClose = startPriceItem.PreviousClose,
                ItemPeriod = Period,
                High = high,
                Low = low,
                Amount = amount,
                Volumn = volumn
            };
        }
    }
}
