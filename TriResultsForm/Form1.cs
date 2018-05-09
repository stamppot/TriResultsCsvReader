using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Microsoft.WindowsAPICodePack.Dialogs;
using Optional;
using Optional.Unsafe;
using TriResultsCsvReader;
using TriResultsCsvReader.Utils;
using UrlResultsFetcher;

namespace TriResultsForm
{
    public partial class Form1 : Form
    {
        public bool IsFileSelected { get; set; }

        public ProgramOptions Options { get; private set; }

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

            // TODO: be able to choose file or folder
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
                var raceData = new FileUtils().GetRaceDataFromFilename(Options.InputFolderOrFile);

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

            var runner = new ProgramRunner();

            bool success = false;

            try
            {
                success = runner.Process(Options);
            }
            catch (CsvFormatException ex)
            {

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

            // TODO: run something like runner.Process which tests a single csv file
            var runner = new ProgramRunner();

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
            }

            var urlFetcher = new FetchUrlContents();
            var doc = urlFetcher.GetPage(url);
            var resultsTable = urlFetcher.GetResultsTable(doc);

            urlDataGridView1.DataSource = resultsTable;

        }
    }
}
