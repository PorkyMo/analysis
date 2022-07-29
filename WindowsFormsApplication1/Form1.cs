using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await CleanFile();
            ////await TestAsync();
            //var task = AsyncSleep();
            //await task;
            //MessageBox.Show(task.Result.ToString());
        }

        private async Task TestAsync()
        {
            await Task.Delay(10000);
        }

        private Task<int> AsyncSleep()
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();

            //Task.Run(async () => await Task.Delay(10000)).ConfigureAwait(false).GetAwaiter().GetResult();
            Task.Run(() => { Sleep(); tcs.SetResult(100); });

            return tcs.Task;
        }

        private void Sleep()
        {
            System.Threading.Thread.Sleep(10000);
        }

        private async Task CleanFile()
        {
            //System.Text.Encoding.GetEncoding(936);
            StreamWriter sw = new StreamWriter(txtFilename.Text + "1", false, Encoding.GetEncoding(936));
            StreamReader sr = new StreamReader(txtFilename.Text, System.Text.Encoding.GetEncoding(936));
            var line = await sr.ReadLineAsync();
            while (line != null)
            {
                if (!line.Contains("申购配号") && !line.Contains("港股通组合费收取"))
                {
                    await sw.WriteLineAsync(line);
                }

                line = await sr.ReadLineAsync();
            }
            sr.Close();
            sw.Close();
        }
    }
}
