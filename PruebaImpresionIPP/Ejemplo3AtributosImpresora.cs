using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using SharpIpp;
using SharpIpp.Exceptions;
using SharpIpp.Models;

class Ejemplo3AtributosImpresora
{
    public static async Task RunAsync()
    {
        string printerUriString = "ipp://172.17.170.56:631/ipp/print";

        Console.Clear();
        Console.WriteLine("Ejecutando Ejemplo 3: Get-Printer-Attributes");
        Console.WriteLine("============================================");

        var client = new SharpIppClient();
        try
        {
            var request = new GetPrinterAttributesRequest
            {
                PrinterUri = new Uri(printerUriString)
            };

            Console.WriteLine($"Consultando atributos de la impresora: {request.PrinterUri}...");
            var response = await client.GetPrinterAttributesAsync(request);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("¡Atributos recibidos con éxito!");
            Console.ResetColor();
            Console.WriteLine("------------------------------------------");
            Console.WriteLine($"  -> Nombre de la Impresora: {response.PrinterName}");
            Console.WriteLine($"  -> Estado de la Impresora: {response.PrinterState}");
            Console.WriteLine($"  -> Formatos Soportados: {string.Join(", ", response.DocumentFormatSupported ?? Array.Empty<string>())}");
            Console.WriteLine("------------------------------------------");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ha ocurrido un error: {e.Message}");
        }

        Console.WriteLine("\nPresiona una tecla para volver al menú...");
        Console.ReadKey();
    }
}