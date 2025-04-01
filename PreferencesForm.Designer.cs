namespace LostKit
{
    partial class PreferencesForm
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
            DetailModeLabel = new Label();
            comboBox1 = new ComboBox();
            WorldLabel = new Label();
            FavWorldTextBox = new TextBox();
            SaveButton = new Button();
            label1 = new Label();
            showChatCheckBox = new CheckBox();
            SuspendLayout();
            // 
            // DetailModeLabel
            // 
            DetailModeLabel.AutoSize = true;
            DetailModeLabel.BackColor = SystemColors.ActiveCaptionText;
            DetailModeLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            DetailModeLabel.ForeColor = SystemColors.AppWorkspace;
            DetailModeLabel.Location = new Point(12, 9);
            DetailModeLabel.Name = "DetailModeLabel";
            DetailModeLabel.Size = new Size(126, 15);
            DetailModeLabel.TabIndex = 0;
            DetailModeLabel.Text = "Prefered detail mode";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(135, 6);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(121, 23);
            comboBox1.TabIndex = 1;
            // 
            // WorldLabel
            // 
            WorldLabel.AutoSize = true;
            WorldLabel.BackColor = SystemColors.ActiveCaptionText;
            WorldLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            WorldLabel.ForeColor = SystemColors.AppWorkspace;
            WorldLabel.Location = new Point(12, 34);
            WorldLabel.Name = "WorldLabel";
            WorldLabel.Size = new Size(90, 15);
            WorldLabel.TabIndex = 2;
            WorldLabel.Text = "Favorite World";
            // 
            // FavWorldTextBox
            // 
            FavWorldTextBox.Location = new Point(102, 31);
            FavWorldTextBox.Name = "FavWorldTextBox";
            FavWorldTextBox.Size = new Size(37, 23);
            FavWorldTextBox.TabIndex = 3;
            // 
            // SaveButton
            // 
            SaveButton.Location = new Point(12, 415);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(75, 23);
            SaveButton.TabIndex = 4;
            SaveButton.Text = "Save";
            SaveButton.UseVisualStyleBackColor = true;
            SaveButton.MouseDown += SaveButton_MouseDown;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = SystemColors.ActiveCaptionText;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label1.ForeColor = SystemColors.AppWorkspace;
            label1.Location = new Point(12, 61);
            label1.Name = "label1";
            label1.Size = new Size(65, 15);
            label1.TabIndex = 5;
            label1.Text = "Show chat";
            // 
            // showChatCheckBox
            // 
            showChatCheckBox.AutoSize = true;
            showChatCheckBox.Location = new Point(83, 61);
            showChatCheckBox.Name = "showChatCheckBox";
            showChatCheckBox.Size = new Size(82, 19);
            showChatCheckBox.TabIndex = 6;
            showChatCheckBox.Text = "checkBox1";
            showChatCheckBox.UseVisualStyleBackColor = true;
            // 
            // PreferencesForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaptionText;
            ClientSize = new Size(800, 450);
            Controls.Add(showChatCheckBox);
            Controls.Add(label1);
            Controls.Add(SaveButton);
            Controls.Add(FavWorldTextBox);
            Controls.Add(WorldLabel);
            Controls.Add(comboBox1);
            Controls.Add(DetailModeLabel);
            Name = "PreferencesForm";
            Text = "PreferencesForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label DetailModeLabel;
        private ComboBox comboBox1;
        private Label WorldLabel;
        private TextBox FavWorldTextBox;
        private Button SaveButton;
        private Label label1;
        private CheckBox showChatCheckBox;
    }
}