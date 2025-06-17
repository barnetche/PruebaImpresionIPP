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

Ejemplo 1: Imprimir desde un URI 

using System; 

using System.IO; 

using System.Net.Http; 

using System.Threading.Tasks; 

using SharpIpp; 

using SharpIpp.Exceptions; 

using SharpIpp.Models; 

using SharpIpp.Protocol.Models; 

  

class Ejemplo1ImprimirUri 

{ 

    public static async Task RunAsync() 

    { 

        string printerUriString = "ipp://172.17.170.56:631/ipp/print"; 

        string documentFileName = "test.pdf"; 

  

        Console.Clear(); 

        Console.WriteLine("Ejecutando Ejemplo 0: Imprimir PDF (Básico)"); 

        Console.WriteLine("=========================================="); 

  

        var client = new SharpIppClient(); 

        try 

        { 

            var filePath = Path.Combine(AppContext.BaseDirectory, documentFileName); 

            if (!File.Exists(filePath)) 

            { 

                Console.WriteLine($"Error: El archivo '{documentFileName}' no se encontró..."); 

                return; 

            } 

  

            await using var fileStream = File.OpenRead(filePath); 

            var request = new PrintJobRequest 

            { 

                PrinterUri = new Uri(printerUriString), 

                Document = fileStream, 

                NewJobAttributes = new NewJobAttributes { JobName = "Mi Prueba desde SharpIpp" }, 

                DocumentAttributes = new DocumentAttributes { DocumentName = documentFileName } 

            }; 

  

            Console.WriteLine($"Enviando '{documentFileName}' a la impresora: {printerUriString}"); 

            var response = await client.PrintJobAsync(request); 

  

            Console.WriteLine("¡Trabajo enviado exitosamente!"); 

            Console.WriteLine($"  - ID del Trabajo (JobId): {response.JobId}"); 

            Console.WriteLine($"  - Estado del Trabajo: {response.JobState}"); 

        } 

        catch (Exception e) 

        { 

            Console.WriteLine($"Ha ocurrido un error: {e.Message}"); 

        } 

  

        // Eliminamos el Console.ReadKey() de aquí, el menú principal lo manejará 

        Console.WriteLine("\nPresiona una tecla para volver al menú..."); 

        Console.ReadKey(); 

    } 

} 

Ejemplo 2: Imprimir un archivo PDF 

Esta es la operación más básica. Se necesita un Stream del archivo a imprimir y la URI de la impresora. 

using System; 

using System.IO; 

using System.Net.Http; 

using System.Threading.Tasks; 

using SharpIpp; 

using SharpIpp.Exceptions; 

using SharpIpp.Models; 

using SharpIpp.Protocol.Models; 

  

class Ejemplo2ImprimirPDF 

{ 

    public static async Task RunAsync() 

    { 

        string printerUriString = "ipp://172.17.170.56:631/ipp/print"; 

        string documentFileName = "test.pdf"; 

  

        Console.Clear(); 

        Console.WriteLine("Ejecutando Ejemplo 1: Imprimir un PDF con Atributos"); 

        Console.WriteLine("=================================================="); 

  

        var client = new SharpIppClient(); 

        try 

        { 

            var filePath = Path.Combine(AppContext.BaseDirectory, documentFileName); 

            if (!File.Exists(filePath)) 

            { 

                Console.ForegroundColor = ConsoleColor.Red; 

                Console.WriteLine($"Error: No se encontró el archivo '{documentFileName}'."); 

                Console.ResetColor(); 

                return; 

            } 

  

            await using var fileStream = File.OpenRead(filePath); 

            var request = new PrintJobRequest 

            { 

                PrinterUri = new Uri(printerUriString), 

                Document = fileStream, 

                NewJobAttributes = new NewJobAttributes 

                { 

                    JobName = "Mi Primer Trabajo de Impresión", 

                    Copies = 1, 

                    Sides = Sides.OneSided 

                } 

            }; 

  

            Console.WriteLine($"Enviando trabajo a la impresora: {request.PrinterUri}..."); 

            var response = await client.PrintJobAsync(request); 

  

            Console.ForegroundColor = ConsoleColor.Green; 

            Console.WriteLine("¡Trabajo enviado con éxito!"); 

            Console.ResetColor(); 

            Console.WriteLine($"  -> ID del Trabajo: {response.JobId}"); 

            Console.WriteLine($"  -> Estado del Trabajo: {response.JobState}"); 

        } 

        catch (Exception e) 

        { 

            Console.WriteLine($"Ha ocurrido un error: {e.Message}"); 

        } 

  

        Console.WriteLine("\nPresiona una tecla para volver al menú..."); 

        Console.ReadKey(); 

    } 

} 

Ejemplo 3: Consultar los Atributos de una Impresora 

Para saber qué puede hacer una impresora (formatos, si imprime a color, etc.) antes de enviarle un trabajo. 

using System; 

using System.IO; 

using System.Net.Http; 

using System.Threading.Tasks; 

using SharpIpp; 

using SharpIpp.Exceptions; 

using SharpIpp.Models; 

  

class Ejemplo3AtributosImpresora 

{ 

    public static async Task RunAsync() 

    { 

        string printerUriString = "ipp://172.17.170.56:631/ipp/print"; 

  

        Console.Clear(); 

        Console.WriteLine("Ejecutando Ejemplo 2: Get-Printer-Attributes"); 

        Console.WriteLine("============================================"); 

  

        var client = new SharpIppClient(); 

        try 

        { 

            var request = new GetPrinterAttributesRequest 

            { 

                PrinterUri = new Uri(printerUriString) 

            }; 

  

            Console.WriteLine($"Consultando atributos de la impresora: {request.PrinterUri}..."); 

            var response = await client.GetPrinterAttributesAsync(request); 

  

            Console.ForegroundColor = ConsoleColor.Green; 

            Console.WriteLine("¡Atributos recibidos con éxito!"); 

            Console.ResetColor(); 

            Console.WriteLine("------------------------------------------"); 

            Console.WriteLine($"  -> Nombre de la Impresora: {response.PrinterName}"); 

            Console.WriteLine($"  -> Estado de la Impresora: {response.PrinterState}"); 

            Console.WriteLine($"  -> Formatos Soportados: {string.Join(", ", response.DocumentFormatSupported ?? Array.Empty<string>())}"); 

            Console.WriteLine("------------------------------------------"); 

        } 

        catch (Exception e) 

        { 

            Console.WriteLine($"Ha ocurrido un error: {e.Message}"); 

        } 

  

        Console.WriteLine("\nPresiona una tecla para volver al menú..."); 

        Console.ReadKey(); 

    } 

} 

Ejemplo 4: Listar Trabajos de Impresión 

Para ver qué trabajos están en la cola de la impresora. 

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

Ejemplo 5: Validate-Job 

Esta operación se utiliza para comprobar si una impresora aceptaría un trabajo con ciertos atributos, pero sin llegar a imprimirlo. Es ideal para validar la configuración antes de enviar el documento final. 

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

Ejemplo 6: Get-Jobs 

Permite obtener una lista de los trabajos de impresión en la impresora, filtrando por su estado. 

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

Ejemplo 7: Get-Job-Attributes 

Obtiene información detallada de un trabajo de impresión específico a través de su ID. 

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

            if(e.ResponseMessage.StatusCode == SharpIpp.Protocol.Models.IppStatusCode.ClientErrorNotFound) 

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

 

Ejemplo 8: Cancel-Job 

Cancela un trabajo que está en la cola de impresión. 

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

Ejemplo 9: Hold-Job y Release-Job 

Estas operaciones permiten poner en espera un trabajo y luego liberarlo para que se imprima. 

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

Ejemplo 10: Restart-Job 

Reinicia un trabajo que ya se completó pero que la impresora ha retenido en su historial. 

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

Ejemplo 11: Pause-Printer, Resume-Printer y Purge-Jobs 

Estas operaciones gestionan el estado de la cola de la impresora en su totalidad. 

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

6. Manejo de Errores 🚨 

La librería gestiona los errores de manera robusta. Es importante estar preparado para capturar dos tipos principales de excepciones: 

HttpRequestException: Ocurre cuando hay un problema de red y no se puede comunicar con la impresora. 

IppResponseException: Esta es la excepción más común. Se lanza cuando la impresora responde con un código de estado de error IPP (por ejemplo, server-error-operation-not-supported). El objeto de la excepción contiene la respuesta completa de la impresora (ResponseMessage), lo cual es extremadamente útil para depurar el problema exacto. 
