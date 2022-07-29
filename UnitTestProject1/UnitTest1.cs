using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAnalyst.Base;
using System.Collections.Generic;
using DataAnalyst.Pole;
using DataAnalyst.Portfolio;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        private StockData GetStockData()
        {
            return new StockData()
            {
                Code = "Test",
                RawData = new PriceList()
                {
                    Period = Period.Day,
                    interval = 1,
                    Items = new List<PriceItem>() {
                        new PriceItem(new DateTime(2017, 8, 1), 13.42m, 13.49m, 13.32m, 13.43m, 64198000, 860914624.00m, Period.Day),
                        new PriceItem(new DateTime(2017, 8, 2), 13.44m,13.60m,13.39m,13.44m,61644100m,830433600.00m, Period.Day),
                        new PriceItem(new DateTime(2017, 8, 3), 13.42m,13.42m,13.04m,13.08m,78581800m,1036842112.00m, Period.Day),
                        new PriceItem(new DateTime(2017, 8, 4), 13.09m,13.10m,12.85m,12.87m,63181900m,818014656.00m, Period.Day),
                        new PriceItem(new DateTime(2017, 8, 7), 12.87m,12.95m,12.83m,12.93m,29528400m,380518688.00m, Period.Day),
                        new PriceItem(new DateTime(2017, 8, 8), 12.91m,12.93m,12.83m,12.87m,23570200m,303469952.00m, Period.Day),
                        new PriceItem(new DateTime(2017, 8, 9), 12.84m,12.88m,12.75m,12.76m,32303800m,413575776.00m, Period.Day),
                        new PriceItem(new DateTime(2017, 8, 10), 12.74m,12.88m,12.70m,12.79m,44056000m,563372032.00m, Period.Day),
                        new PriceItem(new DateTime(2017, 8, 11), 12.72m,12.73m,12.46m,12.68m,88890000m,1118537344.00m, Period.Day),
                        new PriceItem(new DateTime(2017, 8, 14), 12.62m,12.63m,12.52m,12.56m,45562000m,572465216.00m, Period.Day),
                        new PriceItem(new DateTime(2017, 8, 15), 12.58m,12.76m,12.56m,12.59m,62591900m,792887808.00m, Period.Day),
                        new PriceItem(new DateTime(2017, 8, 16), 12.56m,12.56m,12.47m,12.49m,33886500m,423706592.00m, Period.Day),
                        new PriceItem(new DateTime(2017, 8, 17), 12.49m,12.55m,12.47m,12.53m,39091100m,488847392.00m, Period.Day),
                        new PriceItem(new DateTime(2017, 8, 18), 12.50m,12.57m,12.46m,12.51m,35976632m,450071392.00m, Period.Day),
                        new PriceItem(new DateTime(2017, 8, 21), 12.50m,12.57m,12.46m,12.51m,33810237m,422478336.00m, Period.Day),
                        new PriceItem(new DateTime(2017, 8, 22), 12.51m,12.53m,12.41m,12.43m,58829900m,731960000.00m, Period.Day),
                        new PriceItem(new DateTime(2017, 8, 23), 12.44m,12.66m,12.41m,12.57m,96871500m,1215804672.00m, Period.Day),
                        new PriceItem(new DateTime(2017, 8, 24), 12.58m,12.66m,12.45m,12.47m,62153100m,779043392.00m, Period.Day),
                        new PriceItem(new DateTime(2017, 8, 25), 12.47m,12.80m,12.47m,12.78m,145590600m,1842766848.00m, Period.Day),
                        new PriceItem(new DateTime(2017, 8, 28), 12.83m,12.96m,12.75m,12.89m,104113100m,1339965952.00m, Period.Day),
                        new PriceItem(new DateTime(2017, 8, 29), 12.85m,12.98m,12.77m,12.96m,60063900m,772995392.00m, Period.Day),
                        new PriceItem(new DateTime(2017, 8, 30), 12.91m,12.98m,12.82m,12.87m,60479900m,779708160.00m, Period.Day),
                        new PriceItem(new DateTime(2017, 8, 31), 12.83m,12.84m,12.65m,12.71m,44790300m,570138112.00m, Period.Day),
                        new PriceItem(new DateTime(2017, 9, 1), 12.68m,12.87m,12.68m,12.77m,39264100m,501796416.00m, Period.Day),
                        new PriceItem(new DateTime(2017, 9, 4), 12.78m,12.84m,12.65m,12.78m,35667400m,454951456.00m, Period.Day),
                        new PriceItem(new DateTime(2017, 9, 5), 12.78m,13.09m,12.78m,13.03m,73493000m,954528896.00m, Period.Day),
                        new PriceItem(new DateTime(2017, 9, 6), 12.99m,13.05m,12.90m,12.96m,33823500m,438219616.00m, Period.Day),
                        new PriceItem(new DateTime(2017, 9, 7), 12.94m,13.02m,12.82m,12.85m,38224700m,494212416.00m, Period.Day),
                        new PriceItem(new DateTime(2017, 9, 8), 12.85m,13.12m,12.83m,13.03m,64778087m,842285760.00m, Period.Day)
                    }
                }
            };
        }

        [TestMethod]
        public void Test_GetDateForPeriod()
        {
            Assert.AreEqual(MathLib.GetDateForPeriod(new DateTime(2017, 11, 1), Period.Day), new DateTime(2017, 11, 1));
            Assert.AreEqual(MathLib.GetDateForPeriod(new DateTime(2017, 11, 1), Period.Week), new DateTime(2017, 11, 3));
            Assert.AreEqual(MathLib.GetDateForPeriod(new DateTime(2017, 11, 1), Period.Month), new DateTime(2017, 11, 30));
        }

        [TestMethod]
        public void Test_GetPreviousDate()
        {
            Assert.AreEqual(MathLib.GetPreviousDate(new DateTime(2017, 11, 1), Period.Day), new DateTime(2017, 10, 31));
            Assert.AreEqual(MathLib.GetPreviousDate(new DateTime(2017, 11, 1), Period.Week), new DateTime(2017, 10, 27));
            Assert.AreEqual(MathLib.GetPreviousDate(new DateTime(2017, 11, 1), Period.Month), new DateTime(2017, 10, 31));
        }

        [TestMethod]
        public void Test_FindIndex()
        {
            var list = new PriceList();
            list.Period = Period.Day;
            list.interval = 1;
            list.Items.Add(new PriceItem() { Date = new DateTime(2017, 1, 2)});
            list.Items.Add(new PriceItem() { Date = new DateTime(2012, 11, 22) });
            list.Items.Add(new PriceItem() { Date = new DateTime(2017, 4, 12) });
            list.Items.Add(new PriceItem() { Date = new DateTime(1988, 1, 2) });
            list.Items.Add(new PriceItem() { Date = new DateTime(1976, 1, 2) });
            Assert.AreEqual(list.FindIndex(new DateTime(2017, 1, 2), DateNotFound.None), 0);
            Assert.AreEqual(list.FindIndex(new DateTime(2017, 4, 12), DateNotFound.None), 2);
            Assert.AreEqual(list.FindIndex(new DateTime(1976, 1, 2), DateNotFound.None), 4);
            Assert.AreEqual(list.FindIndex(new DateTime(2017, 11, 2), DateNotFound.None), -1);
        }

        [TestMethod]
        public void Test_Cross()
        {
            var stockData = GetStockData();
            stockData.AddAveragedData(Period.Day, 5);
            stockData.AddAveragedData(Period.Day, 10);
            stockData.AddAveragedData(Period.Day, 20);
            stockData.AddAveragedData(Period.Week, 5);
            stockData.AddAveragedData(Period.Week, 10);
            stockData.AddAveragedData(Period.Week, 20);
            stockData.AddAveragedData(Period.Month, 5);
            stockData.AddAveragedData(Period.Month, 10);
            stockData.AddAveragedData(Period.Month, 20);

            Assert.IsTrue(stockData.AveragedPriceCrossed(stockData.FindAverageDataList(Period.Day, 5), stockData.FindAverageDataList(Period.Day, 10), new DateTime(2017, 8, 25), true));
            Assert.IsFalse(stockData.AveragedPriceCrossed(stockData.FindAverageDataList(Period.Day, 5), stockData.FindAverageDataList(Period.Day, 10), new DateTime(2017, 8, 24), true));
            Assert.IsTrue(stockData.AveragedPriceCrossed(stockData.FindAverageDataList(Period.Day, 5), stockData.FindAverageDataList(Period.Day, 10), new DateTime(2017, 8, 29), 5, true));
            Assert.IsFalse(stockData.AveragedPriceCrossed(stockData.FindAverageDataList(Period.Day, 5), stockData.FindAverageDataList(Period.Day, 10), new DateTime(2017, 8, 29), 2, true));
        }

        [TestMethod]
        public void Test_GetPoles()
        {
            var items = Utils.ReadData(@"c:\tt\ConsoleApplication1\UnitTestProject1\SH#000001.txt", new DateTime(2019, 10, 9), new DateTime(2019, 11, 13));
            var pl = new PriceList
            {
                Items = items,
                interval = 1,
                Period = Period.Day
            };
            var poles = PoleLib.FindPoles(pl, new DateTime(2019, 10, 9), new DateTime(2019, 11, 13));
            Assert.AreEqual(poles.Count, 8);
        }

        [TestMethod]
        public void Test_FindDoublePole()
        {
            var items = Utils.ReadData(@"c:\tt\ConsoleApplication1\UnitTestProject1\SH#000001.txt", new DateTime(2019, 10, 9), new DateTime(2019, 11, 13));
            var stockData = new StockData
            {
                Code = "test",
                Name = "test",
                RawData = new PriceList
                {
                    Items = items,
                    interval = 1,
                    Period = Period.Day
                }
            };

            PoleLib.FindDoublePole(stockData, Period.Day, new DateTime(2019, 10, 9), new DateTime(2019, 11, 13));
            Assert.IsNotNull(stockData.DoublePoles.Count > 0);
        }

        [TestMethod]
        public void Test_StringStartWith()
        {
            var ETFCodes = new List<string> { "159", "510", "511", "512", "513", "515", "518" };

            Assert.IsTrue("159980".StartsWithThese(ETFCodes));
            Assert.IsFalse("0159980".StartsWithThese(ETFCodes));
            Assert.IsTrue("5118788adsb".StartsWithThese(ETFCodes));
            Assert.IsTrue("518lji".StartsWithThese(ETFCodes));
            Assert.IsFalse("5511kjljlk".StartsWithThese(ETFCodes));
        }

        [TestMethod]
        public void Test_Portfolio()
        {
            var p = new Portfolio();
            Assert.IsNotNull(p.Holdings[0]);

            p.Check();
        }

        [TestMethod]
        public void TestEmail()
        {
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("Hello", "gingin.gui@gmail.com"));
            mailMessage.To.Add(new MailboxAddress("Jian", "ljianl@yahoo.com.au"));
            mailMessage.Subject = "Test";
            mailMessage.Body = new TextPart("plain")
            {
                Text = "Hello"
            };

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Connect("smtp.gmail.com", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);
                smtpClient.Authenticate("gingin.gui@gmail.com", "Porky20151228");
                smtpClient.Send(mailMessage);
                smtpClient.Disconnect(true);
            }
        }
    }
}