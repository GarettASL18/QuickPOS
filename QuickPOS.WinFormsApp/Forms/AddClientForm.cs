using System;
using System.Windows.Forms;
using QuickPOS.Data;
using QuickPOS.Models;

namespace QuickPOS.WinFormsApp
{
    public class AddClientForm : Form
    {
        private readonly IClienteRepository _repo;
        TextBox txtNombre, txtNIT;
        Button btnSave, btnCancel;

        public AddClientForm(IClienteRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            Text = "Agregar Cliente";
            Width = 420; Height = 200; StartPosition = FormStartPosition.CenterParent;
            Initialize();
        }

        void Initialize()
        {
            var tl = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10), ColumnCount = 2, RowCount = 3 };
            tl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            tl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
            tl.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            tl.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            tl.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            tl.Controls.Add(new Label { Text = "Nombre:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 0);
            txtNombre = new TextBox { Dock = DockStyle.Fill }; tl.Controls.Add(txtNombre, 1, 0);

            tl.Controls.Add(new Label { Text = "NIT:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 1);
            txtNIT = new TextBox { Dock = DockStyle.Fill }; tl.Controls.Add(txtNIT, 1, 1);

            var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            btnSave = new Button { Text = "Guardar" }; btnSave.Click += BtnSave_Click; panel.Controls.Add(btnSave);
            btnCancel = new Button { Text = "Cancelar" }; btnCancel.Click += (_, __) => { DialogResult = DialogResult.Cancel; Close(); }; panel.Controls.Add(btnCancel);
            tl.Controls.Add(panel, 0, 2); tl.SetColumnSpan(panel, 2);

            Controls.Add(tl);
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                var c = new Cliente { Nombre = txtNombre.Text ?? "", NIT = string.IsNullOrWhiteSpace(txtNIT.Text) ? null : txtNIT.Text };
                _repo.Create(c);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message);
            }
        }
    }
}
