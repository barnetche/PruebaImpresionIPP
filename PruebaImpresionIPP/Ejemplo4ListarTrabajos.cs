using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SharpIpp;
using SharpIpp.Exceptions;
using SharpIpp.Models;
using SharpIpp.Protocol.Models; // Necesario para el enum WhichJobs

class Ejemplo4ListarTrabajos
{
    public static async Task RunAsync()
    {
        // --- CONFIGURACIÓN ---
        string printerUriString = "ipp://172.17.170.56:631/ipp/print";
        // ---------------------

        Console.Clear();
        Console.WriteLine("Ejemplo 4: Get-Jobs (Listar Trabajos de Impresión)");
        Console.WriteLine("==================================================");
        Console.WriteLine("Esta operación obtiene una lista de trabajos de una impresora, filtrando por su estado.");

        // Hacemos el ejemplo interactivo
        WhichJobs whichJobs;
        while (true)
        {
            Console.Write("\n¿Qué trabajos deseas ver? (1: No Completados, 2: Completados): ");
            var choice = Console.ReadLine();
            if (choice == "1")
            {
                whichJobs = WhichJobs.NotCompleted;
                break;
            }
            if (choice == "2")
            {
                whichJobs = WhichJobs.Completed;
                break;
            }
            Console.WriteLine("Opción no válida. Por favor, introduce 1 o 2.");
        }

        var client = new SharpIppClient();
        try
        {
            var request = new GetJobsRequest
            {
                PrinterUri = new Uri(printerUriString),
                WhichJobs = whichJobs,
                // Opcional: puedes pedir que solo te muestre tus trabajos
                // MyJobs = true, 
                // RequestingUserName = "tu_usuario"
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
                    // La respuesta de GetJobs puede no traer todos los atributos,
                    // algunos pueden ser nulos si no se solicitan explícitamente.
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