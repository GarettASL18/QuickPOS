using QuickPOS.Data;
using QuickPOS.Models;
using QuickPOS.Services;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace QuickPOS.WinFormsApp
{
    // ahora partial: la parte UI estará en MainForm.Designer.cs
    public partial class MainForm : Form
    {
        // Dependencias (inyectadas en runtime). Nullable para permitir constructor parameterless (designer).
        private User? _user;
        private FacturaService? _facturaService;
        private IClienteRepository? _clienteRepo;
        private IItemRepository? _item_repo;
        private IUsuarioRepository? _usuarioRepo;
        private ISettingRepository? _settingRepo;

        // Constructor parameterless para que el Designer pueda instanciar la clase.
        public MainForm()
        {
            InitializeComponent();

            // Solo en modo diseño: crear tiles de vista previa y forzar layout
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
             
                // asegurar propiedades que ayudan al layout en diseñador
                if (tilesPanel != null)
                {
                    tilesPanel.WrapContents = true;
                    tilesPanel.FlowDirection = FlowDirection.LeftToRight;
                    CreateDesignTimeTiles_WithLayout();
                }
            }
        }

        // Constructor real con DI — llama al ctor parameterless para que InitializeComponent ya haya corrido.
        public MainForm(
            User user,
            FacturaService facturaService,
            IClienteRepository clienteRepo,
            IItemRepository itemRepo,
            IUsuarioRepository usuarioRepo,
            ISettingRepository settingRepo) : this()
        {
            // Asignar dependencias
            _user = user ?? throw new ArgumentNullException(nameof(user));
            _facturaService = facturaService ?? throw new ArgumentNullException(nameof(facturaService));
            _clienteRepo = clienteRepo ?? throw new ArgumentNullException(nameof(clienteRepo));
            _item_repo = itemRepo ?? throw new ArgumentNullException(nameof(itemRepo));
            _usuarioRepo = usuarioRepo ?? throw new ArgumentNullException(nameof(usuarioRepo));
            _settingRepo = settingRepo ?? throw new ArgumentNullException(nameof(settingRepo));

            // Ahora que InitializeComponent ya corrió, actualizamos UI dependiente de runtime
            Text = $"QuickPOS - {_user.Username} ({_user.Role})";
            if (lblUser != null)
                lblUser.Text = $"Usuario: {_user.Username}   Rol: {_user.Role}";
        }

        // OnLoad: ejecutar BuildTiles sólo en runtime (evita que el diseñador lo intente)
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (LicenseManager.UsageMode == LicenseUsageMode.Runtime)
            {
                BuildTiles();
            }
        }

        // BuildTiles: crea los tiles visuales y los añade a tilesPanel.
        // Se ejecuta solo en runtime, cuando las dependencias están disponibles.
        private void BuildTiles()
        {
            if (_facturaService == null || _item_repo == null || _clienteRepo == null)
            {
                // Si por alguna razón no están las dependencias, no crear tiles (evitar NRE).
                return;
            }

            tilesPanel.Controls.Clear();

            AddTile("Clientes", "clients.png", OpenClientes);
            AddTile("Items", "items.png", OpenItems);
            AddTile("Crear Factura", "invoice.png", OpenFacturacion);
            AddTile("Venta Rápida", "quicksale.png", () =>
            {
                using var f = new QuickSaleForm(_facturaService, _item_repo, _clienteRepo);
                f.ShowDialog();
            });

            if (IsAdmin())
            {
                AddTile("Usuarios", "users.png", OpenUsuarios);
                AddTile("Configuración", "settings.png", OpenConfiguracion);
            }
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            Close();
        }

        bool IsAdmin()
        {
            return _user != null && _user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        }

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

        void OpenClientes()
        {
            if (_clienteRepo == null) return;
            using var f = new ClientesForm(_clienteRepo);
            f.ShowDialog();
        }
        void OpenItems()
        {
            if (_item_repo == null) return;
            using var f = new ItemsForm(_item_repo);
            f.ShowDialog();
        }
        void OpenFacturacion()
        {
            if (_facturaService == null || _item_repo == null || _clienteRepo == null) return;
            using var f = new FacturacionForm(_facturaService, _item_repo, _clienteRepo);
            f.ShowDialog();
        }
        void OpenUsuarios()
        {
            if (_usuarioRepo == null) return;
            using var f = new UsuariosForm(_usuarioRepo);
            f.ShowDialog();
        } // usa el campo inyectado
        void OpenConfiguracion()
        {
            if (_settingRepo == null) return;
            using var f = new ConfigForm(_settingRepo);
            f.ShowDialog();
        } // usa el campo inyectado

        // ----------------- Design-time preview helpers (moved out of Designer file) -----------------
        /// <summary>
        /// Crea tiles de vista previa para que el diseñador muestre la UI.
        /// Se ejecuta sólo en modo DesignTime.
        /// </summary>
        private void CreateDesignTimeTiles_WithLayout()
        {
            if (tilesPanel == null) return;

            // Evitar que el designer haga renders intermedios
            tilesPanel.SuspendLayout();

            try
            {
                tilesPanel.Controls.Clear();

                for (int i = 0; i < 6; i++)
                {
                    var demoTile = new Panel
                    {
                        Width = 220,
                        Height = 170,
                        Margin = new Padding(12),
                        BackColor = Color.White,
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    var table = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
                    table.RowStyles.Add(new RowStyle(SizeType.Absolute, 110));
                    table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

                    var imgLabel = new Label
                    {
                        Text = "(img)",
                        Dock = DockStyle.Fill,
                        TextAlign = ContentAlignment.MiddleCenter,
                        AutoSize = false
                    };

                    var txtLabel = new Label
                    {
                        Text = i switch
                        {
                            0 => "Clientes",
                            1 => "Items",
                            2 => "Crear Factura",
                            3 => "Venta Rápida",
                            4 => "Usuarios",
                            _ => "Configuración",
                        },
                        Dock = DockStyle.Fill,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font("Segoe UI", 9, FontStyle.Bold)
                    };

                    table.Controls.Add(imgLabel, 0, 0);
                    table.Controls.Add(txtLabel, 0, 1);
                    demoTile.Controls.Add(table);

                    // Añadir al panel de tiles
                    tilesPanel.Controls.Add(demoTile);
                }
            }
            finally
            {
                // Forzar layout final y refresco en Designer
                tilesPanel.ResumeLayout(true);
                tilesPanel.PerformLayout();
                tilesPanel.Refresh();
            }
        }

    }
}
