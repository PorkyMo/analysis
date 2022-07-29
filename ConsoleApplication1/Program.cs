using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAnalyst.Cross;
using DataAnalyst.Base;

namespace DataAnalyst
{
    class Program
    {
        const string basePath = @"C:\zd_gfzq\T0002\export\";
        static Period CrossPeriod = Period.Week;
        static DateTime CrossDateTime = new DateTime(2020, 5, 8);

        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            var dataSet = new StockDataSet();
            var files = Directory.GetFiles(basePath);

            foreach (var file in files)
            {
                dataSet.AddStockData(await StockData.ReadData(file, new DateTime(2017, 01, 01), DateTime.Today));
            }
            
            CrossDataLib.FindCross(dataSet, CrossPeriod, CrossDateTime);
        }
    }
}
