using System;
using System.Windows.Forms;
using System.Linq;
using QuickPOS.Data;
using QuickPOS.Models;

namespace QuickPOS.WinFormsApp
{
    public class ItemsForm : Form
    {
        private readonly IItemRepository _repo;
        private DataGridView grid;

        public ItemsForm(IItemRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            Text = "Items";
            Width = 800; Height = 500; StartPosition = FormStartPosition.CenterParent;
            Initialize();
        }

        void Initialize()
        {
            var tl = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2 };
            tl.RowStyles.Add(new RowStyle(SizeType.Percent, 90));
            tl.RowStyles.Add(new RowStyle(SizeType.Percent, 10));

            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            // Columnas explícitas (más control)
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ItemId", HeaderText = "Id", Width = 60 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre", HeaderText = "Nombre", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Precio", HeaderText = "Precio", Width = 120, DefaultCellStyle = { Format = "0.00" } });
            grid.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "Activo", HeaderText = "Activo", Width = 70 });

            tl.Controls.Add(grid, 0, 0);

            var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, Padding = new Padding(6) };
            var btnAdd = new Button { Text = "Agregar", AutoSize = true }; btnAdd.Click += (_, __) => AddItem();
            var btnEdit = new Button { Text = "Editar", AutoSize = true }; btnEdit.Click += (_, __) => EditItem();
            var btnDelete = new Button { Text = "Eliminar", AutoSize = true }; btnDelete.Click += (_, __) => DeleteItem();
            panel.Controls.Add(btnAdd); panel.Controls.Add(btnEdit); panel.Controls.Add(btnDelete);

            tl.Controls.Add(panel, 0, 1);

            Controls.Add(tl);
            LoadData();
        }

        void LoadData()
        {
            try
            {
                var items = _repo.GetAll();
                grid.DataSource = items;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando items: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void AddItem()
        {
            using var f = new AddEditItemForm(_repo);
            if (f.ShowDialog() == DialogResult.OK) LoadData();
        }

        void EditItem()
        {
            if (grid.CurrentRow == null) { MessageBox.Show("Seleccione un item para editar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            var item = grid.CurrentRow.DataBoundItem as Item;
            if (item == null) return;
            using var f = new AddEditItemForm(_repo, item);
            if (f.ShowDialog() == DialogResult.OK) LoadData();
        }

        void DeleteItem()
        {
            if (grid.CurrentRow == null) { MessageBox.Show("Seleccione un item para eliminar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            var item = grid.CurrentRow.DataBoundItem as Item;
            if (item == null) return;
            var r = MessageBox.Show($"Eliminar item '{item.Nombre}'?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r == DialogResult.Yes)
            {
                try
                {
                    _repo.Delete(item.ItemId);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error eliminando item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
