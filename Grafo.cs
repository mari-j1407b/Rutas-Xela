
using System;

public class NodoRuta
{
    public string CodigoDestino;
    public int TiempoMinutos;
    public NodoRuta? Siguiente;

    public NodoRuta(string destino, int tiempo)
    {
        CodigoDestino = destino;
        TiempoMinutos = tiempo;
        Siguiente = null;
    }
}

public class GrafoTransporte
{
    private NodoRuta?[] listaAdyacencia;
    private string?[] codigosParadas;
    private int cantidadActualParadas;

    public GrafoTransporte(int cantidadParadas)
    {
        if (cantidadParadas <= 0)
        {
            throw new ArgumentException("La capacidad debe ser mayor que cero.");
        }

        listaAdyacencia = new NodoRuta?[cantidadParadas];
        codigosParadas = new string?[cantidadParadas];
        cantidadActualParadas = 0;
    }

    // Busca una parada y devuelve su posición.
    public int ObtenerIndiceParada(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
        {
            return -1;
        }

        codigo = codigo.Trim();

        for (int i = 0; i < codigosParadas.Length; i++)
        {
            if (codigosParadas[i] != null &&
                string.Equals(codigosParadas[i], codigo, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        return -1;
    }

    // Comprueba si una parada está registrada.
    public bool ExisteParada(string codigo)
    {
        return ObtenerIndiceParada(codigo) != -1;
    }

    // Agrega una parada utilizando una posición disponible.
    public void AgregarParada(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
        {
            Console.WriteLine("ERROR: El código de la parada no es válido.");
            return;
        }

        codigo = codigo.Trim();

        if (ExisteParada(codigo))
        {
            Console.WriteLine($"La parada {codigo} ya existe.");
            return;
        }

        for (int i = 0; i < codigosParadas.Length; i++)
        {
            if (codigosParadas[i] == null)
            {
                codigosParadas[i] = codigo;
                listaAdyacencia[i] = null;
                cantidadActualParadas++;
                return;
            }
        }

        Console.WriteLine("ERROR: El grafo está lleno.");
    }

    // Muestra todas las paradas registradas.
    public void MostrarParadas()
    {
        Console.WriteLine("-- Paradas Registradas --");

        if (cantidadActualParadas == 0)
        {
            Console.WriteLine("No hay paradas en el sistema.");
            return;
        }

        for (int i = 0; i < codigosParadas.Length; i++)
        {
            if (codigosParadas[i] != null)
            {
                Console.WriteLine($"Índice {i}: {codigosParadas[i]}");
            }
        }

        Console.WriteLine("---------------------------\n");
    }

    // Agrega una conexión entre dos paradas.
    public void AgregarRuta(string origen, string destino, int tiempo, bool unSoloSentido = false)
    {
        if (tiempo < 0)
        {
            Console.WriteLine("ERROR: El tiempo no puede ser negativo.");
            return;
        }

        int indiceOrigen = ObtenerIndiceParada(origen);
        int indiceDestino = ObtenerIndiceParada(destino);

        if (indiceOrigen == -1 || indiceDestino == -1)
        {
            Console.WriteLine("ERROR: Una o ambas paradas no existen.");
            return;
        }

        origen = codigosParadas[indiceOrigen]!;
        destino = codigosParadas[indiceDestino]!;

        AgregarConexion(indiceOrigen, destino, tiempo);

        if (!unSoloSentido)
        {
            AgregarConexion(indiceDestino, origen, tiempo);
        }
    }

    // Agrega una conexión o actualiza su tiempo si ya existe.
    private void AgregarConexion(int indiceOrigen, string destino, int tiempo)
    {
        NodoRuta? actual = listaAdyacencia[indiceOrigen];

        while (actual != null)
        {
            if (string.Equals(actual.CodigoDestino, destino, StringComparison.OrdinalIgnoreCase))
            {
                actual.TiempoMinutos = tiempo;
                return;
            }

            actual = actual.Siguiente;
        }

        NodoRuta nuevaRuta = new NodoRuta(destino, tiempo);
        nuevaRuta.Siguiente = listaAdyacencia[indiceOrigen];
        listaAdyacencia[indiceOrigen] = nuevaRuta;
    }

    // Muestra todas las conexiones del grafo.
    public void MostrarGrafo()
    {
        Console.WriteLine("-- Rutas del sistema --");

        for (int i = 0; i < codigosParadas.Length; i++)
        {
            if (codigosParadas[i] == null)
            {
                continue;
            }

            Console.Write($"Parada {codigosParadas[i]}: ");

            NodoRuta? actual = listaAdyacencia[i];

            if (actual == null)
            {
                Console.WriteLine("Nada");
                continue;
            }

            while (actual != null)
            {
                Console.Write($" -> {actual.CodigoDestino} ({actual.TiempoMinutos} min)");
                actual = actual.Siguiente;
            }

            Console.WriteLine();
        }

        Console.WriteLine("---------------------------\n");
    }

    // Realiza el recorrido en anchura.
    public void RecorridoBFS(string codigoInicio)
    {
        int indiceInicio = ObtenerIndiceParada(codigoInicio);

        if (indiceInicio == -1)
        {
            Console.WriteLine($"Error: La parada {codigoInicio} no existe para iniciar el BFS.");
            return;
        }

        bool[] visitados = new bool[codigosParadas.Length];
        ColaCircular cola = new ColaCircular(codigosParadas.Length);

        visitados[indiceInicio] = true;
        cola.Encolar(indiceInicio);

        Console.WriteLine($"\n-- Recorrido BFS (Desde {codigosParadas[indiceInicio]}) --");

        while (!cola.EstaVacia())
        {
            int actual = cola.Desencolar();

            Console.Write($"{codigosParadas[actual]} -> ");

            NodoRuta? vecino = listaAdyacencia[actual];

            while (vecino != null)
            {
                int indiceVecino = ObtenerIndiceParada(vecino.CodigoDestino);

                if (indiceVecino != -1 && !visitados[indiceVecino])
                {
                    visitados[indiceVecino] = true;
                    cola.Encolar(indiceVecino);
                }

                vecino = vecino.Siguiente;
            }
        }

        Console.WriteLine("FIN\n------------------------------------");
    }

    // Realiza el recorrido en profundidad.
    public void RecorridoDFS(string codigoInicio)
    {
        int indiceInicio = ObtenerIndiceParada(codigoInicio);

        if (indiceInicio == -1)
        {
            Console.WriteLine($"Error: La parada {codigoInicio} no existe para iniciar el DFS.");
            return;
        }

        bool[] visitados = new bool[codigosParadas.Length];
        PilaEstatica pila = new PilaEstatica(codigosParadas.Length);

        visitados[indiceInicio] = true;
        pila.Apilar(indiceInicio);

        Console.WriteLine($"\n--- Recorrido DFS (Desde {codigosParadas[indiceInicio]}) ---");

        while (!pila.EstaVacia())
        {
            int actual = pila.Desapilar();

            Console.Write($"{codigosParadas[actual]} -> ");

            NodoRuta? vecino = listaAdyacencia[actual];

            while (vecino != null)
            {
                int indiceVecino = ObtenerIndiceParada(vecino.CodigoDestino);

                if (indiceVecino != -1 && !visitados[indiceVecino])
                {
                    visitados[indiceVecino] = true;
                    pila.Apilar(indiceVecino);
                }

                vecino = vecino.Siguiente;
            }
        }

        Console.WriteLine("FIN\n------------------------------------");
    }

    // Calcula la ruta de menor tiempo mediante Dijkstra.
    public void CalcularRutaMasRapida(string origen, string destino)
    {
        int indiceOrigen = ObtenerIndiceParada(origen);
        int indiceDestino = ObtenerIndiceParada(destino);

        if (indiceOrigen == -1 || indiceDestino == -1)
        {
            Console.WriteLine($"Error: No se puede calcular la ruta. {origen} o {destino} no existen.");
            return;
        }

        int capacidad = codigosParadas.Length;

        int[] distancias = new int[capacidad];
        int[] padres = new int[capacidad];
        bool[] procesados = new bool[capacidad];

        for (int i = 0; i < capacidad; i++)
        {
            distancias[i] = int.MaxValue;
            padres[i] = -1;
        }

        distancias[indiceOrigen] = 0;

        ColaPrioridadMinima cola = new ColaPrioridadMinima(capacidad);
        cola.Insertar(indiceOrigen, 0);

        while (!cola.EstaVacia())
        {
            NodoPrioridad actual = cola.ExtraerMinimo();
            int u = actual.IndiceParada;

            // Ignora entradas antiguas de la cola.
            if (actual.DistanciaAcumulada != distancias[u])
            {
                continue;
            }

            if (procesados[u])
            {
                continue;
            }

            procesados[u] = true;

            if (u == indiceDestino)
            {
                break;
            }

            NodoRuta? vecino = listaAdyacencia[u];

            while (vecino != null)
            {
                int v = ObtenerIndiceParada(vecino.CodigoDestino);

                // Ignora conexiones que ya no apuntan a paradas registradas.
                if (v != -1 && !procesados[v])
                {
                    int peso = vecino.TiempoMinutos;

                    if (distancias[u] != int.MaxValue &&
                        distancias[u] <= int.MaxValue - peso)
                    {
                        int nuevaDistancia = distancias[u] + peso;

                        if (nuevaDistancia < distancias[v])
                        {
                            distancias[v] = nuevaDistancia;
                            padres[v] = u;
                            cola.Insertar(v, nuevaDistancia);
                        }
                    }
                }

                vecino = vecino.Siguiente;
            }
        }

        if (distancias[indiceDestino] == int.MaxValue)
        {
            Console.WriteLine($"\nNo existe una ruta conectada entre {origen} y {destino}.");
            return;
        }

        Console.WriteLine($"\n--- Ruta mas rapida ({codigosParadas[indiceOrigen]} a {codigosParadas[indiceDestino]}) ---");
        Console.WriteLine($"Tiempo total estimado: {distancias[indiceDestino]} minutos");

        int[] camino = new int[capacidad];
        int cantidadCamino = 0;

        int pasoActual = indiceDestino;

        while (pasoActual != -1 && cantidadCamino < capacidad)
        {
            camino[cantidadCamino] = pasoActual;
            cantidadCamino++;
            pasoActual = padres[pasoActual];
        }

        Console.Write("Ruta a seguir: ");

        for (int i = cantidadCamino - 1; i >= 0; i--)
        {
            Console.Write(codigosParadas[camino[i]]);

            if (i > 0)
            {
                Console.Write(" -> ");
            }
        }

        Console.WriteLine("\n----------------------------------------\n");
    }

    // Elimina todas las conexiones hacia un destino.
    private NodoRuta? EliminarNodoDeLista(NodoRuta? cabeza, string destinoABorrar)
    {
        while (cabeza != null &&
               string.Equals(cabeza.CodigoDestino, destinoABorrar, StringComparison.OrdinalIgnoreCase))
        {
            cabeza = cabeza.Siguiente;
        }

        NodoRuta? actual = cabeza;

        while (actual != null && actual.Siguiente != null)
        {
            if (string.Equals(actual.Siguiente.CodigoDestino, destinoABorrar, StringComparison.OrdinalIgnoreCase))
            {
                actual.Siguiente = actual.Siguiente.Siguiente;
            }
            else
            {
                actual = actual.Siguiente;
            }
        }

        return cabeza;
    }

    // Elimina una conexión.

    public bool EliminarRuta(string origen, string destino, bool unSoloSentido = false)
    {
        int indiceOrigen = ObtenerIndiceParada(origen);
        int indiceDestino = ObtenerIndiceParada(destino);

        if (indiceOrigen == -1 || indiceDestino == -1)
        {
            Console.WriteLine("Error: Una o ambas paradas no existen.");
            return false;
        }

        string codigoOrigen = codigosParadas[indiceOrigen]!;
        string codigoDestino = codigosParadas[indiceDestino]!;

        bool eliminada = false;

        NodoRuta? actual = listaAdyacencia[indiceOrigen];

        while (actual != null)
        {
            if (string.Equals(actual.CodigoDestino, codigoDestino, StringComparison.OrdinalIgnoreCase))
            {
                eliminada = true;
                break;
            }

            actual = actual.Siguiente;
        }

        listaAdyacencia[indiceOrigen] =
            EliminarNodoDeLista(listaAdyacencia[indiceOrigen], codigoDestino);

        if (!unSoloSentido)
        {
            listaAdyacencia[indiceDestino] =
                EliminarNodoDeLista(listaAdyacencia[indiceDestino], codigoOrigen);
        }

        if (eliminada)
        {
            Console.WriteLine($"Ruta entre {codigoOrigen} y {codigoDestino} eliminada.");
        }
        else
        {
            Console.WriteLine("No existía una ruta con esos códigos.");
        }

        return eliminada;
    }


    // Elimina una parada y todas sus conexiones.
    public bool EliminarParada(string codigo)
    {
        int indice = ObtenerIndiceParada(codigo);

        if (indice == -1)
        {
            Console.WriteLine($"Error: La parada {codigo} no existe.");
            return false;
        }

        string codigoEliminado = codigosParadas[indice]!;

        // Elimina las conexiones que llegan a la parada.
        for (int i = 0; i < listaAdyacencia.Length; i++)
        {
            listaAdyacencia[i] = EliminarNodoDeLista(listaAdyacencia[i], codigoEliminado);
        }

        // Elimina las conexiones que salen de la parada.
        listaAdyacencia[indice] = null;

        // Libera la posición para futuras paradas.
        codigosParadas[indice] = null;

        cantidadActualParadas--;

        Console.WriteLine($"\nLa parada {codigoEliminado} y todas sus rutas entrantes/salientes fueron eliminadas.");

        return true;
    }

    // Construye Union-Find utilizando las paradas y conexiones actuales.
    public UnionFind CrearUnionFind()
    {
        UnionFind unionFind = new UnionFind();

        // Primero registra todas las paradas existentes.
        for (int i = 0; i < codigosParadas.Length; i++)
        {
            if (codigosParadas[i] != null)
            {
                unionFind.AgregarParada(codigosParadas[i]!);
            }
        }

        // Une las paradas conectadas.
        // Las conexiones dirigidas se consideran no dirigidas para
        // comprobar la conectividad débil de la red.
        for (int i = 0; i < listaAdyacencia.Length; i++)
        {
            if (codigosParadas[i] == null)
            {
                continue;
            }

            NodoRuta? actual = listaAdyacencia[i];

            while (actual != null)
            {
                int indiceDestino = ObtenerIndiceParada(actual.CodigoDestino);

                if (indiceDestino != -1)
                {
                    unionFind.Union(codigosParadas[i]!, codigosParadas[indiceDestino]!);
                }

                actual = actual.Siguiente;
            }
        }

        return unionFind;
    }
}

// Cola circular utilizada por BFS.
public class ColaCircular
{
    private int[] arreglo;
    private int frente;
    private int final;
    private int capacidad;
    private int tamañoActual;

    public ColaCircular(int capacidad)
    {
        this.capacidad = capacidad;
        arreglo = new int[capacidad];
        frente = 0;
        final = -1;
        tamañoActual = 0;
    }

    public bool EstaVacia()
    {
        return tamañoActual == 0;
    }

    public void Encolar(int elemento)
    {
        if (tamañoActual == capacidad)
        {
            return;
        }

        final = (final + 1) % capacidad;
        arreglo[final] = elemento;
        tamañoActual++;
    }

    public int Desencolar()
    {
        if (EstaVacia())
        {
            return -1;
        }

        int elemento = arreglo[frente];
        frente = (frente + 1) % capacidad;
        tamañoActual--;

        return elemento;
    }
}

// Pila estática utilizada por DFS.
public class PilaEstatica
{
    private int[] arreglo;
    private int tope;
    private int capacidad;

    public PilaEstatica(int capacidad)
    {
        this.capacidad = capacidad;
        arreglo = new int[capacidad];
        tope = -1;
    }

    public bool EstaVacia()
    {
        return tope == -1;
    }

    public void Apilar(int elemento)
    {
        if (tope == capacidad - 1)
        {
            return;
        }

        tope++;
        arreglo[tope] = elemento;
    }

    public int Desapilar()
    {
        if (EstaVacia())
        {
            return -1;
        }

        int elemento = arreglo[tope];
        tope--;

        return elemento;
    }
}
