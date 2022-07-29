using DataAnalyst.Base;
using DataAnalyst.Score;
using DataAnalyst.Cross;
using DataAnalyst.Pole;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using DataAnalyst.ETF;
using DataAnalyst.Watchlist;
using System.Collections.Generic;
using DataAnalyst.Portfolio;

namespace gp
{
    public partial class Form1 : Form
    {
        private const string FilePath = @"C:\zd_gfzq\T0002\export\";
        private StockDataSet data = new StockDataSet();

        public Form1()
        {
            InitializeComponent();

            txtPath.Text = FilePath;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private async void btnLoadData_Click(object sender, EventArgs e)
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            var start = DateTime.Now;
            var files = Directory.GetFiles(txtPath.Text);

            data.Clear();
            int count = 0;
            foreach (var file in files)
            {
                count++;
                if (count % 10 == 0)
                    lblProgress.Text = count.ToString();

                data.AddStockData(await StockData.ReadData(file, new DateTime(2015, 01, 01), DateTime.Today));
            }
            ETFTrial.Init(data);
            WatchlistTrial.Init(data);

            MessageBox.Show($"Total time: {DateTime.Now.Subtract(start).TotalSeconds.ToString()}");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (tabFeatures.SelectedTab == tabCross)
                FindCrosses();
            else if (tabFeatures.SelectedTab == tabScores)
                FindScores();
            else if (tabFeatures.SelectedTab == tabPole)
                FindDoublePoles();
            else if (tabFeatures.SelectedTab == tabETF)
                ListETFWinners();
            else if (tabFeatures.SelectedTab == tabTurning)
                FindTurning();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtResult.Clear();
        }

        private void FindCrosses()
        {
            Period period = Period.Day;
            if (this.rbMonth.Checked)
                period = Period.Month;
            else if (this.rbWeek.Checked)
                period = Period.Week;

            CrossDataLib.FindCross(data, period, calendarCross.SelectionRange.Start);

            StockDataSet subSet;
            if (cbIndices.Checked)
            {
                subSet = data.GetSubSet(s => s.Code.StartsWithThese(new List<string> { "880", "399" }) 
                                            || ETFTrial.ETFCodes.Contains(s.Code));
            }
            else
            {
                subSet = data;
            }

            if (chkFindBottom.Checked)
            {
                var crossData = subSet.GetSubSet(stockData => stockData.StockCrossData.Count > 0);
                PoleLib.FindDoublePolesWithCross(crossData, period, 60);
                crossData.DataList.Where(d => d.DoublePoles.Count > 0)
                 .ToList()
                 .ForEach(d => txtResult.AppendText(
                     $"{d.Code}, {d.Name}, {d.DoublePoles[0].Pole1.Item.Date.ToString("yyyy-MM-dd")}, {d.DoublePoles[0].Pole2.Item.Date.ToString("yyyy-MM-dd")}, {Environment.NewLine}"));
            }
            else
            {
                foreach (var c in subSet.GetCrossedData())
                {
                    var d = c.StockCrossData[0];
                    txtResult.AppendText($"{d.Period}, {c.Code}, {c.Name} crossed at {d.CrossDate.ToString("yyyy-MM-dd")} {Environment.NewLine}");
                }
            }
        }

        private void FindScores()
        {
            var startDate = calendarScore.SelectionStart;
            var endDate = calendarScore.SelectionEnd;

            StockDataSet subSet;
            if (rbScoresStock.Checked)
            {
                subSet = data.GetSubSet(s => s.StockExchange == Exchange.SZ && s.Code.StartsWithThese(new List<string> { "00", "30" })
                                                || s.StockExchange == Exchange.SH && s.Code.StartsWithThese(new List<string> { "6" }));
            }
            else
            {
                subSet = data.GetSubSet(s => s.Code.StartsWithThese(new List<string> { "880", "399" }));
            }

            ScoreLib.FindScores(subSet, startDate, endDate);

            if (rbByScore.Checked)
                subSet.SummaryList.Sort((cs1, cs2) => cs1.GetTotalScore(startDate, endDate).CompareTo(cs2.GetTotalScore(startDate, endDate)));
            else
                subSet.SummaryList.Sort((cs1, cs2) => -1 * cs1.GetChangedPercentage(startDate, endDate).CompareTo(cs2.GetChangedPercentage(startDate, endDate)));

            subSet.SummaryList
                //.Take(50)
                .ToList()
                .ForEach(s => txtResult.AppendText($"{s.Code}, {s.Name}, {s.GetTotalScore(startDate, endDate)}, {s.GetChangedPercentage(startDate, endDate).ToString("0.0000")}, {Environment.NewLine}"));
        }

        private void FindDoublePoles()
        {
            Period period = Period.Day;
            if (this.rbMonthPole.Checked)
                period = Period.Month;
            else if (this.rbWeekPole.Checked)
                period = Period.Week;

            PoleLib.FindDoublePoles(data, period, calendarPoleStart.SelectionStart, calendarPoleEnd.SelectionEnd);
            data.DataList.Where(d => d.DoublePoles.Count > 0)
                .ToList()
                .ForEach(d => txtResult.AppendText(
                    $"{d.Code}, {d.Name}, {d.DoublePoles[0].Pole1.Item.Date.ToString("yyyy-MM-dd")}, {d.DoublePoles[0].Pole2.Item.Date.ToString("yyyy-MM-dd")}, {Environment.NewLine}"));
        }

        private void ListETFWinners()
        {
            var startDate = calendarETF.SelectionStart;
            var endDate = calendarETF.SelectionEnd;

            var result = ETFTrial.GetTopNCodes(startDate, endDate, 1000);
            foreach (var t in result)
            {
                if (t.Item3 != null)
                    txtResult.AppendText($"{t.Item1}, {t.Item2}, {t.Item3.ChangePercentage.ToString("0.0000")} {Environment.NewLine}");
            }
            
            if (endDate.Subtract(startDate).TotalDays > 20)
            {
                return;
            }

            while (startDate <= endDate)
            {
                ETFTrial.DataSet.SummaryList
                    .Sort((cs1, cs2) => -1 * cs1.GetChangedPercentage(startDate, startDate).CompareTo(cs2.GetChangedPercentage(startDate, startDate)));

                txtResult.AppendText(startDate.ToString("yyyy-MM-dd"));
                txtResult.AppendText(Environment.NewLine);

                ETFTrial.DataSet.SummaryList
                    .Take(10)
                    .ToList()
                    .ForEach(s => txtResult.AppendText($"{s.Code}, {s.Name}, {s.GetChangedPercentage(startDate, startDate).ToString("0.0000")}, {Environment.NewLine}"));
                txtResult.AppendText(Environment.NewLine);

                startDate = startDate.AddDays(1);
            }
        }

        private void btnNewStocks_Click(object sender, EventArgs e)
        {
            int days;
            if (!int.TryParse(txtDays.Text, out days))
            {
                MessageBox.Show("Please input days");
                return;
            }

            foreach(var stockData in data.DataList.Where(list => list.Code.StartsWith("0") || list.Code.StartsWith("3") || list.Code.StartsWith("6")))
            {
                if (stockData.RawData.Count > 0 && stockData.RawData.Count <= days)
                {
                    txtResult.AppendText($"{stockData.Code}, {stockData.Name} {Environment.NewLine}");
                }
            }
        }

        private void FindTurning()
        {
            var dataset = chkWatchlist.Checked ? WatchlistTrial.DataSet
                : chkBlockIndices.Checked ? this.data.GetSubSet(s => s.Code.StartsWith("880")) : this.data; 

            if (chkAllStocks.Checked || chkWatchlist.Checked || chkBlockIndices.Checked)
            {
                List<Tuple<string, string, DateTime>> results = new List<Tuple<string, string, DateTime>>();

                dataset.DataList.ForEach(d => {
                    d.FindTurningBackward(Period.Day, DateTime.Today.AddDays(-30), 3);
                    if (d.Turnings.Count > 0)
                    {
                        results.Add(new Tuple<string, string, DateTime>(d.Code, d.Name, d.Turnings[0]));
                    }
                });

                //results.Sort((t1, t2) => t1.Item3 >= t2.Item3 ? 1 : 0);
                results.Sort((t1, t2) => t1.Item3.CompareTo(t2.Item3));
                results.ForEach(t => txtResult.AppendText($"{t.Item1}, {t.Item2}, {t.Item3.ToString("dd-MM-yyyy")} {Environment.NewLine}"));
            }
            else if (textTurningStock.TextLength > 0)
            {
                var stocks = textTurningStock.Text.Split(',').ToList();
                dataset.DataList.ForEach(d => {
                    if (stocks.Contains(d.Code))
                    {
                        d.FindTurningForward(Period.Day, DateTime.Now.AddYears(-5).AddMonths(-3), 3);
                        d.Turnings.ForEach(t => txtResult.AppendText($"{t.ToString()} {Environment.NewLine}"));
                    }
                });
            }

        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            var p = new Portfolio();
            p.StartWorking();
        }
    }
}
