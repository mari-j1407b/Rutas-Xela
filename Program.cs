using System;

class Program
{
    static void Main(string[] args)
    {
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
            Console.WriteLine("3. Consultar rutas entre paradas");
            Console.WriteLine("4. Comprobar conectividad");
            Console.WriteLine("5. Ejecutar pruebas de Union-Find");
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
                    Console.WriteLine("Consulta de rutas.");
                    break;

                case 4:
                    Console.WriteLine("Comprobación de conectividad.");
                    break;

                case 5:
                    PruebasUnionFind.Ejecutar();
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