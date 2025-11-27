using System;
using System.Windows.Forms;
using QuickPOS.Data;
using QuickPOS.Models;

namespace QuickPOS.WinFormsApp
{
    public class AddEditItemForm : Form
    {
        private readonly IItemRepository _repo;
        private readonly Item? _item;

        TextBox txtNombre;
        NumericUpDown nudPrecio;
        CheckBox chkActivo;
        Button btnSave;
        Button btnCancel;

        public AddEditItemForm(IItemRepository repo, Item? item = null)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _item = item;
            Text = item == null ? "Agregar Item" : "Editar Item";
            Width = 420;
            Height = 220;
            StartPosition = FormStartPosition.CenterParent;
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

            tl.Controls.Add(new Label { Text = "Precio:", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 1);
            nudPrecio = new NumericUpDown { Dock = DockStyle.Left, DecimalPlaces = 2, Minimum = 0, Maximum = 100000, Width = 120 }; tl.Controls.Add(nudPrecio, 1, 1);

            chkActivo = new CheckBox { Text = "Activo", Checked = true, Dock = DockStyle.Left }; tl.Controls.Add(chkActivo, 1, 2);

            var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, Height = 40, Padding = new Padding(8) };
            btnSave = new Button { Text = "Guardar", AutoSize = true }; btnSave.Click += BtnSave_Click;
            btnCancel = new Button { Text = "Cancelar", AutoSize = true }; btnCancel.Click += (_, __) => { DialogResult = DialogResult.Cancel; Close(); };
            btnPanel.Controls.Add(btnSave);
            btnPanel.Controls.Add(btnCancel);

            Controls.Add(tl);
            Controls.Add(btnPanel);

            if (_item != null)
            {
                txtNombre.Text = _item.Nombre;
                nudPrecio.Value = _item.Precio;
                chkActivo.Checked = _item.Activo;
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es requerido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var it = new Item
            {
                ItemId = _item?.ItemId ?? 0,
                Nombre = txtNombre.Text.Trim(),
                Precio = nudPrecio.Value,
                Activo = chkActivo.Checked
            };

            try
            {
                if (_item == null)
                {
                    var newId = _repo.Create(it);
                    // opcional: it.ItemId = newId;
                }
                else
                {
                    _repo.Update(it);
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar el item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
