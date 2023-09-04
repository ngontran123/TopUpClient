namespace NaptienMaster
{
    partial class Setting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Setting));
            this.kryptonPanel1 = new Krypton.Toolkit.KryptonPanel();
            this.kryptonButton2 = new Krypton.Toolkit.KryptonButton();
            this.kryptonButton1 = new Krypton.Toolkit.KryptonButton();
            this.baudrateList = new Krypton.Toolkit.KryptonComboBox();
            this.kryptonLabel2 = new Krypton.Toolkit.KryptonLabel();
            this.blackList = new Krypton.Toolkit.KryptonRichTextBox();
            this.kryptonLabel1 = new Krypton.Toolkit.KryptonLabel();
            this.kryptonBreadCrumb1 = new Krypton.Toolkit.KryptonBreadCrumb();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.baudrateList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonBreadCrumb1)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.kryptonPanel1.Controls.Add(this.kryptonButton2);
            this.kryptonPanel1.Controls.Add(this.kryptonButton1);
            this.kryptonPanel1.Controls.Add(this.baudrateList);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Controls.Add(this.blackList);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(498, 368);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // kryptonButton2
            // 
            this.kryptonButton2.CornerRoundingRadius = -1F;
            this.kryptonButton2.Location = new System.Drawing.Point(259, 302);
            this.kryptonButton2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.kryptonButton2.Name = "kryptonButton2";
            this.kryptonButton2.Size = new System.Drawing.Size(103, 38);
            this.kryptonButton2.TabIndex = 5;
            this.kryptonButton2.Values.Text = "Hủy";
            this.kryptonButton2.Click += new System.EventHandler(this.kryptonButton2_Click);
            // 
            // kryptonButton1
            // 
            this.kryptonButton1.CornerRoundingRadius = -1F;
            this.kryptonButton1.Location = new System.Drawing.Point(42, 302);
            this.kryptonButton1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.kryptonButton1.Name = "kryptonButton1";
            this.kryptonButton1.Size = new System.Drawing.Size(103, 38);
            this.kryptonButton1.TabIndex = 4;
            this.kryptonButton1.Values.Text = "Lưu";
            this.kryptonButton1.Click += new System.EventHandler(this.kryptonButton1_Click);
            // 
            // baudrateList
            // 
            this.baudrateList.CornerRoundingRadius = -1F;
            this.baudrateList.DropDownWidth = 121;
            this.baudrateList.IntegralHeight = false;
            this.baudrateList.Items.AddRange(new object[] {
            "4800",
            "9600",
            "19200",
            "38400",
            "57600",
            "115200",
            "230400"});
            this.baudrateList.Location = new System.Drawing.Point(94, 227);
            this.baudrateList.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.baudrateList.Name = "baudrateList";
            this.baudrateList.Size = new System.Drawing.Size(91, 21);
            this.baudrateList.StateCommon.ComboBox.Content.TextH = Krypton.Toolkit.PaletteRelativeAlign.Near;
            this.baudrateList.TabIndex = 3;
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(9, 228);
            this.kryptonLabel2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(62, 20);
            this.kryptonLabel2.TabIndex = 2;
            this.kryptonLabel2.Values.Text = "Baudrate:";
            // 
            // blackList
            // 
            this.blackList.Location = new System.Drawing.Point(94, 52);
            this.blackList.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.blackList.Name = "blackList";
            this.blackList.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Horizontal;
            this.blackList.Size = new System.Drawing.Size(222, 120);
            this.blackList.TabIndex = 1;
            this.blackList.Text = "";
            this.blackList.TextChanged += new System.EventHandler(this.kryptonRichTextBox1_TextChanged);
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(2, 67);
            this.kryptonLabel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(94, 20);
            this.kryptonLabel1.TabIndex = 0;
            this.kryptonLabel1.Values.Text = "Black List Ports:";
            // 
            // kryptonBreadCrumb1
            // 
            this.kryptonBreadCrumb1.AutoSize = false;
            this.kryptonBreadCrumb1.Location = new System.Drawing.Point(566, 167);
            this.kryptonBreadCrumb1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.kryptonBreadCrumb1.Name = "kryptonBreadCrumb1";
            // 
            // 
            // 
            this.kryptonBreadCrumb1.RootItem.ShortText = "Root";
            this.kryptonBreadCrumb1.SelectedItem = this.kryptonBreadCrumb1.RootItem;
            this.kryptonBreadCrumb1.Size = new System.Drawing.Size(6, 6);
            this.kryptonBreadCrumb1.TabIndex = 1;
            // 
            // Setting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 366);
            this.Controls.Add(this.kryptonBreadCrumb1);
            this.Controls.Add(this.kryptonPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Setting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Setting";
            this.Load += new System.EventHandler(this.Setting_Load);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.baudrateList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonBreadCrumb1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private Krypton.Toolkit.KryptonBreadCrumb kryptonBreadCrumb1;
        private Krypton.Toolkit.KryptonRichTextBox blackList;
        private Krypton.Toolkit.KryptonButton kryptonButton2;
        private Krypton.Toolkit.KryptonButton kryptonButton1;
        private Krypton.Toolkit.KryptonComboBox baudrateList;
        private Krypton.Toolkit.KryptonLabel kryptonLabel2;
    }
}