using System;
using System.Windows.Forms;

namespace QuickPOS.WinFormsApp.Forms
{
    partial class LoginForm
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador.
        /// No se puede modificar el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            card = new RoundedPanel();
            lblTitle = new Label();
            lblSub = new Label();
            btnCancel = new Button();
            btnLogin = new Button();
            lnkForgot = new LinkLabel();
            chkRemember = new CheckBox();
            pbTogglePass = new PictureBox();
            txtPass = new TextBox();
            lblPass = new Label();
            txtUser = new TextBox();
            lblUser = new Label();
            card.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbTogglePass).BeginInit();
            SuspendLayout();
            // 
            // card
            // 
            card.BackColor = Color.Transparent;
            card.CardColor = Color.White;
            card.Controls.Add(lblTitle);
            card.Controls.Add(lblSub);
            card.Controls.Add(btnCancel);
            card.Controls.Add(btnLogin);
            card.Controls.Add(lnkForgot);
            card.Controls.Add(chkRemember);
            card.Controls.Add(pbTogglePass);
            card.Controls.Add(txtPass);
            card.Controls.Add(lblPass);
            card.Controls.Add(txtUser);
            card.Controls.Add(lblUser);
            card.CornerRadius = 30;
            card.Location = new Point(179, 139);
            card.Name = "card";
            card.Padding = new Padding(10);
            card.Shadow = true;
            card.ShadowDepth = 5;
            card.ShadowOpacity = 30;
            card.ShadowYOffset = 3;
            card.Size = new Size(650, 450);
            card.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Segoe UI Black", 21.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.Location = new Point(237, 46);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(177, 40);
            lblTitle.TabIndex = 1;
            lblTitle.Text = "Bienvenido";
            lblTitle.Click += lblTitle_Click;
            // 
            // lblSub
            // 
            lblSub.AutoSize = true;
            lblSub.BackColor = Color.Transparent;
            lblSub.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSub.ForeColor = Color.Gray;
            lblSub.Location = new Point(226, 86);
            lblSub.Name = "lblSub";
            lblSub.Size = new Size(199, 21);
            lblSub.TabIndex = 2;
            lblSub.Text = "Inicie sesión para continuar";
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.White;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatStyle = FlatStyle.System;
            btnCancel.ForeColor = Color.Gray;
            btnCancel.Location = new Point(288, 382);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 8;
            btnCancel.Text = "Salir";
            btnCancel.UseVisualStyleBackColor = false;
            // 
            // btnLogin
            // 
            btnLogin.BackColor = Color.Black;
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatStyle = FlatStyle.System;
            btnLogin.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnLogin.ForeColor = Color.White;
            btnLogin.Location = new Point(252, 337);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(146, 39);
            btnLogin.TabIndex = 7;
            btnLogin.Text = "INICIAR SESIÓN";
            btnLogin.UseVisualStyleBackColor = false;
            // 
            // lnkForgot
            // 
            lnkForgot.AutoSize = true;
            lnkForgot.LinkColor = Color.RoyalBlue;
            lnkForgot.Location = new Point(161, 292);
            lnkForgot.Name = "lnkForgot";
            lnkForgot.Size = new Size(141, 15);
            lnkForgot.TabIndex = 6;
            lnkForgot.TabStop = true;
            lnkForgot.Text = "¿Olvidaste tu contraseña?";
            // 
            // chkRemember
            // 
            chkRemember.AutoSize = true;
            chkRemember.Cursor = Cursors.Hand;
            chkRemember.Location = new Point(161, 270);
            chkRemember.Name = "chkRemember";
            chkRemember.Size = new Size(90, 19);
            chkRemember.TabIndex = 5;
            chkRemember.Text = "Recordarme";
            chkRemember.UseVisualStyleBackColor = true;
            // 
            // pbTogglePass
            // 
            pbTogglePass.Cursor = Cursors.Hand;
            pbTogglePass.Image = (Image)resources.GetObject("pbTogglePass.Image");
            pbTogglePass.Location = new Point(461, 237);
            pbTogglePass.Name = "pbTogglePass";
            pbTogglePass.Size = new Size(24, 24);
            pbTogglePass.TabIndex = 4;
            pbTogglePass.TabStop = false;
            // 
            // txtPass
            // 
            txtPass.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtPass.Location = new Point(161, 235);
            txtPass.Name = "txtPass";
            txtPass.Size = new Size(328, 29);
            txtPass.TabIndex = 3;
            txtPass.UseSystemPasswordChar = true;
            // 
            // lblPass
            // 
            lblPass.AutoSize = true;
            lblPass.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblPass.ForeColor = Color.DimGray;
            lblPass.Location = new Point(161, 217);
            lblPass.Name = "lblPass";
            lblPass.Size = new Size(85, 15);
            lblPass.TabIndex = 2;
            lblPass.Text = "CONTRASEÑA";
            // 
            // txtUser
            // 
            txtUser.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtUser.Location = new Point(161, 163);
            txtUser.Name = "txtUser";
            txtUser.Size = new Size(328, 29);
            txtUser.TabIndex = 1;
            // 
            // lblUser
            // 
            lblUser.AutoSize = true;
            lblUser.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblUser.ForeColor = Color.DimGray;
            lblUser.Location = new Point(161, 145);
            lblUser.Name = "lblUser";
            lblUser.Size = new Size(61, 15);
            lblUser.TabIndex = 0;
            lblUser.Text = "USUARIO";
            // 
            // LoginForm
            // 
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1008, 729);
            Controls.Add(card);
            Name = "LoginForm";
            Text = "LoginForm";
            WindowState = FormWindowState.Maximized;
            Load += LoginForm_Load;
            Resize += LoginForm_Resize;
            card.ResumeLayout(false);
            card.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbTogglePass).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private RoundedPanel card;
        private Label lblTitle;
        private Label lblSub;
        private TextBox txtUser;
        private Label lblUser;
        private TextBox txtPass;
        private Label lblPass;
        private PictureBox pbTogglePass;
        private CheckBox chkRemember;
        private Button btnLogin;
        private LinkLabel lnkForgot;
        private Button btnCancel;
    }
}