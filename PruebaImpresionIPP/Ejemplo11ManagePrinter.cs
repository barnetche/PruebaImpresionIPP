using System;
using System.Net.Http;
using System.Threading.Tasks;
using SharpIpp;
using SharpIpp.Exceptions;
using SharpIpp.Models;

class Ejemplo11ManagePrinter
{
    public static async Task RunAsync()
    {
        // --- CONFIGURACIÓN ---
        string printerUriString = "ipp://172.17.170.56:631/ipp/print";
        // ---------------------

        Console.Clear();
        Console.WriteLine("Ejemplo 11: Gestionar Impresora (Pause/Resume/Purge)");
        Console.WriteLine("=====================================================");
        Console.WriteLine("Este ejemplo demuestra cómo pausar, reanudar y purgar la cola de una impresora.");

        var client = new SharpIppClient();
        try
        {
            // --- 1. Pausar la Impresora ---
            Console.WriteLine($"\nEnviando orden para pausar la impresora en {printerUriString}...");
            var pauseRequest = new PausePrinterRequest { PrinterUri = new Uri(printerUriString) };
            await client.PausePrinterAsync(pauseRequest);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("¡Impresora pausada! No aceptará nuevos trabajos para impresión.");
            Console.ResetColor();

            Console.WriteLine("\nEsperando 5 segundos antes de reanudar...");
            await Task.Delay(5000);

            // --- 2. Reanudar la Impresora ---
            Console.WriteLine($"\nEnviando orden para reanudar la impresora...");
            var resumeRequest = new ResumePrinterRequest { PrinterUri = new Uri(printerUriString) };
            await client.ResumePrinterAsync(resumeRequest);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("¡Impresora reanudada! La cola de impresión vuelve a estar activa.");
            Console.ResetColor();

            Console.WriteLine("\nEsperando 5 segundos antes de purgar...");
            await Task.Delay(5000);

            // --- 3. Purgar todos los trabajos ---
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nADVERTENCIA: La siguiente acción eliminará TODOS los trabajos de la cola de la impresora.");
            Console.ResetColor();
            Console.Write("¿Estás seguro de que deseas continuar? (s/n): ");
            string confirmation = Console.ReadLine() ?? "";

            if (confirmation.Equals("s", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"\nEnviando orden para purgar todos los trabajos...");
                var purgeRequest = new PurgeJobsRequest { PrinterUri = new Uri(printerUriString) };
                await client.PurgeJobsAsync(purgeRequest);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("¡Todos los trabajos han sido eliminados de la impresora!");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("\nOperación de purgado cancelada por el usuario.");
            }
        }
        catch (IppResponseException e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nError de IPP: La impresora respondió con un error. Código: {e.ResponseMessage.StatusCode}");
            Console.ResetColor();
            if (e.ResponseMessage.StatusCode == SharpIpp.Protocol.Models.IppStatusCode.ServerErrorOperationNotSupported)
            {
                Console.WriteLine("  -> Causa probable: La impresora no soporta esta operación de gestión.");
            }
            else
            {
                Console.WriteLine($"  -> Detalles: {e.Message}");
            }
        }
        catch (HttpRequestException e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nError de Red: No se pudo conectar con la impresora.");
            Console.ResetColor();
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nOcurrió un error inesperado: {e.GetType().Name}");
            Console.ResetColor();
        }
        finally
        {
            Console.WriteLine("\nPresiona una tecla para volver al menú...");
            Console.ReadKey();
        }
    }
}