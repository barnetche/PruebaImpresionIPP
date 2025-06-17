using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using SharpIpp;
using SharpIpp.Exceptions;
using SharpIpp.Models;
using SharpIpp.Protocol.Models; // Necesario para el enum Sides

class Ejemplo5ValidateJob
{
    public static async Task RunAsync()
    {
        // --- CONFIGURACIÓN ---
        string printerUriString = "ipp://172.17.170.56:631/ipp/print";
        // ---------------------

        Console.Clear();
        Console.WriteLine("Ejemplo 5: Validate-Job");
        Console.WriteLine("=======================");
        Console.WriteLine("Esta operación comprueba si una impresora aceptaría un trabajo con ciertos atributos, sin imprimirlo.");

        var client = new SharpIppClient();

        try
        {
            // Para Validate-Job, se necesita un Stream, pero no se envían sus datos.
            // Un MemoryStream vacío es suficiente y no requiere un archivo físico.
            await using var stream = new MemoryStream(new byte[0]);

            var attributesToValidate = new NewJobAttributes
            {
                Copies = 2,
                Sides = Sides.TwoSidedLongEdge // Validar si la impresora puede imprimir a doble cara
            };

            var request = new ValidateJobRequest
            {
                PrinterUri = new Uri(printerUriString),
                Document = stream,
                NewJobAttributes = attributesToValidate
            };

            Console.WriteLine($"\nValidando los siguientes atributos en la impresora {printerUriString}:");
            Console.WriteLine($"  - Copias: {attributesToValidate.Copies}");
            Console.WriteLine($"  - Caras: {attributesToValidate.Sides}");

            await client.ValidateJobAsync(request);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nResultado: ¡Validación exitosa! La impresora acepta estos atributos.");
            Console.ResetColor();
        }
        catch (IppResponseException e)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nResultado: La validación ha fallado (lo cual es normal si un atributo no es soportado).");
            Console.ResetColor();
            Console.WriteLine($"  -> La impresora respondió con el estado: {e.ResponseMessage.StatusCode}");
            Console.WriteLine($"  -> Detalles: {e.Message}");
        }
        catch (HttpRequestException e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nError de Red: No se pudo conectar con la impresora.");
            Console.ResetColor();
            Console.WriteLine($"  -> Verifica que la URI '{printerUriString}' sea correcta y que la impresora esté accesible en la red.");
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nOcurrió un error inesperado: {e.GetType().Name}");
            Console.ResetColor();
            Console.WriteLine($"  -> Detalles: {e.Message}");
        }
        finally
        {
            Console.WriteLine("\nPresiona una tecla para volver al menú...");
            Console.ReadKey();
        }
    }
}