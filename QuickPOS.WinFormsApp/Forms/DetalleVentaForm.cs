using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QuickPOS.Data;
using QuickPOS.Models;
using QuickPOS.Services; // Necesario para PdfService

namespace QuickPOS.WinFormsApp.Forms
{
    public partial class DetalleVentaForm : Form
    {
        private readonly IFacturaRepository _repo;
        private readonly ISettingRepository _settings;
        private readonly long _facturaId;
        private readonly string _nombreCliente;

        // Constructor
        public DetalleVentaForm(IFacturaRepository repo, ISettingRepository settings, long facturaId, string nombreCliente)
        {
            InitializeComponent();
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _facturaId = facturaId;
            _nombreCliente = nombreCliente;

            // Configuración inicial
            lblTitulo.Text = $"Venta #{_facturaId} - {_nombreCliente}";
            SetupEvents();
        }

        private void SetupEvents()
        {
            // Evento Load
            this.Load += DetalleVentaForm_Load;

            // Botones
            btnCerrar.Click += (s, e) => this.Close();
            btnPdf.Click += BtnPdf_Click;
        }

        private void DetalleVentaForm_Load(object sender, EventArgs e)
        {
            ConfigurarGrid();
            CargarDetalles();
        }

        private void ConfigurarGrid()
        {
            // Estilo de la tabla (Igual que el carrito de ventas)
            dgvDetalles.AutoGenerateColumns = false;
            dgvDetalles.BackgroundColor = System.Drawing.Color.White;
            dgvDetalles.BorderStyle = BorderStyle.None;
            dgvDetalles.RowHeadersVisible = false;
            dgvDetalles.AllowUserToAddRows = false;
            dgvDetalles.ReadOnly = true;
            dgvDetalles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Encabezado
            dgvDetalles.EnableHeadersVisualStyles = false;
            dgvDetalles.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvDetalles.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(24, 30, 54);
            dgvDetalles.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            dgvDetalles.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            dgvDetalles.ColumnHeadersHeight = 40;

            // Filas
            dgvDetalles.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10);
            dgvDetalles.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(248, 248, 248);
            dgvDetalles.RowTemplate.Height = 35;

            // Columnas
            dgvDetalles.Columns.Clear();

            dgvDetalles.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NombreProducto",
                HeaderText = "PRODUCTO",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dgvDetalles.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "PrecioUnitario",
                HeaderText = "PRECIO",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
            });

            dgvDetalles.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Cantidad",
                HeaderText = "CANT",
                Width = 70
            });

            dgvDetalles.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TotalLinea",
                HeaderText = "TOTAL",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold) }
            });
        }

        private void CargarDetalles()
        {
            try
            {
                var detalles = _repo.GetDetalles(_facturaId);
                dgvDetalles.DataSource = detalles;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos: " + ex.Message);
            }
        }

        private void BtnPdf_Click(object? sender, EventArgs e)
        {
            using var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Archivo PDF (*.pdf)|*.pdf";
            saveDialog.FileName = $"Factura_{_facturaId}_{DateTime.Now:yyyyMMdd}.pdf";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var detalles = (List<FacturaDetalle>)dgvDetalles.DataSource;

                    // Reconstruimos totales para el PDF
                    decimal total = 0;
                    foreach (var d in detalles) total += d.TotalLinea;

                    var facturaParaPdf = new Factura
                    {
                        FacturaId = _facturaId,
                        NombreCliente = _nombreCliente,
                        Fecha = DateTime.Now, // Nota: Lo ideal es traer la fecha original de la BD
                        Subtotal = total / 1.15m,
                        Impuesto = total - (total / 1.15m),
                        Total = total
                    };

                    // Obtenemos nombre de la empresa de la configuración
                    string nombreEmpresa = _settings.Get("NombreEmpresa") ?? "Mi Negocio POS";

                    // Generar
                    var pdfService = new PdfService();
                    pdfService.ExportarFactura(facturaParaPdf, detalles, nombreEmpresa, saveDialog.FileName);

                    if (MessageBox.Show("PDF Generado correctamente.\n¿Desea abrirlo ahora?", "Éxito", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        new System.Diagnostics.Process
                        {
                            StartInfo = new System.Diagnostics.ProcessStartInfo(saveDialog.FileName) { UseShellExecute = true }
                        }.Start();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error generando PDF: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}