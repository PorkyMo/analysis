﻿using DataAnalyst.Base;
using DataAnalyst.Score;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataAnalyst.ETF
{
    public class ETFTrial
    {
        public static List<string> ETFCodes = new List<string>();
        public static StockDataSet DataSet { get; set; }

        public static void Init(StockDataSet fullData)
        {
            ETFCodes.Clear();
            StreamReader sr = new StreamReader(@"c:\tt\ConsoleApplication1\ConsoleApplication1\ETF\etflist.txt");
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                var stockData = fullData.GetStockDataByCode(line);
                if (stockData == null || stockData.Name.Contains("货币") || stockData.Name.Contains("债"))
                    continue;

                ETFCodes.Add(line);
            }
            sr.Close();

            DataSet = fullData.GetSubSet(s => ETFCodes.Contains(s.Code));
        }

        public static void GetScores(DateTime startDate, DateTime endDate)
        {
            ScoreLib.FindScores(DataSet, startDate, endDate);
        }

        public static List<Tuple<string, string, PriceItem>> GetTopNCodes(DateTime startDate, DateTime endDate, int n)
        {
            ScoreLib.FindScores(DataSet, startDate, endDate);
            DataSet.SummaryList.Sort((cs1, cs2) => -1 * cs1.GetChangedPercentage(startDate, endDate).CompareTo(cs2.GetChangedPercentage(startDate, endDate)));
            //DataSet.SummaryList.Sort((cs1, cs2) => cs1.GetTotalScore(startDate, endDate).CompareTo(cs2.GetTotalScore(startDate, endDate)));

            var topList = new List<Tuple<string, string, PriceItem>>();
            foreach(var changedScoreSummary in DataSet.SummaryList.Take(n))
            {
                var stockData = DataSet.GetStockDataByCode(changedScoreSummary.Code);
                if (stockData != null)
                {
                    topList.Add(new Tuple<string, string, PriceItem>
                        (changedScoreSummary.Code, 
                        changedScoreSummary.Name, 
                        stockData.RawData.GetItemByRange(startDate, endDate)));
                }
            }

            return topList;
        }
    }
}
