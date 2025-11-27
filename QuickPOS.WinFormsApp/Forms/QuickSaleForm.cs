using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using QuickPOS.Services;
using QuickPOS.Data;
using QuickPOS.Models;

namespace QuickPOS.WinFormsApp
{
    public class QuickSaleForm : Form
    {
        private readonly FacturaService _service;
        private readonly IItemRepository _itemRepo;
        private readonly IClienteRepository _clienteRepo;

        private DataTable dt;
        private DataGridView grid;
        private Button btnAdd, btnFacturar, btnClose;
        private Label lblSubtotal, lblImpuesto, lblTotal;

        public QuickSaleForm(FacturaService service, IItemRepository itemRepo, IClienteRepository clienteRepo)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _itemRepo = itemRepo ?? throw new ArgumentNullException(nameof(itemRepo));
            _clienteRepo = clienteRepo ?? throw new ArgumentNullException(nameof(clienteRepo));

            Text = "Venta rápida";
            Width = 780;
            Height = 520;
            StartPosition = FormStartPosition.CenterParent;

            Initialize();
        }

        void Initialize()
        {
            var tl = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, Padding = new Padding(8) };
            tl.RowStyles.Add(new RowStyle(SizeType.Absolute, 48)); // top controls
            tl.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // grid
            tl.RowStyles.Add(new RowStyle(SizeType.Absolute, 72)); // totals + actions

            // TOP
            var top = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, Padding = new Padding(4) };
            btnAdd = new Button { Text = "Agregar Item", AutoSize = true }; btnAdd.Click += BtnAdd_Click;
            top.Controls.Add(btnAdd);
            tl.Controls.Add(top, 0, 0);

            // GRID
            dt = new DataTable();
            dt.Columns.Add("ItemId", typeof(int));
            dt.Columns.Add("Nombre", typeof(string));
            dt.Columns.Add("Cantidad", typeof(int));
            dt.Columns.Add("Precio", typeof(decimal));
            dt.Columns.Add("Importe", typeof(decimal));

            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                DataSource = dt,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = false,
                AutoGenerateColumns = false
            };

            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ItemId", HeaderText = "Id", Width = 60, ReadOnly = true });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre", HeaderText = "Nombre", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, ReadOnly = true });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Cantidad", HeaderText = "Cantidad", Width = 90, ReadOnly = false });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Precio", HeaderText = "Precio", Width = 120, ReadOnly = true, DefaultCellStyle = { Format = "0.00" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Importe", HeaderText = "Importe", Width = 120, ReadOnly = true, DefaultCellStyle = { Format = "0.00" } });

            grid.CellEndEdit += Grid_CellEndEdit;
            tl.Controls.Add(grid, 0, 1);

            // Totals + actions row
            var bottom = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
            bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            var totPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(6) };
            lblTotal = new Label { Text = "Total: 0.00", AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold), Padding = new Padding(8) };
            lblImpuesto = new Label { Text = "Impuesto: 0.00", AutoSize = true, Padding = new Padding(8) };
            lblSubtotal = new Label { Text = "Subtotal: 0.00", AutoSize = true, Padding = new Padding(8) };
            totPanel.Controls.Add(lblTotal);
            totPanel.Controls.Add(lblImpuesto);
            totPanel.Controls.Add(lblSubtotal);
            bottom.Controls.Add(totPanel, 0, 0);

            var actions = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8) };
            btnFacturar = new Button { Text = "Facturar", Width = 120 }; btnFacturar.Click += BtnFacturar_Click;
            btnClose = new Button { Text = "Cerrar", Width = 120 }; btnClose.Click += (_, __) => Close();
            actions.Controls.Add(btnFacturar);
            actions.Controls.Add(btnClose);
            bottom.Controls.Add(actions, 1, 0);

            tl.Controls.Add(bottom, 0, 2);

            Controls.Add(tl);
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            using var pick = new PickItemForm(_itemRepo);
            if (pick.ShowDialog() == DialogResult.OK && pick.SelectedItem != null)
            {
                var it = pick.SelectedItem;
                var existing = dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("ItemId") == it.ItemId);
                if (existing != null)
                {
                    var c = existing.Field<int>("Cantidad") + 1;
                    existing.SetField("Cantidad", c);
                    existing.SetField("Importe", Math.Round(c * existing.Field<decimal>("Precio"), 2));
                }
                else
                {
                    var row = dt.NewRow();
                    row["ItemId"] = it.ItemId;
                    row["Nombre"] = it.Nombre;
                    row["Cantidad"] = 1;
                    row["Precio"] = it.Precio;
                    row["Importe"] = Math.Round(it.Precio * 1, 2);
                    dt.Rows.Add(row);
                }
                UpdateTotals();
            }
        }

        private void Grid_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = grid.Rows[e.RowIndex];
            try
            {
                var cantidad = Convert.ToInt32(row.Cells["Cantidad"].Value ?? 1);
                if (cantidad <= 0) cantidad = 1;
                row.Cells["Cantidad"].Value = cantidad;
                var precio = Convert.ToDecimal(row.Cells["Precio"].Value ?? 0m);
                row.Cells["Importe"].Value = Math.Round(cantidad * precio, 2);
            }
            catch
            {
                row.Cells["Cantidad"].Value = 1;
                row.Cells["Importe"].Value = Math.Round(Convert.ToDecimal(row.Cells["Precio"].Value ?? 0m) * 1, 2);
            }
            UpdateTotals();
        }

        private void UpdateTotals()
        {
            decimal subtotal = 0m;
            try { subtotal = dt.AsEnumerable().Sum(r => r.Field<decimal>("Importe")); } catch { subtotal = 0m; }
            decimal impuesto = Math.Round(subtotal * Config.Impuesto, 2, MidpointRounding.AwayFromZero);
            decimal total = Math.Round(subtotal + impuesto, 2, MidpointRounding.AwayFromZero);
            lblSubtotal.Text = $"Subtotal: {subtotal:0.00}";
            lblImpuesto.Text = $"Impuesto: {impuesto:0.00}";
            lblTotal.Text = $"Total: {total:0.00}";
        }

        private void BtnFacturar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No hay líneas en la venta.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Siempre venta rápida: ClienteId = NULL (Consumidor Final)
                int? clienteId = null;

                var lineas = dt.AsEnumerable()
                    .Select(r => (ItemId: r.Field<int>("ItemId"), Cantidad: r.Field<int>("Cantidad"), PrecioUnitario: r.Field<decimal>("Precio")))
                    .ToList();

                var facturaId = _service.CrearFactura(clienteId, lineas);

                MessageBox.Show($"Venta registrada. Factura #{facturaId}", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar venta:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
