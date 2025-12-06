using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using QuickPOS.Data;
using QuickPOS.Models;
using QuickPOS.Services;

namespace QuickPOS.WinFormsApp.Forms
{
    public partial class RecoveryForm : Form
    {
        // Quitamos el 'readonly' para poder iniciarlos después
        private UsuarioRepository _userRepo;
        private EmailService _emailService;

        private string _generatedCode = "";
        private Usuario _currentUser = null;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);
        private const int EM_SETCUEBANNER = 0x1501;

        public RecoveryForm()
        {
            InitializeComponent();

            // --- AQUÍ ESTABA EL PROBLEMA ---
            // Eliminamos toda conexión a BD de aquí.
            // El constructor ahora solo toca cosas visuales.

            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "QuickPOS - Recuperar Contraseña";

            // Centrar
            CentrarPanel();

            // Eventos
            this.Resize += (s, e) => CentrarPanel();
            this.Load += RecoveryForm_Load;

            btnSend.Click += BtnSend_Click;
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;
            this.CancelButton = btnCancel;
        }

        private void RecoveryForm_Load(object? sender, EventArgs e)
        {
            // Solo configuración visual ligera
            txtCode.Text = "";
            txtNewPass.Text = "";

            SetCue(txtUser, "Ej. juan.perez");
            SetCue(txtCode, "Código de 6 dígitos");
            SetCue(txtNewPass, "Nueva contraseña segura");
        }

        // --- MÉTODO SEGURO PARA INICIAR SERVICIOS ---
        // Este método solo se llamará cuando el usuario haga clic, 
        // nunca cuando el diseñador esté abierto.
        private void InicializarServicios()
        {
            if (_userRepo == null) // Solo si no se han iniciado
            {
                var factory = new SqlConnectionFactory(Config.ConnectionString);
                _userRepo = new UsuarioRepository(factory);
                _emailService = new EmailService();
            }
        }

        private void BtnSend_Click(object? sender, EventArgs e)
        {
            // 1. INICIALIZAMOS AQUÍ (El diseñador nunca llega a esta línea)
            try
            {
                InicializarServicios();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar con la base de datos: " + ex.Message);
                return;
            }

            // 2. Lógica normal
            string username = txtUser.Text.Trim();
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Ingresa tu usuario.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var user = _userRepo.GetByUsername(username);

            if (user == null)
            {
                MessageBox.Show("Usuario no encontrado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(user.Email))
            {
                MessageBox.Show("Este usuario no tiene correo configurado.", "Sin Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnSend.Enabled = false;
            btnSend.Text = "Enviando...";
            Application.DoEvents();

            Random rand = new Random();
            _generatedCode = rand.Next(100000, 999999).ToString();

            bool enviado = _emailService.SendRecoveryCode(user.Email, _generatedCode);

            if (enviado)
            {
                _currentUser = user;
                MessageBox.Show($"Código enviado a: {OcultarCorreo(user.Email)}", "¡Enviado!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtUser.Enabled = false;
                btnSend.Enabled = false;
                btnSend.Text = "CÓDIGO ENVIADO";
                btnSend.BackColor = Color.Gray;

                txtCode.Enabled = true;
                txtNewPass.Enabled = true;
                btnSave.Enabled = true;
                btnSave.BackColor = Color.FromArgb(0, 120, 215);

                txtCode.Focus();
            }
            else
            {
                MessageBox.Show("Error de conexión al enviar el correo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSend.Enabled = true;
                btnSend.Text = "ENVIAR CÓDIGO";
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            // Aseguramos servicios iniciados por si acaso
            InicializarServicios();

            if (txtCode.Text.Trim() != _generatedCode)
            {
                MessageBox.Show("El código es incorrecto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(txtNewPass.Text.Trim()))
            {
                MessageBox.Show("Ingresa una contraseña.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _currentUser.PasswordHash = txtNewPass.Text.Trim();
                _userRepo.Update(_currentUser);

                MessageBox.Show("¡Contraseña actualizada con éxito!", "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error guardando en BD: " + ex.Message);
            }
        }

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

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private string OcultarCorreo(string email)
        {
            try
            {
                var parts = email.Split('@');
                if (parts[0].Length > 3)
                    return parts[0].Substring(0, 3) + "***@" + parts[1];
                return email;
            }
            catch { return email; }
        }

        private void SetCue(TextBox tb, string cue)
        {
            try { SendMessage(tb.Handle, EM_SETCUEBANNER, (IntPtr)1, cue); } catch { }
        }
    }
}