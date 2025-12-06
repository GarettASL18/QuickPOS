using System;
using System.Drawing;
using System.Windows.Forms;
using QuickPOS.Data;
using QuickPOS.Models;

namespace QuickPOS.WinFormsApp.Forms
{
    public partial class ClientesForm : Form
    {
        private readonly IClienteRepository _repo;

        // Constructor 1: Para el Diseñador Visual
        public ClientesForm()
        {
            InitializeComponent();
            SetupEvents(); // Conectar botones
        }

        // Constructor 2: Para el Programa (Runtime)
        public ClientesForm(IClienteRepository repo) : this()
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        private void SetupEvents()
        {
            // Conexión manual de eventos
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClose.Click += (s, e) => this.Close();
        }

        private void ClientesForm_Load(object sender, EventArgs e)
        {
            ConfigureGrid(); // Estilo de la tabla
            LoadData();      // Cargar datos
        }

        private void ConfigureGrid()
        {
            // Configuración Visual de la Tabla (Estilo Moderno)
            dgvClientes.AutoGenerateColumns = false;
            dgvClientes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClientes.MultiSelect = false;
            dgvClientes.RowHeadersVisible = false;
            dgvClientes.AllowUserToAddRows = false;
            dgvClientes.AllowUserToResizeRows = false;
            dgvClientes.ReadOnly = true;
            dgvClientes.BackgroundColor = Color.White;
            dgvClientes.BorderStyle = BorderStyle.None;

            // Encabezado Azul
            dgvClientes.EnableHeadersVisualStyles = false;
            dgvClientes.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvClientes.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(24, 30, 54);
            dgvClientes.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvClientes.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvClientes.ColumnHeadersHeight = 40;

            // Filas
            dgvClientes.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvClientes.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 230, 255);
            dgvClientes.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvClientes.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);
            dgvClientes.RowTemplate.Height = 35;

            // Definir Columnas si no existen
            if (dgvClientes.Columns.Count == 0)
            {
                dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ClienteId", HeaderText = "ID", Width = 60 });
                dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre", HeaderText = "NOMBRE COMPLETO" });
                dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NIT", HeaderText = "NIT / DOCUMENTO", Width = 150 });
            }
        }

        private void LoadData()
        {
            if (_repo == null) return; // Modo diseño
            try
            {
                var clientes = _repo.GetAll();
                dgvClientes.DataSource = clientes;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando clientes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- BOTONES DE ACCIÓN ---

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            // Abre el formulario pequeño para crear
            using var f = new AddEditClienteForm(_repo);
            if (f.ShowDialog() == DialogResult.OK)
            {
                LoadData(); // Recargar tabla
            }
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgvClientes.CurrentRow?.DataBoundItem is Cliente cliente)
            {
                // Abre el formulario pequeño para editar (pasando el cliente)
                using var f = new AddEditClienteForm(_repo, cliente);
                if (f.ShowDialog() == DialogResult.OK)
                {
                    LoadData(); // Recargar tabla
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un cliente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvClientes.CurrentRow?.DataBoundItem is Cliente cliente)
            {
                if (MessageBox.Show($"¿Estás seguro de eliminar a '{cliente.Nombre}'?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        _repo.Delete(cliente.ClienteId);
                        LoadData();
                        MessageBox.Show("Cliente eliminado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        // Manejo amigable de error por Clave Foránea (Si el cliente tiene facturas)
                        if (ex.Message.Contains("FK") || ex.Message.Contains("REFERENCE"))
                        {
                            MessageBox.Show("No se puede eliminar este cliente porque ya tiene ventas registradas en el historial.", "Protección de datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Selecciona un cliente para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}