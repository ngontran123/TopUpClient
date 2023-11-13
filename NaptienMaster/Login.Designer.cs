namespace NaptienMaster
{
    partial class Login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            this.loginLabel = new System.Windows.Forms.Label();
            this.loginBtn = new Guna.UI2.WinForms.Guna2Button();
            this.save_password_switch = new Guna.UI2.WinForms.Guna2ToggleSwitch();
            this.label1 = new System.Windows.Forms.Label();
            this.login_connect_state = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.tokenTxtBox = new Guna.UI2.WinForms.Guna2TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.login_connect_state)).BeginInit();
            this.SuspendLayout();
            // 
            // loginLabel
            // 
            resources.ApplyResources(this.loginLabel, "loginLabel");
            this.loginLabel.Name = "loginLabel";
            // 
            // loginBtn
            // 
            resources.ApplyResources(this.loginBtn, "loginBtn");
            this.loginBtn.Animated = true;
            this.loginBtn.AutoRoundedCorners = true;
            this.loginBtn.BorderRadius = 26;
            this.loginBtn.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.loginBtn.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.loginBtn.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.loginBtn.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.loginBtn.FillColor = System.Drawing.Color.Teal;
            this.loginBtn.ForeColor = System.Drawing.Color.White;
            this.loginBtn.Name = "loginBtn";
            this.loginBtn.Click += new System.EventHandler(this.loginBtn_Click);
            // 
            // save_password_switch
            // 
            resources.ApplyResources(this.save_password_switch, "save_password_switch");
            this.save_password_switch.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.save_password_switch.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.save_password_switch.CheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.save_password_switch.CheckedState.InnerColor = System.Drawing.Color.White;
            this.save_password_switch.Name = "save_password_switch";
            this.save_password_switch.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.save_password_switch.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.save_password_switch.UncheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.save_password_switch.UncheckedState.InnerColor = System.Drawing.Color.White;
            this.save_password_switch.CheckedChanged += new System.EventHandler(this.save_password_switch_CheckedChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // login_connect_state
            // 
            resources.ApplyResources(this.login_connect_state, "login_connect_state");
            this.login_connect_state.ImageRotate = 0F;
            this.login_connect_state.Name = "login_connect_state";
            this.login_connect_state.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.login_connect_state.TabStop = false;
            // 
            // tokenTxtBox
            // 
            resources.ApplyResources(this.tokenTxtBox, "tokenTxtBox");
            this.tokenTxtBox.BorderColor = System.Drawing.Color.Teal;
            this.tokenTxtBox.BorderThickness = 2;
            this.tokenTxtBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tokenTxtBox.DefaultText = "";
            this.tokenTxtBox.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.tokenTxtBox.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.tokenTxtBox.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.tokenTxtBox.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.tokenTxtBox.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.tokenTxtBox.HoverState.BorderColor = System.Drawing.Color.LightSeaGreen;
            this.tokenTxtBox.IconLeft = global::NaptienMaster.Properties.Resources.security;
            this.tokenTxtBox.IconLeftSize = new System.Drawing.Size(35, 35);
            this.tokenTxtBox.Multiline = true;
            this.tokenTxtBox.Name = "tokenTxtBox";
            this.tokenTxtBox.PasswordChar = '\0';
            this.tokenTxtBox.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.tokenTxtBox.PlaceholderText = "";
            this.tokenTxtBox.SelectedText = "";
            this.tokenTxtBox.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            this.tokenTxtBox.TextChanged += new System.EventHandler(this.tokenTxtBox_TextChanged);
            this.tokenTxtBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tokenTxtBox_KeyDown);
            this.tokenTxtBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tokenTxtBox_KeyPress);
            // 
            // Login
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.login_connect_state);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.save_password_switch);
            this.Controls.Add(this.loginBtn);
            this.Controls.Add(this.tokenTxtBox);
            this.Controls.Add(this.loginLabel);
            this.Name = "Login";
            this.Load += new System.EventHandler(this.Login_Load);
            ((System.ComponentModel.ISupportInitialize)(this.login_connect_state)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label loginLabel;
        private Guna.UI2.WinForms.Guna2TextBox tokenTxtBox;
        private Guna.UI2.WinForms.Guna2Button loginBtn;
        private Guna.UI2.WinForms.Guna2ToggleSwitch save_password_switch;
        private System.Windows.Forms.Label label1;
        private Guna.UI2.WinForms.Guna2CirclePictureBox login_connect_state;
    }
}