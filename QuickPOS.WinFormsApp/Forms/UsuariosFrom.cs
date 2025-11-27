using System;
using System.Windows.Forms;
using QuickPOS.Data;
using QuickPOS.Models;

namespace QuickPOS.WinFormsApp
{
    public class UsuariosForm : Form
    {
        private readonly IUsuarioRepository _repo;
        private DataGridView grid;

        public UsuariosForm(IUsuarioRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            Text = "Usuarios";
            Width = 800; Height = 450; StartPosition = FormStartPosition.CenterParent;
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

            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "UsuarioId", HeaderText = "Id", Width = 60 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Username", HeaderText = "Usuario", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Role", HeaderText = "Rol", Width = 120 });

            tl.Controls.Add(grid, 0, 0);

            var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, Padding = new Padding(6) };
            var btnAdd = new Button { Text = "Agregar" }; btnAdd.Click += (_, __) => Add();
            var btnEdit = new Button { Text = "Editar" }; btnEdit.Click += (_, __) => Edit();
            var btnDel = new Button { Text = "Eliminar" }; btnDel.Click += (_, __) => Delete();
            panel.Controls.Add(btnAdd); panel.Controls.Add(btnEdit); panel.Controls.Add(btnDel);
            tl.Controls.Add(panel, 0, 1);

            Controls.Add(tl);
            LoadData();
        }

        void LoadData()
        {
            try { grid.DataSource = _repo.GetAll(); }
            catch (Exception ex) { MessageBox.Show("Error cargando usuarios: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        void Add()
        {
            using var f = new AddEditUsuarioForm(_repo);
            if (f.ShowDialog() == DialogResult.OK) LoadData();
        }

        void Edit()
        {
            if (grid.CurrentRow == null) { MessageBox.Show("Seleccione un usuario."); return; }
            var u = grid.CurrentRow.DataBoundItem as Usuario;
            if (u == null) return;
            using var f = new AddEditUsuarioForm(_repo, u);
            if (f.ShowDialog() == DialogResult.OK) LoadData();
        }

        void Delete()
        {
            if (grid.CurrentRow == null) { MessageBox.Show("Seleccione un usuario."); return; }
            var u = grid.CurrentRow.DataBoundItem as Usuario;
            if (u == null) return;
            var r = MessageBox.Show($"Eliminar usuario {u.Username}?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r == DialogResult.Yes)
            {
                try
                {
                    _repo.Delete(u.UsuarioId);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error eliminando usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
