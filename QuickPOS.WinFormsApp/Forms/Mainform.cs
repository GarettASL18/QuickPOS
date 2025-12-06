using QuickPOS.Data;
using QuickPOS.Models;
using QuickPOS.Services;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QuickPOS.WinFormsApp.Forms;

namespace QuickPOS.WinFormsApp
{
    public partial class MainForm : Form
    {
        private User? _user;
        private FacturaService? _facturaService;
        private IClienteRepository? _clienteRepo;
        private IItemRepository? _itemRepo;
        private IUsuarioRepository? _usuarioRepo;
        private ISettingRepository? _settingRepo;
        private IFacturaRepository? _facturaRepo;

        public bool IsLogout { get; private set; } = false;
        private Button? _currentButton;
        private readonly Color _activeColor = Color.FromArgb(70, 100, 180);
        private readonly Color _hoverColor = Color.FromArgb(40, 50, 100);
        private readonly Color _defaultColor = Color.MidnightBlue;

        public MainForm()
        {
            InitializeComponent();
            SetupButtonLogic();
            StartClock();
        }

        public MainForm(
            User user,
            FacturaService facturaService,
            IClienteRepository clienteRepo,
            IItemRepository itemRepo,
            IUsuarioRepository usuarioRepo,
            ISettingRepository settingRepo,
            IFacturaRepository facturaRepo) : this()
        {
            _user = user ?? throw new ArgumentNullException(nameof(user));
            _facturaService = facturaService ?? throw new ArgumentNullException(nameof(facturaService));
            _clienteRepo = clienteRepo ?? throw new ArgumentNullException(nameof(clienteRepo));
            _itemRepo = itemRepo ?? throw new ArgumentNullException(nameof(itemRepo));
            _usuarioRepo = usuarioRepo ?? throw new ArgumentNullException(nameof(usuarioRepo));
            _settingRepo = settingRepo ?? throw new ArgumentNullException(nameof(settingRepo));
            _facturaRepo = facturaRepo ?? throw new ArgumentNullException(nameof(facturaRepo));
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (_user != null)
            {
                lblUserInfo.Text = $"Usuario: {_user.Username} | Rol: {_user.Role}";
                lblUserInfo.Top = (pnlHeader.Height - lblUserInfo.Height) / 2;

                if (!IsAdmin())
                {
                    btnUsuarios.Visible = false;
                    btnConfig.Visible = false;
                    string permisoHistorial = _settingRepo?.Get("Permiso_VerHistorial") ?? "False";
                    btnHistorial.Visible = (permisoHistorial == "True");
                }

                FixLabelSizing(label2);
                FixLabelSizing(label3);
                FixLabelSizing(label5);
                FixLabelSizing(label7);

                LoadDashboardData();
            }
        }

        private void LoadDashboardData()
        {
            try
            {
                if (_clienteRepo != null) label5.Text = _clienteRepo.GetAll().Count.ToString();
                if (_itemRepo != null) label7.Text = _itemRepo.GetAll().Count.ToString();
                if (_facturaRepo != null) label3.Text = _facturaRepo.GetAll().Count.ToString();

                if (_facturaRepo != null)
                {
                    decimal totalHoy = _facturaRepo.GetTotalVentasHoy();
                    label2.Text = totalHoy.ToString("C2");
                }
            }
            catch { }
        }

        private void FixLabelSizing(Label lbl)
        {
            if (lbl != null)
            {
                lbl.AutoSize = false;
                lbl.Dock = DockStyle.Bottom;
                lbl.Height = 45;
                lbl.TextAlign = ContentAlignment.MiddleRight;
                lbl.AutoEllipsis = true;
            }
        }

        private void SetupButtonLogic()
        {
            btnVenta.Click += OpenFacturacion;
            btnHistorial.Click += OpenHistorial;
            btnClientes.Click += OpenClientes;
            btnProductos.Click += OpenItems;
            btnUsuarios.Click += OpenUsuarios;
            btnConfig.Click += OpenConfiguracion;
            btnSalir.Click += Logout;

            ConfigureButton(btnVenta);
            ConfigureButton(btnHistorial);
            ConfigureButton(btnClientes);
            ConfigureButton(btnProductos);
            ConfigureButton(btnUsuarios);
            ConfigureButton(btnConfig);
            ConfigureButton(btnSalir);
        }

        private void ConfigureButton(Button btn)
        {
            btn.BackColor = _defaultColor;
            btn.MouseEnter += (s, e) => { if (_currentButton != btn) { btn.BackColor = _hoverColor; btn.Cursor = Cursors.Hand; } };
            btn.MouseLeave += (s, e) => { if (_currentButton != btn) { btn.BackColor = _defaultColor; btn.Cursor = Cursors.Default; } };
            btn.Click += (s, e) => ActivateButton(btn);
        }

        private void ActivateButton(Button btnSender)
        {
            if (btnSender == null) return;
            if (_currentButton != null) _currentButton.BackColor = _defaultColor;
            _currentButton = btnSender;
            _currentButton.BackColor = _activeColor;
        }

        private bool IsAdmin() => _user != null && _user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase);

        // --- NAVEGACIÓN ---

        private void OpenFacturacion(object? sender, EventArgs e)
        {
            if (_facturaService == null) return;
            using var f = new FacturacionForm(_facturaService, _itemRepo, _clienteRepo);
            f.ShowDialog();
            LoadDashboardData();
        }

        // --- MÉTODO MODIFICADO PARA PASAR SETTINGS ---
        private void OpenHistorial(object? sender, EventArgs e)
        {
            if (_facturaRepo == null || _settingRepo == null) return; // Verificamos ambos

            // Pasamos ambos repositorios
            using var f = new VentasForm(_facturaRepo, _settingRepo);
            f.ShowDialog();
            LoadDashboardData();
        }

        private void OpenClientes(object? sender, EventArgs e)
        {
            if (_clienteRepo == null) return;
            using var f = new ClientesForm(_clienteRepo);
            f.ShowDialog();
            LoadDashboardData();
        }

        private void OpenItems(object? sender, EventArgs e)
        {
            if (_itemRepo == null || _settingRepo == null || _user == null) return;
            using var f = new ItemsForm(_itemRepo, _settingRepo, _user);
            f.ShowDialog();
            LoadDashboardData();
        }

        private void OpenUsuarios(object? sender, EventArgs e)
        {
            if (_usuarioRepo == null || _user == null) return;
            using var f = new UsuariosForm(_usuarioRepo, _user.UsuarioId);
            var result = f.ShowDialog();
            if (result == DialogResult.Abort)
            {
                IsLogout = true;
                this.Close();
            }
        }

        private void OpenConfiguracion(object? sender, EventArgs e)
        {
            if (_settingRepo == null) return;
            using var f = new ConfigForm(_settingRepo);
            f.ShowDialog();
        }

        private void Logout(object? sender, EventArgs e)
        {
            if (MessageBox.Show("¿Seguro que desea cerrar sesión?", "Salir", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                IsLogout = true;
                this.Close();
            }
        }

        private void StartClock()
        {
            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
            t.Interval = 1000;
            t.Tick += (s, e) => {
                lblClock.Text = DateTime.Now.ToString("dd MMM yyyy - hh:mm tt");
                lblClock.Top = (pnlHeader.Height - lblClock.Height) / 2;
            };
            t.Start();
        }
    }
}