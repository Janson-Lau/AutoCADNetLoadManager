using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace AutoCADNetLoadManager
{
    partial class AssemblySelectorForm
    {
        private string m_assemName;

        private bool m_found;

        public string m_resultPath;

        private Button okButton;

        private Button cancelButton;

        private TextBox assemPathTextBox;

        private Button browseButton;

        private Label missingAssemDescripLabel;

        private TextBox assemNameTextBox;

        private Label selectAssemLabel;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public AssemblySelectorForm(string assemName)
        {
            InitializeComponent();
            m_assemName = assemName;
            assemNameTextBox.Text = assemName;
        }
        private void AssemblySelectorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!m_found)
            {
                ShowWarning();
            }
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Assembly files (*.dll;*.exe,*.mcl)|*.dll;*.exe;*.mcl|All files|*.*||";
                string str = m_assemName.Substring(0, m_assemName.IndexOf(','));
                openFileDialog.FileName = str + ".*";
                if (openFileDialog.ShowDialog() != DialogResult.OK)
                {
                    ShowWarning();
                }
                assemPathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (File.Exists(assemPathTextBox.Text))
            {
                m_resultPath = assemPathTextBox.Text;
                m_found = true;
            }
            else
            {
                ShowWarning();
            }
            Close();
        }

        private void ShowWarning()
        {
            string text = new StringBuilder("The dependent assembly can't be loaded: \"").Append(m_assemName).AppendFormat("\".").ToString();
            MessageBox.Show(text, "Add-in Manager Internal", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

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
            okButton = new System.Windows.Forms.Button();
            cancelButton = new System.Windows.Forms.Button();
            assemPathTextBox = new System.Windows.Forms.TextBox();
            browseButton = new System.Windows.Forms.Button();
            missingAssemDescripLabel = new System.Windows.Forms.Label();
            assemNameTextBox = new System.Windows.Forms.TextBox();
            selectAssemLabel = new System.Windows.Forms.Label();
            SuspendLayout();
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Location = new System.Drawing.Point(213, 100);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(63, 23);
            okButton.TabIndex = 0;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += new System.EventHandler(okButton_Click);
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Location = new System.Drawing.Point(282, 100);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(62, 23);
            cancelButton.TabIndex = 1;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            assemPathTextBox.Location = new System.Drawing.Point(9, 74);
            assemPathTextBox.Name = "assemPathTextBox";
            assemPathTextBox.Size = new System.Drawing.Size(290, 20);
            assemPathTextBox.TabIndex = 2;
            browseButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 134);
            browseButton.Location = new System.Drawing.Point(305, 74);
            browseButton.Name = "browseButton";
            browseButton.Size = new System.Drawing.Size(39, 20);
            browseButton.TabIndex = 3;
            browseButton.Text = "&...";
            browseButton.UseVisualStyleBackColor = true;
            browseButton.Click += new System.EventHandler(browseButton_Click);
            missingAssemDescripLabel.AutoSize = true;
            missingAssemDescripLabel.Location = new System.Drawing.Point(6, 6);
            missingAssemDescripLabel.Name = "missingAssemDescripLabel";
            missingAssemDescripLabel.Size = new System.Drawing.Size(309, 13);
            missingAssemDescripLabel.TabIndex = 4;
            missingAssemDescripLabel.Text = "The following assembly name can not be resolved automatically:";
            assemNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            assemNameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 134);
            assemNameTextBox.ForeColor = System.Drawing.SystemColors.WindowText;
            assemNameTextBox.Location = new System.Drawing.Point(9, 23);
            assemNameTextBox.Multiline = true;
            assemNameTextBox.Name = "assemNameTextBox";
            assemNameTextBox.ReadOnly = true;
            assemNameTextBox.Size = new System.Drawing.Size(294, 28);
            assemNameTextBox.TabIndex = 5;
            assemNameTextBox.Text = "I'm a text box!\r\nI'm a text box!";
            selectAssemLabel.AutoSize = true;
            selectAssemLabel.Location = new System.Drawing.Point(6, 56);
            selectAssemLabel.Name = "selectAssemLabel";
            selectAssemLabel.Size = new System.Drawing.Size(197, 13);
            selectAssemLabel.TabIndex = 6;
            selectAssemLabel.Text = "Please select the assembly file manually:";
            base.AutoScaleDimensions = new System.Drawing.SizeF(96f, 96f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            base.CancelButton = cancelButton;
            base.ClientSize = new System.Drawing.Size(354, 129);
            base.Controls.Add(selectAssemLabel);
            base.Controls.Add(assemNameTextBox);
            base.Controls.Add(missingAssemDescripLabel);
            base.Controls.Add(browseButton);
            base.Controls.Add(assemPathTextBox);
            base.Controls.Add(cancelButton);
            base.Controls.Add(okButton);
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "AssemblySelectorForm";
            base.ShowInTaskbar = false;
            base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Assembly File Selector";
            base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(AssemblySelectorForm_FormClosing);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}