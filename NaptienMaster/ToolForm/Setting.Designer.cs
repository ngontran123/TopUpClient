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
            this.languageComboBox = new Krypton.Toolkit.KryptonComboBox();
            this.kryptonLabel3 = new Krypton.Toolkit.KryptonLabel();
            this.kryptonButton2 = new Krypton.Toolkit.KryptonButton();
            this.kryptonButton1 = new Krypton.Toolkit.KryptonButton();
            this.baudrateList = new Krypton.Toolkit.KryptonComboBox();
            this.kryptonLabel2 = new Krypton.Toolkit.KryptonLabel();
            this.blackList = new Krypton.Toolkit.KryptonRichTextBox();
            this.kryptonLabel1 = new Krypton.Toolkit.KryptonLabel();
            this.kryptonBreadCrumb1 = new Krypton.Toolkit.KryptonBreadCrumb();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.languageComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.baudrateList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonBreadCrumb1)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            resources.ApplyResources(this.kryptonPanel1, "kryptonPanel1");
            this.kryptonPanel1.Controls.Add(this.languageComboBox);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel3);
            this.kryptonPanel1.Controls.Add(this.kryptonButton2);
            this.kryptonPanel1.Controls.Add(this.kryptonButton1);
            this.kryptonPanel1.Controls.Add(this.baudrateList);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Controls.Add(this.blackList);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.StateCommon.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(247)))), ((int)(((byte)(201)))));
            // 
            // languageComboBox
            // 
            resources.ApplyResources(this.languageComboBox, "languageComboBox");
            this.languageComboBox.CornerRoundingRadius = -1F;
            this.languageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.languageComboBox.DropDownWidth = 121;
            this.languageComboBox.IntegralHeight = false;
            this.languageComboBox.Items.AddRange(new object[] {
            resources.GetString("languageComboBox.Items"),
            resources.GetString("languageComboBox.Items1"),
            resources.GetString("languageComboBox.Items2")});
            this.languageComboBox.Name = "languageComboBox";
            this.languageComboBox.StateCommon.ComboBox.Content.TextH = Krypton.Toolkit.PaletteRelativeAlign.Near;
            // 
            // kryptonLabel3
            // 
            resources.ApplyResources(this.kryptonLabel3, "kryptonLabel3");
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Values.ExtraText = resources.GetString("kryptonLabel3.Values.ExtraText");
            this.kryptonLabel3.Values.ImageTransparentColor = ((System.Drawing.Color)(resources.GetObject("kryptonLabel3.Values.ImageTransparentColor")));
            this.kryptonLabel3.Values.Text = resources.GetString("kryptonLabel3.Values.Text");
            // 
            // kryptonButton2
            // 
            resources.ApplyResources(this.kryptonButton2, "kryptonButton2");
            this.kryptonButton2.CornerRoundingRadius = -1F;
            this.kryptonButton2.Name = "kryptonButton2";
            this.kryptonButton2.Values.ExtraText = resources.GetString("kryptonButton2.Values.ExtraText");
            this.kryptonButton2.Values.ImageTransparentColor = ((System.Drawing.Color)(resources.GetObject("kryptonButton2.Values.ImageTransparentColor")));
            this.kryptonButton2.Values.Text = resources.GetString("kryptonButton2.Values.Text");
            this.kryptonButton2.Click += new System.EventHandler(this.kryptonButton2_Click);
            // 
            // kryptonButton1
            // 
            resources.ApplyResources(this.kryptonButton1, "kryptonButton1");
            this.kryptonButton1.CornerRoundingRadius = -1F;
            this.kryptonButton1.Name = "kryptonButton1";
            this.kryptonButton1.Values.ExtraText = resources.GetString("kryptonButton1.Values.ExtraText");
            this.kryptonButton1.Values.ImageTransparentColor = ((System.Drawing.Color)(resources.GetObject("kryptonButton1.Values.ImageTransparentColor")));
            this.kryptonButton1.Values.Text = resources.GetString("kryptonButton1.Values.Text");
            this.kryptonButton1.Click += new System.EventHandler(this.kryptonButton1_Click);
            // 
            // baudrateList
            // 
            resources.ApplyResources(this.baudrateList, "baudrateList");
            this.baudrateList.CornerRoundingRadius = -1F;
            this.baudrateList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.baudrateList.DropDownWidth = 121;
            this.baudrateList.IntegralHeight = false;
            this.baudrateList.Items.AddRange(new object[] {
            resources.GetString("baudrateList.Items"),
            resources.GetString("baudrateList.Items1"),
            resources.GetString("baudrateList.Items2"),
            resources.GetString("baudrateList.Items3"),
            resources.GetString("baudrateList.Items4"),
            resources.GetString("baudrateList.Items5"),
            resources.GetString("baudrateList.Items6")});
            this.baudrateList.Name = "baudrateList";
            this.baudrateList.StateCommon.ComboBox.Content.TextH = Krypton.Toolkit.PaletteRelativeAlign.Near;
            // 
            // kryptonLabel2
            // 
            resources.ApplyResources(this.kryptonLabel2, "kryptonLabel2");
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Values.ExtraText = resources.GetString("kryptonLabel2.Values.ExtraText");
            this.kryptonLabel2.Values.ImageTransparentColor = ((System.Drawing.Color)(resources.GetObject("kryptonLabel2.Values.ImageTransparentColor")));
            this.kryptonLabel2.Values.Text = resources.GetString("kryptonLabel2.Values.Text");
            // 
            // blackList
            // 
            resources.ApplyResources(this.blackList, "blackList");
            this.blackList.Name = "blackList";
            this.blackList.TextChanged += new System.EventHandler(this.kryptonRichTextBox1_TextChanged);
            // 
            // kryptonLabel1
            // 
            resources.ApplyResources(this.kryptonLabel1, "kryptonLabel1");
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Values.ExtraText = resources.GetString("kryptonLabel1.Values.ExtraText");
            this.kryptonLabel1.Values.ImageTransparentColor = ((System.Drawing.Color)(resources.GetObject("kryptonLabel1.Values.ImageTransparentColor")));
            this.kryptonLabel1.Values.Text = resources.GetString("kryptonLabel1.Values.Text");
            // 
            // kryptonBreadCrumb1
            // 
            resources.ApplyResources(this.kryptonBreadCrumb1, "kryptonBreadCrumb1");
            this.kryptonBreadCrumb1.Name = "kryptonBreadCrumb1";
            // 
            // 
            // 
            this.kryptonBreadCrumb1.RootItem.ShortText = resources.GetString("kryptonBreadCrumb1.RootItem.ShortText");
            this.kryptonBreadCrumb1.SelectedItem = this.kryptonBreadCrumb1.RootItem;
            // 
            // Setting
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.kryptonBreadCrumb1);
            this.Controls.Add(this.kryptonPanel1);
            this.Name = "Setting";
            this.Load += new System.EventHandler(this.Setting_Load);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.languageComboBox)).EndInit();
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
        private Krypton.Toolkit.KryptonComboBox languageComboBox;
        private Krypton.Toolkit.KryptonLabel kryptonLabel3;
    }
}