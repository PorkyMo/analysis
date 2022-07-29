using System;
using System.Collections.Generic;
using System.Linq;
using DataAnalyst.Base;

namespace DataAnalyst.Score
{
    public class ScoreLib
    {
        public static void FindScores(StockDataSet data, DateTime startDate, DateTime endDate)
        {
            data.SummaryList.Clear();

            var currentDate = startDate;

            while (currentDate <= endDate)
            {
                while (!MathLib.IsTradingDay(currentDate))
                {
                    currentDate = currentDate.AddDays(1);
                }

                FindScoresInternal(data, currentDate);
                currentDate = currentDate.AddDays(1);
            }
        }

        private static void FindScoresInternal(StockDataSet data, DateTime date)
        {
            var scoreSummaryList = data.SummaryList;

            List<Tuple<string, string, PriceItem>> dataForDate = 
                data.DataList.Select(d => new Tuple<string, string, PriceItem>(d.Code, d.Name, d.RawData.FindItemByDate(date)))
                .ToList();

            dataForDate.Sort((d1, d2) =>
            {
                if (d1.Item3 == null && d2.Item3 == null)
                    return 0;

                return d1.Item3 == null ? 1 : (d2.Item3 == null ? -1 : -1 * (d1.Item3.ChangePercentage.CompareTo(d2.Item3.ChangePercentage)));
            });

            for (int i = 0; i < dataForDate.Count; i++)
            {
                var d = dataForDate[i];
                var scoreSummary = scoreSummaryList.Find(s => s.Code == d.Item1);
                if (scoreSummary == null)
                {
                    scoreSummary = new ChangedScoreSummary(d.Item1, d.Item2);
                    scoreSummaryList.Add(scoreSummary);
                }
                if (scoreSummary.Index(date) > -1)
                {
                    scoreSummary.Scores.RemoveAt(scoreSummary.Index(date));
                }

                scoreSummary.Scores.Add(new ChangedScore {
                    DateForChange = date,
                    Score = i,
                    PriceItem = d.Item3
                });
            };
        }
    }
}
