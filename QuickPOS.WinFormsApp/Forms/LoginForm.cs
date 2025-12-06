using System;
using System.Drawing;
using System.IO; // Necesario para guardar "Recordarme"
using System.Runtime.InteropServices;
using System.Windows.Forms;
using QuickPOS.Services;
using QuickPOS.Models;

namespace QuickPOS.WinFormsApp.Forms
{
    public partial class LoginForm : Form
    {
        private readonly AuthService _auth;
        public User? AuthenticatedUser { get; private set; }

        // Archivo donde guardaremos el usuario recordado
        private readonly string _rememberFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "user_config.dat");

        // P/Invoke para Placeholders
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);
        private const int EM_SETCUEBANNER = 0x1501;

        public LoginForm()
        {
            InitializeComponent();
            _auth = null!;

            // Centrar tarjeta al iniciar
            CentrarPanel();

            // Conexiones de eventos manuales (Backend)
            btnLogin.Click += BtnLogin_Click;
            btnCancel.Click += BtnCancel_Click;
            pbTogglePass.Click += PbTogglePass_Click;
            lnkForgot.Click += LnkForgot_Click; // <--- AQUÍ ESTÁ EL CAMBIO IMPORTANTE

            this.AcceptButton = btnLogin;
            this.CancelButton = btnCancel;
        }

        public LoginForm(AuthService auth) : this()
        {
            _auth = auth ?? throw new ArgumentNullException(nameof(auth));
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            SetCue(txtUser, "Ej. juan.perez");
            SetCue(txtPass, "Su contraseña");

            // --- LÓGICA DE RECORDARME ---
            if (File.Exists(_rememberFile))
            {
                try
                {
                    string savedUser = File.ReadAllText(_rememberFile).Trim();
                    if (!string.IsNullOrEmpty(savedUser))
                    {
                        txtUser.Text = savedUser;
                        chkRemember.Checked = true;
                        txtPass.Select();
                        return;
                    }
                }
                catch { }
            }
            txtUser.Select();
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            if (_auth == null) return;

            btnLogin.Enabled = false;
            btnLogin.Text = "Verificando...";
            Application.DoEvents();

            System.Threading.Thread.Sleep(200); // Pequeña pausa estética

            var user = _auth.Authenticate(txtUser.Text?.Trim() ?? "", txtPass.Text ?? "");

            if (user != null)
            {
                // --- GUARDAR O BORRAR "RECORDARME" ---
                try
                {
                    if (chkRemember.Checked)
                        File.WriteAllText(_rememberFile, txtUser.Text.Trim());
                    else
                        if (File.Exists(_rememberFile)) File.Delete(_rememberFile);
                }
                catch { }

                AuthenticatedUser = user;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Credenciales incorrectas.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnLogin.Text = "INICIAR SESIÓN";
                btnLogin.Enabled = true;
                txtPass.Clear();
                txtPass.Focus();
            }
        }

        private void PbTogglePass_Click(object? sender, EventArgs e)
        {
            txtPass.UseSystemPasswordChar = !txtPass.UseSystemPasswordChar;
        }

        // --- AQUÍ ESTÁ LA CONEXIÓN CON TU NUEVA VENTANA ---
        private void LnkForgot_Click(object? sender, EventArgs e)
        {
            // Ocultamos el login momentáneamente para que se vea limpio
            this.Hide();

            // Abrimos el formulario de recuperación que acabas de diseñar
            using (var recovery = new RecoveryForm())
            {
                recovery.ShowDialog();
            }

            // Cuando cierren la recuperación, volvemos a mostrar el login
            this.Show();
        }

        // --- MÉTODOS VISUALES ---
        private void CentrarPanel()
        {
            if (card != null)
            {
                card.Location = new Point(
                    (this.ClientSize.Width - card.Width) / 2,
                    (this.ClientSize.Height - card.Height) / 2
                );
            }
        }

        private void LoginForm_Resize(object sender, EventArgs e) => CentrarPanel();

        private void SetCue(TextBox tb, string cue)
        {
            try { SendMessage(tb.Handle, EM_SETCUEBANNER, (IntPtr)1, cue); } catch { }
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void lblTitle_Click(object sender, EventArgs e) { }
    }
}