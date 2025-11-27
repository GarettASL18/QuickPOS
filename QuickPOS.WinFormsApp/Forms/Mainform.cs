using QuickPOS.Data;
using QuickPOS.Models;
using QuickPOS.Services;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace QuickPOS.WinFormsApp
{
    public class MainForm : Form
    {
        private readonly User _user;
        private readonly FacturaService _facturaService;
        private readonly IClienteRepository _clienteRepo;
        private readonly IItemRepository _item_repo;
        private readonly IUsuarioRepository _usuarioRepo;      // <-- nuevo
        private readonly ISettingRepository _settingRepo;      // <-- nuevo (opcional)
        private FlowLayoutPanel tilesPanel;

        public MainForm(
            User user,
            FacturaService facturaService,
            IClienteRepository clienteRepo,
            IItemRepository itemRepo,
            IUsuarioRepository usuarioRepo,        // <-- nuevo parametro
            ISettingRepository settingRepo)        // <-- nuevo parametro
        {
            _user = user ?? throw new ArgumentNullException(nameof(user));
            _facturaService = facturaService ?? throw new ArgumentNullException(nameof(facturaService));
            _clienteRepo = clienteRepo ?? throw new ArgumentNullException(nameof(clienteRepo));
            _item_repo = itemRepo ?? throw new ArgumentNullException(nameof(itemRepo));
            _usuarioRepo = usuarioRepo ?? throw new ArgumentNullException(nameof(usuarioRepo));
            _settingRepo = settingRepo ?? throw new ArgumentNullException(nameof(settingRepo));
            InitializeComponent();
        }


        void InitializeComponent()
        {
            Text = $"QuickPOS - {_user.Username} ({_user.Role})";
            Width = 1000; Height = 650; StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(240, 240, 240);

            // Header compacto
            var header = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = Color.FromArgb(30, 144, 255) };
            Controls.Add(header); // header arriba

            var lbl = new Label
            {
                Text = "QuickPOS",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = false,
                Width = 300,
                Height = 48,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(16, 8)
            };
            header.Controls.Add(lbl);

            var lblUser = new Label
            {
                Text = $"Usuario: {_user.Username}   Rol: {_user.Role}",
                ForeColor = Color.White,
                AutoSize = false,
                Width = 360,
                Height = 48,
                TextAlign = ContentAlignment.MiddleRight,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(ClientSize.Width - 380, 8)
            };
            // ajusta posición al redimensionar
            header.Resize += (_, __) =>
            {
                lblUser.Location = new Point(Math.Max(16, header.Width - lblUser.Width - 16), lblUser.Location.Y);
            };
            header.Controls.Add(lblUser);

            Controls.Add(header); // header arriba

            // Tiles panel
            tilesPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20, header.Height + 12, 20, 20), // <-- aquí empujamos hacia abajo
                AutoScroll = true,
                WrapContents = true,
                BackColor = Color.Transparent
            };
            Controls.Add(tilesPanel);

            // Tiles
            AddTile("Clientes", "clients.png", OpenClientes);
            AddTile("Items", "items.png", OpenItems);
            AddTile("Crear Factura", "invoice.png", OpenFacturacion);
            AddTile("Venta Rápida", "quicksale.png", () => {
                using var f = new QuickSaleForm(_facturaService, _item_repo, _clienteRepo);
                f.ShowDialog();
            });


            if (IsAdmin())
            {
                AddTile("Usuarios", "users.png", OpenUsuarios);
                AddTile("Configuración", "settings.png", OpenConfiguracion);
            }

            // Bottom bar
            var bottom = new Panel { Dock = DockStyle.Bottom, Height = 60 };
            var btnLogout = new Button { Text = "Cerrar sesión", Dock = DockStyle.Right, Width = 140, Margin = new Padding(10) };
            btnLogout.Click += (_, __) => { Close(); };
            bottom.Controls.Add(btnLogout);
            Controls.Add(bottom);
        }

        bool IsAdmin() => _user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase);

        void AddTile(string title, string imageFileName, Action onClick)
        {
            // Tile container
            var tile = new Panel
            {
                Width = 220,
                Height = 170,
                Margin = new Padding(12),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Table layout to keep image area fixed and label below
            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 110)); // espacio para la imagen
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // PictureBox inside a panel to center it
            var pic = new PictureBox
            {
                Size = new Size(160, 90),
                SizeMode = PictureBoxSizeMode.Zoom,
                Anchor = AnchorStyles.None,
                Margin = new Padding(0)
            };

            var picPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            picPanel.Controls.Add(pic);
            pic.Location = new Point((picPanel.Width - pic.Width) / 2, (picPanel.Height - pic.Height) / 2);
            picPanel.Resize += (_, __) =>
            {
                pic.Location = new Point(Math.Max(0, (picPanel.Width - pic.Width) / 2), Math.Max(0, (picPanel.Height - pic.Height) / 2));
            };

            // Load image robustly from images folder in output
            try
            {
                var imgPath = System.IO.Path.Combine(Application.StartupPath, "images", imageFileName);
                if (System.IO.File.Exists(imgPath))
                {
                    using var tmp = Image.FromFile(imgPath);
                    pic.Image = new Bitmap(tmp); // avoid locking file
                }
                else
                {
                    pic.Image = null;
                }
            }
            catch
            {
                pic.Image = null;
            }

            table.Controls.Add(picPanel, 0, 0);

            var lbl = new Label
            {
                Text = title,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.Transparent
            };
            table.Controls.Add(lbl, 0, 1);

            tile.Controls.Add(table);

            // Click handlers for whole tile
            tile.Cursor = Cursors.Hand;
            void invoke() { try { onClick(); } catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); } }
            tile.Click += (_, __) => invoke();
            pic.Click += (_, __) => invoke();
            lbl.Click += (_, __) => invoke();

            tilesPanel.Controls.Add(tile);
        }
        void OpenClientes() { using var f = new ClientesForm(_clienteRepo); f.ShowDialog(); }
        void OpenItems() { using var f = new ItemsForm(_item_repo); f.ShowDialog(); }
        void OpenFacturacion() { using var f = new FacturacionForm(_facturaService, _item_repo, _clienteRepo); f.ShowDialog(); }
        void OpenUsuarios() { using var f = new UsuariosForm(_usuarioRepo); f.ShowDialog(); } // usa el campo inyectado
        void OpenConfiguracion() { using var f = new ConfigForm(_settingRepo); f.ShowDialog(); } // usa el campo inyectado
    }
}
