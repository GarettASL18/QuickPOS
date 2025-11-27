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
        private readonly IClienteRepository _clienteRepo;

        private DataTable dtDetalles;
        private DataGridView gridDetalles;
        private TextBox txtClienteNombre, txtClienteNIT;
        private Label lblSubtotal, lblImpuesto, lblTotal;
        private Button btnAgregarItem, btnEliminarLinea, btnFacturar, btnCerrar;

        public FacturacionForm(FacturaService service, IItemRepository itemRepo, IClienteRepository clienteRepo)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _itemRepo = itemRepo ?? throw new ArgumentNullException(nameof(itemRepo));
            _clienteRepo = clienteRepo ?? throw new ArgumentNullException(nameof(clienteRepo));

            Text = "Facturación";
            Width = 880;
            Height = 600;
            StartPosition = FormStartPosition.CenterParent;

            Initialize();
        }

        void Initialize()
        {
            var main = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 4, ColumnCount = 1, Padding = new Padding(8) };
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 72)); // top: cliente + botones
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // grid
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 72)); // totales
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 56)); // acciones

            // TOP PANEL: columna IZQ flexible (100%) + columna DER autosize (botones)
            var top = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(6),
                AutoSize = false
            };

            // Column styles: izquierda = 100% (flexible), derecha = AutoSize (solo lo necesario para botones)
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            top.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            // ---- LEFT: cliente + NIT (usar un TableLayoutPanel para que los TextBox se anclen bien) ----
            var leftInner = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                AutoSize = false
            };
            // columnas: label, textbox (flexible), label, textbox (fija)
            leftInner.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Label Cliente
            leftInner.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F)); // TextBox Cliente (flexible)
            leftInner.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Label NIT
            leftInner.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F)); // TextBox NIT (flexible)

            // Label Cliente
            leftInner.Controls.Add(new Label { Text = "Cliente (opcional):", AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Top }, 0, 0);

            // TextBox Cliente: anclable para reducir ancho en ventanas pequeñas
            txtClienteNombre = new TextBox { Margin = new Padding(6, 3, 6, 3), Dock = DockStyle.Fill };
            leftInner.Controls.Add(txtClienteNombre, 1, 0);

            // Label NIT
            leftInner.Controls.Add(new Label { Text = "NIT (opcional):", AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Top }, 2, 0);

            // TextBox NIT
            txtClienteNIT = new TextBox { Margin = new Padding(6, 3, 6, 3), Dock = DockStyle.Fill };
            leftInner.Controls.Add(txtClienteNIT, 3, 0);

            // ---- RIGHT: botones en un FlowLayoutPanel autosize y anclado a la derecha ----
            var rightPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Dock = DockStyle.Fill, // el TableLayout cuidará el AutoSize de la columna
                Padding = new Padding(6, 3, 6, 3)
            };

            // Botones: tamaño moderado y con MinimumSize para que no desaparezcan
            btnAgregarItem = new Button { Text = "Agregar Ítem", AutoSize = true, Height = 32, MinimumSize = new System.Drawing.Size(90, 32), Margin = new Padding(6, 3, 6, 3) };
            btnAgregarItem.Click += BtnAgregarItem_Click;

            btnEliminarLinea = new Button { Text = "Eliminar Línea", AutoSize = true, Height = 32, MinimumSize = new System.Drawing.Size(100, 32), Margin = new Padding(6, 3, 6, 3) };
            btnEliminarLinea.Click += BtnEliminarLinea_Click;

            rightPanel.Controls.Add(btnAgregarItem);
            rightPanel.Controls.Add(btnEliminarLinea);

            // Añadir a top
            top.Controls.Add(leftInner, 0, 0);
            top.Controls.Add(rightPanel, 1, 0);

            // Alineación: forzar que el panel derecho quede a la derecha
            top.SetColumnSpan(leftInner, 1);
            rightPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            main.Controls.Add(top, 0, 0);



            // Grid (detalle)
            gridDetalles = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = System.Drawing.SystemColors.Window,      // fondo blanco en lugar de gris
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToResizeRows = false,
                EnableHeadersVisualStyles = false
            };

            // Estilo de cabecera
            gridDetalles.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridDetalles.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            // Asignar datasource AFTER crear dtDetalles (se hace abajo en tu código),
            // pero aquí te dejo settings generales. Si quieres crear columnas manuales usa:
            // gridDetalles.AutoGenerateColumns = false; y agrega DataGridViewTextBoxColumn para controlar headers/formatos.
            // En tu implementación actual dejamos AutoGenerateColumns = true y ajustamos formato después de setear el DataSource.

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
            btnFacturar = new Button { Text = "Facturar", Width = 140, Height = 36, Margin = new Padding(6) };
            btnFacturar.Click += BtnFacturar_Click;
            btnCerrar = new Button { Text = "Cerrar", Width = 120, Height = 36, Margin = new Padding(6) };
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

            // Después de asignar DataSource:
            // Ajustes de columnas y formato
            if (gridDetalles.Columns.Contains("ItemId"))
            {
                gridDetalles.Columns["ItemId"].HeaderText = "ItemId";
                gridDetalles.Columns["ItemId"].ReadOnly = true;
                gridDetalles.Columns["ItemId"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            if (gridDetalles.Columns.Contains("Nombre"))
            {
                gridDetalles.Columns["Nombre"].HeaderText = "Nombre";
                gridDetalles.Columns["Nombre"].ReadOnly = true;
            }
            if (gridDetalles.Columns.Contains("Cantidad"))
            {
                gridDetalles.Columns["Cantidad"].HeaderText = "Cantidad";
                gridDetalles.Columns["Cantidad"].ReadOnly = false;
                gridDetalles.Columns["Cantidad"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            if (gridDetalles.Columns.Contains("PrecioUnitario"))
            {
                gridDetalles.Columns["PrecioUnitario"].HeaderText = "PrecioUnitario";
                gridDetalles.Columns["PrecioUnitario"].ReadOnly = true;
                gridDetalles.Columns["PrecioUnitario"].DefaultCellStyle.Format = "0.00";
                gridDetalles.Columns["PrecioUnitario"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            if (gridDetalles.Columns.Contains("Importe"))
            {
                gridDetalles.Columns["Importe"].HeaderText = "Importe";
                gridDetalles.Columns["Importe"].ReadOnly = true;
                gridDetalles.Columns["Importe"].DefaultCellStyle.Format = "0.00";
                gridDetalles.Columns["Importe"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

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
            if (!(char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar)))
                e.Handled = true;
        }

        private void GridDetalles_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = gridDetalles.Rows[e.RowIndex];
            try
            {
                var cantidad = Convert.ToInt32(row.Cells["Cantidad"].Value ?? 1);
                var precio = Convert.ToDecimal(row.Cells["PrecioUnitario"].Value ?? 0m);
                if (cantidad <= 0) cantidad = 1;
                row.Cells["Cantidad"].Value = cantidad;
                row.Cells["Importe"].Value = Math.Round(precio * cantidad, 2);
                UpdateTotales();
            }
            catch
            {
                row.Cells["Cantidad"].Value = 1;
                row.Cells["Importe"].Value = Math.Round(Convert.ToDecimal(row.Cells["PrecioUnitario"].Value ?? 0m) * 1, 2);
                UpdateTotales();
            }
        }

        private void BtnAgregarItem_Click(object? sender, EventArgs e)
        {
            using var pick = new PickItemForm(_itemRepo);
            if (pick.ShowDialog() == DialogResult.OK && pick.SelectedItem != null)
            {
                var it = pick.SelectedItem;
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
                var nombre = txtClienteNombre.Text?.Trim();
                var nit = txtClienteNIT.Text?.Trim();

                if (!string.IsNullOrWhiteSpace(nombre))
                {
                    var cliente = new Cliente { Nombre = nombre, NIT = string.IsNullOrWhiteSpace(nit) ? null : nit };

                    // Intentar crear cliente. Si Create devuelve int, lo usaremos.
                    try
                    {
                        var method = _clienteRepo.GetType().GetMethod("Create");
                        if (method != null && method.ReturnType == typeof(int))
                        {
                            clienteId = (int)method.Invoke(_clienteRepo, new object[] { cliente })!;
                        }
                        else
                        {
                            // si Create es void, intentar insertar y luego buscar
                            method?.Invoke(_clienteRepo, new object[] { cliente });

                            var found = _clienteRepo.GetAll()
                                        .FirstOrDefault(x => string.Equals(x.Nombre, cliente.Nombre, StringComparison.OrdinalIgnoreCase)
                                                          && (string.IsNullOrWhiteSpace(cliente.NIT) || x.NIT == cliente.NIT));
                            if (found != null) clienteId = found.ClienteId;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Si hay conflicto de UNIQUE o error al insertar, intentar localizar cliente existente
                        try
                        {
                            var foundRetry = _clienteRepo.GetAll()
                                .FirstOrDefault(x => string.Equals(x.Nombre, cliente.Nombre, StringComparison.OrdinalIgnoreCase)
                                                     && (string.IsNullOrWhiteSpace(cliente.NIT) || x.NIT == cliente.NIT));
                            if (foundRetry != null) clienteId = foundRetry.ClienteId;
                            else
                            {
                                var r = MessageBox.Show("No fue posible crear o localizar el cliente. ¿Desea facturar igual como Consumidor Final?", "Cliente", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (r == DialogResult.No) return;
                                clienteId = null;
                            }
                        }
                        catch
                        {
                            var r = MessageBox.Show("No fue posible crear o localizar el cliente. ¿Desea facturar igual como Consumidor Final?", "Cliente", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (r == DialogResult.No) return;
                            clienteId = null;
                        }
                    }
                }
                else
                {
                    // si no hay nombre, dejamos clienteId = null (Consumidor Final)
                    clienteId = null;
                }

                // Construir lista de lineas
                var lineas = dtDetalles.AsEnumerable()
                    .Select(r => (ItemId: r.Field<int>("ItemId"), Cantidad: r.Field<int>("Cantidad"), PrecioUnitario: r.Field<decimal>("PrecioUnitario")))
                    .ToList();

                var id = _service.CrearFactura(clienteId, lineas);

                MessageBox.Show($"Factura creada correctamente. Id: {id}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                dtDetalles.Rows.Clear();
                UpdateTotales();
                this.Close();
            }
            catch (ArgumentException aex)
            {
                MessageBox.Show(aex.Message, "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear la factura:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
