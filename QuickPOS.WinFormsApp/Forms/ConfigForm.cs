using System;
using System.Drawing;
using System.Windows.Forms;
using QuickPOS.Data;

namespace QuickPOS.WinFormsApp.Forms
{
    public partial class ConfigForm : Form
    {
        private readonly ISettingRepository _settings;

        // --- PESTAÑA GENERAL ---
        private NumericUpDown nudImpuesto;
        private TextBox txtEmpresa;
        private TextBox txtDireccion;
        private TextBox txtTelefono;

        // --- PESTAÑA PERMISOS ---
        private CheckBox chkAdminHistory; // Ver Historial
        private CheckBox chkAdminDelete;  // Borrar Productos
        private CheckBox chkAdminEdit;    // NUEVO: Editar/Agregar Productos

        private Button btnSave, btnCancel;

        public ConfigForm(ISettingRepository settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            InitializeCustomComponent();
            LoadValues();
        }

        private void LoadValues()
        {
            // 1. GENERAL
            var sImpuesto = _settings.Get("Impuesto");
            nudImpuesto.Value = decimal.TryParse(sImpuesto, out var v) ? v * 100 : 15;

            txtEmpresa.Text = _settings.Get("NombreEmpresa");
            txtDireccion.Text = _settings.Get("DireccionEmpresa");
            txtTelefono.Text = _settings.Get("TelefonoEmpresa");

            // 2. PERMISOS (Por defecto "False" para seguridad)
            chkAdminHistory.Checked = _settings.Get("Permiso_VerHistorial") == "True";
            chkAdminDelete.Checked = _settings.Get("Permiso_BorrarItems") == "True";
            chkAdminEdit.Checked = _settings.Get("Permiso_EditarItems") == "True"; // <--- NUEVO
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                // Guardar General
                _settings.Set("Impuesto", (nudImpuesto.Value / 100m).ToString());
                _settings.Set("NombreEmpresa", txtEmpresa.Text.Trim());
                _settings.Set("DireccionEmpresa", txtDireccion.Text.Trim());
                _settings.Set("TelefonoEmpresa", txtTelefono.Text.Trim());

                // Guardar Permisos
                _settings.Set("Permiso_VerHistorial", chkAdminHistory.Checked.ToString());
                _settings.Set("Permiso_BorrarItems", chkAdminDelete.Checked.ToString());
                _settings.Set("Permiso_EditarItems", chkAdminEdit.Checked.ToString()); // <--- NUEVO

                // Actualizar memoria
                QuickPOS.Config.SetImpuesto(nudImpuesto.Value / 100m);

                MessageBox.Show("Configuración guardada.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message);
            }
        }

        private void InitializeCustomComponent()
        {
            this.Text = "Configuración del Sistema";
            this.Size = new Size(500, 500); // Un poco más alto para que quepa todo
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            var tabControl = new TabControl { Dock = DockStyle.Top, Height = 380 };

            // --- TAB 1: GENERAL ---
            var tabGeneral = new TabPage { Text = "Datos del Negocio", BackColor = Color.White, Padding = new Padding(20) };

            int y = 20;
            tabGeneral.Controls.Add(new Label { Text = "Impuesto (IVA) %", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) });
            nudImpuesto = new NumericUpDown { Location = new Point(20, y + 20), Width = 100, DecimalPlaces = 2 };
            tabGeneral.Controls.Add(nudImpuesto);

            y += 60;
            tabGeneral.Controls.Add(new Label { Text = "Nombre del Negocio", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) });
            txtEmpresa = new TextBox { Location = new Point(20, y + 20), Width = 400, Font = new Font("Segoe UI", 10) };
            tabGeneral.Controls.Add(txtEmpresa);

            y += 60;
            tabGeneral.Controls.Add(new Label { Text = "Dirección", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) });
            txtDireccion = new TextBox { Location = new Point(20, y + 20), Width = 400, Font = new Font("Segoe UI", 10) };
            tabGeneral.Controls.Add(txtDireccion);

            y += 60;
            tabGeneral.Controls.Add(new Label { Text = "Teléfono", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) });
            txtTelefono = new TextBox { Location = new Point(20, y + 20), Width = 200, Font = new Font("Segoe UI", 10) };
            tabGeneral.Controls.Add(txtTelefono);

            tabControl.TabPages.Add(tabGeneral);

            // --- TAB 2: PERMISOS ---
            var tabPermisos = new TabPage { Text = "Permisos de Empleados", BackColor = Color.White, Padding = new Padding(20) };

            tabPermisos.Controls.Add(new Label
            {
                Text = "Seleccione qué acciones pueden realizar los empleados:",
                Location = new Point(20, 20),
                AutoSize = true,
                ForeColor = Color.DimGray
            });

            // Opción 1
            chkAdminHistory = new CheckBox { Text = "Ver Historial de Ventas", Location = new Point(30, 60), AutoSize = true, Font = new Font("Segoe UI", 10) };
            tabPermisos.Controls.Add(chkAdminHistory);

            // Opción 2 (NUEVA)
            chkAdminEdit = new CheckBox { Text = "Crear y Editar Productos (Catálogo)", Location = new Point(30, 100), AutoSize = true, Font = new Font("Segoe UI", 10) };
            tabPermisos.Controls.Add(chkAdminEdit);

            // Opción 3
            chkAdminDelete = new CheckBox { Text = "Eliminar Productos (Desactivar)", Location = new Point(30, 140), AutoSize = true, Font = new Font("Segoe UI", 10) };
            tabPermisos.Controls.Add(chkAdminDelete);

            tabControl.TabPages.Add(tabPermisos);
            this.Controls.Add(tabControl);

            // Botones
            btnSave = new Button { Text = "Guardar Cambios", BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(150, 40), Location = new Point(80, 400), Cursor = Cursors.Hand };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnCancel = new Button { Text = "Cancelar", BackColor = Color.WhiteSmoke, ForeColor = Color.Black, FlatStyle = FlatStyle.Flat, Size = new Size(100, 40), Location = new Point(250, 400), Cursor = Cursors.Hand };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => Close();
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }
    }
}