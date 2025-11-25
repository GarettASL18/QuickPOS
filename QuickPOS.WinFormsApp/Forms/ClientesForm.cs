using QuickPOS.Data;
using QuickPOS.Models;
using System;
using System.Windows.Forms;

namespace QuickPOS.WinFormsApp
{
    public class ClientesForm : Form
    {
        private readonly IClienteRepository _repo;
        private DataGridView grid;

        public ClientesForm(IClienteRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            Text = "Clientes";
            Width = 700; Height = 450; StartPosition = FormStartPosition.CenterParent;
            Initialize();
        }

        void Initialize()
        {
            var tl = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2 };
            tl.RowStyles.Add(new RowStyle(SizeType.Percent, 90));
            tl.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AutoGenerateColumns = true };
            tl.Controls.Add(grid, 0, 0);

            var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
            var btnAdd = new Button { Text = "Agregar Cliente" }; btnAdd.Click += (_, __) => { using var f = new AddClientForm(_repo); if (f.ShowDialog() == DialogResult.OK) LoadData(); };
            btnPanel.Controls.Add(btnAdd);
            tl.Controls.Add(btnPanel, 0, 1);

            Controls.Add(tl);
            LoadData();
        }

        void LoadData()
        {
            try { grid.DataSource = _repo.GetAll(); }
            catch (Exception ex) { MessageBox.Show("Error cargando clientes: " + ex.Message); }
        }
    }
}
