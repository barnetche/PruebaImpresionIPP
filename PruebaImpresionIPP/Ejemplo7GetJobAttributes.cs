using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SharpIpp;
using SharpIpp.Exceptions;
using SharpIpp.Models;

class Ejemplo7GetJobAttributes
{
    public static async Task RunAsync()
    {
        // --- CONFIGURACIÓN ---
        string printerUriString = "ipp://172.17.170.56:631/ipp/print";
        // ---------------------

        Console.Clear();
        Console.WriteLine("Ejemplo 7: Get-Job-Attributes");
        Console.WriteLine("=============================");
        Console.WriteLine("Esta operación obtiene los atributos detallados de un trabajo de impresión específico.");

        int jobId;
        while (true)
        {
            Console.Write("\nPor favor, introduce el ID del trabajo que deseas consultar: ");
            if (int.TryParse(Console.ReadLine(), out jobId))
            {
                break;
            }
            Console.WriteLine("ID no válido. Debe ser un número entero.");
        }

        var client = new SharpIppClient();
        try
        {
            var request = new GetJobAttributesRequest
            {
                PrinterUri = new Uri(printerUriString),
                JobId = jobId
            };

            Console.WriteLine($"\nConsultando atributos del Job ID {jobId}...");
            var response = await client.GetJobAttributesAsync(request);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("¡Atributos del trabajo recibidos con éxito!");
            Console.ResetColor();
            Console.WriteLine("------------------------------------------");
            var job = response.JobAttributes;
            Console.WriteLine($"  -> Job ID: {job.JobId}");
            Console.WriteLine($"  -> Nombre del Job: {job.JobName ?? "N/A"}");
            Console.WriteLine($"  -> Estado: {job.JobState}");
            Console.WriteLine($"  -> Razones del Estado: {string.Join(", ", job.JobStateReasons ?? Array.Empty<string>())}");
            Console.WriteLine($"  -> Creado por: {job.JobOriginatingUserName ?? "N/A"}");
            Console.WriteLine($"  -> Páginas completadas: {job.JobImpressionsCompleted}");
            Console.WriteLine($"  -> K-Octetos procesados: {job.JobKOctetsProcessed}");
            Console.WriteLine($"  -> Creado en: {job.DateTimeAtCreation}");
            Console.WriteLine("------------------------------------------");
        }
        catch (IppResponseException e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nError de IPP: La impresora respondió con un error. Código: {e.ResponseMessage.StatusCode}");
            Console.ResetColor();
            // Un error común aquí es 'ClientErrorNotFound' si el Job ID no existe.
            if (e.ResponseMessage.StatusCode == SharpIpp.Protocol.Models.IppStatusCode.ClientErrorNotFound)
            {
                Console.WriteLine("  -> Causa probable: El Job ID introducido no existe en la impresora.");
            }
        }
        catch (HttpRequestException e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nError de Red: No se pudo conectar con la impresora.");
            Console.ResetColor();
            Console.WriteLine($"  -> Verifica que la URI '{printerUriString}' sea correcta.");
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