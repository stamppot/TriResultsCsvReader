namespace TriResultsForm
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.inputFolderLabel1 = new System.Windows.Forms.Label();
            this.inputGroupBox1 = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.columnsConfigFileTextBox = new System.Windows.Forms.TextBox();
            this.configFileLabel1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.filterMemberTextBox = new System.Windows.Forms.TextBox();
            this.filterMemberLabel = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.inputFolderTextBox1 = new System.Windows.Forms.TextBox();
            this.inputGroupBox = new System.Windows.Forms.GroupBox();
            this.verboseCheckBox1 = new System.Windows.Forms.CheckBox();
            this.outputHtmlCheckBox = new System.Windows.Forms.CheckBox();
            this.filteredCsvOutputCheckBox = new System.Windows.Forms.CheckBox();
            this.outputSqlCheckBox = new System.Windows.Forms.CheckBox();
            this.button4 = new System.Windows.Forms.Button();
            this.outputFolderTextBox = new System.Windows.Forms.TextBox();
            this.outputFolderLabel = new System.Windows.Forms.Label();
            this.outputGroupBox1 = new System.Windows.Forms.GroupBox();
            this.outputTextBox = new System.Windows.Forms.TextBox();
            this.runButton5 = new System.Windows.Forms.Button();
            this.openFileButton = new System.Windows.Forms.Button();
            this.validateFileButton5 = new System.Windows.Forms.Button();
            this.inputGroupBox1.SuspendLayout();
            this.inputGroupBox.SuspendLayout();
            this.outputGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // inputFolderLabel1
            // 
            this.inputFolderLabel1.AutoSize = true;
            this.inputFolderLabel1.Location = new System.Drawing.Point(6, 30);
            this.inputFolderLabel1.Name = "inputFolderLabel1";
            this.inputFolderLabel1.Size = new System.Drawing.Size(78, 17);
            this.inputFolderLabel1.TabIndex = 0;
            this.inputFolderLabel1.Text = "File / folder";
            // 
            // inputGroupBox1
            // 
            this.inputGroupBox1.Controls.Add(this.openFileButton);
            this.inputGroupBox1.Controls.Add(this.button3);
            this.inputGroupBox1.Controls.Add(this.columnsConfigFileTextBox);
            this.inputGroupBox1.Controls.Add(this.configFileLabel1);
            this.inputGroupBox1.Controls.Add(this.button2);
            this.inputGroupBox1.Controls.Add(this.filterMemberTextBox);
            this.inputGroupBox1.Controls.Add(this.filterMemberLabel);
            this.inputGroupBox1.Controls.Add(this.button1);
            this.inputGroupBox1.Controls.Add(this.inputFolderTextBox1);
            this.inputGroupBox1.Controls.Add(this.inputFolderLabel1);
            this.inputGroupBox1.Location = new System.Drawing.Point(12, 21);
            this.inputGroupBox1.Name = "inputGroupBox1";
            this.inputGroupBox1.Size = new System.Drawing.Size(396, 245);
            this.inputGroupBox1.TabIndex = 1;
            this.inputGroupBox1.TabStop = false;
            this.inputGroupBox1.Text = "Input options";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(315, 210);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 25);
            this.button3.TabIndex = 8;
            this.button3.Text = "Open";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // columnsConfigFileTextBox
            // 
            this.columnsConfigFileTextBox.Location = new System.Drawing.Point(123, 182);
            this.columnsConfigFileTextBox.Name = "columnsConfigFileTextBox";
            this.columnsConfigFileTextBox.Size = new System.Drawing.Size(267, 22);
            this.columnsConfigFileTextBox.TabIndex = 7;
            this.columnsConfigFileTextBox.Text = ".\\columns_config.xml";
            // 
            // configFileLabel1
            // 
            this.configFileLabel1.AutoSize = true;
            this.configFileLabel1.Location = new System.Drawing.Point(6, 182);
            this.configFileLabel1.Name = "configFileLabel1";
            this.configFileLabel1.Size = new System.Drawing.Size(111, 17);
            this.configFileLabel1.TabIndex = 6;
            this.configFileLabel1.Text = "Columns cfg File";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(315, 126);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 25);
            this.button2.TabIndex = 5;
            this.button2.Text = "Open";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.selectFilterMemberButton_Click);
            // 
            // filterMemberTextBox
            // 
            this.filterMemberTextBox.Location = new System.Drawing.Point(125, 98);
            this.filterMemberTextBox.Name = "filterMemberTextBox";
            this.filterMemberTextBox.Size = new System.Drawing.Size(265, 22);
            this.filterMemberTextBox.TabIndex = 4;
            this.filterMemberTextBox.Text = "leden2017.csv";
            // 
            // filterMemberLabel
            // 
            this.filterMemberLabel.AutoSize = true;
            this.filterMemberLabel.Location = new System.Drawing.Point(6, 98);
            this.filterMemberLabel.Name = "filterMemberLabel";
            this.filterMemberLabel.Size = new System.Drawing.Size(120, 17);
            this.filterMemberLabel.TabIndex = 3;
            this.filterMemberLabel.Text = "Filter/member File";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(315, 58);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 25);
            this.button1.TabIndex = 2;
            this.button1.Text = "Folder...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.selectInputFolderButton_Click);
            // 
            // inputFolderTextBox1
            // 
            this.inputFolderTextBox1.Location = new System.Drawing.Point(95, 30);
            this.inputFolderTextBox1.Name = "inputFolderTextBox1";
            this.inputFolderTextBox1.Size = new System.Drawing.Size(295, 22);
            this.inputFolderTextBox1.TabIndex = 1;
            // 
            // inputGroupBox
            // 
            this.inputGroupBox.Controls.Add(this.verboseCheckBox1);
            this.inputGroupBox.Controls.Add(this.outputHtmlCheckBox);
            this.inputGroupBox.Controls.Add(this.filteredCsvOutputCheckBox);
            this.inputGroupBox.Controls.Add(this.outputSqlCheckBox);
            this.inputGroupBox.Controls.Add(this.button4);
            this.inputGroupBox.Controls.Add(this.outputFolderTextBox);
            this.inputGroupBox.Controls.Add(this.outputFolderLabel);
            this.inputGroupBox.Location = new System.Drawing.Point(12, 280);
            this.inputGroupBox.Name = "inputGroupBox";
            this.inputGroupBox.Size = new System.Drawing.Size(396, 158);
            this.inputGroupBox.TabIndex = 2;
            this.inputGroupBox.TabStop = false;
            this.inputGroupBox.Text = "Output options";
            // 
            // verboseCheckBox1
            // 
            this.verboseCheckBox1.AutoSize = true;
            this.verboseCheckBox1.Checked = true;
            this.verboseCheckBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.verboseCheckBox1.Location = new System.Drawing.Point(307, 134);
            this.verboseCheckBox1.Name = "verboseCheckBox1";
            this.verboseCheckBox1.Size = new System.Drawing.Size(83, 21);
            this.verboseCheckBox1.TabIndex = 11;
            this.verboseCheckBox1.Text = "Verbose";
            this.verboseCheckBox1.UseVisualStyleBackColor = true;
            this.verboseCheckBox1.CheckedChanged += new System.EventHandler(this.verboseCheckBox1_CheckedChanged);
            // 
            // outputHtmlCheckBox
            // 
            this.outputHtmlCheckBox.AutoSize = true;
            this.outputHtmlCheckBox.Checked = true;
            this.outputHtmlCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.outputHtmlCheckBox.Location = new System.Drawing.Point(9, 79);
            this.outputHtmlCheckBox.Name = "outputHtmlCheckBox";
            this.outputHtmlCheckBox.Size = new System.Drawing.Size(68, 21);
            this.outputHtmlCheckBox.TabIndex = 10;
            this.outputHtmlCheckBox.Text = "HTML";
            this.outputHtmlCheckBox.UseVisualStyleBackColor = true;
            this.outputHtmlCheckBox.CheckedChanged += new System.EventHandler(this.outputHtmlCheckBox_CheckedChanged);
            // 
            // filteredCsvOutputCheckBox
            // 
            this.filteredCsvOutputCheckBox.AutoSize = true;
            this.filteredCsvOutputCheckBox.Checked = true;
            this.filteredCsvOutputCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.filteredCsvOutputCheckBox.Location = new System.Drawing.Point(9, 133);
            this.filteredCsvOutputCheckBox.Name = "filteredCsvOutputCheckBox";
            this.filteredCsvOutputCheckBox.Size = new System.Drawing.Size(108, 21);
            this.filteredCsvOutputCheckBox.TabIndex = 9;
            this.filteredCsvOutputCheckBox.Text = "Filtered CSV";
            this.filteredCsvOutputCheckBox.UseVisualStyleBackColor = true;
            this.filteredCsvOutputCheckBox.CheckedChanged += new System.EventHandler(this.filteredCsvOutputCheckBox_CheckedChanged);
            // 
            // outputSqlCheckBox
            // 
            this.outputSqlCheckBox.AutoSize = true;
            this.outputSqlCheckBox.Location = new System.Drawing.Point(9, 106);
            this.outputSqlCheckBox.Name = "outputSqlCheckBox";
            this.outputSqlCheckBox.Size = new System.Drawing.Size(104, 21);
            this.outputSqlCheckBox.TabIndex = 8;
            this.outputSqlCheckBox.Text = "SQL inserts";
            this.outputSqlCheckBox.UseVisualStyleBackColor = true;
            this.outputSqlCheckBox.CheckedChanged += new System.EventHandler(this.outputSqlCheckBox_CheckedChanged);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(315, 59);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 25);
            this.button4.TabIndex = 5;
            this.button4.Text = "Open";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.outputFolderButton4_Click);
            // 
            // outputFolderTextBox
            // 
            this.outputFolderTextBox.Location = new System.Drawing.Point(119, 31);
            this.outputFolderTextBox.Name = "outputFolderTextBox";
            this.outputFolderTextBox.Size = new System.Drawing.Size(271, 22);
            this.outputFolderTextBox.TabIndex = 4;
            this.outputFolderTextBox.Text = ".\\Output";
            // 
            // outputFolderLabel
            // 
            this.outputFolderLabel.AutoSize = true;
            this.outputFolderLabel.Location = new System.Drawing.Point(6, 31);
            this.outputFolderLabel.Name = "outputFolderLabel";
            this.outputFolderLabel.Size = new System.Drawing.Size(95, 17);
            this.outputFolderLabel.TabIndex = 3;
            this.outputFolderLabel.Text = "Output Folder";
            // 
            // outputGroupBox1
            // 
            this.outputGroupBox1.Controls.Add(this.outputTextBox);
            this.outputGroupBox1.Location = new System.Drawing.Point(415, 21);
            this.outputGroupBox1.Name = "outputGroupBox1";
            this.outputGroupBox1.Size = new System.Drawing.Size(373, 382);
            this.outputGroupBox1.TabIndex = 3;
            this.outputGroupBox1.TabStop = false;
            this.outputGroupBox1.Text = "Output";
            // 
            // outputTextBox
            // 
            this.outputTextBox.Location = new System.Drawing.Point(7, 22);
            this.outputTextBox.Multiline = true;
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.Size = new System.Drawing.Size(360, 354);
            this.outputTextBox.TabIndex = 0;
            this.outputTextBox.Text = "Select Input file...";
            // 
            // runButton5
            // 
            this.runButton5.Location = new System.Drawing.Point(707, 409);
            this.runButton5.Name = "runButton5";
            this.runButton5.Size = new System.Drawing.Size(75, 26);
            this.runButton5.TabIndex = 4;
            this.runButton5.Text = "Run";
            this.runButton5.UseVisualStyleBackColor = true;
            this.runButton5.Click += new System.EventHandler(this.runButton5_Click);
            // 
            // openFileButton
            // 
            this.openFileButton.Location = new System.Drawing.Point(234, 58);
            this.openFileButton.Name = "openFileButton";
            this.openFileButton.Size = new System.Drawing.Size(75, 25);
            this.openFileButton.TabIndex = 9;
            this.openFileButton.Text = "File...";
            this.openFileButton.UseVisualStyleBackColor = true;
            this.openFileButton.Click += new System.EventHandler(this.openFileButton_Click);
            // 
            // validateFileButton5
            // 
            this.validateFileButton5.Location = new System.Drawing.Point(422, 412);
            this.validateFileButton5.Name = "validateFileButton5";
            this.validateFileButton5.Size = new System.Drawing.Size(75, 26);
            this.validateFileButton5.TabIndex = 5;
            this.validateFileButton5.Text = "Test file";
            this.validateFileButton5.UseVisualStyleBackColor = true;
            this.validateFileButton5.Visible = false;
            this.validateFileButton5.Click += new System.EventHandler(this.validateFileButton5_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.validateFileButton5);
            this.Controls.Add(this.runButton5);
            this.Controls.Add(this.outputGroupBox1);
            this.Controls.Add(this.inputGroupBox);
            this.Controls.Add(this.inputGroupBox1);
            this.Name = "Form1";
            this.Text = "De uitslagengenerator";
            this.inputGroupBox1.ResumeLayout(false);
            this.inputGroupBox1.PerformLayout();
            this.inputGroupBox.ResumeLayout(false);
            this.inputGroupBox.PerformLayout();
            this.outputGroupBox1.ResumeLayout(false);
            this.outputGroupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label inputFolderLabel1;
        private System.Windows.Forms.GroupBox inputGroupBox1;
        private System.Windows.Forms.TextBox inputFolderTextBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox filterMemberTextBox;
        private System.Windows.Forms.Label filterMemberLabel;
        private System.Windows.Forms.GroupBox inputGroupBox;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox columnsConfigFileTextBox;
        private System.Windows.Forms.Label configFileLabel1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TextBox outputFolderTextBox;
        private System.Windows.Forms.Label outputFolderLabel;
        private System.Windows.Forms.CheckBox outputHtmlCheckBox;
        private System.Windows.Forms.CheckBox filteredCsvOutputCheckBox;
        private System.Windows.Forms.CheckBox outputSqlCheckBox;
        private System.Windows.Forms.GroupBox outputGroupBox1;
        private System.Windows.Forms.TextBox outputTextBox;
        private System.Windows.Forms.Button runButton5;
        private System.Windows.Forms.CheckBox verboseCheckBox1;
        private System.Windows.Forms.Button openFileButton;
        private System.Windows.Forms.Button validateFileButton5;
    }
}

