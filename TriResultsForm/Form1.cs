using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AppServiceInterfaces;
using FileAppServices;
using Microsoft.WindowsAPICodePack.Dialogs;
using Optional;
using Optional.Unsafe;
using TriResultsDomainService;
using UrlResultsFetcher;
using RaceDataFileUtils = TriResultsCsvReader.Utils.RaceDataFileUtils;

namespace TriResultsForm
{
    public partial class Form1 : Form
    {
        public bool IsFileSelected { get; set; }

        public ProgramOptions Options { get; private set; }

        public Tuple<string,DateTime> UrlRaceData { get; set; }


        private DataSet _previewDataSet;
        private DataTable _currentPreviewDataTable;

        public Form1()
        {
            Options = new ProgramOptions()
            {
                // default parameters
                ConfigFile = ".\\column_config.xml",
                MemberFile = ".\\leden2017.csv",
                OutputFolder = ".\\Output",
                Verbose = true
            };

            InitializeComponent();


            var start = 2010;
            var years = Enumerable.Range(start, DateTime.UtcNow.Year - start + 1).Reverse().Select(y => new SelectItem() {Text = y.ToString(), Value = y}).ToList();
            years.Add(new SelectItem() {Text = "All", Value = -1});
            outputYearComboBox.DataSource = years;
            outputYearComboBox.DisplayMember = "Text";
            outputYearComboBox.ValueMember = "Value";
        }

        public string InputFolder { get; set; }

        private void selectInputFolderButton_Click(object sender, EventArgs e)
        {
            var dirDialog = new CommonOpenFileDialog("Select input file or folder")
            {
                EnsurePathExists = true,
                AllowNonFileSystemItems = false,
                IsFolderPicker = true
            };

            var dialogResult = dirDialog.ShowDialog();
            if (dialogResult == CommonFileDialogResult.Ok)
            {
                Options.InputFolderOrFile = dirDialog.FileName;
                inputFolderTextBox1.Text = Options.InputFolderOrFile;
            }
        }

        private void selectFilterMemberButton_Click(object sender, EventArgs e)
        {
            var fileDialog = new OpenFileDialog();

            var dialogResult = fileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                Options.MemberFile = fileDialog.FileName;
                filterMemberTextBox.Text = Options.MemberFile;
            }
        }

        private void outputFolderButton4_Click(object sender, EventArgs e)
        {
            raceGroupBox1.Visible = false;

            var dirDialog = new CommonOpenFileDialog("Select input file or folder")
            {
                EnsurePathExists = true,
                AllowNonFileSystemItems = false,
                IsFolderPicker = true
            };

            var dialogResult = dirDialog.ShowDialog();
            if (dialogResult == CommonFileDialogResult.Ok)
            {
                Options.OutputFolder = dirDialog.FileName;
                outputFolderTextBox.Text = Options.OutputFolder;
            }
        }


        private void openFileButton_Click(object sender, EventArgs e)
        {
            var fileDialog = new CommonOpenFileDialog("Select input file")
            {
                EnsurePathExists = true,
                AllowNonFileSystemItems = false,
                IsFolderPicker = false
            };

            var dialogResult = fileDialog.ShowDialog();
            if (dialogResult == CommonFileDialogResult.Ok)
            {
                inputFolderTextBox1.Text = fileDialog.FileName;
                Options.InputFolderOrFile = fileDialog.FileName;

                // show validate file button
                IsFileSelected = true;
                validateFileButton5.Visible = IsFileSelected;

                raceGroupBox1.Visible = true;

                // parse file datetime
                var raceData = new RaceDataFileUtils().GetRaceDataFromFilename(Options.InputFolderOrFile);

                if (raceData.HasValue)
                {
                    var date = raceData.ValueOrDefault().Date;
                    var race = raceData.ValueOrDefault().Name;

                    SetRaceName(date, race, Options);
                }
            }
        }

        private void openConfigFileButton_Click(object sender, EventArgs e)
        {
            var fileDialog = new CommonOpenFileDialog("Select columns config file")
            {
                EnsurePathExists = true,
                AllowNonFileSystemItems = false,
                IsFolderPicker = false
            };

            var dialogResult = fileDialog.ShowDialog();
            if (dialogResult == CommonFileDialogResult.Ok)
            {
                columnsConfigFileTextBox.Text = fileDialog.FileName;
                Options.ConfigFile = fileDialog.FileName;

            }
        }

        private void SetRaceName(Option<DateTime> date, Option<string> raceName, ProgramOptions options)
        {
            if (date.HasValue)
            {
                raceDateTimePicker.Value = date.ValueOr(DateTime.Now);
            }

            if (raceName.HasValue)
            {
                raceNameTextBox1.Text = raceName.ValueOrDefault();
            }
            if (raceName.HasValue && date.HasValue)
            {
                options.RaceName = raceName.ValueOrDefault();
            }
        }

        private void filteredCsvOutputCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Options.OutputCsv = filteredCsvOutputCheckBox.Checked;
        }

        private void outputSqlCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Options.OutputSql = outputSqlCheckBox.Checked;
        }

        private void outputHtmlCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Options.OutputSql = outputHtmlCheckBox.Checked;
        }

        private void runButton5_Click(object sender, EventArgs e)
        {
            outputTextBox.Text += "Running... please wait";

            var csvRaceResultsReader = new CsvResultsReader();
            var runner = new ProgramRunner(csvRaceResultsReader);

            bool success = false;

            Options.InputFiles = new FileUtils().GetAllFiles(Options.InputFolderOrFile);
            try
            {
                success = runner.Process(Options);
            }
            catch (CsvFormatException ex)
            {
                outputTextBox.Text += ex.Message;
            }

            if (!success)
            {
                outputTextBox.Text = string.Join("\n", runner.Errors);
            }
            else
            {
                outputTextBox.Text = string.Join("\n", runner.Info);
                runner.Info.Clear();
            }
        }

        private void verboseCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            Options.Verbose = verboseCheckBox1.Checked;
        }

        private void validateFileButton5_Click(object sender, EventArgs e)
        {
            if (IsFileSelected)
            {
                Options.RaceName = raceNameTextBox1.Text;

                if (string.IsNullOrEmpty(Options.RaceName))
                {
                    outputTextBox.Text += Environment.NewLine + "Do you want to add a race name or is that in the csv file?" + Environment.NewLine;
                }
            }

            var csvRaceResultsReader = new CsvResultsReader();
            var runner = new ProgramRunner(csvRaceResultsReader);

            Options.InputFiles = new FileUtils().GetAllFiles(Options.InputFolderOrFile);

            var valid = runner.Test(Options);

            outputTextBox.Text += string.Join(Environment.NewLine, runner.Info);

            if (!valid)
            {
                outputTextBox.Text += string.Join(Environment.NewLine, runner.Errors);
            }
            else
            {
                outputTextBox.Text += $"File '{Options.InputFolderOrFile}' is GOOD!" + Environment.NewLine;

                outputTextBox.Text += String.Join(Environment.NewLine, runner.Output);
            }
        }

        private void raceDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            var date = raceDateTimePicker.Value;
            Options.RaceDate = date;

            var raceName = raceNameLabel1.Text;

            SetRaceName(Option.Some(date), string.IsNullOrEmpty(raceName) ? Option.None<string>() : Option.Some(raceName), Options);
        }

        private void readUrlButton3_Click(object sender, EventArgs e)
        {
            var urlText = urlTextBox1.Text;

            Uri url = null;

            try
            {
                url = new Uri(urlText);
            }
            catch (UriFormatException ex)
            {
                urlOutputTextBox2.Text += Environment.NewLine + "Invalid URL: " + urlText;
                return;
            }

            if (string.IsNullOrEmpty(url.Host))
            {
                urlOutputTextBox2.Text += Environment.NewLine + "Invalid URL: " + urlText;
                return;
            }

            IHtmlTableParser htmlTableParser = HtmlTableParserFactory.Get(url.ToString());
            IHtmlResultsFetch resultsFetcher = new ResultsPageFetcher(htmlTableParser, url);

            var raceAndDate = resultsFetcher.GetRaceData();
            var raceData = raceAndDate.ValueOr(new Tuple<string, DateTime>("Couldn't get racename", DateTime.Now));
            UrlRaceData = raceData;

            _previewDataSet = resultsFetcher.GetData();
            tableSelectorUpDown.Maximum = _previewDataSet.Tables.Count + 1;

            urlOutputTextBox2.Text = raceData.Item1;
            urlRacenameTextBox1.Text = raceData.Item1;
            urlRaceDateTimePicker.Value = raceData.Item2;
            urlRaceGroupBox2.Visible = true;

            UpdateFont();
            urlDataGridView1.DataSource = _previewDataSet;

            SetCurrentPreviewTable(0);
        }

        private void SetCurrentPreviewTable(int tableIndex)
        {
            var table = _previewDataSet.Tables[tableIndex];
            _currentPreviewDataTable = table;
            urlDataGridView1.DataMember = table.TableName;
        }

        private void UpdateFont()
        {
            foreach (DataGridViewColumn c in urlDataGridView1.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Arial", 9F, GraphicsUnit.Pixel);
            }
        }

        private void urlSaveButton3_Click(object sender, EventArgs e)
        {
            UrlRaceData = new Tuple<string, DateTime>(urlRacenameTextBox1.Text, urlRaceDateTimePicker.Value); // raceData;

            var filename = TriResultsCsvReader.DateUtils.ToRaceFilename(UrlRaceData.Item2, UrlRaceData.Item1);

            var fileDialog = new CommonSaveFileDialog("Save file as csv")
            {
                //EnsurePathExists = true,
                DefaultExtension = "csv",
                DefaultFileName = filename + ".csv"
            };
            
            var dialogResult = fileDialog.ShowDialog();
            if (dialogResult == CommonFileDialogResult.Ok)
            {
                var resultsList = TableToLists(_currentPreviewDataTable);
                WriteToCsv(resultsList, fileDialog.FileName);
            }
        }

        private void WriteToCsv(List<List<string>> results, string filename)
        {
            File.WriteAllLines(filename, results.Select(row => String.Join(",", row)));
        }

        private List<List<string>> TableToLists(DataTable table)
        {
            var results = new List<List<string>>();

            int numberOfColumns = table.Columns.Count;

            // go through each row
            foreach (DataRow dr in table.Rows)
            {
                var rowList = new List<string>(table.Rows.Count);

                // go through each column in the row
                for (int i = 0; i < numberOfColumns; i++)
                {
                    
                    // access cell as set or get
                    string cell = Convert.ToString(dr[i]);
                    rowList.Add(cell);
                }
                results.Add(rowList);
            }

            return results;
        }

        private void yearComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selected = (SelectItem) outputYearComboBox.SelectedItem;

            Options.FilterYear = selected.Value;
        }

        private void nextPrevTableButton_Click(object sender, EventArgs e)
        {
            var currTable = (int) tableSelectorUpDown.Value - 1;
            var numTables = _previewDataSet.Tables.Count;

            if (numTables > currTable+1)
            {
                tableSelectorUpDown.Value = tableSelectorUpDown.Value + 1;
                SetCurrentPreviewTable(currTable + 1);
            }
        }

        private void previousPreviewTableButton_Click(object sender, EventArgs e)
        {
            var currTable = (int)tableSelectorUpDown.Value - 1;
            
            if (currTable > 0)
            {
                tableSelectorUpDown.Value = tableSelectorUpDown.Value - 1;
                SetCurrentPreviewTable(currTable - 1);
            }

        }
    }
}
