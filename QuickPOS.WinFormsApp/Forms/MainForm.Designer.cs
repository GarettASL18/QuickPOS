namespace QuickPOS.WinFormsApp
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            pnlSidebar = new Panel();
            btnSalir = new Button();
            btnConfig = new Button();
            btnUsuarios = new Button();
            btnProductos = new Button();
            btnClientes = new Button();
            btnHistorial = new Button();
            btnVenta = new Button();
            panel1 = new Panel();
            lblQuickPOS = new Label();
            pnlHeader = new Panel();
            lblClock = new Label();
            lblUserInfo = new Label();
            pnlContent = new Panel();
            flowLayoutPanel1 = new FlowLayoutPanel();
            roundedPanel1 = new QuickPOS.WinFormsApp.Forms.RoundedPanel();
            label2 = new Label();
            label1 = new Label();
            pictureBox1 = new PictureBox();
            roundedPanel2 = new QuickPOS.WinFormsApp.Forms.RoundedPanel();
            label3 = new Label();
            label4 = new Label();
            pictureBox2 = new PictureBox();
            roundedPanel3 = new QuickPOS.WinFormsApp.Forms.RoundedPanel();
            label5 = new Label();
            label6 = new Label();
            pictureBox3 = new PictureBox();
            roundedPanel4 = new QuickPOS.WinFormsApp.Forms.RoundedPanel();
            label7 = new Label();
            label8 = new Label();
            pictureBox4 = new PictureBox();
            pnlSidebar.SuspendLayout();
            panel1.SuspendLayout();
            pnlHeader.SuspendLayout();
            pnlContent.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            roundedPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            roundedPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            roundedPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            roundedPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            SuspendLayout();
            // 
            // pnlSidebar
            // 
            pnlSidebar.BackColor = Color.MidnightBlue;
            pnlSidebar.Controls.Add(btnSalir);
            pnlSidebar.Controls.Add(btnConfig);
            pnlSidebar.Controls.Add(btnUsuarios);
            pnlSidebar.Controls.Add(btnProductos);
            pnlSidebar.Controls.Add(btnClientes);
            pnlSidebar.Controls.Add(btnHistorial);
            pnlSidebar.Controls.Add(btnVenta);
            pnlSidebar.Controls.Add(panel1);
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Location = new Point(0, 0);
            pnlSidebar.Name = "pnlSidebar";
            pnlSidebar.Size = new Size(250, 684);
            pnlSidebar.TabIndex = 0;
            // 
            // btnSalir
            // 
            btnSalir.Dock = DockStyle.Top;
            btnSalir.FlatAppearance.BorderSize = 0;
            btnSalir.FlatStyle = FlatStyle.Flat;
            btnSalir.Font = new Font("Segoe UI", 11.25F);
            btnSalir.ForeColor = Color.White;
            btnSalir.Image = Properties.Resources.icons8_logout_32;
            btnSalir.ImageAlign = ContentAlignment.MiddleLeft;
            btnSalir.Location = new Point(0, 400);
            btnSalir.Name = "btnSalir";
            btnSalir.Padding = new Padding(20, 0, 0, 0);
            btnSalir.Size = new Size(250, 50);
            btnSalir.TabIndex = 7;
            btnSalir.Text = " Salir";
            btnSalir.TextAlign = ContentAlignment.MiddleLeft;
            btnSalir.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnSalir.UseVisualStyleBackColor = true;
            // 
            // btnConfig
            // 
            btnConfig.Dock = DockStyle.Top;
            btnConfig.FlatAppearance.BorderSize = 0;
            btnConfig.FlatStyle = FlatStyle.Flat;
            btnConfig.Font = new Font("Segoe UI", 11.25F);
            btnConfig.ForeColor = Color.White;
            btnConfig.Image = Properties.Resources.icons8_settings_32;
            btnConfig.ImageAlign = ContentAlignment.MiddleLeft;
            btnConfig.Location = new Point(0, 350);
            btnConfig.Name = "btnConfig";
            btnConfig.Padding = new Padding(20, 0, 0, 0);
            btnConfig.Size = new Size(250, 50);
            btnConfig.TabIndex = 6;
            btnConfig.Text = " Configuración";
            btnConfig.TextAlign = ContentAlignment.MiddleLeft;
            btnConfig.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnConfig.UseVisualStyleBackColor = true;
            // 
            // btnUsuarios
            // 
            btnUsuarios.Dock = DockStyle.Top;
            btnUsuarios.FlatAppearance.BorderSize = 0;
            btnUsuarios.FlatStyle = FlatStyle.Flat;
            btnUsuarios.Font = new Font("Segoe UI", 11.25F);
            btnUsuarios.ForeColor = Color.White;
            btnUsuarios.Image = Properties.Resources.icons8_user_shield_32;
            btnUsuarios.ImageAlign = ContentAlignment.MiddleLeft;
            btnUsuarios.Location = new Point(0, 300);
            btnUsuarios.Name = "btnUsuarios";
            btnUsuarios.Padding = new Padding(20, 0, 0, 0);
            btnUsuarios.Size = new Size(250, 50);
            btnUsuarios.TabIndex = 5;
            btnUsuarios.Text = " Usuarios";
            btnUsuarios.TextAlign = ContentAlignment.MiddleLeft;
            btnUsuarios.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnUsuarios.UseVisualStyleBackColor = true;
            // 
            // btnProductos
            // 
            btnProductos.Dock = DockStyle.Top;
            btnProductos.FlatAppearance.BorderSize = 0;
            btnProductos.FlatStyle = FlatStyle.Flat;
            btnProductos.Font = new Font("Segoe UI", 11.25F);
            btnProductos.ForeColor = Color.White;
            btnProductos.Image = Properties.Resources.icons8_box_32;
            btnProductos.ImageAlign = ContentAlignment.MiddleLeft;
            btnProductos.Location = new Point(0, 250);
            btnProductos.Name = "btnProductos";
            btnProductos.Padding = new Padding(20, 0, 0, 0);
            btnProductos.Size = new Size(250, 50);
            btnProductos.TabIndex = 4;
            btnProductos.Text = " Productos / Items";
            btnProductos.TextAlign = ContentAlignment.MiddleLeft;
            btnProductos.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnProductos.UseVisualStyleBackColor = true;
            // 
            // btnClientes
            // 
            btnClientes.Dock = DockStyle.Top;
            btnClientes.FlatAppearance.BorderSize = 0;
            btnClientes.FlatStyle = FlatStyle.Flat;
            btnClientes.Font = new Font("Segoe UI", 11.25F);
            btnClientes.ForeColor = Color.White;
            btnClientes.Image = Properties.Resources.icons8_users_32;
            btnClientes.ImageAlign = ContentAlignment.MiddleLeft;
            btnClientes.Location = new Point(0, 200);
            btnClientes.Name = "btnClientes";
            btnClientes.Padding = new Padding(20, 0, 0, 0);
            btnClientes.Size = new Size(250, 50);
            btnClientes.TabIndex = 3;
            btnClientes.Text = " Clientes";
            btnClientes.TextAlign = ContentAlignment.MiddleLeft;
            btnClientes.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnClientes.UseVisualStyleBackColor = true;
            // 
            // btnHistorial
            // 
            btnHistorial.Dock = DockStyle.Top;
            btnHistorial.FlatAppearance.BorderSize = 0;
            btnHistorial.FlatStyle = FlatStyle.Flat;
            btnHistorial.Font = new Font("Segoe UI", 11.25F);
            btnHistorial.ForeColor = Color.White;
            btnHistorial.Image = Properties.Resources.icons8_resume_32;
            btnHistorial.ImageAlign = ContentAlignment.MiddleLeft;
            btnHistorial.Location = new Point(0, 150);
            btnHistorial.Name = "btnHistorial";
            btnHistorial.Padding = new Padding(20, 0, 0, 0);
            btnHistorial.Size = new Size(250, 50);
            btnHistorial.TabIndex = 8;
            btnHistorial.Text = " Historial Ventas";
            btnHistorial.TextAlign = ContentAlignment.MiddleLeft;
            btnHistorial.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnHistorial.UseVisualStyleBackColor = true;
            // 
            // btnVenta
            // 
            btnVenta.Dock = DockStyle.Top;
            btnVenta.FlatAppearance.BorderSize = 0;
            btnVenta.FlatStyle = FlatStyle.Flat;
            btnVenta.Font = new Font("Segoe UI", 11.25F);
            btnVenta.ForeColor = Color.White;
            btnVenta.Image = Properties.Resources.icons8_shopping_cart_32;
            btnVenta.ImageAlign = ContentAlignment.MiddleLeft;
            btnVenta.Location = new Point(0, 100);
            btnVenta.Name = "btnVenta";
            btnVenta.Padding = new Padding(20, 0, 0, 0);
            btnVenta.Size = new Size(250, 50);
            btnVenta.TabIndex = 2;
            btnVenta.Text = " Venta / Facturación";
            btnVenta.TextAlign = ContentAlignment.MiddleLeft;
            btnVenta.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnVenta.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Controls.Add(lblQuickPOS);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(250, 100);
            panel1.TabIndex = 0;
            // 
            // lblQuickPOS
            // 
            lblQuickPOS.AutoSize = true;
            lblQuickPOS.BackColor = Color.Transparent;
            lblQuickPOS.Font = new Font("Segoe UI Black", 21.75F, FontStyle.Bold);
            lblQuickPOS.ForeColor = Color.White;
            lblQuickPOS.Location = new Point(48, 30);
            lblQuickPOS.Name = "lblQuickPOS";
            lblQuickPOS.Size = new Size(155, 40);
            lblQuickPOS.TabIndex = 0;
            lblQuickPOS.Text = "QuickPOS";
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.White;
            pnlHeader.Controls.Add(lblClock);
            pnlHeader.Controls.Add(lblUserInfo);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(250, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1050, 60);
            pnlHeader.TabIndex = 1;
            // 
            // lblClock
            // 
            lblClock.AutoSize = true;
            lblClock.Dock = DockStyle.Right;
            lblClock.Font = new Font("Segoe UI", 14.25F);
            lblClock.ForeColor = Color.DimGray;
            lblClock.Location = new Point(944, 0);
            lblClock.Name = "lblClock";
            lblClock.Size = new Size(56, 25);
            lblClock.TabIndex = 1;
            lblClock.Text = "00:00";
            // 
            // lblUserInfo
            // 
            lblUserInfo.AutoSize = true;
            lblUserInfo.Dock = DockStyle.Right;
            lblUserInfo.Font = new Font("Segoe UI", 14.25F);
            lblUserInfo.Location = new Point(1000, 0);
            lblUserInfo.Name = "lblUserInfo";
            lblUserInfo.Size = new Size(50, 25);
            lblUserInfo.TabIndex = 0;
            lblUserInfo.Text = "User";
            // 
            // pnlContent
            // 
            pnlContent.BackColor = Color.Gainsboro;
            pnlContent.Controls.Add(flowLayoutPanel1);
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.Location = new Point(250, 60);
            pnlContent.Name = "pnlContent";
            pnlContent.Size = new Size(1050, 624);
            pnlContent.TabIndex = 2;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(roundedPanel1);
            flowLayoutPanel1.Controls.Add(roundedPanel2);
            flowLayoutPanel1.Controls.Add(roundedPanel3);
            flowLayoutPanel1.Controls.Add(roundedPanel4);
            flowLayoutPanel1.Dock = DockStyle.Top;
            flowLayoutPanel1.Location = new Point(0, 0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Padding = new Padding(20, 20, 0, 0);
            flowLayoutPanel1.Size = new Size(1050, 153);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // roundedPanel1
            // 
            roundedPanel1.BackColor = Color.Transparent;
            roundedPanel1.CardColor = Color.White;
            roundedPanel1.Controls.Add(label2);
            roundedPanel1.Controls.Add(label1);
            roundedPanel1.Controls.Add(pictureBox1);
            roundedPanel1.CornerRadius = 30;
            roundedPanel1.Location = new Point(23, 23);
            roundedPanel1.Name = "roundedPanel1";
            roundedPanel1.Padding = new Padding(10);
            roundedPanel1.Shadow = true;
            roundedPanel1.ShadowDepth = 6;
            roundedPanel1.ShadowOpacity = 30;
            roundedPanel1.ShadowYOffset = 3;
            roundedPanel1.Size = new Size(250, 110);
            roundedPanel1.TabIndex = 0;
            // 
            // label2
            // 
            label2.AutoEllipsis = true;
            label2.Dock = DockStyle.Bottom;
            label2.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label2.Location = new Point(10, 60);
            label2.Name = "label2";
            label2.Size = new Size(230, 40);
            label2.TabIndex = 2;
            label2.Text = "$ 0.00";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = Color.Gray;
            label1.Location = new Point(94, 27);
            label1.Name = "label1";
            label1.Size = new Size(66, 15);
            label1.TabIndex = 1;
            label1.Text = "Ventas Hoy";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.icons8_dolar_32;
            pictureBox1.Location = new Point(23, 17);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(32, 32);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // roundedPanel2
            // 
            roundedPanel2.BackColor = Color.Transparent;
            roundedPanel2.CardColor = Color.White;
            roundedPanel2.Controls.Add(label3);
            roundedPanel2.Controls.Add(label4);
            roundedPanel2.Controls.Add(pictureBox2);
            roundedPanel2.CornerRadius = 30;
            roundedPanel2.Location = new Point(279, 23);
            roundedPanel2.Name = "roundedPanel2";
            roundedPanel2.Padding = new Padding(10);
            roundedPanel2.Shadow = true;
            roundedPanel2.ShadowDepth = 6;
            roundedPanel2.ShadowOpacity = 30;
            roundedPanel2.ShadowYOffset = 3;
            roundedPanel2.Size = new Size(250, 110);
            roundedPanel2.TabIndex = 1;
            // 
            // label3
            // 
            label3.AutoEllipsis = true;
            label3.Dock = DockStyle.Bottom;
            label3.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label3.Location = new Point(10, 60);
            label3.Name = "label3";
            label3.Size = new Size(230, 40);
            label3.TabIndex = 2;
            label3.Text = "0";
            label3.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = Color.Gray;
            label4.Location = new Point(74, 27);
            label4.Name = "label4";
            label4.Size = new Size(110, 15);
            label4.TabIndex = 1;
            label4.Text = "Facturas (Cantidad)";
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.icons8_bills_32;
            pictureBox2.Location = new Point(23, 17);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(32, 32);
            pictureBox2.TabIndex = 0;
            pictureBox2.TabStop = false;
            // 
            // roundedPanel3
            // 
            roundedPanel3.BackColor = Color.Transparent;
            roundedPanel3.CardColor = Color.White;
            roundedPanel3.Controls.Add(label5);
            roundedPanel3.Controls.Add(label6);
            roundedPanel3.Controls.Add(pictureBox3);
            roundedPanel3.CornerRadius = 30;
            roundedPanel3.Location = new Point(535, 23);
            roundedPanel3.Name = "roundedPanel3";
            roundedPanel3.Padding = new Padding(10);
            roundedPanel3.Shadow = true;
            roundedPanel3.ShadowDepth = 6;
            roundedPanel3.ShadowOpacity = 30;
            roundedPanel3.ShadowYOffset = 3;
            roundedPanel3.Size = new Size(250, 110);
            roundedPanel3.TabIndex = 2;
            // 
            // label5
            // 
            label5.AutoEllipsis = true;
            label5.Dock = DockStyle.Bottom;
            label5.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label5.Location = new Point(10, 60);
            label5.Name = "label5";
            label5.Size = new Size(230, 40);
            label5.TabIndex = 2;
            label5.Text = "0";
            label5.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.ForeColor = Color.Gray;
            label6.Location = new Point(77, 27);
            label6.Name = "label6";
            label6.Size = new Size(49, 15);
            label6.TabIndex = 1;
            label6.Text = "Clientes";
            // 
            // pictureBox3
            // 
            pictureBox3.Image = Properties.Resources.icons8_client_32;
            pictureBox3.Location = new Point(23, 17);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(32, 32);
            pictureBox3.TabIndex = 0;
            pictureBox3.TabStop = false;
            // 
            // roundedPanel4
            // 
            roundedPanel4.BackColor = Color.Transparent;
            roundedPanel4.CardColor = Color.White;
            roundedPanel4.Controls.Add(label7);
            roundedPanel4.Controls.Add(label8);
            roundedPanel4.Controls.Add(pictureBox4);
            roundedPanel4.CornerRadius = 30;
            roundedPanel4.Location = new Point(791, 23);
            roundedPanel4.Name = "roundedPanel4";
            roundedPanel4.Padding = new Padding(10);
            roundedPanel4.Shadow = true;
            roundedPanel4.ShadowDepth = 6;
            roundedPanel4.ShadowOpacity = 30;
            roundedPanel4.ShadowYOffset = 3;
            roundedPanel4.Size = new Size(250, 110);
            roundedPanel4.TabIndex = 3;
            // 
            // label7
            // 
            label7.AutoEllipsis = true;
            label7.Dock = DockStyle.Bottom;
            label7.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label7.Location = new Point(10, 60);
            label7.Name = "label7";
            label7.Size = new Size(230, 40);
            label7.TabIndex = 2;
            label7.Text = "0";
            label7.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.ForeColor = Color.Gray;
            label8.Location = new Point(77, 27);
            label8.Name = "label8";
            label8.Size = new Size(61, 15);
            label8.TabIndex = 1;
            label8.Text = "Productos";
            // 
            // pictureBox4
            // 
            pictureBox4.Image = Properties.Resources.icons8_product_32;
            pictureBox4.Location = new Point(23, 17);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(32, 32);
            pictureBox4.TabIndex = 0;
            pictureBox4.TabStop = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1300, 684);
            Controls.Add(pnlContent);
            Controls.Add(pnlHeader);
            Controls.Add(pnlSidebar);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "QuickPOS - Dashboard";
            WindowState = FormWindowState.Maximized;
            Load += MainForm_Load;
            pnlSidebar.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlContent.ResumeLayout(false);
            flowLayoutPanel1.ResumeLayout(false);
            roundedPanel1.ResumeLayout(false);
            roundedPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            roundedPanel2.ResumeLayout(false);
            roundedPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            roundedPanel3.ResumeLayout(false);
            roundedPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            roundedPanel4.ResumeLayout(false);
            roundedPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlSidebar;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblQuickPOS;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.Button btnConfig;
        private System.Windows.Forms.Button btnUsuarios;
        private System.Windows.Forms.Button btnProductos;
        private System.Windows.Forms.Button btnClientes;
        private System.Windows.Forms.Button btnHistorial;
        private System.Windows.Forms.Button btnVenta;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblUserInfo;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Label lblClock;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private QuickPOS.WinFormsApp.Forms.RoundedPanel roundedPanel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private QuickPOS.WinFormsApp.Forms.RoundedPanel roundedPanel2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox2;
        private QuickPOS.WinFormsApp.Forms.RoundedPanel roundedPanel3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox pictureBox3;
        private QuickPOS.WinFormsApp.Forms.RoundedPanel roundedPanel4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.PictureBox pictureBox4;
    }
}