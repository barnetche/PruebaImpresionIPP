using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using SharpIpp;
using SharpIpp.Exceptions;
using SharpIpp.Models;
using SharpIpp.Protocol.Models;

class Ejemplo1ImprimirUri
{
    public static async Task RunAsync()
    {
        string printerUriString = "ipp://172.17.170.56:631/ipp/print";
        string documentFileName = "test.pdf";

        Console.Clear();
        Console.WriteLine("Ejecutando Ejemplo 1: Imprimir PDF (Básico)");
        Console.WriteLine("==========================================");

        var client = new SharpIppClient();
        try
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, documentFileName);
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error: El archivo '{documentFileName}' no se encontró...");
                return;
            }

            await using var fileStream = File.OpenRead(filePath);
            var request = new PrintJobRequest
            {
                PrinterUri = new Uri(printerUriString),
                Document = fileStream,
                NewJobAttributes = new NewJobAttributes { JobName = "Mi Prueba desde SharpIpp" },
                DocumentAttributes = new DocumentAttributes { DocumentName = documentFileName }
            };

            Console.WriteLine($"Enviando '{documentFileName}' a la impresora: {printerUriString}");
            var response = await client.PrintJobAsync(request);

            Console.WriteLine("¡Trabajo enviado exitosamente!");
            Console.WriteLine($"  - ID del Trabajo (JobId): {response.JobId}");
            Console.WriteLine($"  - Estado del Trabajo: {response.JobState}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ha ocurrido un error: {e.Message}");
        }

        // Eliminamos el Console.ReadKey() de aquí, el menú principal lo manejará
        Console.WriteLine("\nPresiona una tecla para volver al menú...");
        Console.ReadKey();
    }
}