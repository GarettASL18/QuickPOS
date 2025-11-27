using System;
using System.Windows.Forms;
using QuickPOS.Data;

namespace QuickPOS.WinFormsApp
{
    public class ConfigForm : Form
    {
        private readonly ISettingRepository _settings;
        private NumericUpDown nudImpuesto;
        private Button btnSave, btnCancel;

        public ConfigForm(ISettingRepository settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Text = "Configuración";
            Width = 360; Height = 180; StartPosition = FormStartPosition.CenterParent;
            Initialize();
            LoadValues();
        }

        void Initialize()
        {
            var tl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Padding = new Padding(8) };
            tl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            tl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
            tl.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            tl.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));

            tl.Controls.Add(new Label { Text = "Impuesto (%):", TextAlign = System.Drawing.ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 0);
            nudImpuesto = new NumericUpDown { DecimalPlaces = 2, Minimum = 0, Maximum = 1, Increment = 0.01M, Dock = DockStyle.Left, Width = 120 }; // value 0..1
            tl.Controls.Add(nudImpuesto, 1, 0);

            var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            btnSave = new Button { Text = "Guardar" }; btnSave.Click += BtnSave_Click;
            btnCancel = new Button { Text = "Cancelar" }; btnCancel.Click += (_, __) => Close();
            btnPanel.Controls.Add(btnSave); btnPanel.Controls.Add(btnCancel);

            tl.Controls.Add(btnPanel, 0, 1);
            tl.SetColumnSpan(btnPanel, 2);

            Controls.Add(tl);
        }

        void LoadValues()
        {
            // Get setting "Impuesto" as decimal (stored like 0.15)
            var s = _settings.Get("Impuesto");
            if (decimal.TryParse(s, out var v)) nudImpuesto.Value = v;
            else nudImpuesto.Value = QuickPOS.Config.Impuesto;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            var v = nudImpuesto.Value;
            _settings.Set("Impuesto", v.ToString()); // store as 0.15
            // update runtime config
            QuickPOS.Config.SetImpuesto(v);
            MessageBox.Show("Configuración guardada.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }
    }
}
