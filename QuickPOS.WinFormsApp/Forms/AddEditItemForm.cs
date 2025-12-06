using System;
using System.Drawing;
using System.Windows.Forms;
using QuickPOS.Data;
using QuickPOS.Models;

namespace QuickPOS.WinFormsApp.Forms
{
    public partial class AddEditItemForm : Form
    {
        private readonly IItemRepository _repo;
        private readonly Item? _itemToEdit;

        // Controles
        private TextBox txtName;
        private TextBox txtPrice;
        private CheckBox chkActive;
        private Button btnSave;
        private Button btnCancel;
        private Label lblTitle;

        public AddEditItemForm(IItemRepository repo) : this(repo, null) { }

        public AddEditItemForm(IItemRepository repo, Item? itemToEdit)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _itemToEdit = itemToEdit;

            InitializeCustomComponent(); // Diseño visual
            LoadData(); // Cargar datos si es edición
        }

        private void LoadData()
        {
            if (_itemToEdit != null)
            {
                lblTitle.Text = "Editar Producto";
                txtName.Text = _itemToEdit.Nombre;
                txtPrice.Text = _itemToEdit.Precio.ToString("0.00");
                chkActive.Checked = _itemToEdit.Activo;
            }
            else
            {
                lblTitle.Text = "Nuevo Producto";
                chkActive.Checked = true; // Por defecto activo
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            // 1. Validaciones
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("El nombre es obligatorio.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Ingresa un precio válido (mayor o igual a 0).", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrice.Focus();
                return;
            }

            try
            {
                if (_itemToEdit == null)
                {
                    // --- CREAR ---
                    var newItem = new Item
                    {
                        Nombre = txtName.Text.Trim(),
                        Precio = price,
                        Activo = chkActive.Checked
                    };
                    _repo.Create(newItem);
                    MessageBox.Show("Producto creado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // --- EDITAR ---
                    _itemToEdit.Nombre = txtName.Text.Trim();
                    _itemToEdit.Precio = price;
                    _itemToEdit.Activo = chkActive.Checked;
                    _repo.Update(_itemToEdit);
                    MessageBox.Show("Producto actualizado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- DISEÑO VISUAL ---
        private void InitializeCustomComponent()
        {
            this.Size = new Size(400, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;
            this.Text = "Gestión de Producto";

            lblTitle = new Label { Text = "Producto", Font = new Font("Segoe UI", 16, FontStyle.Bold), Location = new Point(20, 20), AutoSize = true, ForeColor = Color.MidnightBlue };
            this.Controls.Add(lblTitle);

            // Nombre
            this.Controls.Add(new Label { Text = "Nombre del Producto *", Location = new Point(25, 70), AutoSize = true, ForeColor = Color.DimGray, Font = new Font("Segoe UI", 9, FontStyle.Bold) });
            txtName = new TextBox { Location = new Point(25, 90), Width = 330, Font = new Font("Segoe UI", 11) };
            this.Controls.Add(txtName);

            // Precio
            this.Controls.Add(new Label { Text = "Precio Unitario ($) *", Location = new Point(25, 140), AutoSize = true, ForeColor = Color.DimGray, Font = new Font("Segoe UI", 9, FontStyle.Bold) });
            txtPrice = new TextBox { Location = new Point(25, 160), Width = 150, Font = new Font("Segoe UI", 11) };
            this.Controls.Add(txtPrice);

            // Activo
            chkActive = new CheckBox { Text = "Producto disponible para venta", Location = new Point(25, 210), AutoSize = true, Font = new Font("Segoe UI", 10) };
            this.Controls.Add(chkActive);

            // Botones
            btnSave = new Button { Text = "Guardar", BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Size = new Size(120, 40), Location = new Point(140, 250), Cursor = Cursors.Hand };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnCancel = new Button { Text = "Cancelar", BackColor = Color.WhiteSmoke, ForeColor = Color.Black, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10), Size = new Size(100, 40), Location = new Point(270, 250), Cursor = Cursors.Hand };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }
    }
}