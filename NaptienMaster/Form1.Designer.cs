namespace NaptienMaster
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.kryptonPanel1 = new Krypton.Toolkit.KryptonPanel();
            this.dataGSM = new Krypton.Toolkit.KryptonDataGridView();
            this.stt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.port = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.port_custom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.telco = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sdt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.simtype = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tkc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tkkm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.charge = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.charged = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.expire = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.message = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.guna2ContextMenuStrip1 = new Guna.UI2.WinForms.Guna2ContextMenuStrip();
            this.tảiLạiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copySĐTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyDanhSáchCổngToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToExcelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.excelMẫuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkPortsBtn = new Krypton.Toolkit.KryptonButton();
            this.loadPortsBtn = new Krypton.Toolkit.KryptonButton();
            this.num_of_port = new Krypton.Toolkit.KryptonLabel();
            this.guna2TabControl1 = new Guna.UI2.WinForms.Guna2TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.connection_state = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.pc_id_label = new System.Windows.Forms.Label();
            this.version = new System.Windows.Forms.Label();
            this.guna2Button1 = new Guna.UI2.WinForms.Guna2Button();
            this.settingBtn = new Guna.UI2.WinForms.Guna2Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.refreshBtn = new Guna.UI2.WinForms.Guna2Button();
            this.task_id_filter = new Guna.UI2.WinForms.Guna2Button();
            this.status_filter = new Guna.UI2.WinForms.Guna2Button();
            this.task_id_textbox = new Guna.UI2.WinForms.Guna2TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.statusFilter = new Guna.UI2.WinForms.Guna2ComboBox();
            this.kryptonPanel2 = new Krypton.Toolkit.KryptonPanel();
            this.transactionGridView = new Krypton.Toolkit.KryptonDataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.logTxtBox = new Guna.UI2.WinForms.Guna2TextBox();
            this.stt_trans = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.phone_trans = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.network_transac = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.simtype_transac = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.task_id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.card_code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.charge_before = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.charge_after = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.charged_time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.status_charge = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.money_topup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.real_topup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.error_msg = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGSM)).BeginInit();
            this.guna2ContextMenuStrip1.SuspendLayout();
            this.guna2TabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.connection_state)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel2)).BeginInit();
            this.kryptonPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.transactionGridView)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.kryptonPanel1.Controls.Add(this.dataGSM);
            this.kryptonPanel1.Controls.Add(this.checkPortsBtn);
            this.kryptonPanel1.Controls.Add(this.loadPortsBtn);
            this.kryptonPanel1.Location = new System.Drawing.Point(7, 90);
            this.kryptonPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(1521, 654);
            this.kryptonPanel1.TabIndex = 2;
            this.kryptonPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.kryptonPanel1_Paint);
            // 
            // dataGSM
            // 
            this.dataGSM.AllowUserToAddRows = false;
            this.dataGSM.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGSM.ColumnHeadersHeight = 36;
            this.dataGSM.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.stt,
            this.port,
            this.port_custom,
            this.telco,
            this.sdt,
            this.simtype,
            this.status,
            this.tkc,
            this.tkkm,
            this.charge,
            this.charged,
            this.expire,
            this.message});
            this.dataGSM.ContextMenuStrip = this.guna2ContextMenuStrip1;
            this.dataGSM.Location = new System.Drawing.Point(-9, 123);
            this.dataGSM.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dataGSM.Name = "dataGSM";
            this.dataGSM.RowHeadersWidth = 51;
            this.dataGSM.RowTemplate.Height = 24;
            this.dataGSM.Size = new System.Drawing.Size(1516, 543);
            this.dataGSM.TabIndex = 0;
            this.dataGSM.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dataGSM_RowPostPaint);
            this.dataGSM.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dataGSM_SortCompare);
            // 
            // stt
            // 
            this.stt.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.stt.HeaderText = "STT";
            this.stt.MinimumWidth = 6;
            this.stt.Name = "stt";
            this.stt.ReadOnly = true;
            this.stt.Width = 66;
            // 
            // port
            // 
            this.port.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.port.HeaderText = "Port[Windows]";
            this.port.MinimumWidth = 6;
            this.port.Name = "port";
            this.port.ReadOnly = true;
            this.port.Width = 139;
            // 
            // port_custom
            // 
            this.port_custom.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.port_custom.HeaderText = "Port[Tự đặt]";
            this.port_custom.MinimumWidth = 6;
            this.port_custom.Name = "port_custom";
            this.port_custom.Width = 121;
            // 
            // telco
            // 
            this.telco.HeaderText = "Nhà mạng";
            this.telco.MinimumWidth = 6;
            this.telco.Name = "telco";
            this.telco.ReadOnly = true;
            this.telco.Width = 200;
            // 
            // sdt
            // 
            this.sdt.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.sdt.HeaderText = "Số điện thoại";
            this.sdt.MinimumWidth = 6;
            this.sdt.Name = "sdt";
            this.sdt.ReadOnly = true;
            this.sdt.Width = 130;
            // 
            // simtype
            // 
            this.simtype.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.simtype.HeaderText = "Loại Sim";
            this.simtype.MinimumWidth = 6;
            this.simtype.Name = "simtype";
            this.simtype.Width = 99;
            // 
            // status
            // 
            this.status.HeaderText = "Trạng thái";
            this.status.MinimumWidth = 6;
            this.status.Name = "status";
            this.status.ReadOnly = true;
            this.status.Width = 130;
            // 
            // tkc
            // 
            this.tkc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.tkc.HeaderText = "Số dư";
            this.tkc.MinimumWidth = 6;
            this.tkc.Name = "tkc";
            this.tkc.ReadOnly = true;
            this.tkc.Width = 81;
            // 
            // tkkm
            // 
            this.tkkm.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.tkkm.HeaderText = "TKKM";
            this.tkkm.MinimumWidth = 6;
            this.tkkm.Name = "tkkm";
            this.tkkm.ReadOnly = true;
            this.tkkm.Width = 81;
            // 
            // charge
            // 
            this.charge.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.charge.HeaderText = "Cần nạp";
            this.charge.MinimumWidth = 6;
            this.charge.Name = "charge";
            this.charge.ReadOnly = true;
            this.charge.Width = 96;
            // 
            // charged
            // 
            this.charged.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.charged.HeaderText = "Đã nạp";
            this.charged.MinimumWidth = 6;
            this.charged.Name = "charged";
            this.charged.ReadOnly = true;
            this.charged.Width = 90;
            // 
            // expire
            // 
            this.expire.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.expire.HeaderText = "HSD";
            this.expire.MinimumWidth = 6;
            this.expire.Name = "expire";
            this.expire.ReadOnly = true;
            this.expire.Width = 72;
            // 
            // message
            // 
            this.message.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.message.HeaderText = "Thông báo";
            this.message.MinimumWidth = 6;
            this.message.Name = "message";
            this.message.ReadOnly = true;
            this.message.Width = 114;
            // 
            // guna2ContextMenuStrip1
            // 
            this.guna2ContextMenuStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.guna2ContextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.guna2ContextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tảiLạiToolStripMenuItem,
            this.copySĐTToolStripMenuItem,
            this.copyDanhSáchCổngToolStripMenuItem,
            this.exportToExcelToolStripMenuItem,
            this.excelMẫuToolStripMenuItem});
            this.guna2ContextMenuStrip1.Name = "guna2ContextMenuStrip1";
            this.guna2ContextMenuStrip1.RenderStyle.ArrowColor = System.Drawing.Color.FromArgb(((int)(((byte)(151)))), ((int)(((byte)(143)))), ((int)(((byte)(255)))));
            this.guna2ContextMenuStrip1.RenderStyle.BorderColor = System.Drawing.Color.Gainsboro;
            this.guna2ContextMenuStrip1.RenderStyle.ColorTable = null;
            this.guna2ContextMenuStrip1.RenderStyle.RoundedEdges = true;
            this.guna2ContextMenuStrip1.RenderStyle.SelectionArrowColor = System.Drawing.Color.White;
            this.guna2ContextMenuStrip1.RenderStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            this.guna2ContextMenuStrip1.RenderStyle.SelectionForeColor = System.Drawing.Color.White;
            this.guna2ContextMenuStrip1.RenderStyle.SeparatorColor = System.Drawing.Color.Gainsboro;
            this.guna2ContextMenuStrip1.RenderStyle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.guna2ContextMenuStrip1.Size = new System.Drawing.Size(220, 124);
            this.guna2ContextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.guna2ContextMenuStrip1_Opening);
            // 
            // tảiLạiToolStripMenuItem
            // 
            this.tảiLạiToolStripMenuItem.Name = "tảiLạiToolStripMenuItem";
            this.tảiLạiToolStripMenuItem.Size = new System.Drawing.Size(219, 24);
            this.tảiLạiToolStripMenuItem.Text = "Tải lại";
            this.tảiLạiToolStripMenuItem.Click += new System.EventHandler(this.tảiLạiToolStripMenuItem_Click);
            // 
            // copySĐTToolStripMenuItem
            // 
            this.copySĐTToolStripMenuItem.Name = "copySĐTToolStripMenuItem";
            this.copySĐTToolStripMenuItem.Size = new System.Drawing.Size(219, 24);
            this.copySĐTToolStripMenuItem.Text = "Copy SĐT";
            this.copySĐTToolStripMenuItem.Click += new System.EventHandler(this.copySĐTToolStripMenuItem_Click);
            // 
            // copyDanhSáchCổngToolStripMenuItem
            // 
            this.copyDanhSáchCổngToolStripMenuItem.Name = "copyDanhSáchCổngToolStripMenuItem";
            this.copyDanhSáchCổngToolStripMenuItem.Size = new System.Drawing.Size(219, 24);
            this.copyDanhSáchCổngToolStripMenuItem.Text = "Copy danh sách cổng";
            this.copyDanhSáchCổngToolStripMenuItem.Click += new System.EventHandler(this.copyDanhSáchCổngToolStripMenuItem_Click);
            // 
            // exportToExcelToolStripMenuItem
            // 
            this.exportToExcelToolStripMenuItem.Name = "exportToExcelToolStripMenuItem";
            this.exportToExcelToolStripMenuItem.Size = new System.Drawing.Size(219, 24);
            this.exportToExcelToolStripMenuItem.Text = "Xuất Excel";
            this.exportToExcelToolStripMenuItem.Click += new System.EventHandler(this.exportToExcelToolStripMenuItem_Click);
            // 
            // excelMẫuToolStripMenuItem
            // 
            this.excelMẫuToolStripMenuItem.Name = "excelMẫuToolStripMenuItem";
            this.excelMẫuToolStripMenuItem.Size = new System.Drawing.Size(219, 24);
            this.excelMẫuToolStripMenuItem.Text = "Xuất Excel mẫu";
            this.excelMẫuToolStripMenuItem.Click += new System.EventHandler(this.excelMẫuToolStripMenuItem_Click);
            // 
            // checkPortsBtn
            // 
            this.checkPortsBtn.CornerRoundingRadius = -1F;
            this.checkPortsBtn.Location = new System.Drawing.Point(513, 14);
            this.checkPortsBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkPortsBtn.Name = "checkPortsBtn";
            this.checkPortsBtn.Size = new System.Drawing.Size(288, 66);
            this.checkPortsBtn.TabIndex = 5;
            this.checkPortsBtn.Values.Text = "Kiểm tra toàn bộ cổng";
            this.checkPortsBtn.Click += new System.EventHandler(this.checkPortsBtn_Click);
            // 
            // loadPortsBtn
            // 
            this.loadPortsBtn.CornerRoundingRadius = -1F;
            this.loadPortsBtn.Location = new System.Drawing.Point(52, 14);
            this.loadPortsBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.loadPortsBtn.Name = "loadPortsBtn";
            this.loadPortsBtn.Size = new System.Drawing.Size(288, 66);
            this.loadPortsBtn.TabIndex = 4;
            this.loadPortsBtn.Values.Text = "Tải danh sách cổng";
            this.loadPortsBtn.Click += new System.EventHandler(this.loadPortsBtn_Click);
            // 
            // num_of_port
            // 
            this.num_of_port.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.num_of_port.Location = new System.Drawing.Point(4, 763);
            this.num_of_port.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.num_of_port.Name = "num_of_port";
            this.num_of_port.Size = new System.Drawing.Size(1728, 24);
            this.num_of_port.TabIndex = 4;
            this.num_of_port.Values.Text = "Số cổng:";
            this.num_of_port.Click += new System.EventHandler(this.num_of_port_Click);
            // 
            // guna2TabControl1
            // 
            this.guna2TabControl1.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.guna2TabControl1.Controls.Add(this.tabPage1);
            this.guna2TabControl1.Controls.Add(this.tabPage2);
            this.guna2TabControl1.Controls.Add(this.tabPage3);
            this.guna2TabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guna2TabControl1.ItemSize = new System.Drawing.Size(180, 40);
            this.guna2TabControl1.Location = new System.Drawing.Point(0, 0);
            this.guna2TabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.guna2TabControl1.Name = "guna2TabControl1";
            this.guna2TabControl1.SelectedIndex = 0;
            this.guna2TabControl1.Size = new System.Drawing.Size(1924, 799);
            this.guna2TabControl1.TabButtonHoverState.BorderColor = System.Drawing.Color.Empty;
            this.guna2TabControl1.TabButtonHoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(52)))), ((int)(((byte)(70)))));
            this.guna2TabControl1.TabButtonHoverState.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.guna2TabControl1.TabButtonHoverState.ForeColor = System.Drawing.Color.White;
            this.guna2TabControl1.TabButtonHoverState.InnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(52)))), ((int)(((byte)(70)))));
            this.guna2TabControl1.TabButtonIdleState.BorderColor = System.Drawing.Color.Empty;
            this.guna2TabControl1.TabButtonIdleState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.guna2TabControl1.TabButtonIdleState.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.guna2TabControl1.TabButtonIdleState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(160)))), ((int)(((byte)(167)))));
            this.guna2TabControl1.TabButtonIdleState.InnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.guna2TabControl1.TabButtonSelectedState.BorderColor = System.Drawing.Color.Empty;
            this.guna2TabControl1.TabButtonSelectedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(37)))), ((int)(((byte)(49)))));
            this.guna2TabControl1.TabButtonSelectedState.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.guna2TabControl1.TabButtonSelectedState.ForeColor = System.Drawing.Color.White;
            this.guna2TabControl1.TabButtonSelectedState.InnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(132)))), ((int)(((byte)(255)))));
            this.guna2TabControl1.TabButtonSize = new System.Drawing.Size(180, 40);
            this.guna2TabControl1.TabIndex = 5;
            this.guna2TabControl1.TabMenuBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.guna2TabControl1.SelectedIndexChanged += new System.EventHandler(this.guna2TabControl1_SelectedIndexChanged);
            this.guna2TabControl1.TabIndexChanged += new System.EventHandler(this.guna2TabControl1_TabIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.connection_state);
            this.tabPage1.Controls.Add(this.pc_id_label);
            this.tabPage1.Controls.Add(this.version);
            this.tabPage1.Controls.Add(this.guna2Button1);
            this.tabPage1.Controls.Add(this.settingBtn);
            this.tabPage1.Controls.Add(this.num_of_port);
            this.tabPage1.Controls.Add(this.kryptonPanel1);
            this.tabPage1.Location = new System.Drawing.Point(184, 4);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage1.Size = new System.Drawing.Size(1736, 791);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Thông tin SIM";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.tabPage1.Click += new System.EventHandler(this.tabPage1_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(1505, 766);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(151, 20);
            this.label2.TabIndex = 12;
            this.label2.Text = "Connection Status:";
            // 
            // connection_state
            // 
            this.connection_state.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.connection_state.ImageRotate = 0F;
            this.connection_state.Location = new System.Drawing.Point(1664, 761);
            this.connection_state.Margin = new System.Windows.Forms.Padding(4);
            this.connection_state.Name = "connection_state";
            this.connection_state.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.connection_state.Size = new System.Drawing.Size(31, 25);
            this.connection_state.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.connection_state.TabIndex = 11;
            this.connection_state.TabStop = false;
            // 
            // pc_id_label
            // 
            this.pc_id_label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pc_id_label.AutoSize = true;
            this.pc_id_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pc_id_label.Location = new System.Drawing.Point(964, 766);
            this.pc_id_label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.pc_id_label.Name = "pc_id_label";
            this.pc_id_label.Size = new System.Drawing.Size(53, 20);
            this.pc_id_label.TabIndex = 9;
            this.pc_id_label.Text = "label2";
            // 
            // version
            // 
            this.version.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.version.AutoSize = true;
            this.version.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.version.Location = new System.Drawing.Point(469, 764);
            this.version.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.version.Name = "version";
            this.version.Size = new System.Drawing.Size(53, 20);
            this.version.TabIndex = 8;
            this.version.Text = "label2";
            // 
            // guna2Button1
            // 
            this.guna2Button1.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button1.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button1.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2Button1.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2Button1.FillColor = System.Drawing.Color.LightSteelBlue;
            this.guna2Button1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.guna2Button1.ForeColor = System.Drawing.Color.White;
            this.guna2Button1.Image = global::NaptienMaster.Properties.Resources.file_system__1_;
            this.guna2Button1.ImageSize = new System.Drawing.Size(25, 25);
            this.guna2Button1.Location = new System.Drawing.Point(852, 9);
            this.guna2Button1.Margin = new System.Windows.Forms.Padding(4);
            this.guna2Button1.Name = "guna2Button1";
            this.guna2Button1.Size = new System.Drawing.Size(193, 65);
            this.guna2Button1.TabIndex = 7;
            this.guna2Button1.Text = "Lưu cấu hình cổng";
            this.guna2Button1.Click += new System.EventHandler(this.guna2Button1_Click);
            // 
            // settingBtn
            // 
            this.settingBtn.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.settingBtn.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.settingBtn.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.settingBtn.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.settingBtn.FillColor = System.Drawing.Color.LightSteelBlue;
            this.settingBtn.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.settingBtn.ForeColor = System.Drawing.Color.White;
            this.settingBtn.Image = global::NaptienMaster.Properties.Resources.settings__1_;
            this.settingBtn.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.settingBtn.Location = new System.Drawing.Point(8, 7);
            this.settingBtn.Margin = new System.Windows.Forms.Padding(4);
            this.settingBtn.Name = "settingBtn";
            this.settingBtn.Size = new System.Drawing.Size(193, 66);
            this.settingBtn.TabIndex = 6;
            this.settingBtn.Text = "Cài đặt";
            this.settingBtn.Click += new System.EventHandler(this.settingBtn_Click_1);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.refreshBtn);
            this.tabPage2.Controls.Add(this.task_id_filter);
            this.tabPage2.Controls.Add(this.status_filter);
            this.tabPage2.Controls.Add(this.task_id_textbox);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.statusFilter);
            this.tabPage2.Controls.Add(this.kryptonPanel2);
            this.tabPage2.Location = new System.Drawing.Point(184, 4);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage2.Size = new System.Drawing.Size(1736, 791);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Lịch sử nạp tiền";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // refreshBtn
            // 
            this.refreshBtn.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.refreshBtn.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.refreshBtn.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.refreshBtn.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.refreshBtn.FillColor = System.Drawing.Color.Transparent;
            this.refreshBtn.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.refreshBtn.ForeColor = System.Drawing.Color.White;
            this.refreshBtn.HoverState.BorderColor = System.Drawing.Color.Transparent;
            this.refreshBtn.HoverState.FillColor = System.Drawing.Color.Transparent;
            this.refreshBtn.HoverState.ForeColor = System.Drawing.Color.Transparent;
            this.refreshBtn.Image = global::NaptienMaster.Properties.Resources.icons8_refresh_50;
            this.refreshBtn.ImageSize = new System.Drawing.Size(30, 30);
            this.refreshBtn.Location = new System.Drawing.Point(1392, 16);
            this.refreshBtn.Margin = new System.Windows.Forms.Padding(4);
            this.refreshBtn.Name = "refreshBtn";
            this.refreshBtn.Size = new System.Drawing.Size(120, 55);
            this.refreshBtn.TabIndex = 8;
            this.refreshBtn.Click += new System.EventHandler(this.refreshBtn_Click);
            // 
            // task_id_filter
            // 
            this.task_id_filter.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.task_id_filter.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.task_id_filter.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.task_id_filter.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.task_id_filter.FillColor = System.Drawing.Color.Transparent;
            this.task_id_filter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.task_id_filter.ForeColor = System.Drawing.Color.White;
            this.task_id_filter.HoverState.BorderColor = System.Drawing.Color.Transparent;
            this.task_id_filter.HoverState.FillColor = System.Drawing.Color.Transparent;
            this.task_id_filter.HoverState.ForeColor = System.Drawing.Color.Transparent;
            this.task_id_filter.Image = global::NaptienMaster.Properties.Resources.search;
            this.task_id_filter.ImageSize = new System.Drawing.Size(30, 30);
            this.task_id_filter.Location = new System.Drawing.Point(1165, 16);
            this.task_id_filter.Margin = new System.Windows.Forms.Padding(4);
            this.task_id_filter.Name = "task_id_filter";
            this.task_id_filter.Size = new System.Drawing.Size(120, 55);
            this.task_id_filter.TabIndex = 7;
            this.task_id_filter.Click += new System.EventHandler(this.task_id_filter_Click);
            // 
            // status_filter
            // 
            this.status_filter.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.status_filter.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.status_filter.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.status_filter.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.status_filter.FillColor = System.Drawing.Color.Transparent;
            this.status_filter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.status_filter.ForeColor = System.Drawing.Color.White;
            this.status_filter.HoverState.BorderColor = System.Drawing.Color.Transparent;
            this.status_filter.HoverState.FillColor = System.Drawing.Color.Transparent;
            this.status_filter.Image = global::NaptienMaster.Properties.Resources.icons8_filter_30;
            this.status_filter.ImageSize = new System.Drawing.Size(25, 25);
            this.status_filter.Location = new System.Drawing.Point(463, 27);
            this.status_filter.Margin = new System.Windows.Forms.Padding(4);
            this.status_filter.Name = "status_filter";
            this.status_filter.Size = new System.Drawing.Size(120, 43);
            this.status_filter.TabIndex = 6;
            this.status_filter.Click += new System.EventHandler(this.guna2Button2_Click);
            // 
            // task_id_textbox
            // 
            this.task_id_textbox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.task_id_textbox.DefaultText = "";
            this.task_id_textbox.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.task_id_textbox.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.task_id_textbox.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.task_id_textbox.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.task_id_textbox.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.task_id_textbox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.task_id_textbox.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.task_id_textbox.Location = new System.Drawing.Point(865, 27);
            this.task_id_textbox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.task_id_textbox.Name = "task_id_textbox";
            this.task_id_textbox.PasswordChar = '\0';
            this.task_id_textbox.PlaceholderText = "";
            this.task_id_textbox.SelectedText = "";
            this.task_id_textbox.Size = new System.Drawing.Size(267, 44);
            this.task_id_textbox.TabIndex = 4;
            this.task_id_textbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.task_id_textbox_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(615, 48);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(216, 24);
            this.label1.TabIndex = 3;
            this.label1.Text = "Mã giao dịch|SĐT|Telco:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // statusFilter
            // 
            this.statusFilter.BackColor = System.Drawing.Color.Transparent;
            this.statusFilter.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.statusFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.statusFilter.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.statusFilter.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.statusFilter.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.statusFilter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.statusFilter.ItemHeight = 30;
            this.statusFilter.Items.AddRange(new object[] {
            "Trạng thái:",
            "Thất bại",
            "Thành công",
            "Không xác định"});
            this.statusFilter.Location = new System.Drawing.Point(59, 27);
            this.statusFilter.Margin = new System.Windows.Forms.Padding(4);
            this.statusFilter.Name = "statusFilter";
            this.statusFilter.Size = new System.Drawing.Size(345, 36);
            this.statusFilter.TabIndex = 1;
            this.statusFilter.SelectedIndexChanged += new System.EventHandler(this.statusFilter_SelectedIndexChanged);
            // 
            // kryptonPanel2
            // 
            this.kryptonPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.kryptonPanel2.Controls.Add(this.transactionGridView);
            this.kryptonPanel2.Location = new System.Drawing.Point(4, 105);
            this.kryptonPanel2.Margin = new System.Windows.Forms.Padding(4);
            this.kryptonPanel2.Name = "kryptonPanel2";
            this.kryptonPanel2.Size = new System.Drawing.Size(2461, 684);
            this.kryptonPanel2.TabIndex = 0;
            // 
            // transactionGridView
            // 
            this.transactionGridView.AllowUserToAddRows = false;
            this.transactionGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.transactionGridView.ColumnHeadersHeight = 36;
            this.transactionGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.stt_trans,
            this.phone_trans,
            this.network_transac,
            this.simtype_transac,
            this.task_id,
            this.card_code,
            this.charge_before,
            this.charge_after,
            this.charged_time,
            this.status_charge,
            this.money_topup,
            this.real_topup,
            this.error_msg});
            this.transactionGridView.Location = new System.Drawing.Point(0, 4);
            this.transactionGridView.Margin = new System.Windows.Forms.Padding(4);
            this.transactionGridView.Name = "transactionGridView";
            this.transactionGridView.RowHeadersWidth = 51;
            this.transactionGridView.Size = new System.Drawing.Size(1728, 490);
            this.transactionGridView.TabIndex = 0;
            this.transactionGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.transactionGridView_CellContentClick);
            this.transactionGridView.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.transactionGridView_RowPostPaint);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.logTxtBox);
            this.tabPage3.Location = new System.Drawing.Point(184, 4);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1736, 791);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Log ";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // logTxtBox
            // 
            this.logTxtBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.logTxtBox.DefaultText = "";
            this.logTxtBox.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.logTxtBox.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.logTxtBox.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.logTxtBox.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.logTxtBox.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.logTxtBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.logTxtBox.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.logTxtBox.Location = new System.Drawing.Point(4, 28);
            this.logTxtBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.logTxtBox.Multiline = true;
            this.logTxtBox.Name = "logTxtBox";
            this.logTxtBox.PasswordChar = '\0';
            this.logTxtBox.PlaceholderText = "";
            this.logTxtBox.ReadOnly = true;
            this.logTxtBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logTxtBox.SelectedText = "";
            this.logTxtBox.Size = new System.Drawing.Size(1424, 731);
            this.logTxtBox.TabIndex = 0;
            // 
            // stt_trans
            // 
            this.stt_trans.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.stt_trans.HeaderText = "STT";
            this.stt_trans.MinimumWidth = 6;
            this.stt_trans.Name = "stt_trans";
            this.stt_trans.ReadOnly = true;
            this.stt_trans.Width = 66;
            // 
            // phone_trans
            // 
            this.phone_trans.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.phone_trans.HeaderText = "Số điện thoại";
            this.phone_trans.MinimumWidth = 6;
            this.phone_trans.Name = "phone_trans";
            this.phone_trans.ReadOnly = true;
            this.phone_trans.Width = 130;
            // 
            // network_transac
            // 
            this.network_transac.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.network_transac.HeaderText = "Nhà mạng";
            this.network_transac.MinimumWidth = 6;
            this.network_transac.Name = "network_transac";
            this.network_transac.ReadOnly = true;
            this.network_transac.Width = 111;
            // 
            // simtype_transac
            // 
            this.simtype_transac.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.simtype_transac.HeaderText = "Loại Sim";
            this.simtype_transac.MinimumWidth = 6;
            this.simtype_transac.Name = "simtype_transac";
            this.simtype_transac.Width = 99;
            // 
            // task_id
            // 
            this.task_id.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.task_id.HeaderText = "Mã giao dịch";
            this.task_id.MinimumWidth = 6;
            this.task_id.Name = "task_id";
            this.task_id.ReadOnly = true;
            this.task_id.Width = 129;
            // 
            // card_code
            // 
            this.card_code.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.card_code.HeaderText = "Mã Seri thẻ cào";
            this.card_code.MinimumWidth = 6;
            this.card_code.Name = "card_code";
            this.card_code.ReadOnly = true;
            this.card_code.Width = 145;
            // 
            // charge_before
            // 
            this.charge_before.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.charge_before.HeaderText = "Số dư tài khoản|Nợ cước trước";
            this.charge_before.MinimumWidth = 6;
            this.charge_before.Name = "charge_before";
            this.charge_before.ReadOnly = true;
            this.charge_before.Width = 245;
            // 
            // charge_after
            // 
            this.charge_after.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.charge_after.HeaderText = "Số dư tài khoản|Nợ cước sau";
            this.charge_after.MinimumWidth = 6;
            this.charge_after.Name = "charge_after";
            this.charge_after.ReadOnly = true;
            this.charge_after.Width = 232;
            // 
            // charged_time
            // 
            this.charged_time.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.charged_time.HeaderText = "Thời gian nạp";
            this.charged_time.MinimumWidth = 6;
            this.charged_time.Name = "charged_time";
            this.charged_time.ReadOnly = true;
            this.charged_time.Width = 133;
            // 
            // status_charge
            // 
            this.status_charge.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.status_charge.HeaderText = "Trạng thái";
            this.status_charge.MinimumWidth = 6;
            this.status_charge.Name = "status_charge";
            this.status_charge.ReadOnly = true;
            this.status_charge.Width = 108;
            // 
            // money_topup
            // 
            this.money_topup.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.money_topup.HeaderText = "Số tiền nạp";
            this.money_topup.MinimumWidth = 6;
            this.money_topup.Name = "money_topup";
            this.money_topup.ReadOnly = true;
            this.money_topup.Width = 117;
            // 
            // real_topup
            // 
            this.real_topup.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.real_topup.HeaderText = "Số tiền thực nhận";
            this.real_topup.MinimumWidth = 6;
            this.real_topup.Name = "real_topup";
            this.real_topup.ReadOnly = true;
            this.real_topup.Width = 157;
            // 
            // error_msg
            // 
            this.error_msg.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.error_msg.HeaderText = "Thông báo";
            this.error_msg.MinimumWidth = 6;
            this.error_msg.Name = "error_msg";
            this.error_msg.ReadOnly = true;
            this.error_msg.Width = 114;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1924, 799);
            this.Controls.Add(this.guna2TabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NaptienMaster";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGSM)).EndInit();
            this.guna2ContextMenuStrip1.ResumeLayout(false);
            this.guna2TabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.connection_state)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel2)).EndInit();
            this.kryptonPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.transactionGridView)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private Krypton.Toolkit.KryptonButton checkPortsBtn;
        private Krypton.Toolkit.KryptonButton loadPortsBtn;
        public Krypton.Toolkit.KryptonDataGridView dataGSM;
        private Krypton.Toolkit.KryptonLabel num_of_port;
        private Guna.UI2.WinForms.Guna2TabControl guna2TabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private Guna.UI2.WinForms.Guna2ComboBox statusFilter;
        private Guna.UI2.WinForms.Guna2TextBox task_id_textbox;
        private System.Windows.Forms.Label label1;
        private Guna.UI2.WinForms.Guna2TextBox logTxtBox;
        private Guna.UI2.WinForms.Guna2Button guna2Button1;
        private Guna.UI2.WinForms.Guna2Button settingBtn;
        private Guna.UI2.WinForms.Guna2Button task_id_filter;
        private Guna.UI2.WinForms.Guna2Button status_filter;
        private Guna.UI2.WinForms.Guna2ContextMenuStrip guna2ContextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tảiLạiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copySĐTToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyDanhSáchCổngToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToExcelToolStripMenuItem;
        private Krypton.Toolkit.KryptonPanel kryptonPanel2;
        private Krypton.Toolkit.KryptonDataGridView transactionGridView;
        private Guna.UI2.WinForms.Guna2Button refreshBtn;
        private System.Windows.Forms.Label version;
        private System.Windows.Forms.Label pc_id_label;
        private Guna.UI2.WinForms.Guna2CirclePictureBox connection_state;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn stt;
        private System.Windows.Forms.DataGridViewTextBoxColumn port;
        private System.Windows.Forms.DataGridViewTextBoxColumn port_custom;
        private System.Windows.Forms.DataGridViewTextBoxColumn telco;
        private System.Windows.Forms.DataGridViewTextBoxColumn sdt;
        private System.Windows.Forms.DataGridViewTextBoxColumn simtype;
        private System.Windows.Forms.DataGridViewTextBoxColumn status;
        private System.Windows.Forms.DataGridViewTextBoxColumn tkc;
        private System.Windows.Forms.DataGridViewTextBoxColumn tkkm;
        private System.Windows.Forms.DataGridViewTextBoxColumn charge;
        private System.Windows.Forms.DataGridViewTextBoxColumn charged;
        private System.Windows.Forms.DataGridViewTextBoxColumn expire;
        private System.Windows.Forms.DataGridViewTextBoxColumn message;
        private System.Windows.Forms.ToolStripMenuItem excelMẫuToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn stt_trans;
        private System.Windows.Forms.DataGridViewTextBoxColumn phone_trans;
        private System.Windows.Forms.DataGridViewTextBoxColumn network_transac;
        private System.Windows.Forms.DataGridViewTextBoxColumn simtype_transac;
        private System.Windows.Forms.DataGridViewTextBoxColumn task_id;
        private System.Windows.Forms.DataGridViewTextBoxColumn card_code;
        private System.Windows.Forms.DataGridViewTextBoxColumn charge_before;
        private System.Windows.Forms.DataGridViewTextBoxColumn charge_after;
        private System.Windows.Forms.DataGridViewTextBoxColumn charged_time;
        private System.Windows.Forms.DataGridViewTextBoxColumn status_charge;
        private System.Windows.Forms.DataGridViewTextBoxColumn money_topup;
        private System.Windows.Forms.DataGridViewTextBoxColumn real_topup;
        private System.Windows.Forms.DataGridViewTextBoxColumn error_msg;
    }
}

