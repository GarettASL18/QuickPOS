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
                var c = new Cliente
                {
                    Nombre = (txtNombre.Text ?? "").Trim(),
                    NIT = string.IsNullOrWhiteSpace(txtNIT.Text) ? null : txtNIT.Text.Trim()
                };

                if (string.IsNullOrWhiteSpace(c.Nombre))
                {
                    MessageBox.Show("El nombre del cliente es requerido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Llamada flexible a Create: si Create devuelve int, capturamos el id; si no, solo invocamos.
                var method = _repo.GetType().GetMethod("Create");
                if (method == null)
                    throw new InvalidOperationException("El repositorio no implementa Create(Cliente).");

                if (method.ReturnType == typeof(void))
                {
                    method.Invoke(_repo, new object[] { c });
                    MessageBox.Show("Cliente guardado.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                    return;
                }

                if (method.ReturnType == typeof(int))
                {
                    var result = method.Invoke(_repo, new object[] { c });
                    var newId = Convert.ToInt32(result);
                    MessageBox.Show($"Cliente creado (Id: {newId}).", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                    return;
                }

                // Si retorna otro tipo, intentar invocar y cerrar
                method.Invoke(_repo, new object[] { c });
                MessageBox.Show("Cliente guardado.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (System.Reflection.TargetInvocationException tie) when (tie.InnerException != null)
            {
                // Mostrar el error real producido en el repositorio, p.ej. violación UNIQUE
                MessageBox.Show("Error al guardar: " + tie.InnerException.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
