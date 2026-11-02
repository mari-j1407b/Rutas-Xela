using System;
using System.Collections.Generic;
using System.IO;

// Clase que representa el registro de una parada de bus
class Parada
{
    public int Codigo;
    public string Nombre;
    public double Latitud;
    public double Longitud;

    public Parada(int codigo, string nombre, double latitud, double longitud)
    {
        this.Codigo = codigo;
        this.Nombre = nombre;
        this.Latitud = latitud;
        this.Longitud = longitud;
    }
}

class TablaHash
{
    // Arreglo de listas.
    // Cada posición puede almacenar varias paradas (colisiones).
    private List<Parada>[] tabla;

    private int tamanio;


    // Constructor
    public TablaHash(int tamanio)
    {
        this.tamanio = tamanio;

        // Creamos el arreglo.
        tabla = new List<Parada>[tamanio];

        // Inicializamos una lista
        // en cada posición.
        for (int i = 0; i < tamanio; i++)
        {
            tabla[i] = new List<Parada>();
        }
    }


    // FUNCIÓN HASH
    private int FuncionHash(int codigo)
    {
        return Math.Abs(codigo) % tamanio;
    }


    // INSERTAR
    public void Insertar(Parada nuevaParada)
    {
        // Calculamos el índice.
        int indice = FuncionHash(nuevaParada.Codigo);

        // Verificamos que el código único no exista previamente en la cadena.
        foreach (Parada parada in tabla[indice])
        {
            if (parada.Codigo == nuevaParada.Codigo)
            {
                Console.WriteLine(
                    $"El código de parada {nuevaParada.Codigo} ya existe."
                );
                return;
            }
        }

        // Agregamos la parada a la lista.
        tabla[indice].Add(nuevaParada);

        Console.WriteLine(
            $"Parada {nuevaParada.Codigo} ({nuevaParada.Nombre}) insertada en índice {indice}"
        );
    }


    // BUSCAR
    public Parada Buscar(int codigo)
    {
        // Calculamos el índice.
        int indice = FuncionHash(codigo);

        // Buscamos dentro de esa cadena.
        foreach (Parada parada in tabla[indice])
        {
            if (parada.Codigo == codigo)
            {
                return parada;
            }
        }

        return null;
    }


    // ELIMINAR
    public void Eliminar(int codigo)
    {
        // Calculamos el índice.
        int indice = FuncionHash(codigo);

        // Buscamos la parada dentro de la cadena para eliminarla.
        for (int i = 0; i < tabla[indice].Count; i++)
        {
            if (tabla[indice][i].Codigo == codigo)
            {
                tabla[indice].RemoveAt(i);

                Console.WriteLine(
                    $"Parada {codigo} eliminada."
                );
                return;
            }
        }

        Console.WriteLine(
            "Parada no encontrada."
        );
    }


    // MOSTRAR
    public void Mostrar()
    {
        Console.WriteLine("\nTabla Hash de Paradas");

        // Recorremos las posiciones.
        for (int i = 0; i < tabla.Length; i++)
        {
            Console.Write($"{i} → ");

            // Recorremos la cadena.
            foreach (Parada parada in tabla[i])
            {
                Console.Write(
                    $"[{parada.Codigo} - {parada.Nombre} ({parada.Latitud}, {parada.Longitud})] → "
                );
            }

            Console.WriteLine("FIN");
        }
    }


    // APOYO PARA CARGA INICIAL DESDE CSV EN C#
    public void CargarDesdeCSV(string rutaArchivo)
    {
        if (!File.Exists(rutaArchivo))
        {
            Console.WriteLine("El archivo CSV no existe.");
            return;
        }

        string[] lineas = File.ReadAllLines(rutaArchivo);

        // Empezamos en i = 1 para saltar el encabezado del CSV
        for (int i = 1; i < lineas.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lineas[i])) continue;

            string[] datos = lineas[i].Split(',');

            int codigo = int.Parse(datos[0]);
            string nombre = datos[1];
            double latitud = double.Parse(datos[2], System.Globalization.CultureInfo.InvariantCulture);
            double longitud = double.Parse(datos[3], System.Globalization.CultureInfo.InvariantCulture);

            Parada nuevaParada = new Parada(codigo, nombre, latitud, longitud);
            Insertar(nuevaParada);
        }

        Console.WriteLine("Carga inicial desde CSV completada.");
    }
}
