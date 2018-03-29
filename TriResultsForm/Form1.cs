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
using TriResultsCsvReader;

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
                Options.OutputFolder = fileDialog.FileName;
                outputFolderTextBox.Text = Options.OutputFolder;

                // show validate file button
                IsFileSelected = true;
                validateFileButton5.Visible = IsFileSelected;
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
        }

        private void verboseCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            Options.Verbose = verboseCheckBox1.Checked;
        }

        private void validateFileButton5_Click(object sender, EventArgs e)
        {
            // TODO: run something like runner.Process which tests a single csv file
        }
    }
}
