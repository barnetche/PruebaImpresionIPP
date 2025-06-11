Documentación de la Librería SharpIpp 

Esta documentación detalla las funcionalidades, el uso y la arquitectura de la librería SharpIpp, una implementación en C# del Protocolo de Impresión de Internet (IPP). 

1. ¿Qué es SharpIpp?  

SharpIpp es una librería de .NET que permite la comunicación con impresoras que soportan el Protocolo de Impresión de Internet. Implementa la especificación IPP/1.1 y partes de las extensiones de CUPS 1.0. Esto permite realizar operaciones como enviar trabajos de impresión, consultar el estado de una impresora, gestionar trabajos en cola y más, todo a través de la red. 

2. Instalación  

La librería está disponible como un paquete de NuGet, lo que facilita su integración en cualquier proyecto .NET. 

PM> Install-Package SharpIpp 

3. Arquitectura y Componentes Clave  

Entender su estructura interna es clave para usarla eficientemente y, si es necesario, crear un wrapper o fachada sobre ella. 

SharpIppClient: Es la clase principal y el punto de entrada para todas las operaciones. Utiliza una instancia de HttpClient para la comunicación, lo que permite flexibilidad en la configuración de red (timeouts, reintentos, etc.). 

Modelos de Solicitud/Respuesta (/Models): La interacción con la librería se basa en un patrón de Solicitud/Respuesta. Para cada operación (ej. Print-Job), existe una clase Request (ej. PrintJobRequest) y una clase Response (ej. PrintJobResponse). Estos modelos son la API pública de la librería. 

Protocolo (/Protocol): Esta es la capa de bajo nivel que se encarga de serializar las solicitudes a formato binario IPP y deserializar las respuestas binarias del servidor. Es el motor que traduce los objetos .NET al lenguaje que la impresora entiende. 

Mapeo (/Mapping): Una de las características más interesantes de su diseño es el uso de un sistema de mapeo para convertir los modelos públicos (en SharpIpp/Models) a los objetos internos del protocolo. Esto desacopla la API pública de la implementación interna, una excelente práctica de diseño. 

 

4. Funcionalidades Soportadas  

La librería ofrece una cobertura amplia de las operaciones estándar de IPP y algunas específicas de CUPS. 

Operaciones IPP/1.1 

Operación 

Método en SharpIpp 

Descripción 

Print-Job 

PrintJobAsync 

Envía un documento (como un Stream) para impresión directa. 

Print-URI 

PrintUriAsync 

Envía la URI de un documento para que la impresora lo descargue e imprima. 

Validate-Job 

ValidateJobAsync 

Verifica si un trabajo de impresión sería aceptado, sin enviarlo realmente. 

Create-Job 

CreateJobAsync 

Crea un nuevo trabajo de impresión vacío en la impresora, para luego enviarle documentos. 

Send-Document 

SendDocumentAsync 

Envía un documento como parte de un trabajo creado previamente con Create-Job. 

Get-Printer-Attributes 

GetPrinterAttributesAsync 

Obtiene los atributos y capacidades de una impresora (ej. formatos soportados, estado). 

Get-Jobs 

GetJobsAsync 

Obtiene una lista de los trabajos de impresión en la cola. 

Get-Job-Attributes 

GetJobAttributesAsync 

Obtiene los atributos de un trabajo de impresión específico. 

Cancel-Job 

CancelJobAsync 

Cancela un trabajo de impresión que está en la cola. 

Pause-Printer 

PausePrinterAsync 

Pausa la cola de impresión de la impresora. 

Resume-Printer 

ResumePrinterAsync 

Reanuda la cola de impresión de la impresora. 

Purge-Jobs 

PurgeJobsAsync 

Elimina todos los trabajos de la cola de la impresora. 

Referencia: La lista de operaciones está documentada en el README.md y en la interfaz ISharpIppClient. 

Operaciones CUPS 

Operación 

Método en SharpIpp 

Descripción 

CUPS-Get-Printers 

GetCUPSPrintersAsync 

Obtiene una lista de todas las impresoras conocidas por un servidor CUPS. 

 

5. Guía de Uso y Ejemplos de Código 📝 

A continuación, se muestran ejemplos prácticos para las operaciones más comunes. 

Ejemplo 0: Imprimir desde un URI 

using SharpIpp; 

using SharpIpp.Exceptions; 

using SharpIpp.Models; 

using System.IO; 

using System; 

using System.Threading.Tasks; 

  

class Program 

{ 

    static async Task Main(string[] args) 

    { 

        string printerUriString = "ipp://10.107.51.5:631/ipp/print";  

        string documentFileName = "word-2-pages.pdf"; 

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

Ejemplo 1: Imprimir un archivo PDF 

Esta es la operación más básica. Se necesita un Stream del archivo a imprimir y la URI de la impresora. 

using SharpIpp; 

using SharpIpp.Models; 

  

// 1. Crear el cliente 

var client = new SharpIppClient(); 

  

// 2. Definir la URI de la impresora 

var printerUri = new Uri("ipp://192.168.1.100/ipp/print"); 

  

// 3. Abrir el archivo como un Stream 

await using var fileStream = File.OpenRead(@"C:\ruta\al\documento.pdf"); 

  

// 4. Crear la solicitud de impresión 

var request = new PrintJobRequest 

{ 

    PrinterUri = printerUri, 

    Document = fileStream, 

    NewJobAttributes = new NewJobAttributes 

    { 

        JobName = "Mi Primer Trabajo de Impresión", 

        Copies = 1, 

        Sides = SharpIpp.Protocol.Models.Sides.OneSided 

    } 

}; 

  

// 5. Enviar la solicitud y esperar la respuesta 

try 

{ 

    var response = await client.PrintJobAsync(request); 

    Console.WriteLine($"Trabajo enviado. JobId: {response.JobId}, Estado: {response.JobState}"); 

} 

catch (IppResponseException e) 

{ 

    // El servidor IPP respondió con un error (ej. formato no soportado) 

    Console.WriteLine($"Error de IPP: {e.Message}. StatusCode: {e.ResponseMessage.StatusCode}"); 

} 

catch (HttpRequestException e) 

{ 

    // Error de red 

    Console.WriteLine($"Error de red: {e.Message}"); 

} 

Ejemplo 2: Consultar los Atributos de una Impresora 

Para saber qué puede hacer una impresora (formatos, si imprime a color, etc.) antes de enviarle un trabajo. 

using SharpIpp; 

using SharpIpp.Models; 

  

var client = new SharpIppClient(); 

var printerUri = new Uri("ipp://192.168.1.100/ipp/print"); 

  

// Crear la solicitud para obtener todos los atributos 

var request = new GetPrinterAttributesRequest 

{ 

    PrinterUri = printerUri 

}; 

  

try 

{ 

    var response = await client.GetPrinterAttributesAsync(request); 

  

    Console.WriteLine($"Nombre de la impresora: {response.PrinterName}"); 

    Console.WriteLine($"Estado: {response.PrinterState}"); 

    Console.WriteLine($"Formatos soportados: {string.Join(", ", response.DocumentFormatSupported ?? Array.Empty<string>())}"); 

    Console.WriteLine($"¿Soporta color?: {response.ColorSupported}"); 

} 

catch (IppResponseException e) 

{ 

    Console.WriteLine($"Error de IPP: {e.ResponseMessage.StatusCode}"); 

} 

Ejemplo 3: Listar Trabajos de Impresión 

Para ver qué trabajos están en la cola de la impresora. 

using SharpIpp; 

using SharpIpp.Models; 

using SharpIpp.Protocol.Models; 

  

var client = new SharpIppClient(); 

var printerUri = new Uri("ipp://192.168.1.100/ipp/print"); 

  

var request = new GetJobsRequest 

{ 

    PrinterUri = printerUri, 

    WhichJobs = WhichJobs.NotCompleted // Opciones: Completed, NotCompleted 

}; 

  

try 

{ 

    var response = await client.GetJobsAsync(request); 

  

    if (response.Jobs.Any()) 

    { 

        Console.WriteLine("Trabajos en la cola:"); 

        foreach (var job in response.Jobs) 

        { 

            Console.WriteLine($"- Job ID: {job.JobId}, Nombre: {job.JobName}, Estado: {job.JobState}"); 

        } 

    } 

    else 

    { 

        Console.WriteLine("No hay trabajos en la cola."); 

    } 

} 

catch (IppResponseException e) 

{ 

    Console.WriteLine($"Error de IPP: {e.ResponseMessage.StatusCode}"); 

} 

Ejemplo 4: Validate-Job 

Esta operación se utiliza para comprobar si una impresora aceptaría un trabajo con ciertos atributos, pero sin llegar a imprimirlo. Es ideal para validar la configuración antes de enviar el documento final. 

using SharpIpp; 

using SharpIpp.Models; 

  

public async Task ValidatePrintJobAsync() 

{ 

    var client = new SharpIppClient(); 

    var printerUri = new Uri("ipp://192.168.1.100/ipp/print"); 

  

    // Se necesita un Stream del documento, aunque no se envíe. 

    // Puede ser un stream vacío o el del documento real. 

    await using var stream = new MemoryStream(new byte[0]); 

  

    var request = new ValidateJobRequest 

    { 

        PrinterUri = printerUri, 

        Document = stream, 

        NewJobAttributes = new NewJobAttributes 

        { 

            Copies = 2, 

            Sides = SharpIpp.Protocol.Models.Sides.TwoSidedLongEdge 

        } 

    }; 

  

    try 

    { 

        var response = await client.ValidateJobAsync(request); 

        // Si no hay excepción, la configuración es válida. 

        Console.WriteLine("La impresora ha validado la configuración del trabajo correctamente."); 

    } 

    catch (IppResponseException e) 

    { 

        Console.WriteLine($"La validación ha fallado. StatusCode: {e.ResponseMessage.StatusCode}"); 

    } 

} 

Ejemplo 5: Get-Jobs 

Permite obtener una lista de los trabajos de impresión en la impresora, filtrando por su estado. 

using SharpIpp; 

using SharpIpp.Models; 

using SharpIpp.Protocol.Models; 

  

public async Task ListPendingJobsAsync() 

{ 

    var client = new SharpIppClient(); 

    var printerUri = new Uri("ipp://192.168.1.100/ipp/print"); 

  

    var request = new GetJobsRequest 

    { 

        PrinterUri = printerUri, 

        WhichJobs = WhichJobs.NotCompleted, // Opciones: Completed, NotCompleted 

        RequestingUserName = "mi-usuario" // Opcional: para ver solo tus trabajos 

    }; 

  

    try 

    { 

        var response = await client.GetJobsAsync(request); 

        Console.WriteLine($"Se encontraron {response.Jobs.Length} trabajos no completados."); 

        foreach (var job in response.Jobs) 

        { 

            Console.WriteLine($"- Job ID: {job.JobId}, Estado: {job.JobState}, Nombre: {job.JobName}"); 

        } 

    } 

    catch (IppResponseException e) 

    { 

        Console.WriteLine($"Error al obtener los trabajos. StatusCode: {e.ResponseMessage.StatusCode}"); 

    } 

} 

Ejemplo 6: Get-Job-Attributes 

Obtiene información detallada de un trabajo de impresión específico a través de su ID. 

using SharpIpp; 

using SharpIpp.Models; 

  

public async Task GetJobDetailsAsync(int jobId) 

{ 

    // Nota: El 'jobId' se habría obtenido de una llamada previa a PrintJobAsync o CreateJobAsync. 

    var client = new SharpIppClient(); 

    var printerUri = new Uri("ipp://192.168.1.100/ipp/print"); 

  

    var request = new GetJobAttributesRequest 

    { 

        PrinterUri = printerUri, 

        JobId = jobId 

    }; 

  

    try 

    { 

        var response = await client.GetJobAttributesAsync(request); 

        var job = response.JobAttributes; 

        Console.WriteLine($"Detalles del Job ID {job.JobId}:"); 

        Console.WriteLine($"  Estado: {job.JobState}"); 

        Console.WriteLine($"  Razón del estado: {string.Join(", ", job.JobStateReasons ?? Array.Empty<string>())}"); 

        Console.WriteLine($"  Páginas completadas: {job.JobImpressionsCompleted}"); 

    } 

    catch (IppResponseException e) 

    { 

        Console.WriteLine($"Error al obtener los atributos del trabajo. StatusCode: {e.ResponseMessage.StatusCode}"); 

    } 

} 

 

Ejemplo 7: Cancel-Job 

Cancela un trabajo que está en la cola de impresión. 

using SharpIpp; 

using SharpIpp.Models; 

  

public async Task CancelSpecificJobAsync(int jobId) 

{ 

    // Nota: El 'jobId' debe corresponder a un trabajo existente. 

    var client = new SharpIppClient(); 

    var printerUri = new Uri("ipp://192.168.1.100/ipp/print"); 

  

    var request = new CancelJobRequest 

    { 

        PrinterUri = printerUri, 

        JobId = jobId, 

        RequestingUserName = "mi-usuario" 

    }; 

  

    try 

    { 

        var response = await client.CancelJobAsync(request); 

        // Una respuesta exitosa (sin excepción) significa que la orden de cancelación fue aceptada. 

        Console.WriteLine($"La orden para cancelar el trabajo {jobId} ha sido enviada."); 

    } 

    catch (IppResponseException e) 

    { 

        Console.WriteLine($"Error al cancelar el trabajo. StatusCode: {e.ResponseMessage.StatusCode}"); 

    } 

} 

Ejemplo 8: Hold-Job y Release-Job 

Estas operaciones permiten poner en espera un trabajo y luego liberarlo para que se imprima. 

using SharpIpp; 

using SharpIpp.Models; 

  

public async Task HoldAndReleaseJobAsync(int jobId) 

{ 

    var client = new SharpIppClient(); 

    var printerUri = new Uri("ipp://192.168.1.100/ipp/print"); 

  

    // 1. Poner el trabajo en espera (Hold) 

    var holdRequest = new HoldJobRequest { PrinterUri = printerUri, JobId = jobId }; 

    try 

    { 

        await client.HoldJobAsync(holdRequest); 

        Console.WriteLine($"El trabajo {jobId} ha sido puesto en espera."); 

    } 

    catch (IppResponseException e) 

    { 

        Console.WriteLine($"No se pudo poner en espera el trabajo. StatusCode: {e.ResponseMessage.StatusCode}"); 

        return; 

    } 

  

    // ... (Aquí podría haber lógica de espera o una acción del usuario) ... 

    Console.WriteLine("Liberando el trabajo en 5 segundos..."); 

    await Task.Delay(5000); 

  

    // 2. Liberar el trabajo (Release) 

    var releaseRequest = new ReleaseJobRequest { PrinterUri = printerUri, JobId = jobId }; 

    try 

    { 

        await client.ReleaseJobAsync(releaseRequest); 

        Console.WriteLine($"El trabajo {jobId} ha sido liberado y está listo para imprimirse."); 

    } 

    catch (IppResponseException e) 

    { 

        Console.WriteLine($"No se pudo liberar el trabajo. StatusCode: {e.ResponseMessage.StatusCode}"); 

    } 

} 

Ejemplo 9: Restart-Job 

Reinicia un trabajo que ya se completó pero que la impresora ha retenido en su historial. 

using SharpIpp; 
using SharpIpp.Models;  
public async Task RestartSpecificJobAsync(int jobId) 
{ 
    // Nota: La impresora debe soportar la retención de trabajos completados. 
    var client = new SharpIppClient(); 
    var printerUri = new Uri("ipp://192.168.1.100/ipp/print"); 
    var request = new RestartJobRequest
    { 
        PrinterUri = printerUri, 
        JobId = jobId 
    }; 
    try 
    { 
        await client.RestartJobAsync(request); 
        Console.WriteLine($"El trabajo {jobId} ha sido reiniciado."); 
    } 
    catch (IppResponseException e) 
    { 
        Console.WriteLine($"No se pudo reiniciar el trabajo. StatusCode: {e.ResponseMessage.StatusCode}"); 
    } 
} 

Ejemplo 7: Pause-Printer, Resume-Printer y Purge-Jobs 

Estas operaciones gestionan el estado de la cola de la impresora en su totalidad. 

using SharpIpp; 
using SharpIpp.Models;   
public async Task ManagePrinterQueueAsync() 
{ 
    var client = new SharpIppClient(); 
    var printerUri = new Uri("ipp://192.168.1.100/ipp/print"); 
    // 1. Pausar la impresora 
    try 
    { 
        var pauseRequest = new PausePrinterRequest { PrinterUri = printerUri }; 
        await client.PausePrinterAsync(pauseRequest); 
        Console.WriteLine("La impresora ha sido pausada. No procesará nuevos trabajos."); 
    } 
    catch (IppResponseException e) { Console.WriteLine($"Error al pausar: {e.ResponseMessage.StatusCode}"); } 
    await Task.Delay(2000); 
    // 2. Reanudar la impresora 
    try 
    { 
        var resumeRequest = new ResumePrinterRequest { PrinterUri = printerUri }; 
        await client.ResumePrinterAsync(resumeRequest); 
        Console.WriteLine("La impresora ha sido reanudada."); 
    } 
    catch (IppResponseException e) { Console.WriteLine($"Error al reanudar: {e.ResponseMessage.StatusCode}"); } 
    await Task.Delay(2000); 
    // 3. Purgar (eliminar) todos los trabajos 
    // ¡¡¡CUIDADO: ESTA ACCIÓN ES DESTRUCTIVA Y NO SE PUEDE DESHACER!!! 
    try 
    { 
        var purgeRequest = new PurgeJobsRequest { PrinterUri = printerUri }; 
        await client.PurgeJobsAsync(purgeRequest); 
        Console.WriteLine("Todos los trabajos han sido eliminados de la cola de la impresora."); 
    } 
    catch (IppResponseException e) { Console.WriteLine($"Error al purgar: {e.ResponseMessage.StatusCode}"); } 
} 

6. Manejo de Errores 🚨 

La librería gestiona los errores de manera robusta. Es importante estar preparado para capturar dos tipos principales de excepciones: 

HttpRequestException: Ocurre cuando hay un problema de red y no se puede comunicar con la impresora. 

IppResponseException: Esta es la excepción más común. Se lanza cuando la impresora responde con un código de estado de error IPP (por ejemplo, server-error-operation-not-supported). El objeto de la excepción contiene la respuesta completa de la impresora (ResponseMessage), lo cual es extremadamente útil para depurar el problema exacto. 
