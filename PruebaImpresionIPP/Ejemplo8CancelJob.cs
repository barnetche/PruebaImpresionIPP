using System;
using System.Net.Http;
using System.Threading.Tasks;
using SharpIpp;
using SharpIpp.Exceptions;
using SharpIpp.Models;

class Ejemplo8CancelJob
{
    public static async Task RunAsync()
    {
        // --- CONFIGURACIÓN ---
        string printerUriString = "ipp://172.17.170.56:631/ipp/print";
        // ---------------------

        Console.Clear();
        Console.WriteLine("Ejemplo 8: Cancel-Job");
        Console.WriteLine("=====================");
        Console.WriteLine("Esta operación cancela un trabajo de impresión que esté en la cola.");
        Console.WriteLine("Nota: Para probarlo, primero envía una impresión y rápidamente ejecuta este ejemplo.");

        int jobId;
        while (true)
        {
            Console.Write("\nPor favor, introduce el ID del trabajo que deseas cancelar: ");
            if (int.TryParse(Console.ReadLine(), out jobId))
            {
                break;
            }
            Console.WriteLine("ID no válido. Debe ser un número entero.");
        }

        var client = new SharpIppClient();
        try
        {
            var request = new CancelJobRequest
            {
                PrinterUri = new Uri(printerUriString),
                JobId = jobId,
                // RequestingUserName = Environment.UserName // Se comenta para evitar errores de BadRequest
            };

            Console.WriteLine($"\nEnviando orden para cancelar el Job ID {jobId}...");

            // Envía la solicitud de cancelación
            await client.CancelJobAsync(request);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n¡Solicitud de cancelación para el trabajo {jobId} enviada con éxito!");
            Console.ResetColor();
            Console.WriteLine("El trabajo pasará al estado 'Canceled'. Puedes verificarlo con el Ejemplo 6.");
        }
        catch (IppResponseException e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nError de IPP: La impresora respondió con un error. Código: {e.ResponseMessage.StatusCode}");
            Console.ResetColor();
            if (e.ResponseMessage.StatusCode == SharpIpp.Protocol.Models.IppStatusCode.ClientErrorNotFound)
            {
                Console.WriteLine("  -> Causa probable: El Job ID no existe, o el trabajo ya se completó o canceló.");
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
            Console.WriteLine($"  -> Detalles: {e.Message}");
        }
        finally
        {
            Console.WriteLine("\nPresiona una tecla para volver al menú...");
            Console.ReadKey();
        }
    }
}