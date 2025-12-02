using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace QuickPOS.WinFormsApp
{
    partial class MainForm
    {
        // Declaraciones de controles — el diseñador las necesita
        private FlowLayoutPanel tilesPanel;
        private Panel header;
        private Label lblTitle;
        private Panel bottom;
        private Button btnLogout;
        private Label lblUser;
        private Label placeholder;

        /// <summary>
        /// Método requerido para el soporte del Diseñador — no modificar manualmente.
        /// La lógica runtime (carga de imágenes, creación dinámica de tiles reales) debe ir en MainForm.cs.
        /// </summary>
        private void InitializeComponent()
        {
            header = new Panel();
            lblTitle = new Label();
            lblUser = new Label();
            tilesPanel = new FlowLayoutPanel();
            placeholder = new Label();
            bottom = new Panel();
            btnLogout = new Button();

            header.SuspendLayout();
            tilesPanel.SuspendLayout();
            bottom.SuspendLayout();
            SuspendLayout();

            // 
            // header
            // 
            header.BackColor = Color.FromArgb(30, 144, 255);
            header.Controls.Add(lblTitle);
            header.Controls.Add(lblUser);
            header.Dock = DockStyle.Top;
            header.Location = new Point(0, 0);
            header.Name = "header";
            header.Padding = new Padding(8);
            header.Size = new Size(982, 56);
            header.TabIndex = 2;

            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(8, 8);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(300, 40);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "QuickPOS";
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;

            // 
            // lblUser
            // 
            lblUser.Dock = DockStyle.Right;
            lblUser.ForeColor = Color.White;
            lblUser.Location = new Point(614, 8);
            lblUser.Name = "lblUser";
            lblUser.Size = new Size(360, 40);
            lblUser.TabIndex = 1;
            lblUser.Text = "Usuario: —   Rol: —";
            lblUser.TextAlign = ContentAlignment.MiddleRight;

            // 
            // tilesPanel
            // 
            tilesPanel.AutoScroll = true;
            tilesPanel.BackColor = Color.Transparent;
            tilesPanel.Dock = DockStyle.Fill;
            tilesPanel.Location = new Point(0, 56);
            tilesPanel.Name = "tilesPanel";
            tilesPanel.Padding = new Padding(20, 12, 20, 20);
            tilesPanel.Size = new Size(982, 477);
            tilesPanel.TabIndex = 0;

            // placeholder
            placeholder.Dock = DockStyle.Fill;
            placeholder.Location = new Point(23, 12);
            placeholder.Name = "placeholder";
            placeholder.Size = new Size(100, 0);
            placeholder.TabIndex = 0;
            placeholder.Text = "Vista previa del panel de tiles (se llenará en runtime).";
            placeholder.TextAlign = ContentAlignment.MiddleCenter;

            tilesPanel.Controls.Add(placeholder);

            // 
            // bottom
            // 
            bottom.Controls.Add(btnLogout);
            bottom.Dock = DockStyle.Bottom;
            bottom.Location = new Point(0, 533);
            bottom.Name = "bottom";
            bottom.Padding = new Padding(8);
            bottom.Size = new Size(982, 60);
            bottom.TabIndex = 1;

            // 
            // btnLogout
            // 
            btnLogout.Dock = DockStyle.Right;
            btnLogout.Location = new Point(834, 8);
            btnLogout.Margin = new Padding(10);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(140, 44);
            btnLogout.TabIndex = 0;
            btnLogout.Text = "Cerrar sesión";
            btnLogout.Click += BtnLogout_Click;

            // 
            // MainForm
            // 
            BackColor = Color.FromArgb(240, 240, 240);
            ClientSize = new Size(982, 593);
            Controls.Add(tilesPanel);
            Controls.Add(bottom);
            Controls.Add(header);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "QuickPOS";

            header.ResumeLayout(false);
            tilesPanel.ResumeLayout(false);
            bottom.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
