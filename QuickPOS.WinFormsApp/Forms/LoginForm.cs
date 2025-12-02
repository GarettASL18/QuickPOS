using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using QuickPOS.Services;
using QuickPOS.Models;

namespace QuickPOS.WinFormsApp
{
    public class LoginForm : Form
    {
        private readonly AuthService _auth;
        public User? AuthenticatedUser { get; private set; }

        TextBox txtUser, txtPass;
        Button btnLogin, btnCancel;
        CheckBox chkRemember;
        PictureBox pbTogglePassword;

        // P/Invoke to set cue banner (placeholder) on TextBox
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);
        private const int EM_SETCUEBANNER = 0x1501;

        public LoginForm(AuthService auth)
        {
            _auth = auth ?? throw new ArgumentNullException(nameof(auth));
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form
            Text = "QuickPOS - Iniciar sesión";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(900, 600);
            MinimumSize = new Size(800, 520);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.White;

            // Background image (place image at project/images/login_bg.jpg -> Copy if newer)
            var pbBackground = new PictureBox
            {
                Dock = DockStyle.Fill,
                ImageLocation = "images\\login_bg.jpg", // ruta relativa al ejecutable / proyecto
                SizeMode = PictureBoxSizeMode.StretchImage,
            };
            Controls.Add(pbBackground);

            // Dark translucent overlay (drawn as a Panel with custom paint to support rounded card appearance)
            var card = new RoundedPanel
            {
                Width = 460,
                Height = 360,
                BackColor = Color.FromArgb(220, Color.White),
                CornerRadius = 18,
                Shadow = true,
                Anchor = AnchorStyles.None
            };

            // Center the card in the form
            card.Location = new Point((ClientSize.Width - card.Width) / 2, (ClientSize.Height - card.Height) / 2);
            // reposition center when resizing
            Resize += (s, e) =>
            {
                card.Location = new Point((ClientSize.Width - card.Width) / 2, (ClientSize.Height - card.Height) / 2);
            };

            // Add card on top of background
            pbBackground.Controls.Add(card); // parent is background to keep visual stacking

            // Title label
            var lblTitle = new Label
            {
                Text = "Bienvenido a QuickPOS",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 60
            };
            card.Controls.Add(lblTitle);

            // Subtitle or small description
            var lblSub = new Label
            {
                Text = "Inicie sesión para continuar",
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 24
            };
            card.Controls.Add(lblSub);

            // Container for inputs
            var pnlInputs = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(18, 8, 18, 18),
                ColumnCount = 1,
                RowCount = 6,
            };
            pnlInputs.RowStyles.Add(new RowStyle(SizeType.Absolute, 12)); // spacer
            pnlInputs.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // usuario
            pnlInputs.RowStyles.Add(new RowStyle(SizeType.Absolute, 12));
            pnlInputs.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // contraseña
            pnlInputs.RowStyles.Add(new RowStyle(SizeType.Absolute, 34)); // remember
            pnlInputs.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // botones

            // Labels and textboxes
            var lblUser = new Label
            {
                Text = "Usuario",
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomLeft
            };
            pnlInputs.Controls.Add(lblUser, 0, 0);
            txtUser = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Dock = DockStyle.Fill,
                Height = 30
            };
            pnlInputs.Controls.Add(txtUser, 0, 1);

            var lblPass = new Label
            {
                Text = "Contraseña",
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomLeft
            };
            pnlInputs.Controls.Add(lblPass, 0, 2);

            // password panel to host textbox + toggle icon
            var passwordHost = new Panel { Dock = DockStyle.Fill, Height = 30 };
            txtPass = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Dock = DockStyle.Fill,
                UseSystemPasswordChar = true
            };
            pbTogglePassword = new PictureBox
            {
                Size = new Size(28, 24),
                Anchor = AnchorStyles.Right,
                Cursor = Cursors.Hand,
                SizeMode = PictureBoxSizeMode.CenterImage,
                Image = SystemIcons.Application.ToBitmap() // placeholder icon; replace with eye icon in resources
            };
            pbTogglePassword.Click += (s, e) =>
            {
                txtPass.UseSystemPasswordChar = !txtPass.UseSystemPasswordChar;
            };
            passwordHost.Controls.Add(txtPass);
            passwordHost.Controls.Add(pbTogglePassword);
            // position toggle to right
            pbTogglePassword.Location = new Point(passwordHost.Width - pbTogglePassword.Width - 2, (passwordHost.Height - pbTogglePassword.Height) / 2);
            passwordHost.Resize += (s, e) =>
            {
                pbTogglePassword.Location = new Point(passwordHost.Width - pbTogglePassword.Width - 2, (passwordHost.Height - pbTogglePassword.Height) / 2);
            };

            pnlInputs.Controls.Add(passwordHost, 0, 3);

            // Remember me and forgot (simple)
            var bottomRow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
            chkRemember = new CheckBox { Text = "Recordarme", AutoSize = true, Font = new Font("Segoe UI", 9F) };
            bottomRow.Controls.Add(chkRemember);
            var lnkForgot = new LinkLabel { Text = "¿Olvidaste tu contraseña?", AutoSize = true, Font = new Font("Segoe UI", 9F) };
            lnkForgot.Click += (s, e) => MessageBox.Show("Función de recuperar contraseña aún no implementada.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            bottomRow.Controls.Add(lnkForgot);
            pnlInputs.Controls.Add(bottomRow, 0, 4);

            // Buttons
            var btns = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, AutoSize = false, Height = 48 };
            btnLogin = new Button
            {
                Text = "Iniciar sesión",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                AutoSize = true,
                Padding = new Padding(12, 6, 12, 6)
            };
            btnLogin.Click += BtnLogin_Click;
            btnCancel = new Button
            {
                Text = "Cancelar",
                Font = new Font("Segoe UI", 10F),
                AutoSize = true,
                Padding = new Padding(12, 6, 12, 6)
            };
            btnCancel.Click += (_, __) => { DialogResult = DialogResult.Cancel; Close(); };
            btns.Controls.Add(btnLogin);
            btns.Controls.Add(btnCancel);
            pnlInputs.Controls.Add(btns, 0, 5);

            // Add inputs container after title/subtitle
            card.Controls.Add(pnlInputs);
            pnlInputs.BringToFront();

            // Set placeholders (cue banners)
            SetCue(txtUser, "ej. juan.perez");
            SetCue(txtPass, "Tu contraseña");

            // Keyboard shortcuts
            AcceptButton = btnLogin;
            CancelButton = btnCancel;

            // small polish: shadow on card done in RoundedPanel; set tab order
            txtUser.TabIndex = 0;
            txtPass.TabIndex = 1;
            chkRemember.TabIndex = 2;
            btnLogin.TabIndex = 3;
            btnCancel.TabIndex = 4;
        }

        private void SetCue(TextBox tb, string cue)
        {
            // EM_SETCUEBANNER works on Vista+; third param IntPtr.Zero to show cue when not focused.
            SendMessage(tb.Handle, EM_SETCUEBANNER, (IntPtr)1, cue);
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            var user = _auth.Authenticate(txtUser.Text?.Trim() ?? string.Empty, txtPass.Text ?? string.Empty);
            if (user == null)
            {
                MessageBox.Show("Credenciales inválidas", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            AuthenticatedUser = user;
            DialogResult = DialogResult.OK;
            Close();
        }
    }

    // Custom rounded panel with optional shadow and smooth corners
    public class RoundedPanel : Panel
    {
        public int CornerRadius { get; set; } = 15;
        public bool Shadow { get; set; } = false;
        public RoundedPanel()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = ClientRectangle;

            // draw shadow
            if (Shadow)
            {
                using (var shadowBrush = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
                {
                    var shadowRect = new Rectangle(rect.X + 4, rect.Y + 6, rect.Width - 8, rect.Height - 6);
                    using (var path = RoundedRect(shadowRect, CornerRadius + 2))
                    {
                        e.Graphics.FillPath(shadowBrush, path);
                    }
                }
            }

            // draw background rounded
            using (var bgBrush = new SolidBrush(BackColor))
            {
                using (var path = RoundedRect(rect, CornerRadius))
                {
                    e.Graphics.FillPath(bgBrush, path);
                }
            }
        }

        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            var d = radius * 2;
            path.AddArc(bounds.Left, bounds.Top, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Top, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
