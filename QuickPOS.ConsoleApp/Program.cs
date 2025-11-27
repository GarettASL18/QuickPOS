using System;
using QuickPOS;
using QuickPOS.Data;
using QuickPOS.Services;
using QuickPOS.Presentation;

Console.WriteLine("Probando configuración...");
Console.WriteLine(Config.ConnectionString);

// Inicializar fábrica y repositorios (no abren conexión todavía)
var factory = new SqlConnectionFactory(Config.ConnectionString);
IItemRepository itemRepo = new ItemRepository(factory);
IClienteRepository clienteRepo = new ClienteRepository(factory);
IFacturaRepository facturaRepo = new FacturaRepository(factory);

// Services
var facturaService = new FacturaService(facturaRepo, itemRepo);

// Crear repositorio de usuarios
IUsuarioRepository usuarioRepo = new UsuarioRepository(factory);

// Auth usando base de datos
var authService = new AuthService(usuarioRepo);

var login = new LoginMenu(authService);


// Loop principal de login / sesión
while (true)
{
    var user = login.Show();
    if (user == null)
    {
        Console.WriteLine("Saliendo de la aplicación. ENTER para cerrar.");
        Console.ReadLine();
        break;
    }

    // Inyectar dependencias y arrancar menú principal para el usuario autenticado
    var menuFact = new MenuFacturacion(facturaService, itemRepo);
    var menu = new MenuPrincipal(menuFact, clienteRepo, itemRepo, user);

    try
    {
        menu.Mostrar();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error durante la sesión: {ex.Message}");
        Console.ReadLine();
    }

    // Al volver del menu, la sesión se cierra; volver al login
}
