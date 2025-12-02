using System;
using System.Windows.Forms;
using QuickPOS;
using QuickPOS.Data;
using QuickPOS.Services;
using QuickPOS.Models;

namespace QuickPOS.WinFormsApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Probar conexión rápida (opcional, deja para debug)
            try
            {
                using var cn = new Microsoft.Data.SqlClient.SqlConnection(Config.ConnectionString);
                cn.Open();
                Console.WriteLine("Conexión a SQL Server OK");
                cn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al abrir conexión: " + ex.Message);
            }

            Console.WriteLine("Probando configuración...");
            Console.WriteLine(Config.ConnectionString);

            // Fábrica y repositorios (no abren conexión hasta que se llamen)
            var factory = new SqlConnectionFactory(Config.ConnectionString);

            IItemRepository itemRepo = new ItemRepository(factory);
            IClienteRepository clienteRepo = new ClienteRepository(factory);
            IFacturaRepository facturaRepo = new FacturaRepository(factory);
            IUsuarioRepository usuarioRepo = new UsuarioRepository(factory);      // para Auth / Usuarios
            ISettingRepository settingRepo = new SettingRepository(factory);      // para Config (opcional)

            // Servicios
            var facturaService = new FacturaService(facturaRepo, itemRepo); // mantengo la firma que tienes ahora

            // AuthService ahora usa BD (UsuarioRepository)
            var authService = new AuthService(usuarioRepo);

            // Login
            using var login = new LoginForm(authService);
            var dr = login.ShowDialog();
            if (dr != DialogResult.OK || login.AuthenticatedUser == null) return;

            var user = login.AuthenticatedUser;

            // Abrir MainForm con dependencias (mantengo la firma actual de MainForm)
            Application.Run(new MainForm(user, facturaService, clienteRepo, itemRepo, usuarioRepo, settingRepo));
        }
    }
}
