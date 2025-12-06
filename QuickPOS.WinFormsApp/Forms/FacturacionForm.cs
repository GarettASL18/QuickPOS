using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QuickPOS.Data;
using QuickPOS.Models;
using QuickPOS.Services;

namespace QuickPOS.WinFormsApp.Forms
{
    public partial class FacturacionForm : Form
    {
        private readonly FacturaService _service;
        private readonly IItemRepository _itemRepo;
        private readonly IClienteRepository _clienteRepo;

        // Carrito en memoria
        private List<FacturaDetalle> _carrito = new List<FacturaDetalle>();
        private List<Item> _todosLosProductos = new List<Item>();

        public FacturacionForm(FacturaService service, IItemRepository itemRepo, IClienteRepository clienteRepo)
        {
            InitializeComponent();
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _itemRepo = itemRepo ?? throw new ArgumentNullException(nameof(itemRepo));
            _clienteRepo = clienteRepo ?? throw new ArgumentNullException(nameof(clienteRepo));

            SetupEvents();
        }

        private void SetupEvents()
        {
            this.Load += FacturacionForm_Load;
            btnCerrar.Click += (s, e) => this.Close();
            btnCobrar.Click += BtnCobrar_Click;

            // --- EVENTOS DEL CATÁLOGO ---
            txtBuscar.TextChanged += (s, e) => FiltrarCatalogo(txtBuscar.Text);

            dgvProductos.CellDoubleClick += (s, e) => {
                if (e.RowIndex >= 0 && dgvProductos.CurrentRow?.DataBoundItem is Item item)
                    AgregarAlCarrito(item);
            };

            // --- EVENTOS DEL CARRITO ---
            dgvCarrito.CellEndEdit += DgvCarrito_CellEndEdit;
            dgvCarrito.CellDoubleClick += DgvCarrito_CellDoubleClick;
        }

        private void FacturacionForm_Load(object sender, EventArgs e)
        {
            ConfigurarGrids(); // Configura las columnas ANTES de cargar datos
            CargarDatos();
            txtBuscar.Focus();
        }

        private void CargarDatos()
        {
            try
            {
                // Cargar Clientes
                cmbClientes.DataSource = _clienteRepo.GetAll();
                cmbClientes.DisplayMember = "Nombre";
                cmbClientes.ValueMember = "ClienteId";
                cmbClientes.SelectedIndex = -1;

                // Cargar Productos en memoria para búsqueda rápida
                _todosLosProductos = _itemRepo.GetAll().Where(x => x.Activo).ToList();
                dgvProductos.DataSource = _todosLosProductos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando datos: " + ex.Message);
            }
        }

        private void ConfigurarGrids()
        {
            // =========================================================
            // 1. GRID CATÁLOGO (Izquierda)
            // =========================================================
            dgvProductos.AutoGenerateColumns = false;
            dgvProductos.BackgroundColor = Color.White;
            dgvProductos.BorderStyle = BorderStyle.None;
            dgvProductos.RowHeadersVisible = false;
            dgvProductos.ReadOnly = true;
            dgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductos.EnableHeadersVisualStyles = false;

            // Estilo Header
            dgvProductos.ColumnHeadersDefaultCellStyle.BackColor = Color.Gray;
            dgvProductos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvProductos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvProductos.ColumnHeadersHeight = 35;

            dgvProductos.Columns.Clear();
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre", HeaderText = "PRODUCTO", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Precio", HeaderText = "$", Width = 70, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight } });


            // =========================================================
            // 2. GRID CARRITO (Centro) - AJUSTADO PARA NO DEFORMARSE
            // =========================================================
            dgvCarrito.AutoGenerateColumns = false;
            dgvCarrito.BackgroundColor = Color.White;
            dgvCarrito.BorderStyle = BorderStyle.None;
            dgvCarrito.RowHeadersVisible = false;
            dgvCarrito.AllowUserToAddRows = false;
            dgvCarrito.EnableHeadersVisualStyles = false;

            // Estilo Header Carrito
            dgvCarrito.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(24, 30, 54);
            dgvCarrito.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCarrito.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvCarrito.ColumnHeadersHeight = 40;
            dgvCarrito.RowTemplate.Height = 35;

            dgvCarrito.Columns.Clear();

            // Columna 1: Descripción (Se estira, pero con mínimo para no desaparecer)
            var colDesc = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NombreProducto",
                HeaderText = "DESCRIPCIÓN",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 150,
                ReadOnly = true
            };
            dgvCarrito.Columns.Add(colDesc);

            // Columna 2: Precio (Ancho Fijo)
            dgvCarrito.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "PrecioUnitario",
                HeaderText = "PRECIO",
                Width = 85,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight },
                ReadOnly = true
            });

            // Columna 3: Cantidad (Ancho Fijo y Editable)
            var colCant = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Cantidad",
                HeaderText = "CANT",
                Width = 60,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, BackColor = Color.FromArgb(255, 255, 220) } // Amarillito
            };
            dgvCarrito.Columns.Add(colCant);

            // Columna 4: Total (Ancho Fijo)
            dgvCarrito.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TotalLinea",
                HeaderText = "TOTAL",
                Width = 95,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight, Font = new Font("Segoe UI", 9, FontStyle.Bold) },
                ReadOnly = true
            });
        }

        // --- LÓGICA ---

        private void FiltrarCatalogo(string filtro)
        {
            if (string.IsNullOrWhiteSpace(filtro))
                dgvProductos.DataSource = _todosLosProductos;
            else
                dgvProductos.DataSource = _todosLosProductos.Where(p => p.Nombre.Contains(filtro, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        private void AgregarAlCarrito(Item item)
        {
            var existente = _carrito.FirstOrDefault(x => x.ItemId == item.ItemId);
            if (existente != null)
            {
                existente.Cantidad++;
            }
            else
            {
                _carrito.Add(new FacturaDetalle
                {
                    ItemId = item.ItemId,
                    NombreProducto = item.Nombre,
                    PrecioUnitario = item.Precio,
                    Cantidad = 1
                });
            }
            RefrescarCarrito();
        }

        private void DgvCarrito_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Recalcular al editar cantidad
            RefrescarCarrito(rebind: false);
            dgvCarrito.Refresh();
        }

        private void DgvCarrito_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvCarrito.Rows[e.RowIndex].DataBoundItem is FacturaDetalle item)
            {
                if (MessageBox.Show($"¿Quitar {item.NombreProducto}?", "Quitar", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _carrito.Remove(item);
                    RefrescarCarrito();
                }
            }
        }

        private void RefrescarCarrito(bool rebind = true)
        {
            if (rebind)
            {
                dgvCarrito.DataSource = null;
                dgvCarrito.DataSource = _carrito;
            }

            decimal subtotal = _carrito.Sum(x => x.TotalLinea);
            decimal impuesto = subtotal * 0.15m;
            decimal total = subtotal + impuesto;

            // Formato de moneda local (C$)
            lblSubtotal.Text = subtotal.ToString("C2");
            lblImpuesto.Text = impuesto.ToString("C2");
            lblTotal.Text = total.ToString("C2");
        }

        private void BtnCobrar_Click(object sender, EventArgs e)
        {
            if (_carrito.Count == 0) { MessageBox.Show("Carrito vacío."); return; }
            if (cmbClientes.SelectedValue == null) { MessageBox.Show("Seleccione cliente."); return; }

            try
            {
                var factura = new Factura
                {
                    ClienteId = (int)cmbClientes.SelectedValue,
                    Fecha = DateTime.Now,
                    Subtotal = _carrito.Sum(x => x.TotalLinea),
                    Impuesto = _carrito.Sum(x => x.TotalLinea) * 0.15m,
                    Total = _carrito.Sum(x => x.TotalLinea) * 1.15m,
                    Detalles = _carrito
                };

                _service.CrearFactura(factura);
                MessageBox.Show("¡Venta Exitosa!", "POS", MessageBoxButtons.OK, MessageBoxIcon.Information);

                _carrito.Clear();
                RefrescarCarrito();
                txtBuscar.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}