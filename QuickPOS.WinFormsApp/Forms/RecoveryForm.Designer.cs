namespace QuickPOS.WinFormsApp.Forms
{
    partial class RecoveryForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecoveryForm));
            card = new RoundedPanel();
            btnCancel = new Button();
            btnSave = new Button();
            txtNewPass = new TextBox();
            txtCode = new TextBox();
            lblTitle = new Label();
            lblSub = new Label();
            btnSend = new Button();
            txtUser = new TextBox();
            lblUser = new Label();
            card.SuspendLayout();
            SuspendLayout();
            // 
            // card
            // 
            card.BackColor = Color.Transparent;
            card.CardColor = Color.White;
            card.Controls.Add(btnCancel);
            card.Controls.Add(btnSave);
            card.Controls.Add(txtNewPass);
            card.Controls.Add(txtCode);
            card.Controls.Add(lblTitle);
            card.Controls.Add(lblSub);
            card.Controls.Add(btnSend);
            card.Controls.Add(txtUser);
            card.Controls.Add(lblUser);
            card.CornerRadius = 30;
            card.Location = new Point(229, 100);
            card.Name = "card";
            card.Padding = new Padding(10);
            card.Shadow = true;
            card.ShadowDepth = 5;
            card.ShadowOpacity = 30;
            card.ShadowYOffset = 3;
            card.Size = new Size(550, 420);
            card.TabIndex = 0;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.Transparent;
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.ForeColor = Color.Gray;
            btnCancel.Location = new Point(236, 360);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(78, 25);
            btnCancel.TabIndex = 8;
            btnCancel.Text = "Cancelar";
            btnCancel.UseVisualStyleBackColor = false;
            // 
            // btnSave
            // 
            btnSave.BackColor = Color.Gray;
            btnSave.Cursor = Cursors.Hand;
            btnSave.Enabled = false;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSave.ForeColor = Color.White;
            btnSave.Location = new Point(190, 315);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(170, 35);
            btnSave.TabIndex = 7;
            btnSave.Text = "CAMBIAR CONTRASEÑA";
            btnSave.UseVisualStyleBackColor = false;
            // 
            // txtNewPass
            // 
            txtNewPass.Enabled = false;
            txtNewPass.Font = new Font("Segoe UI", 12F);
            txtNewPass.Location = new Point(110, 270);
            txtNewPass.Name = "txtNewPass";
            txtNewPass.Size = new Size(330, 29);
            txtNewPass.TabIndex = 6;
            txtNewPass.UseSystemPasswordChar = true;
            // 
            // txtCode
            // 
            txtCode.Enabled = false;
            txtCode.Font = new Font("Segoe UI", 12F);
            txtCode.Location = new Point(110, 230);
            txtCode.Name = "txtCode";
            txtCode.Size = new Size(330, 29);
            txtCode.TabIndex = 5;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitle.Location = new Point(180, 30);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(190, 37);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Recuperación";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblSub
            // 
            lblSub.AutoSize = true;
            lblSub.BackColor = Color.Transparent;
            lblSub.Font = new Font("Segoe UI", 10F);
            lblSub.ForeColor = Color.Gray;
            lblSub.Location = new Point(150, 75);
            lblSub.Name = "lblSub";
            lblSub.Size = new Size(251, 19);
            lblSub.TabIndex = 1;
            lblSub.Text = "Ingresa tu usuario para recibir el código";
            // 
            // btnSend
            // 
            btnSend.BackColor = Color.Black;
            btnSend.Cursor = Cursors.Hand;
            btnSend.FlatAppearance.BorderSize = 0;
            btnSend.FlatStyle = FlatStyle.Flat;
            btnSend.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSend.ForeColor = Color.White;
            btnSend.Location = new Point(190, 170);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(170, 35);
            btnSend.TabIndex = 4;
            btnSend.Text = "ENVIAR CÓDIGO";
            btnSend.UseVisualStyleBackColor = false;
            // 
            // txtUser
            // 
            txtUser.Font = new Font("Segoe UI", 12F);
            txtUser.Location = new Point(110, 130);
            txtUser.Name = "txtUser";
            txtUser.Size = new Size(330, 29);
            txtUser.TabIndex = 3;
            // 
            // lblUser
            // 
            lblUser.AutoSize = true;
            lblUser.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblUser.ForeColor = Color.DimGray;
            lblUser.Location = new Point(110, 110);
            lblUser.Name = "lblUser";
            lblUser.Size = new Size(61, 15);
            lblUser.TabIndex = 2;
            lblUser.Text = "USUARIO";
            // 
            // RecoveryForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1008, 729);
            Controls.Add(card);
            Name = "RecoveryForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "QuickPOS - Recuperar";
            card.ResumeLayout(false);
            card.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private RoundedPanel card;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSub;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtNewPass;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Button btnCancel;
    }
}