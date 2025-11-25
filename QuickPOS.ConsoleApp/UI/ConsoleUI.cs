using System;

namespace QuickPOS.UI;

public static class ConsoleUI
{
    public static void Clear()
    {
        Console.Clear();
    }

    public static void Header(string title)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine($"║   {title.PadRight(50)}║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
        Console.ResetColor();
    }

    public static void Option(int number, string text)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($" [{number}] ");
        Console.ResetColor();
        Console.WriteLine(text);
    }

    public static void Footer()
    {
        Console.WriteLine("──────────────────────────────────────────────────────────");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("Seleccione una opción: ");
        Console.ResetColor();
    }
}
