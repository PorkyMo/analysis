using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace ProcessExposure
{
    public partial class MainForm : Form
    {
        private string Location;
        private string StartFile;
        private string EndFile;
        private double InitialExposure;
        private double IncrementValue;
        private int StartNumber;
        private int EndNumber;
        private string FilePrefix;
        private string FileExtension;
        private int NumberLength;

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnSetValue_Click(object sender, EventArgs e)
        {
            if (!ValidateFile())
                return;

            if (!ValidateValue(txtExposureValue.Text.Trim(), "曝光值不能为空"))
                return;

            InitialExposure = double.Parse(txtExposureValue.Text.Trim());

            SetExposureValue();

            MessageBox.Show("操作完成");
        }

        private void btnIncreaseValue_Click(object sender, EventArgs e)
        {
            if (!ValidateFile())
                return;

            if (!ValidateValue(txtExposureValue.Text.Trim(), "曝光值不能为空"))
                return;

            if (!ValidateValue(txtIncrementValue.Text.Trim(), "递增值不能为空"))
                return;

            InitialExposure = double.Parse(txtExposureValue.Text.Trim());
            IncrementValue = double.Parse(txtIncrementValue.Text.Trim());

            SetExposureValue();

            MessageBox.Show("操作完成");
        }

        private void SetExposureValue()
        {
            var exposureValue = InitialExposure;
            for(int i=StartNumber; i<=EndNumber; i++)
            {
                ChangeExposureNode(Path.Combine(Location, $"{FilePrefix}{i.ToString("0000")}.{FileExtension}"), exposureValue, SetNodeExposure);
                exposureValue = exposureValue + IncrementValue;
            }
        }

        private void IncreaseExposureValue()
        {
            for (int i = StartNumber; i <= EndNumber; i++)
            {
                ChangeExposureNode(Path.Combine(Location, $"{FilePrefix}{i.ToString("0000")}.{FileExtension}"), IncrementValue, IncreaseNodeExposure);
            }
        }

        private bool ValidateFile()
        {
            Location = txtLocation.Text.Trim();
            if (!ValidateLocation(Location))
                return false;

            StartFile = txtStartFile.Text.Trim();
            if (!ValidateFileName(StartFile, "起始文件不能为空"))
                return false;

            StartNumber = ValidateNumber(StartFile);
            if (StartNumber < 0)
                return false;

            EndFile = txtEndFile.Text.Trim();
            if (!ValidateFileName(EndFile, "结束文件不能为空"))
                return false;

            EndNumber = ValidateNumber(EndFile);
            if (EndNumber < 0)
                return false;

            return true;
        }

        private bool ValidateLocation(string location)
        {
            if (!Validate(x => x.Length > 0, location, "文件路径不能为空"))
                return false;

            if (!Validate(x => Directory.Exists(x), location, $"文件路径不存在: {location}"))
                return false;

            return true;
        }

        private bool ValidateFileName(string filename, string errorMessage)
        {
            return Validate(x => x.Length > 0, filename, errorMessage);
        }

        private bool ValidateValue(string inputValue, string errorMessage)
        {
            double value;

            return Validate(x => double.TryParse(inputValue, out value), inputValue, errorMessage);
        }

        private bool Validate(Func<string, bool>validate, string inputValue, string errorMessage)
        {
            if (!validate(inputValue))
            {
                MessageBox.Show(errorMessage);
                return false;
            }

            return true;
        }

        private int ValidateNumber(string filename)
        {
            var part = filename.Split('.')[0].ToCharArray().ToList();
            part.Reverse();
            int firstNotDigit = part.FindIndex(c => !Char.IsDigit(c));
            if (firstNotDigit > 0)
            {
                FilePrefix = filename.Substring(0, part.Count - firstNotDigit);
                FileExtension = new string(filename.Split('.')[1].ToCharArray());
                NumberLength = firstNotDigit;
                return int.Parse(filename.Substring(part.Count - firstNotDigit, firstNotDigit));
            }

            MessageBox.Show($"文件名 {filename} 没有数字");
            return -1;
        }

        private static void ChangeExposureNode(string filePath, double exposure, Action<XmlNode, double> action)
        {
            if (!File.Exists(filePath))
                return;

            var doc = new XmlDocument();
            doc.Load(filePath);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("x", "adobe:ns:meta/");
            nsmgr.AddNamespace("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");

            var nodes = doc.DocumentElement.SelectNodes("/x:xmpmeta/rdf:RDF/rdf:Description", nsmgr);
            action(nodes[0], exposure);
            var settings = new XmlWriterSettings();
            settings.NewLineOnAttributes = true;
            settings.Indent = true;

            var xmlWriter = XmlWriter.Create(filePath, settings);
            doc.Save(xmlWriter);
            xmlWriter.Close();
        }

        private void SetNodeExposure(XmlNode node, double exposure)
        {
            var exposureAttr = node.Attributes["crs:Exposure2012"];
            var sign = exposure >= 0 ? "+" : "";
            exposureAttr.Value = $"{sign}{exposure.ToString()}";
        }

        private void IncreaseNodeExposure(XmlNode node, double delta)
        {
            var exposureAttr = node.Attributes["crs:Exposure2012"];
            double exposure;
            if (!double.TryParse(exposureAttr.Value, out exposure))
                return;

            exposure = exposure + delta;

            var sign = exposure >= 0 ? "+" : "";
            exposureAttr.Value = $"{sign}{exposure.ToString()}";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                txtLocation.Text = folderBrowserDialog.SelectedPath;
            }
        }
    }
}
