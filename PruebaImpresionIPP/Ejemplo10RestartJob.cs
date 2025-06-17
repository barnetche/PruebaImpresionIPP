using System;
using System.Net.Http;
using System.Threading.Tasks;
using SharpIpp;
using SharpIpp.Exceptions;
using SharpIpp.Models;

class Ejemplo10RestartJob
{
    public static async Task RunAsync()
    {
        // --- CONFIGURACIÓN ---
        string printerUriString = "ipp://172.17.170.56:631/ipp/print";
        // ---------------------

        Console.Clear();
        Console.WriteLine("Ejemplo 10: Restart-Job");
        Console.WriteLine("========================");
        Console.WriteLine("Esta operación reinicia un trabajo que ya se completó y fue retenido en el historial.");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("ADVERTENCIA: Muchas impresoras no soportan esta operación.");
        Console.ResetColor();

        int jobId;
        while (true)
        {
            Console.Write("\nPor favor, introduce el ID del trabajo completado que deseas reiniciar: ");
            if (int.TryParse(Console.ReadLine(), out jobId))
            {
                break;
            }
            Console.WriteLine("ID no válido. Debe ser un número entero.");
        }

        var client = new SharpIppClient();
        try
        {
            var request = new RestartJobRequest
            {
                PrinterUri = new Uri(printerUriString),
                JobId = jobId
            };

            Console.WriteLine($"\nEnviando orden para reiniciar el Job ID {jobId}...");

            await client.RestartJobAsync(request);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n¡Trabajo {jobId} reiniciado con éxito!");
            Console.ResetColor();
            Console.WriteLine("El trabajo volverá a la cola de impresión en estado 'pending'.");
        }
        catch (IppResponseException e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nError de IPP: La impresora respondió con un error. Código: {e.ResponseMessage.StatusCode}");
            Console.ResetColor();
            if (e.ResponseMessage.StatusCode == SharpIpp.Protocol.Models.IppStatusCode.ServerErrorOperationNotSupported)
            {
                Console.WriteLine("  -> Causa probable: La impresora no soporta la operación Restart-Job.");
            }
            else if (e.ResponseMessage.StatusCode == SharpIpp.Protocol.Models.IppStatusCode.ClientErrorNotFound)
            {
                Console.WriteLine("  -> Causa probable: El Job ID no existe o ya fue purgado del historial.");
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
            Console.WriteLine($"  -> Detalles: {e.Message}");
        }
        finally
        {
            Console.WriteLine("\nPresiona una tecla para volver al menú...");
            Console.ReadKey();
        }
    }
}