using System.Windows.Forms;

namespace LostKit
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            HiScoreOverall = new Label();
            pictureBox1 = new PictureBox();
            HiscoreSearchButton = new Button();
            HiscoreSearchBox = new TextBox();
            tabPage2 = new TabPage();
            toolTip1 = new ToolTip(components);
            pictureBox2 = new PictureBox();
            HiScoreCombat = new Label();
            ((System.ComponentModel.ISupportInitialize)webView21).BeginInit();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // webView21
            // 
            webView21.AllowExternalDrop = true;
            webView21.CreationProperties = null;
            webView21.DefaultBackgroundColor = Color.White;
            webView21.Location = new Point(137, 71);
            webView21.Name = "webView21";
            webView21.Size = new Size(75, 23);
            webView21.TabIndex = 0;
            webView21.ZoomFactor = 1D;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new Point(814, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(241, 577);
            tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(HiScoreCombat);
            tabPage1.Controls.Add(pictureBox2);
            tabPage1.Controls.Add(HiScoreOverall);
            tabPage1.Controls.Add(pictureBox1);
            tabPage1.Controls.Add(HiscoreSearchButton);
            tabPage1.Controls.Add(HiscoreSearchBox);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(233, 549);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Highscores";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // HiScoreOverall
            // 
            HiScoreOverall.AutoSize = true;
            HiScoreOverall.Location = new Point(62, 80);
            HiScoreOverall.Name = "HiScoreOverall";
            HiScoreOverall.Size = new Size(12, 15);
            HiScoreOverall.TabIndex = 3;
            HiScoreOverall.Text = "-";
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = (Image)resources.GetObject("pictureBox1.BackgroundImage");
            pictureBox1.BackgroundImageLayout = ImageLayout.None;
            pictureBox1.InitialImage = (Image)resources.GetObject("pictureBox1.InitialImage");
            pictureBox1.Location = new Point(21, 70);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(35, 36);
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            // 
            // HiscoreSearchButton
            // 
            HiscoreSearchButton.BackgroundImage = (Image)resources.GetObject("HiscoreSearchButton.BackgroundImage");
            HiscoreSearchButton.BackgroundImageLayout = ImageLayout.None;
            HiscoreSearchButton.Location = new Point(173, 17);
            HiscoreSearchButton.Name = "HiscoreSearchButton";
            HiscoreSearchButton.Size = new Size(36, 41);
            HiscoreSearchButton.TabIndex = 1;
            HiscoreSearchButton.Text = "\r\n";
            HiscoreSearchButton.UseVisualStyleBackColor = true;
            HiscoreSearchButton.MouseClick += HigscoreSearch_onclick;
            // 
            // HiscoreSearchBox
            // 
            HiscoreSearchBox.Location = new Point(21, 18);
            HiscoreSearchBox.Name = "HiscoreSearchBox";
            HiscoreSearchBox.Size = new Size(146, 23);
            HiscoreSearchBox.TabIndex = 0;
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(233, 549);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Notes";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // pictureBox2
            // 
            pictureBox2.BackgroundImage = (Image)resources.GetObject("pictureBox2.BackgroundImage");
            pictureBox2.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox2.InitialImage = (Image)resources.GetObject("pictureBox2.InitialImage");
            pictureBox2.Location = new Point(132, 70);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(35, 36);
            pictureBox2.TabIndex = 4;
            pictureBox2.TabStop = false;
            // 
            // HiScoreCombat
            // 
            HiScoreCombat.AutoSize = true;
            HiScoreCombat.Location = new Point(173, 80);
            HiScoreCombat.Name = "HiScoreCombat";
            HiScoreCombat.Size = new Size(12, 15);
            HiScoreCombat.TabIndex = 5;
            HiScoreCombat.Text = "-";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoValidate = AutoValidate.Disable;
            ClientSize = new Size(1067, 602);
            Controls.Add(tabControl1);
            Controls.Add(webView21);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "LostKit";
            ((System.ComponentModel.ISupportInitialize)webView21).EndInit();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TextBox HiscoreSearchBox;
        private Button HiscoreSearchButton;
        private PictureBox pictureBox1;
        private Label HiScoreOverall;
        private ToolTip toolTip1;
        private PictureBox pictureBox2;
        private Label HiScoreCombat;
    }
}
