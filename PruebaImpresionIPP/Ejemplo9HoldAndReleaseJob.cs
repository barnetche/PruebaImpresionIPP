using System;
using System.Net.Http;
using System.Threading.Tasks;
using SharpIpp;
using SharpIpp.Exceptions;
using SharpIpp.Models;

class Ejemplo9HoldAndReleaseJob
{
    public static async Task RunAsync()
    {
        // --- CONFIGURACIÓN ---
        string printerUriString = "ipp://172.17.170.56:631/ipp/print";
        // ---------------------

        Console.Clear();
        Console.WriteLine("Ejemplo 9: Hold-Job y Release-Job");
        Console.WriteLine("=================================");
        Console.WriteLine("Esta operación primero retiene un trabajo en la cola y luego lo libera.");

        int jobId;
        while (true)
        {
            Console.Write("\nPor favor, introduce el ID del trabajo que deseas retener y liberar: ");
            if (int.TryParse(Console.ReadLine(), out jobId))
            {
                break;
            }
            Console.WriteLine("ID no válido. Debe ser un número entero.");
        }

        var client = new SharpIppClient();
        try
        {
            // --- Parte 1: Retener el Trabajo (Hold-Job) ---
            var holdRequest = new HoldJobRequest { PrinterUri = new Uri(printerUriString), JobId = jobId };
            Console.WriteLine($"\nEnviando orden para retener el Job ID {jobId}...");
            await client.HoldJobAsync(holdRequest);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"¡Trabajo {jobId} puesto en espera (held) exitosamente!");
            Console.ResetColor();

            // --- Pausa para simular un tiempo de espera ---
            Console.WriteLine("\nEl trabajo está retenido. Se liberará en 5 segundos...");
            await Task.Delay(5000);

            // --- Parte 2: Liberar el Trabajo (Release-Job) ---
            var releaseRequest = new ReleaseJobRequest { PrinterUri = new Uri(printerUriString), JobId = jobId };
            Console.WriteLine($"\nEnviando orden para liberar el Job ID {jobId}...");
            await client.ReleaseJobAsync(releaseRequest);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"¡Trabajo {jobId} liberado exitosamente! Ahora es elegible para impresión.");
            Console.ResetColor();
        }
        catch (IppResponseException e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nError de IPP: La impresora respondió con un error. Código: {e.ResponseMessage.StatusCode}");
            Console.ResetColor();
            if (e.ResponseMessage.StatusCode == SharpIpp.Protocol.Models.IppStatusCode.ServerErrorOperationNotSupported)
            {
                Console.WriteLine("  -> Causa probable: La impresora no soporta las operaciones Hold-Job o Release-Job.");
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