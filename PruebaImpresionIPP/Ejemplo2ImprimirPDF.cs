using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using SharpIpp;
using SharpIpp.Exceptions;
using SharpIpp.Models;
using SharpIpp.Protocol.Models;

class Ejemplo2ImprimirPDF
{
    public static async Task RunAsync()
    {
        string printerUriString = "ipp://172.17.170.56:631/ipp/print";
        string documentFileName = "test.pdf";

        Console.Clear();
        Console.WriteLine("Ejecutando Ejemplo 2: Imprimir un PDF con Atributos");
        Console.WriteLine("==================================================");

        var client = new SharpIppClient();
        try
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, documentFileName);
            if (!File.Exists(filePath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: No se encontró el archivo '{documentFileName}'.");
                Console.ResetColor();
                return;
            }

            await using var fileStream = File.OpenRead(filePath);
            var request = new PrintJobRequest
            {
                PrinterUri = new Uri(printerUriString),
                Document = fileStream,
                NewJobAttributes = new NewJobAttributes
                {
                    JobName = "Mi Primer Trabajo de Impresión",
                    Copies = 1,
                    Sides = Sides.OneSided
                }
            };

            Console.WriteLine($"Enviando trabajo a la impresora: {request.PrinterUri}...");
            var response = await client.PrintJobAsync(request);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("¡Trabajo enviado con éxito!");
            Console.ResetColor();
            Console.WriteLine($"  -> ID del Trabajo: {response.JobId}");
            Console.WriteLine($"  -> Estado del Trabajo: {response.JobState}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ha ocurrido un error: {e.Message}");
        }

        Console.WriteLine("\nPresiona una tecla para volver al menú...");
        Console.ReadKey();
    }
}