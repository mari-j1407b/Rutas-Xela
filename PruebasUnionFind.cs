using System;

class PruebasUnionFind
{
    public static void Ejecutar()
    {
        UnionFind conjuntos = new UnionFind();

        Console.WriteLine("Pruebas de Union-Find");
        Console.WriteLine();

        // Agregamos las paradas.
        Console.WriteLine("Agregar paradas:");

        Console.WriteLine(conjuntos.AgregarParada("P001"));
        Console.WriteLine(conjuntos.AgregarParada("P002"));
        Console.WriteLine(conjuntos.AgregarParada("P003"));
        Console.WriteLine(conjuntos.AgregarParada("P004"));
        Console.WriteLine(conjuntos.AgregarParada("P005"));

        // Intentamos agregar una parada repetida.
        Console.WriteLine();
        Console.WriteLine("Agregar código repetido:");
        Console.WriteLine(conjuntos.AgregarParada("P001"));

        // Unimos algunas paradas.
        Console.WriteLine();
        Console.WriteLine("Unir paradas:");

        Console.WriteLine(conjuntos.Union("P001", "P002"));
        Console.WriteLine(conjuntos.Union("P002", "P003"));
        Console.WriteLine(conjuntos.Union("P004", "P005"));

        // Intentamos unir elementos que ya están conectados.
        Console.WriteLine();
        Console.WriteLine("Unir paradas ya conectadas:");
        Console.WriteLine(conjuntos.Union("P001", "P003"));

        // Comprobamos las conexiones.
        Console.WriteLine();
        Console.WriteLine("Comprobar conexiones:");

        Console.WriteLine(
            "P001 y P003: " +
            conjuntos.EstanConectados("P001", "P003"));

        Console.WriteLine(
            "P001 y P004: " +
            conjuntos.EstanConectados("P001", "P004"));

        Console.WriteLine(
            "P004 y P005: " +
            conjuntos.EstanConectados("P004", "P005"));

        // Consultamos los representantes.
        Console.WriteLine();
        Console.WriteLine("Representantes:");

        Console.WriteLine("P001: " + conjuntos.Find("P001"));
        Console.WriteLine("P002: " + conjuntos.Find("P002"));
        Console.WriteLine("P003: " + conjuntos.Find("P003"));
        Console.WriteLine("P004: " + conjuntos.Find("P004"));
        Console.WriteLine("P005: " + conjuntos.Find("P005"));

        // Probamos códigos inexistentes.
        Console.WriteLine();
        Console.WriteLine("Buscar código inexistente:");
        Console.WriteLine("Find: '" + conjuntos.Find("P999") + "'");
        Console.WriteLine(
            "Conexión: " +
            conjuntos.EstanConectados("P001", "P999"));

        // Probamos el aumento de capacidad.
        Console.WriteLine();
        Console.WriteLine("Prueba de capacidad dinámica:");

        for (int indice = 6; indice <= 16; indice++)
        {
            string codigo = "P" + indice.ToString("000");

            Console.WriteLine(
                "Agregar " + codigo + ": " +
                conjuntos.AgregarParada(codigo));
        }

        Console.WriteLine();
        Console.WriteLine("Buscar última parada agregada:");
        Console.WriteLine(conjuntos.Find("P016"));
            
    }
}