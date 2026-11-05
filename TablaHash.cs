using System;
using System.Globalization;
using System.IO;


// Clase que representa el registro de una parada de bus
public class Parada
{
    public string Codigo;
    public string Nombre;
    public double Latitud;
    public double Longitud;

    public Parada(string codigo, string nombre, double latitud, double longitud)
    {
        this.Codigo = codigo != null ? codigo.Trim().ToUpper() : "";
        this.Nombre = nombre != null ? nombre.Trim() : "";
        this.Latitud = latitud;
        this.Longitud = longitud;
    }
}

public class NodoParada // Nodo para la lista enlazada en la tabla hash
{
    public Parada Datos;
    public NodoParada Siguiente;

    public NodoParada(Parada datos)
    {
        this.Datos = datos;
        this.Siguiente = null;
    }
}

public class TablaHash // Implementación de una tabla hash para almacenar paradas de bus
{
    private NodoParada[] tabla;
    private int tamanio;

    public TablaHash(int tamanio)
    {
        if (tamanio <= 0)
        {
            Console.WriteLine("El tamaño debe ser mayor que cero. Se asignará tamaño 11 por defecto.");
            tamanio = 11;
        }

        this.tamanio = tamanio;
        this.tabla = new NodoParada[this.tamanio];
    }


    private int FuncionHash(string codigo) // Función hash para calcular el índice de la tabla
    {
        if (string.IsNullOrWhiteSpace(codigo))
        {
            return -1;
        }

        string claveLimpia = codigo.Trim().ToUpper();
        int suma = 0;

        for (int i = 0; i < claveLimpia.Length; i++)
        {
            suma = (suma * 31 + claveLimpia[i]) % tamanio;
        }

        return Math.Abs(suma) % tamanio;
    }

    public bool Insertar(Parada nuevaParada) // Inserta una nueva parada en la tabla hash
    {
        if (nuevaParada == null)
        {
            Console.WriteLine("La parada no puede ser nula.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(nuevaParada.Codigo) || string.IsNullOrWhiteSpace(nuevaParada.Nombre))
        {
            Console.WriteLine("El código y el nombre de la parada son obligatorios.");
            return false;
        }

        if (double.IsNaN(nuevaParada.Latitud) || double.IsNaN(nuevaParada.Longitud) ||
            nuevaParada.Latitud < -90 || nuevaParada.Latitud > 90 ||
            nuevaParada.Longitud < -180 || nuevaParada.Longitud > 180)
        {
            Console.WriteLine($"Coordenadas inválidas para la parada {nuevaParada.Codigo}.");
            return false;
        }

        nuevaParada.Codigo = nuevaParada.Codigo.Trim().ToUpper();
        nuevaParada.Nombre = nuevaParada.Nombre.Trim();

        int indice = FuncionHash(nuevaParada.Codigo);
        if (indice == -1)
        {
            Console.WriteLine("Error: Código de parada inválido.");
            return false;
        }

        if (tabla[indice] == null)
        {
            tabla[indice] = new NodoParada(nuevaParada);
            Console.WriteLine($"Parada {nuevaParada.Codigo} ({nuevaParada.Nombre}) insertada en índice {indice}");
            return true;
        }

        NodoParada actual = tabla[indice];
        while (true)
        {
            if (string.Equals(actual.Datos.Codigo, nuevaParada.Codigo, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"El código de parada {nuevaParada.Codigo} ya existe.");
                return false;
            }

            if (actual.Siguiente == null)
            {
                break;
            }

            actual = actual.Siguiente;
        }

        actual.Siguiente = new NodoParada(nuevaParada);
        Console.WriteLine($"Parada {nuevaParada.Codigo} ({nuevaParada.Nombre}) insertada en índice {indice}");
        return true;
    }


    public Parada Buscar(string codigo) // Busca una parada en la tabla hash mediante su código
    {
        if (string.IsNullOrWhiteSpace(codigo))
        {
            return null;
        }

        string codigoNormalizado = codigo.Trim().ToUpper();
        int indice = FuncionHash(codigoNormalizado);

        if (indice == -1)
        {
            return null;
        }

        NodoParada actual = tabla[indice];
        while (actual != null)
        {
            if (string.Equals(actual.Datos.Codigo, codigoNormalizado, StringComparison.OrdinalIgnoreCase))
            {
                return actual.Datos;
            }
            actual = actual.Siguiente;
        }

        return null;
    }


    public bool Eliminar(string codigo) // Elimina una parada de la tabla hash mediante su código
    {
        if (string.IsNullOrWhiteSpace(codigo))
        {
            Console.WriteLine("Código inválido.");
            return false;
        }

        string codigoNormalizado = codigo.Trim().ToUpper();
        int indice = FuncionHash(codigoNormalizado);

        if (indice == -1)
        {
            Console.WriteLine("Parada no encontrada.");
            return false;
        }

        NodoParada actual = tabla[indice];
        NodoParada anterior = null;

        while (actual != null)
        {
            if (string.Equals(actual.Datos.Codigo, codigoNormalizado, StringComparison.OrdinalIgnoreCase))
            {
                // El nodo a eliminar es el primero de la cadena
                if (anterior == null)
                {
                    tabla[indice] = actual.Siguiente;
                }
                // El nodo es intermedio o el último de la cadena
                else
                {
                    anterior.Siguiente = actual.Siguiente;
                }

                Console.WriteLine($"Parada {codigoNormalizado} eliminada.");
                return true;
            }

            anterior = actual;
            actual = actual.Siguiente;
        }

        Console.WriteLine("Parada no encontrada.");
        return false;
    }

    public void Mostrar() // Muestra el contenido de la tabla hash
    {
        Console.WriteLine("\nTabla Hash de Paradas");

        for (int i = 0; i < tabla.Length; i++)
        {
            Console.Write($"{i} -> ");

            NodoParada actual = tabla[i];
            while (actual != null)
            {
                Parada parada = actual.Datos;
                string info = "[" + parada.Codigo + " " + parada.Nombre + " (" + parada.Latitud + ", " + parada.Longitud + ")] -> ";
                Console.Write(info);
                actual = actual.Siguiente;
            }

            Console.WriteLine("FIN");
        }
    }

    
    public void CargarDesdeCSV(string rutaArchivo, Action<Parada> alInsertarParada = null) // Carga inicial de paradas desde un archivo CSV
    {
        if (string.IsNullOrWhiteSpace(rutaArchivo) || !File.Exists(rutaArchivo))
        {
            Console.WriteLine("El archivo CSV no existe.");
            return;
        }

        string[] lineas = File.ReadAllLines(rutaArchivo);

        for (int i = 1; i < lineas.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lineas[i])) continue;

            string[] datos = lineas[i].Split(',');

            // Validamos que la fila tenga al menos las 4 columnas requeridas
            if (datos.Length < 4)
            {
                Console.WriteLine($"Fila {i + 1} ignorada: faltan columnas.");
                continue;
            }

            string codigo = datos[0].Trim().ToUpper();
            string nombre = datos[1].Trim();

            if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(nombre))
            {
                Console.WriteLine($"Fila {i + 1} ignorada: código o nombre vacío.");
                continue;
            }

            if (!double.TryParse(datos[2].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double latitud) ||
                !double.TryParse(datos[3].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double longitud))
            {
                Console.WriteLine($"Fila {i + 1} ignorada: coordenadas inválidas para el código {codigo}.");
                continue;
            }

            Parada nuevaParada = new Parada(codigo, nombre, latitud, longitud);

            if (Insertar(nuevaParada))
            {
                alInsertarParada?.Invoke(nuevaParada);
            }
        }

        Console.WriteLine("Carga inicial desde CSV completada.");
    }
}