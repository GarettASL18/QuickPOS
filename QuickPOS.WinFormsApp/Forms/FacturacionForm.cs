using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using QuickPOS.Data;
using QuickPOS.Models;
using QuickPOS.Services;

namespace QuickPOS.WinFormsApp
{
    public class FacturacionForm : Form
    {
        private readonly FacturaService _service;
        private readonly IItemRepository _itemRepo;

        private DataTable dtDetalles;
        private DataGridView gridDetalles;
        private TextBox txtCliente;
        private Label lblSubtotal, lblImpuesto, lblTotal;
        private Button btnAgregarItem, btnEliminarLinea, btnFacturar, btnCerrar;

        public FacturacionForm(FacturaService service, IItemRepository itemRepo)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _itemRepo = itemRepo ?? throw new ArgumentNullException(nameof(itemRepo));

            Text = "Facturación";
            Width = 820;
            Height = 540;
            StartPosition = FormStartPosition.CenterParent;

            Initialize();
        }

        void Initialize()
        {
            var main = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 4, ColumnCount = 1, Padding = new Padding(8) };
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 48)); // top: cliente + botones
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // grid
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 72)); // totales
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 48)); // acciones

            // Top row: cliente id + botones agregar/eliminar
            var top = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
            top.Controls.Add(new Label { Text = "ClienteId (opcional):", AutoSize = true, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, Padding = new Padding(6, 10, 0, 0) });
            txtCliente = new TextBox { Width = 100, Margin = new Padding(6) };
            top.Controls.Add(txtCliente);

            btnAgregarItem = new Button { Text = "Agregar Item", AutoSize = true, Margin = new Padding(12, 6, 6, 6) };
            btnAgregarItem.Click += BtnAgregarItem_Click;
            top.Controls.Add(btnAgregarItem);

            btnEliminarLinea = new Button { Text = "Eliminar Línea", AutoSize = true, Margin = new Padding(6) };
            btnEliminarLinea.Click += BtnEliminarLinea_Click;
            top.Controls.Add(btnEliminarLinea);

            main.Controls.Add(top, 0, 0);

            // Grid (detalle)
            gridDetalles = new DataGridView { Dock = DockStyle.Fill, AllowUserToAddRows = false, ReadOnly = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            main.Controls.Add(gridDetalles, 0, 1);

            // Totales
            var totPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(6) };
            lblTotal = new Label { Text = "Total: 0.00", AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold), Padding = new Padding(12) };
            lblImpuesto = new Label { Text = "Impuesto: 0.00", AutoSize = true, Padding = new Padding(12) };
            lblSubtotal = new Label { Text = "Subtotal: 0.00", AutoSize = true, Padding = new Padding(12) };
            totPanel.Controls.Add(lblTotal);
            totPanel.Controls.Add(lblImpuesto);
            totPanel.Controls.Add(lblSubtotal);
            main.Controls.Add(totPanel, 0, 2);

            // Acción: Facturar / Cerrar
            var actions = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            btnFacturar = new Button { Text = "Facturar", Width = 120, Height = 34, Margin = new Padding(6) };
            btnFacturar.Click += BtnFacturar_Click;
            btnCerrar = new Button { Text = "Cerrar", Width = 120, Height = 34, Margin = new Padding(6) };
            btnCerrar.Click += (_, __) => Close();
            actions.Controls.Add(btnFacturar);
            actions.Controls.Add(btnCerrar);
            main.Controls.Add(actions, 0, 3);

            Controls.Add(main);

            // DataTable para detalles
            dtDetalles = new DataTable();
            dtDetalles.Columns.Add("ItemId", typeof(int));
            dtDetalles.Columns.Add("Nombre", typeof(string));
            dtDetalles.Columns.Add("Cantidad", typeof(int));
            dtDetalles.Columns.Add("PrecioUnitario", typeof(decimal));
            dtDetalles.Columns.Add("Importe", typeof(decimal));

            gridDetalles.DataSource = dtDetalles;

            // Configurar columnas (editable la cantidad)
            gridDetalles.Columns["ItemId"].ReadOnly = true;
            gridDetalles.Columns["Nombre"].ReadOnly = true;
            gridDetalles.Columns["PrecioUnitario"].ReadOnly = true;
            gridDetalles.Columns["Importe"].ReadOnly = true;
            gridDetalles.Columns["Cantidad"].ReadOnly = false;

            // Solo permitir números en Cantidad
            gridDetalles.EditingControlShowing += GridDetalles_EditingControlShowing;
            gridDetalles.CellEndEdit += GridDetalles_CellEndEdit;

            UpdateTotales();
        }

        private void GridDetalles_EditingControlShowing(object? sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (gridDetalles.CurrentCell?.ColumnIndex == gridDetalles.Columns["Cantidad"].Index && e.Control is TextBox tb)
            {
                tb.KeyPress -= Tb_KeyPress;
                tb.KeyPress += Tb_KeyPress;
            }
        }

        private void Tb_KeyPress(object? sender, KeyPressEventArgs e)
        {
            // permitir solo dígitos y control (backspace)
            if (!(char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar)))
                e.Handled = true;
        }

        private void GridDetalles_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            // Recalcular importe cuando cambie la cantidad
            if (e.RowIndex < 0) return;
            var row = gridDetalles.Rows[e.RowIndex];
            try
            {
                var cantidad = Convert.ToInt32(row.Cells["Cantidad"].Value);
                var precio = Convert.ToDecimal(row.Cells["PrecioUnitario"].Value);
                if (cantidad <= 0) cantidad = 1;
                row.Cells["Cantidad"].Value = cantidad;
                row.Cells["Importe"].Value = Math.Round(precio * cantidad, 2);
                UpdateTotales();
            }
            catch
            {
                // si hubo error al editar, restablecer a 1
                row.Cells["Cantidad"].Value = 1;
                row.Cells["Importe"].Value = Math.Round(Convert.ToDecimal(row.Cells["PrecioUnitario"].Value) * 1, 2);
                UpdateTotales();
            }
        }

        private void BtnAgregarItem_Click(object? sender, EventArgs e)
        {
            using var pick = new PickItemForm(_itemRepo);
            if (pick.ShowDialog() == DialogResult.OK && pick.SelectedItem != null)
            {
                var it = pick.SelectedItem;
                // si ya existe la línea, solo incrementar cantidad
                var existing = dtDetalles.AsEnumerable().FirstOrDefault(r => r.Field<int>("ItemId") == it.ItemId);
                if (existing != null)
                {
                    var c = existing.Field<int>("Cantidad") + 1;
                    existing.SetField("Cantidad", c);
                    existing.SetField("Importe", Math.Round(c * existing.Field<decimal>("PrecioUnitario"), 2));
                }
                else
                {
                    var row = dtDetalles.NewRow();
                    row["ItemId"] = it.ItemId;
                    row["Nombre"] = it.Nombre;
                    row["Cantidad"] = 1;
                    row["PrecioUnitario"] = it.Precio;
                    row["Importe"] = Math.Round(it.Precio * 1, 2);
                    dtDetalles.Rows.Add(row);
                }

                UpdateTotales();
            }
        }

        private void BtnEliminarLinea_Click(object? sender, EventArgs e)
        {
            if (gridDetalles.CurrentRow == null)
            {
                MessageBox.Show("Seleccione una línea para eliminar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var dr = (gridDetalles.CurrentRow.DataBoundItem as DataRowView)?.Row;
            if (dr != null)
            {
                dtDetalles.Rows.Remove(dr);
                UpdateTotales();
            }
        }

        private void UpdateTotales()
        {
            decimal subtotal = dtDetalles.AsEnumerable().Sum(r => r.Field<decimal>("Importe"));
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
                if (dtDetalles.Rows.Count == 0)
                {
                    MessageBox.Show("No hay líneas en la factura.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int? clienteId = null;
                if (!string.IsNullOrWhiteSpace(txtCliente.Text))
                {
                    if (!int.TryParse(txtCliente.Text.Trim(), out var cid))
                    {
                        MessageBox.Show("ClienteId inválido.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    clienteId = cid;
                }

                // Construir lista de lineas (ItemId, Cantidad, PrecioUnitario)
                var lineas = dtDetalles.AsEnumerable()
                    .Select(r => (ItemId: r.Field<int>("ItemId"),
                                  Cantidad: r.Field<int>("Cantidad"),
                                  PrecioUnitario: r.Field<decimal>("PrecioUnitario")))
                    .ToList();

                // Llamar al servicio
                var id = _service.CrearFactura(clienteId, lineas);
                MessageBox.Show($"Factura creada correctamente. Id: {id}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Limpiar
                dtDetalles.Rows.Clear();
                UpdateTotales();
                this.Close();
            }
            catch (ArgumentException aex)
            {
                MessageBox.Show(aex.Message, "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (InvalidOperationException iex)
            {
                MessageBox.Show(iex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Mostrar detalle para depuración (puedes simplificar en producción)
                MessageBox.Show("Error al crear la factura:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
