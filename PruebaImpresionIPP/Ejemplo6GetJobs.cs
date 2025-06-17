using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SharpIpp;
using SharpIpp.Exceptions;
using SharpIpp.Models;
using SharpIpp.Protocol.Models;

class Ejemplo6GetJobs
{
    public static async Task RunAsync()
    {
        // --- CONFIGURACIÓN ---
        string printerUriString = "ipp://172.17.170.56:631/ipp/print";
        // ---------------------

        Console.Clear();
        Console.WriteLine("Ejemplo 6: Get-Jobs (Listar Trabajos de Impresión)");
        Console.WriteLine("==================================================");

        WhichJobs whichJobs;
        while (true)
        {
            Console.Write("\n¿Qué trabajos deseas ver? (1: No Completados, 2: Completados): ");
            var choice = Console.ReadLine();
            if (choice == "1") { whichJobs = WhichJobs.NotCompleted; break; }
            if (choice == "2") { whichJobs = WhichJobs.Completed; break; }
            Console.WriteLine("Opción no válida. Por favor, introduce 1 o 2.");
        }

        var client = new SharpIppClient();
        try
        {
            var request = new GetJobsRequest
            {
                PrinterUri = new Uri(printerUriString),
                WhichJobs = whichJobs,
                // Comentamos esta línea.
                // en impresoras que no tienen un sistema de usuarios configurado.
                // RequestingUserName = Environment.UserName 
            };

            Console.WriteLine($"\nConsultando trabajos en la impresora: {request.PrinterUri}...");
            var response = await client.GetJobsAsync(request);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("¡Consulta de trabajos exitosa!");
            Console.ResetColor();
            Console.WriteLine("------------------------------------------");

            if (response.Jobs.Any())
            {
                Console.WriteLine($"Se encontraron {response.Jobs.Length} trabajos:");
                foreach (var job in response.Jobs)
                {
                    Console.WriteLine($"  -> Job ID: {job.JobId}, Nombre: {job.JobName ?? "N/A"}, Estado: {job.JobState ?? 0}");
                }
            }
            else
            {
                Console.WriteLine("No se encontraron trabajos que coincidan con el filtro.");
            }
            Console.WriteLine("------------------------------------------");
        }
        catch (IppResponseException e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nError de IPP: La impresora respondió con un error. Código: {e.ResponseMessage.StatusCode}");
            Console.ResetColor();
            Console.WriteLine($"  -> Detalles: {e.Message}");
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