using System;
using System.Drawing;
using System.Windows.Forms;
using QuickPOS.Data;
using QuickPOS.Models;

namespace QuickPOS.WinFormsApp.Forms
{
    public partial class VentasForm : Form
    {
        private readonly IFacturaRepository _repo;
        private readonly ISettingRepository _settings; // <--- NUEVA DEPENDENCIA

        // Constructor 1: Diseñador
        public VentasForm()
        {
            InitializeComponent();
        }

        // Constructor 2: Real (Ahora pide Settings también)
        public VentasForm(IFacturaRepository repo, ISettingRepository settings) : this()
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            SetupEvents();
        }

        private void SetupEvents()
        {
            btnClose.Click += (s, e) => this.Close();
            btnDetalle.Click += BtnDetalle_Click;
        }

        private void VentasForm_Load(object sender, EventArgs e)
        {
            ConfigurarGrid();
            CargarDatos();
        }

        private void ConfigurarGrid()
        {
            // Estilo General
            dgvVentas.AutoGenerateColumns = false;
            dgvVentas.BackgroundColor = Color.White;
            dgvVentas.BorderStyle = BorderStyle.None;
            dgvVentas.RowHeadersVisible = false;
            dgvVentas.AllowUserToAddRows = false;
            dgvVentas.ReadOnly = true;
            dgvVentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvVentas.MultiSelect = false;

            // Header
            dgvVentas.EnableHeadersVisualStyles = false;
            dgvVentas.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvVentas.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(24, 30, 54);
            dgvVentas.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvVentas.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvVentas.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvVentas.ColumnHeadersHeight = 45;

            // Filas
            dgvVentas.RowTemplate.Height = 40;
            dgvVentas.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);
            dgvVentas.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvVentas.DefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64);
            dgvVentas.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvVentas.GridColor = Color.WhiteSmoke;

            // Columnas
            dgvVentas.Columns.Clear();

            dgvVentas.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "FacturaId",
                HeaderText = "ID",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvVentas.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Fecha",
                HeaderText = "FECHA Y HORA",
                Width = 180,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy hh:mm tt" }
            });

            dgvVentas.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NombreCliente",
                HeaderText = "CLIENTE",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dgvVentas.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Total",
                HeaderText = "TOTAL",
                Width = 140,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 120, 215)
                }
            });
        }

        private void CargarDatos()
        {
            if (_repo == null) return;
            try
            {
                var ventas = _repo.GetAll();
                dgvVentas.DataSource = ventas;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando historial: " + ex.Message);
            }
        }

        private void BtnDetalle_Click(object? sender, EventArgs e)
        {
            if (dgvVentas.CurrentRow?.DataBoundItem is Factura factura)
            {
                // AQUÍ PASAMOS EL SETTINGS AL DETALLE (Para que el PDF sepa el nombre de la empresa)
                using var f = new DetalleVentaForm(_repo, _settings, factura.FacturaId, factura.NombreCliente);
                f.ShowDialog();
            }
            else
            {
                MessageBox.Show("Por favor, selecciona una venta de la lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}