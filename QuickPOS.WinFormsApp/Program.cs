using System;
using System.Windows.Forms;
using QuickPOS;
using QuickPOS.Data;
using QuickPOS.Models;
using QuickPOS.Services;
using QuickPOS.WinFormsApp.Forms;

namespace QuickPOS.WinFormsApp
{
    internal static class Program
    {
        /// <summary>
        ///  Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 1. Configuración visual básica
            ApplicationConfiguration.Initialize();

            // 2. Prueba rápida de conexión
            try
            {
                using var cn = new Microsoft.Data.SqlClient.SqlConnection(Config.ConnectionString);
                cn.Open();
                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error crítico al conectar con la Base de Datos:\n{ex.Message}",
                                "Error de Inicio", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 3. INYECCIÓN DE DEPENDENCIAS (El "Motor")
            var factory = new SqlConnectionFactory(Config.ConnectionString);

            // Repositorios
            IItemRepository itemRepo = new ItemRepository(factory);
            IClienteRepository clienteRepo = new ClienteRepository(factory);
            IFacturaRepository facturaRepo = new FacturaRepository(factory);
            IUsuarioRepository usuarioRepo = new UsuarioRepository(factory);
            ISettingRepository settingRepo = new SettingRepository(factory);

            // Servicios
            var facturaService = new FacturaService(facturaRepo, itemRepo);
            var authService = new AuthService(usuarioRepo);

            // 4. BUCLE DE LA APLICACIÓN (Para permitir Logout)
            bool mantenerAbierto = true;

            while (mantenerAbierto)
            {
                // Paso A: Mostrar Login
                using (var login = new LoginForm(authService))
                {
                    var resultado = login.ShowDialog();

                    // Si el usuario cierra el login o cancela, cerramos la app.
                    if (resultado != DialogResult.OK || login.AuthenticatedUser == null)
                    {
                        mantenerAbierto = false;
                        break;
                    }

                    // Paso B: Si el login fue exitoso, abrimos el Dashboard
                    var mainForm = new MainForm(
                        login.AuthenticatedUser,
                        facturaService,
                        clienteRepo,
                        itemRepo,
                        usuarioRepo,
                        settingRepo,
                        facturaRepo // <--- ¡Importante! Agregado para el Historial de Ventas
                    );

                    Application.Run(mainForm);

                    // Paso C: Al cerrar el Dashboard, verificamos por qué se cerró
                    if (!mainForm.IsLogout)
                    {
                        // Si NO fue un logout voluntario (dio a la X), cerramos el bucle.
                        mantenerAbierto = false;
                    }
                    // Si FUE un logout (IsLogout == true), el bucle se repite y vuelve al Login.
                }
            }
        }
    }
}