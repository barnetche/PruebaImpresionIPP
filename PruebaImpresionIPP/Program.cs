using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine("  Menú de Ejemplos de Impresión SharpIpp  ");
            Console.WriteLine("========================================");
            Console.WriteLine("Selecciona el ejemplo que quieres ejecutar:");
            Console.WriteLine("  1. Ejemplo 1: Imprimir PDF (Básico)");
            Console.WriteLine("  2. Ejemplo 2: Imprimir PDF (con Atributos)");
            Console.WriteLine("  3. Ejemplo 3: Obtener Atributos de la Impresora");
            Console.WriteLine("  4. Ejemplo 4: Listar Trabajos de Impresión");
            Console.WriteLine("  5. Ejemplo 5: Validar un Trabajo (Validate-Job)");
            Console.WriteLine("  6. Ejemplo 6: Listar Trabajos (Get-Jobs)");
            Console.WriteLine("  7. Ejemplo 7: Obtener Atributos de un Trabajo (Get-Job-Attributes)");
            Console.WriteLine("  8. Ejemplo 8: Cancelar un Trabajo (Cancel-Job)");
            Console.WriteLine("  9. Ejemplo 9: Retener y Liberar un Trabajo (Hold/Release-Job)");
            Console.WriteLine(" 10. Ejemplo 10: Reiniciar un Trabajo (Restart-Job)");
            Console.WriteLine(" 11. Ejemplo 11: Gestionar Impresora (Pause/Resume/Purge)"); 
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("  0. Salir");
            Console.WriteLine("========================================");
            Console.Write("Tu opción: ");

            var option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    await Ejemplo1ImprimirUri.RunAsync();
                    break;
                case "2":
                    await Ejemplo2ImprimirPDF.RunAsync();
                    break;
                case "3":
                    await Ejemplo3AtributosImpresora.RunAsync();
                    break;
                case "4":
                    await Ejemplo4ListarTrabajos.RunAsync();
                    break;
                case "5":
                    await Ejemplo5ValidateJob.RunAsync();
                    break;
                case "6":
                    await Ejemplo6GetJobs.RunAsync();
                    break;
                case "7":
                    await Ejemplo7GetJobAttributes.RunAsync();
                    break;
                case "8":
                    await Ejemplo8CancelJob.RunAsync();
                    break;
                case "9":
                    await Ejemplo9HoldAndReleaseJob.RunAsync();
                    break;
                case "10":
                    await Ejemplo10RestartJob.RunAsync();
                    break;
                case "11": 
                    await Ejemplo11ManagePrinter.RunAsync();
                    break;
                case "0":
                    Console.WriteLine("Saliendo...");
                    return;
                default:
                    Console.WriteLine("Opción no válida. Presiona una tecla para continuar...");
                    Console.ReadKey();
                    break;
            }
        }
    }
}