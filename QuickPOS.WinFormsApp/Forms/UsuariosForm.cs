using System;
using System.Drawing;
using System.Windows.Forms;
using QuickPOS.Data;
using QuickPOS.Models;

namespace QuickPOS.WinFormsApp.Forms
{
    public partial class UsuariosForm : Form
    {
        private readonly IUsuarioRepository _repo;
        private readonly int _currentUserId; // ID del usuario conectado

        // Constructor 1 (Diseñador)
        public UsuariosForm()
        {
            InitializeComponent();
        }

        // Constructor 2 (Real - Recibe el ID del usuario actual)
        public UsuariosForm(IUsuarioRepository repo, int currentUserId) : this()
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _currentUserId = currentUserId;
            SetupEvents();
        }

        private void SetupEvents()
        {
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClose.Click += (s, e) => this.Close();
        }

        private void UsuariosForm_Load(object sender, EventArgs e)
        {
            ConfigurarGrid();
            LoadData();
        }

        private void ConfigurarGrid()
        {
            dgvUsuarios.AutoGenerateColumns = false;
            dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsuarios.ReadOnly = true;
            dgvUsuarios.BackgroundColor = Color.White;
            dgvUsuarios.BorderStyle = BorderStyle.None;
            dgvUsuarios.RowHeadersVisible = false;

            // Estilos
            dgvUsuarios.EnableHeadersVisualStyles = false;
            dgvUsuarios.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(24, 30, 54);
            dgvUsuarios.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvUsuarios.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvUsuarios.ColumnHeadersHeight = 40;
            dgvUsuarios.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvUsuarios.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);
            dgvUsuarios.RowTemplate.Height = 35;

            // Columnas
            if (dgvUsuarios.Columns.Count == 0)
            {
                dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "UsuarioId", HeaderText = "ID", Width = 60 });
                dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Username", HeaderText = "USUARIO" });
                dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Role", HeaderText = "ROL", Width = 100 });
                dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Email", HeaderText = "EMAIL", Width = 200 });
            }
        }

        private void LoadData()
        {
            if (_repo == null) return;
            try { dgvUsuarios.DataSource = _repo.GetAll(); }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            using var f = new AddEditUsuarioForm(_repo);
            if (f.ShowDialog() == DialogResult.OK) LoadData();
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgvUsuarios.CurrentRow?.DataBoundItem is Usuario u)
            {
                using var f = new AddEditUsuarioForm(_repo, u);
                if (f.ShowDialog() == DialogResult.OK) LoadData();
            }
            else MessageBox.Show("Selecciona un usuario.");
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvUsuarios.CurrentRow?.DataBoundItem is Usuario userToDelete)
            {
                // 1. SEGURIDAD: No borrar al Admin principal
                if (userToDelete.Username.ToLower() == "admin")
                {
                    MessageBox.Show("Por seguridad, el usuario principal 'admin' no puede ser eliminado.", "Acción Denegada", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                // 2. CASO ESPECIAL: Borrarse a uno mismo
                bool isSelfDelete = (userToDelete.UsuarioId == _currentUserId);

                string mensaje = isSelfDelete
                    ? "¡ADVERTENCIA! Estás a punto de eliminar TU PROPIA cuenta.\nSi continúas, se cerrará tu sesión inmediatamente."
                    : $"¿Estás seguro de eliminar al usuario '{userToDelete.Username}'?";

                if (MessageBox.Show(mensaje, "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        _repo.Delete(userToDelete.UsuarioId);

                        if (isSelfDelete)
                        {
                            MessageBox.Show("Tu cuenta ha sido eliminada. Cerrando sesión...", "Adiós", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // Usamos DialogResult.Abort como señal para el MainForm
                            this.DialogResult = DialogResult.Abort;
                            this.Close();
                        }
                        else
                        {
                            LoadData();
                            MessageBox.Show("Usuario eliminado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("FK") || ex.Message.Contains("REFERENCE"))
                            MessageBox.Show("No se puede eliminar este usuario porque tiene ventas registradas.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        else
                            MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
            else MessageBox.Show("Selecciona un usuario.");
        }
    }
}