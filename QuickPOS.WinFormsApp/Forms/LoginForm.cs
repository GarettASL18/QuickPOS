using System;
using System.Windows.Forms;
using QuickPOS.Services;
using QuickPOS.Models;

namespace QuickPOS.WinFormsApp
{
    public class LoginForm : Form
    {
        private readonly AuthService _auth;
        public User? AuthenticatedUser { get; private set; }

        TextBox txtUser, txtPass;
        Button btnLogin, btnCancel;

        public LoginForm(AuthService auth)
        {
            _auth = auth ?? throw new ArgumentNullException(nameof(auth));
            Text = "QuickPOS - Login";
            Size = new System.Drawing.Size(380, 200);
            StartPosition = FormStartPosition.CenterScreen;

            var pnl = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), ColumnCount = 2, RowCount = 3 };
            pnl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            pnl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));

            pnl.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
            pnl.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
            pnl.RowStyles.Add(new RowStyle(SizeType.Percent, 34));

            pnl.Controls.Add(new Label { Text = "Usuario:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 0);
            txtUser = new TextBox { Dock = DockStyle.Fill }; pnl.Controls.Add(txtUser, 1, 0);

            pnl.Controls.Add(new Label { Text = "Contraseña:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 1);
            txtPass = new TextBox { Dock = DockStyle.Fill, UseSystemPasswordChar = true }; pnl.Controls.Add(txtPass, 1, 1);

            var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            btnLogin = new Button { Text = "Iniciar sesión", AutoSize = true }; btnLogin.Click += BtnLogin_Click; btnPanel.Controls.Add(btnLogin);
            btnCancel = new Button { Text = "Cancelar", AutoSize = true }; btnCancel.Click += (_, __) => { DialogResult = DialogResult.Cancel; Close(); }; btnPanel.Controls.Add(btnCancel);
            pnl.Controls.Add(btnPanel, 0, 2); pnl.SetColumnSpan(btnPanel, 2);

            Controls.Add(pnl);
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            var user = _auth.Authenticate(txtUser.Text?.Trim() ?? string.Empty, txtPass.Text ?? string.Empty);
            if (user == null)
            {
                MessageBox.Show("Credenciales inválidas", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            AuthenticatedUser = user;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
