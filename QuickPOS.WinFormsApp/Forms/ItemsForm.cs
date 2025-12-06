using System;
using System.Drawing;
using System.Linq; // <--- NECESARIO PARA EL FILTRO (Where)
using System.Windows.Forms;
using QuickPOS.Data;
using QuickPOS.Models;

namespace QuickPOS.WinFormsApp.Forms
{
    public partial class ItemsForm : Form
    {
        private readonly IItemRepository _repo;
        private readonly ISettingRepository _settings;
        private readonly User _user;

        // Constructor 1 (Diseñador)
        public ItemsForm()
        {
            InitializeComponent();
        }

        // Constructor 2 (Real)
        public ItemsForm(IItemRepository repo, ISettingRepository settings, User user) : this()
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _user = user ?? throw new ArgumentNullException(nameof(user));

            SetupEvents();
        }

        private void SetupEvents()
        {
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClose.Click += (s, e) => this.Close();
        }

        private void ItemsForm_Load(object sender, EventArgs e)
        {
            ConfigureGrid();
            LoadData();
            AplicarPermisos();
        }

        private void AplicarPermisos()
        {
            if (_user.Role != "Admin")
            {
                string permisoBorrar = _settings.Get("Permiso_BorrarItems");
                string permisoEditar = _settings.Get("Permiso_EditarItems");

                btnDelete.Enabled = (permisoBorrar == "True");
                btnAdd.Enabled = (permisoEditar == "True");
                btnEdit.Enabled = (permisoEditar == "True");

                if (!btnDelete.Enabled) btnDelete.BackColor = Color.LightGray;
                if (!btnAdd.Enabled)
                {
                    btnAdd.BackColor = Color.LightGray;
                    btnEdit.BackColor = Color.LightGray;
                }
            }
        }

        private void ConfigureGrid()
        {
            dgvItems.AutoGenerateColumns = false;
            dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvItems.MultiSelect = false;
            dgvItems.RowHeadersVisible = false;
            dgvItems.AllowUserToAddRows = false;
            dgvItems.ReadOnly = true;
            dgvItems.BackgroundColor = Color.White;
            dgvItems.BorderStyle = BorderStyle.None;

            dgvItems.EnableHeadersVisualStyles = false;
            dgvItems.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvItems.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(24, 30, 54);
            dgvItems.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvItems.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvItems.ColumnHeadersHeight = 40;

            dgvItems.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvItems.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);
            dgvItems.RowTemplate.Height = 35;

            if (dgvItems.Columns.Count == 0)
            {
                dgvItems.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ItemId", HeaderText = "ID", Width = 60 });
                dgvItems.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre", HeaderText = "PRODUCTO" });
                dgvItems.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Precio", HeaderText = "PRECIO", DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }, Width = 120 });
                dgvItems.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "Activo", HeaderText = "ACTIVO", Width = 80 });
            }
        }

        private void LoadData()
        {
            if (_repo == null) return;

            try
            {
                // 1. Obtener todos los productos de la BD
                var items = _repo.GetAll();

                // 2. FILTRAR SEGÚN ROL
                if (_user.Role != "Admin")
                {
                    // Si es empleado, SOLO mostramos los Activos (Activo == true)
                    items = items.Where(x => x.Activo).ToList();
                }
                // Si es Admin, no filtramos nada (ve los activos y los borrados)

                dgvItems.DataSource = items;

                // 3. ESTILO VISUAL: Pintar de gris los desactivados (Solo Admin lo verá)
                PintarInactivos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar: " + ex.Message);
            }
        }

        // Método para cambiar el color de los productos "borrados"
        private void PintarInactivos()
        {
            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                if (row.DataBoundItem is Item item && !item.Activo)
                {
                    // Letra gris y cursiva para indicar que está inactivo
                    row.DefaultCellStyle.ForeColor = Color.DarkGray;
                    row.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Italic);
                    row.DefaultCellStyle.SelectionForeColor = Color.LightGray;
                }
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            using var f = new AddEditItemForm(_repo);
            if (f.ShowDialog() == DialogResult.OK) LoadData();
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgvItems.CurrentRow?.DataBoundItem is Item item)
            {
                using var f = new AddEditItemForm(_repo, item);
                if (f.ShowDialog() == DialogResult.OK) LoadData();
            }
            else MessageBox.Show("Selecciona un producto.");
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (!btnDelete.Enabled) return;

            if (dgvItems.CurrentRow?.DataBoundItem is Item item)
            {
                // Cambiamos el mensaje dependiendo si ya está inactivo o no
                string accion = item.Activo ? "eliminar (desactivar)" : "eliminar permanentemente"; // (En realidad el repo solo desactiva, pero el mensaje ayuda)

                if (MessageBox.Show($"¿Estás seguro de {accion} '{item.Nombre}'?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        _repo.Delete(item.ItemId);
                        LoadData();

                        // Mensaje de éxito inteligente
                        if (item.Activo)
                            MessageBox.Show("Producto desactivado. Ya no aparecerá en ventas.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
                }
            }
            else MessageBox.Show("Selecciona un producto.");
        }
    }
}