using System;
using System.Windows.Forms;
using QuickPOS.Data;
using QuickPOS.Models;

namespace QuickPOS.WinFormsApp
{
    public class PickItemForm : Form
    {
        private readonly IItemRepository _repo;
        public Item? SelectedItem { get; private set; }
        private DataGridView grid;

        public PickItemForm(IItemRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            Text = "Seleccionar Item";
            Width = 500; Height = 400; StartPosition = FormStartPosition.CenterParent;
            Initialize();
        }

        void Initialize()
        {
            grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false };
            Controls.Add(grid);
            var btn = new Button { Text = "Seleccionar", Dock = DockStyle.Bottom, Height = 40 };
            btn.Click += (_, __) => { if (grid.CurrentRow != null) { SelectedItem = grid.CurrentRow.DataBoundItem as Item; DialogResult = DialogResult.OK; Close(); } };
            Controls.Add(btn);
            try { grid.DataSource = _repo.GetAll(); } catch { /* ignore */ }
        }
    }
}
