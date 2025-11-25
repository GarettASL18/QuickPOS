using System;
using System.Linq;
using QuickPOS.Data;
using QuickPOS.Models;
using QuickPOS.UI;

namespace QuickPOS.Presentation;

public class MenuPrincipal
{
    private readonly MenuFacturacion _menuFacturacion;
    private readonly IClienteRepository _clienteRepo;
    private readonly IItemRepository _itemRepo;
    private readonly User _currentUser;

    public MenuPrincipal(MenuFacturacion menuFacturacion, IClienteRepository clienteRepo, IItemRepository itemRepo, User currentUser)
    {
        _menuFacturacion = menuFacturacion ?? throw new ArgumentNullException(nameof(menuFacturacion));
        _clienteRepo = clienteRepo ?? throw new ArgumentNullException(nameof(clienteRepo));
        _itemRepo = itemRepo ?? throw new ArgumentNullException(nameof(itemRepo));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    public void Mostrar()
    {
        while (true)
        {
            ConsoleUI.Clear();
            ConsoleUI.Header($"QUICKPOS - Usuario: {_currentUser.Username}  Rol: {_currentUser.Role}");

            ConsoleUI.Option(1, "Mantenimiento de Clientes");

            // Items visible para ambos roles
            ConsoleUI.Option(2, "Mantenimiento de Items");

            // Facturación visible para ambos
            ConsoleUI.Option(3, "Crear Factura");

            // Opciones administrativas sólo para Admin
            if (IsAdmin())
            {
                ConsoleUI.Option(4, "Configuración (Admin)");
                ConsoleUI.Option(5, "Usuarios (Admin)");
            }

            ConsoleUI.Option(0, "Cerrar sesión / Salir");

            ConsoleUI.Footer();
            var op = Console.ReadLine();

            switch (op)
            {
                case "1":
                    MostrarClientesMenu();
                    break;
                case "2":
                    MostrarItemsMenu();
                    break;
                case "3":
                    _menuFacturacion.CrearFactura();
                    break;
                case "4" when IsAdmin():
                    MostrarAdminConfig();
                    break;
                case "5" when IsAdmin():
                    MostrarUsuariosAdmin();
                    break;
                case "0":
                    return; // salir (cerrar sesión)
                default:
                    Console.WriteLine("Opción inválida. Presione ENTER.");
                    Console.ReadLine();
                    break;
            }
        }
    }

    private bool IsAdmin() => _currentUser.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase);

    // Reusar métodos que ya tenías (Clientes/Items). Los dejo iguales.
    private void MostrarClientesMenu()
    {
        ConsoleUI.Clear();
        ConsoleUI.Header("CLIENTES");
        var clientes = _clienteRepo.GetAll();
        if (clientes == null || !clientes.Any()) Console.WriteLine("No hay clientes registrados.");
        else foreach (var c in clientes) Console.WriteLine($"{c.ClienteId,3} | {c.Nombre,-40} | NIT: {c.NIT}");

        Console.WriteLine();
        ConsoleUI.Option(1, "Crear nuevo cliente");
        ConsoleUI.Option(0, "Volver");
        ConsoleUI.Footer();
        var op = Console.ReadLine();
        if (op == "1")
        {
            Console.Write("Nombre: ");
            var nombre = Console.ReadLine() ?? "";
            Console.Write("NIT (opcional): ");
            var nit = Console.ReadLine();
            _clienteRepo.Create(new Cliente { Nombre = nombre, NIT = string.IsNullOrWhiteSpace(nit) ? null : nit });
            Console.WriteLine("Cliente creado. ENTER para continuar.");
            Console.ReadLine();
        }
    }

    private void MostrarItemsMenu()
    {
        ConsoleUI.Clear();
        ConsoleUI.Header("ITEMS");
        var items = _itemRepo.GetAll();
        if (items == null || !items.Any()) Console.WriteLine("No hay items registrados.");
        else
        {
            Console.WriteLine("Id  Nombre                                Precio   Activo");
            Console.WriteLine("------------------------------------------------------------");
            foreach (var i in items) Console.WriteLine($"{i.ItemId,3}  {i.Nombre,-36}  {i.Precio,7:0.00}   {(i.Activo ? "Sí" : "No")}");
        }
        Console.WriteLine("\nENTER para volver.");
        Console.ReadLine();
    }

    private void MostrarAdminConfig()
    {
        ConsoleUI.Clear();
        ConsoleUI.Header("CONFIGURACIÓN (Admin)");
        Console.WriteLine("Aquí podrías cambiar parámetros del sistema (Impuesto, etc.).");
        Console.WriteLine("Funcionalidad pendiente (se implementa cuando quieras).");
        Console.WriteLine("\nENTER para volver.");
        Console.ReadLine();
    }

    private void MostrarUsuariosAdmin()
    {
        ConsoleUI.Clear();
        ConsoleUI.Header("USUARIOS (Admin)");
        Console.WriteLine("Gestión de usuarios (actualmente in-memory).");
        Console.WriteLine("Funcionalidad pendiente para añadir/editar usuarios.");
        Console.WriteLine("\nENTER para volver.");
        Console.ReadLine();
    }
}
