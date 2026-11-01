using System;

class Program
{
    static void Main(string[] args)
    {
       // datos de prueba para iniciar sin archivo csv
        GrafoTransporte miGrafo = new GrafoTransporte(50);
        miGrafo.AgregarParada("P001");
        miGrafo.AgregarParada("P002");
        miGrafo.AgregarParada("P003");
        miGrafo.AgregarParada("P004");
        miGrafo.AgregarRuta("P001", "P002", 5);
        miGrafo.AgregarRuta("P002", "P003", 8);
        miGrafo.AgregarRuta("P003", "P004", 12, true);

        int opcion;

        do
        {
            Console.Clear();

            Console.WriteLine("================================");
            Console.WriteLine("          RUTAS-XELA");
            Console.WriteLine("    Sistema de transporte urbano");
            Console.WriteLine("================================");
            Console.WriteLine();
            Console.WriteLine("1. Buscar paradas por nombre");
            Console.WriteLine("2. Consultar información de una parada");
            Console.WriteLine("3. Consultar ruta más rápida (Dijkstra)");
            Console.WriteLine("4. Comprobar conectividad");
            Console.WriteLine("5. Ejecutar pruebas de Union-Find");
            Console.WriteLine("6. Mostrar todas las paradas registradas");
            Console.WriteLine("7. Mostrar todas las rutas (Lista de adyacencia)");
            Console.WriteLine("8. Mostrar recorrido BFS");
            Console.WriteLine("9. Mostrar recorrido DFS");
            Console.WriteLine("10. Eliminar una parada o ruta");
            Console.WriteLine("0. Salir");
            Console.WriteLine();

            Console.Write("Seleccione una opción: ");
            int.TryParse(Console.ReadLine(), out opcion);

            Console.WriteLine();

            switch (opcion)
            {
                case 1:
                    Console.WriteLine("Búsqueda de paradas por nombre.");
                    break;

                case 2:
                    Console.WriteLine("Consulta de información de paradas.");
                    break;

                case 3:
                    Console.WriteLine("-- Calcular ruta mas rapida --");
                    Console.Write("Ingrese el código de la parada de origen (Ej. P001): ");
                    string origen = Console.ReadLine();

                    Console.Write("Ingrese el código de la parada de destino (Ej. P004): ");
                    string destino = Console.ReadLine();

                    miGrafo.CalcularRutaMasRapida(origen, destino);
                    break;

                case 4:
                    Console.WriteLine("Comprobación de conectividad.");
                    break;

                case 5:
                    PruebasUnionFind.Ejecutar();
                    break;

                case 6:
                    miGrafo.MostrarParadas();
                    break;

                case 7:
                    miGrafo.MostrarGrafo();
                    break;

                case 8:
                    Console.WriteLine("-- Recorrido BFS --");
                    Console.Write("Ingrese el código de la parada de inicio (Ej. P001): ");
                    string inicioBfs = Console.ReadLine();
                    miGrafo.RecorridoBFS(inicioBfs);
                    break;

                case 9:
                    Console.WriteLine("-- Recorrido DFS --");
                    Console.Write("Ingrese el código de la parada de inicio (Ej. P001): ");
                    string inicioDfs = Console.ReadLine();
                    miGrafo.RecorridoDFS(inicioDfs);
                    break;

                case 10:
                    Console.WriteLine("-- Eliminacion --");
                    Console.WriteLine("A. Eliminar parada completa");
                    Console.WriteLine("B. Eliminar solo una ruta");
                    Console.Write("Elija una opción (A/B): ");
                    string subOpcion = Console.ReadLine()?.ToUpper();

                    if (subOpcion == "A")
                    {
                        Console.Write("Ingrese el código de la parada a eliminar: ");
                        string pBorrar = Console.ReadLine();
                        miGrafo.EliminarParada(pBorrar);
                    }
                    else if (subOpcion == "B")
                    {
                        Console.Write("Ingrese el código de origen: ");
                        string rOrigen = Console.ReadLine();
                        Console.Write("Ingrese el código de destino: ");
                        string rDestino = Console.ReadLine();
                        miGrafo.EliminarRuta(rOrigen, rDestino);
                        Console.WriteLine($"Ruta entre {rOrigen} y {rDestino} eliminada (si existía).");
                    }
                    else
                    {
                        Console.WriteLine("Opción no válida.");
                    }
                    break;

                case 0:
                    Console.WriteLine("Gracias por utilizar Rutas-Xela.");
                    break;

                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }

            if (opcion != 0)
            {
                Console.WriteLine();
                Console.WriteLine("Presione una tecla para continuar...");
                Console.ReadKey();
            }

        } while (opcion != 0);
    }
}