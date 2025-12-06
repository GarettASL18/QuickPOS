using System;
using System.Drawing;
using System.Windows.Forms;
using QuickPOS.Data;
using QuickPOS.Models;

namespace QuickPOS.WinFormsApp.Forms
{
    public partial class AddEditUsuarioForm : Form
    {
        private readonly IUsuarioRepository _repo;
        private readonly Usuario? _usuarioToEdit;

        private TextBox txtUser, txtPass, txtEmail;
        private ComboBox cmbRole;
        private Button btnSave, btnCancel;
        private Label lblTitle;

        public AddEditUsuarioForm(IUsuarioRepository repo, Usuario? u = null)
        {
            _repo = repo;
            _usuarioToEdit = u;
            InitializeCustomComponent();
            LoadData();
        }

        private void LoadData()
        {
            if (_usuarioToEdit != null)
            {
                lblTitle.Text = "Editar Usuario";
                txtUser.Text = _usuarioToEdit.Username;
                txtUser.Enabled = false; // No permitir cambiar el username (id único)
                txtEmail.Text = _usuarioToEdit.Email;
                cmbRole.SelectedItem = _usuarioToEdit.Role;
                txtPass.PlaceholderText = "(Dejar en blanco para no cambiar)";
            }
            else
            {
                lblTitle.Text = "Nuevo Usuario";
                cmbRole.SelectedIndex = 1; // Employee por defecto
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUser.Text)) { MessageBox.Show("Usuario requerido"); return; }
            if (_usuarioToEdit == null && string.IsNullOrWhiteSpace(txtPass.Text)) { MessageBox.Show("Contraseña requerida"); return; }

            try
            {
                if (_usuarioToEdit == null)
                {
                    var u = new Usuario
                    {
                        Username = txtUser.Text.Trim(),
                        PasswordHash = txtPass.Text.Trim(), // En producción usar Hash
                        Role = cmbRole.SelectedItem.ToString(),
                        Email = txtEmail.Text.Trim()
                    };
                    _repo.Create(u);
                }
                else
                {
                    _usuarioToEdit.Role = cmbRole.SelectedItem.ToString();
                    _usuarioToEdit.Email = txtEmail.Text.Trim();
                    if (!string.IsNullOrWhiteSpace(txtPass.Text))
                        _usuarioToEdit.PasswordHash = txtPass.Text.Trim();

                    _repo.Update(_usuarioToEdit);
                }
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void InitializeCustomComponent()
        {
            this.Size = new Size(400, 420);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            lblTitle = new Label { Text = "Usuario", Font = new Font("Segoe UI", 16, FontStyle.Bold), Location = new Point(20, 20), AutoSize = true, ForeColor = Color.MidnightBlue };
            this.Controls.Add(lblTitle);

            // Usuario
            this.Controls.Add(new Label { Text = "Nombre de Usuario *", Location = new Point(25, 70), AutoSize = true });
            txtUser = new TextBox { Location = new Point(25, 90), Width = 330, Font = new Font("Segoe UI", 11) };
            this.Controls.Add(txtUser);

            // Rol
            this.Controls.Add(new Label { Text = "Rol", Location = new Point(25, 130), AutoSize = true });
            cmbRole = new ComboBox { Location = new Point(25, 150), Width = 330, Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRole.Items.AddRange(new object[] { "Admin", "Employee" });
            this.Controls.Add(cmbRole);

            // Email
            this.Controls.Add(new Label { Text = "Correo (Recuperación)", Location = new Point(25, 190), AutoSize = true });
            txtEmail = new TextBox { Location = new Point(25, 210), Width = 330, Font = new Font("Segoe UI", 11) };
            this.Controls.Add(txtEmail);

            // Password
            this.Controls.Add(new Label { Text = "Contraseña", Location = new Point(25, 250), AutoSize = true });
            txtPass = new TextBox { Location = new Point(25, 270), Width = 330, Font = new Font("Segoe UI", 11), UseSystemPasswordChar = true };
            this.Controls.Add(txtPass);

            // Botones
            btnSave = new Button { Text = "Guardar", BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(120, 40), Location = new Point(140, 320) };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnCancel = new Button { Text = "Cancelar", BackColor = Color.WhiteSmoke, FlatStyle = FlatStyle.Flat, Size = new Size(100, 40), Location = new Point(270, 320) };
            btnCancel.Click += (s, e) => Close();
            this.Controls.Add(btnCancel);
        }
    }
}