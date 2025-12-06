using System;
using System.Drawing;
using System.Windows.Forms;
using QuickPOS.Data;
using QuickPOS.Models;

namespace QuickPOS.WinFormsApp.Forms
{
    public partial class AddEditClienteForm : Form
    {
        private readonly IClienteRepository _repo;
        private readonly Cliente? _clienteToEdit;

        // Controles UI
        private TextBox txtName;
        private TextBox txtNIT;
        private Button btnSave;
        private Button btnCancel;
        private Label lblTitle;

        // Constructor Crear
        public AddEditClienteForm(IClienteRepository repo) : this(repo, null) { }

        // Constructor Editar
        public AddEditClienteForm(IClienteRepository repo, Cliente? clienteToEdit)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _clienteToEdit = clienteToEdit;

            InitializeCustomComponent(); // Crear UI
            LoadData(); // Cargar datos
        }

        private void LoadData()
        {
            if (_clienteToEdit != null)
            {
                lblTitle.Text = "Editar Cliente";
                txtName.Text = _clienteToEdit.Nombre;
                txtNIT.Text = _clienteToEdit.NIT;
            }
            else
            {
                lblTitle.Text = "Nuevo Cliente";
                txtName.Focus();
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            // 1. Validación Básica
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("El nombre del cliente es requerido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            try
            {
                // Preparamos el objeto (limpiando espacios)
                string nuevoNombre = txtName.Text.Trim();
                string? nuevoNIT = string.IsNullOrWhiteSpace(txtNIT.Text) ? null : txtNIT.Text.Trim();

                if (_clienteToEdit == null)
                {
                    // --- MODO CREAR ---
                    var newCliente = new Cliente
                    {
                        Nombre = nuevoNombre,
                        NIT = nuevoNIT
                    };

                    // Tu repositorio original parece devolver void o int.
                    // Lo manejamos de forma segura:
                    _repo.Create(newCliente);

                    MessageBox.Show("Cliente creado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // --- MODO EDITAR ---
                    _clienteToEdit.Nombre = nuevoNombre;
                    _clienteToEdit.NIT = nuevoNIT;

                    _repo.Update(_clienteToEdit);

                    MessageBox.Show("Cliente actualizado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                // Aquí capturamos errores como "NIT Duplicado"
                MessageBox.Show("Error al guardar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- DISEÑO VISUAL ---
        private void InitializeCustomComponent()
        {
            this.Size = new Size(400, 320);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;
            this.Text = "Gestión de Cliente";

            // Titulo
            lblTitle = new Label
            {
                Text = "Cliente",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true,
                ForeColor = Color.MidnightBlue
            };
            this.Controls.Add(lblTitle);

            // Campo Nombre
            var lblName = new Label { Text = "Nombre Completo *", Location = new Point(25, 70), AutoSize = true, ForeColor = Color.DimGray, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            this.Controls.Add(lblName);

            txtName = new TextBox
            {
                Location = new Point(25, 90),
                Width = 330,
                Font = new Font("Segoe UI", 11)
            };
            this.Controls.Add(txtName);

            // Campo NIT
            var lblNIT = new Label { Text = "NIT / Documento", Location = new Point(25, 140), AutoSize = true, ForeColor = Color.DimGray, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            this.Controls.Add(lblNIT);

            txtNIT = new TextBox
            {
                Location = new Point(25, 160),
                Width = 330,
                Font = new Font("Segoe UI", 11),
                PlaceholderText = "Opcional"
            };
            this.Controls.Add(txtNIT);

            // Botones
            btnSave = new Button
            {
                Text = "Guardar",
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(120, 40),
                Location = new Point(140, 220),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnCancel = new Button
            {
                Text = "Cancelar",
                BackColor = Color.WhiteSmoke,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10),
                Size = new Size(100, 40),
                Location = new Point(270, 220),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }
    }
}