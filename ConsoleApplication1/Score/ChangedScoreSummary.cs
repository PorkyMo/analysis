using System;
using System.Linq;
using System.Collections.Generic;
using DataAnalyst.Base;

namespace DataAnalyst.Score
{
    public class ChangedScoreSummary
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public List<ChangedScore> Scores { get; set; }

        public ChangedScoreSummary(string code, string name)
        {
            Code = code;
            Name = name;
            Scores = new List<ChangedScore>();
        }

        public decimal GetTotalScore(DateTime startDate, DateTime endDate)
        {
            var startIndex = Index(startDate);
            var endIndex = Index(endDate);

            if (startIndex > endIndex || startIndex == -1)
                return 0;
            if (endIndex == -1)
                endIndex = Scores.Count - 1;

            decimal total = 0;
            for (int i = startIndex; i <= endIndex; i++)
                total += Scores[i].Score;

            return total;
        }

        public decimal GetChangedPercentage(DateTime startDate, DateTime endDate)
        {
            var startIndex = Index(startDate, DateNotFound.MovingForward);
            var endIndex = Index(endDate, DateNotFound.MovingBackward);

            if (startIndex > endIndex || startIndex == -1)
                return 0;
            if (endIndex == -1)
                endIndex = Scores.Count - 1;

            var startPriceItem = Scores[startIndex].PriceItem;
            var endPriceItem = Scores[endIndex].PriceItem;
            if (startPriceItem == null || endPriceItem == null)
                return 0;

            var startPrice = startPriceItem.PreviousClose != 0 ? startPriceItem.PreviousClose : startPriceItem.Open;
            return (endPriceItem.Close - startPrice) / startPrice;
        }

        public int Index(DateTime date)
        {
            return Index(date, DateNotFound.None);
        }

        public int Index(DateTime date, DateNotFound option)
        {
            if (option == DateNotFound.None)
            {
                var s = Scores.Find(score => score.DateForChange.Date == date.Date);
                if (s == null)
                    return -1;

                return Scores.IndexOf(s);
            }

            // for moveforward and movebackward
            ChangedScore cs;
            while (true)
            {
                cs = Scores.Find(score => score.DateForChange.Date == date.Date);
                if (cs?.PriceItem != null)
                {
                    break;
                }

                switch (option)
                {
                    case DateNotFound.None:
                        return -1;
                    case DateNotFound.MovingForward:
                        date = date.AddDays(1);
                        if (Scores[Scores.Count - 1].DateForChange.Date < date)
                        {
                            return -1; //reach the end
                        }
                        break;
                    case DateNotFound.MovingBackward:
                        date = date.AddDays(-1);
                        if (Scores[0].DateForChange.Date > date)
                        {
                            return -1; //reach the end
                        }
                        break;
                }
            }
            return Scores.IndexOf(cs);
        }
    }
}
