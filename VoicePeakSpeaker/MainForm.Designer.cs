namespace VoicePeakSpeaker
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            notifyIcon1 = new NotifyIcon(components);
            contextMenuStrip1 = new ContextMenuStrip(components);
            showMainFormToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            CloseButton = new Button();
            ExitButton = new Button();
            NarratorComboBox = new ComboBox();
            VoicePeakTextBox = new TextBox();
            VoicePeakReferenceButton = new Button();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // notifyIcon1
            // 
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            notifyIcon1.Icon = (Icon)resources.GetObject("notifyIcon1.Icon");
            notifyIcon1.Text = "notifyIcon1";
            notifyIcon1.Visible = true;
            notifyIcon1.MouseDoubleClick += notifyIcon1_MouseDoubleClick;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { showMainFormToolStripMenuItem, exitToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(158, 48);
            // 
            // showMainFormToolStripMenuItem
            // 
            showMainFormToolStripMenuItem.Name = "showMainFormToolStripMenuItem";
            showMainFormToolStripMenuItem.Size = new Size(157, 22);
            showMainFormToolStripMenuItem.Text = "&ShowMainForm";
            showMainFormToolStripMenuItem.Click += showMainFormToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(157, 22);
            exitToolStripMenuItem.Text = "E&xit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // CloseButton
            // 
            CloseButton.Location = new Point(45, 123);
            CloseButton.Name = "CloseButton";
            CloseButton.Size = new Size(91, 25);
            CloseButton.TabIndex = 3;
            CloseButton.Text = "Close";
            CloseButton.UseVisualStyleBackColor = true;
            CloseButton.Click += CloseButton_Click;
            // 
            // ExitButton
            // 
            ExitButton.Location = new Point(158, 123);
            ExitButton.Name = "ExitButton";
            ExitButton.Size = new Size(91, 25);
            ExitButton.TabIndex = 4;
            ExitButton.Text = "Exit";
            ExitButton.UseVisualStyleBackColor = true;
            ExitButton.Click += ExitButton_Click;
            // 
            // NarratorComboBox
            // 
            NarratorComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            NarratorComboBox.FormattingEnabled = true;
            NarratorComboBox.Location = new Point(45, 55);
            NarratorComboBox.Name = "NarratorComboBox";
            NarratorComboBox.Size = new Size(201, 23);
            NarratorComboBox.TabIndex = 2;
            NarratorComboBox.SelectedIndexChanged += NarratorComboBox_SelectedIndexChanged;
            // 
            // VoicePeakTextBox
            // 
            VoicePeakTextBox.Location = new Point(45, 12);
            VoicePeakTextBox.Name = "VoicePeakTextBox";
            VoicePeakTextBox.Size = new Size(201, 23);
            VoicePeakTextBox.TabIndex = 0;
            // 
            // VoicePeakReferenceButton
            // 
            VoicePeakReferenceButton.Location = new Point(269, 12);
            VoicePeakReferenceButton.Name = "VoicePeakReferenceButton";
            VoicePeakReferenceButton.Size = new Size(75, 23);
            VoicePeakReferenceButton.TabIndex = 1;
            VoicePeakReferenceButton.Text = "参照";
            VoicePeakReferenceButton.UseVisualStyleBackColor = true;
            VoicePeakReferenceButton.Click += VoicePeakReferenceButton_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(373, 183);
            Controls.Add(VoicePeakReferenceButton);
            Controls.Add(VoicePeakTextBox);
            Controls.Add(NarratorComboBox);
            Controls.Add(ExitButton);
            Controls.Add(CloseButton);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "MainForm";
            Text = "VoicePeakSpeaker";
            FormClosing += MainForm_FormClosing;
            FormClosed += MainForm_FormClosed;
            Load += MainForm_Load;
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private NotifyIcon notifyIcon1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem showMainFormToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private Button CloseButton;
        private Button ExitButton;
        private ComboBox NarratorComboBox;
        private TextBox VoicePeakTextBox;
        private Button VoicePeakReferenceButton;
    }
}