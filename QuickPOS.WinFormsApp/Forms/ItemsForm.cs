using System;
using System.Windows.Forms;
using QuickPOS.Data;

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
            Width = 700; Height = 450; StartPosition = FormStartPosition.CenterParent;
            Initialize();
        }

        void Initialize()
        {
            var tl = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 1 };
            grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AutoGenerateColumns = true };
            tl.Controls.Add(grid, 0, 0);
            Controls.Add(tl);
            LoadData();
        }

        void LoadData()
        {
            try { grid.DataSource = _repo.GetAll(); }
            catch (Exception ex) { MessageBox.Show("Error cargando items: " + ex.Message); }
        }
    }
}
