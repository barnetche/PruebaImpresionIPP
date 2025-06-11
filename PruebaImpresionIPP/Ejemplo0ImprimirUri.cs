// Asegúrate de tener estas directivas using al principio del archivo
using SharpIpp;
using SharpIpp.Exceptions;
using SharpIpp.Models;
using System.IO;
using System;
using System.Threading.Tasks;
using SharpIpp.Protocol.Models;

// El código principal de tu aplicación de consola
class Ejemplo0ImprimirUri
{
    static async Task Main(string[] args)
    {
        // --- CONFIGURACIÓN: Modifica estos valores si es necesario ---
        string printerUriString = "ipp://172.17.170.56:631/ipp/print";
        string documentFileName = "test.pdf";
        // ---------------------------------------------------------

        Console.WriteLine("Iniciando prueba de impresión con SharpIpp...");
        var client = new SharpIppClient();

        try
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, documentFileName);
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error: El archivo '{documentFileName}' no se encontró en la ruta de ejecución.");
                Console.WriteLine($"Asegúrate de que el archivo esté en el proyecto y configurado para 'Copiar en el directorio de salida'.");
                return;
            }

            await using var fileStream = File.OpenRead(filePath);

            // --- CÓDIGO CORREGIDO ---
            // La solicitud ahora separa correctamente los atributos del trabajo y los del documento.
            var request = new PrintJobRequest
            {
                PrinterUri = new Uri(printerUriString),
                Document = fileStream,
                // Atributos que describen el trabajo de impresión en general
                NewJobAttributes = new NewJobAttributes
                {
                    JobName = "Mi Prueba desde SharpIpp"
                },
                // Atributos que describen el documento que se está enviando
                DocumentAttributes = new DocumentAttributes
                {
                    DocumentName = documentFileName
                }
            };
            // ------------------------

            Console.WriteLine($"Enviando '{documentFileName}' a la impresora: {printerUriString}");
            var response = await client.PrintJobAsync(request);

            Console.WriteLine("¡Trabajo enviado exitosamente!");
            Console.WriteLine($"  - ID del Trabajo (JobId): {response.JobId}");
            Console.WriteLine($"  - Estado del Trabajo: {response.JobState}");
            Console.WriteLine($"  - URI del Trabajo: {response.JobUri}");
        }
        catch (IppResponseException e)
        {
            Console.WriteLine($"Error de IPP: La impresora respondió con el estado '{e.ResponseMessage.StatusCode}'");
            Console.WriteLine($"Mensaje detallado: {e.Message}");
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Error de Red: No se pudo conectar a la impresora. Verifica la URI.");
            Console.WriteLine($"Mensaje detallado: {e.Message}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ha ocurrido un error inesperado: {e.Message}");
        }

        Console.WriteLine("Prueba finalizada. Presiona cualquier tecla para salir.");
        Console.ReadKey();
    }
}