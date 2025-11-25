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

            // Mostrar info (opcional)
            Console.WriteLine("Probando configuración...");
            Console.WriteLine(Config.ConnectionString);

            // Fábrica y repositorios (no abren conexión hasta que se llamen)
            var factory = new SqlConnectionFactory(Config.ConnectionString);

            IItemRepository itemRepo = new ItemRepository(factory);
            IClienteRepository clienteRepo = new ClienteRepository(factory);
            IFacturaRepository facturaRepo = new FacturaRepository(factory);

            // Servicios
            var facturaService = new FacturaService(facturaRepo, itemRepo);

            // Auth (in-memory por ahora)
            var authService = new AuthService();

            // Login
            using var login = new LoginForm(authService);
            var dr = login.ShowDialog();
            if (dr != DialogResult.OK || login.AuthenticatedUser == null) return;

            var user = login.AuthenticatedUser;

            // Abrir MainForm con dependencias
            Application.Run(new MainForm(user, facturaService, clienteRepo, itemRepo));
        }
    }
}
