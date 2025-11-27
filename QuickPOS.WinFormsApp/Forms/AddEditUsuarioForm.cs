using System;
using System.Windows.Forms;
using QuickPOS.Data;
using QuickPOS.Models;

namespace QuickPOS.WinFormsApp
{
    public class AddEditUsuarioForm : Form
    {
        private readonly IUsuarioRepository _repo;
        private readonly Usuario? _usuario;
        TextBox txtUser, txtPass;
        ComboBox cbRole;
        Button btnSave, btnCancel;

        public AddEditUsuarioForm(IUsuarioRepository repo, Usuario? usuario = null)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _usuario = usuario;
            Text = usuario == null ? "Agregar Usuario" : "Editar Usuario";
            Width = 420; Height = 220; StartPosition = FormStartPosition.CenterParent;
            Initialize();
        }

        void Initialize()
        {
            var tl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3, Padding = new Padding(8) };
            tl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            tl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));

            tl.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            tl.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            tl.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

            tl.Controls.Add(new Label { Text = "Usuario:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 0);
            txtUser = new TextBox { Dock = DockStyle.Fill }; tl.Controls.Add(txtUser, 1, 0);

            tl.Controls.Add(new Label { Text = "Contraseña:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 1);
            txtPass = new TextBox { Dock = DockStyle.Fill }; tl.Controls.Add(txtPass, 1, 1);

            tl.Controls.Add(new Label { Text = "Rol:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 2);
            cbRole = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cbRole.Items.AddRange(new[] { "Admin", "Employee" }); cbRole.SelectedIndex = 1;
            tl.Controls.Add(cbRole, 1, 2);

            var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, Height = 40, Padding = new Padding(8) };
            btnSave = new Button { Text = "Guardar" }; btnSave.Click += BtnSave_Click;
            btnCancel = new Button { Text = "Cancelar" }; btnCancel.Click += (_, __) => { DialogResult = DialogResult.Cancel; Close(); };
            btnPanel.Controls.Add(btnSave); btnPanel.Controls.Add(btnCancel);

            Controls.Add(tl); Controls.Add(btnPanel);

            if (_usuario != null)
            {
                txtUser.Text = _usuario.Username;
                txtPass.Text = _usuario.PasswordHash;
                cbRole.SelectedItem = _usuario.Role;
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUser.Text) || string.IsNullOrWhiteSpace(txtPass.Text))
            {
                MessageBox.Show("Usuario y contraseña requeridos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var u = new Usuario
            {
                UsuarioId = _usuario?.UsuarioId ?? 0,
                Username = txtUser.Text.Trim(),
                PasswordHash = txtPass.Text.Trim(), // ideal: hash
                Role = cbRole.SelectedItem?.ToString() ?? "Employee"
            };

            try
            {
                if (_usuario == null) _repo.Create(u);
                else _repo.Update(u);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error guardando usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
