public class NodoRuta
{
    public string CodigoDestino;
    public int TiempoMinutos;
    public NodoRuta Siguiente;

    public NodoRuta(string destino, int tiempo)
    {
        CodigoDestino = destino;
        TiempoMinutos = tiempo;
        Siguiente = null;
    }

}

public class GrafoTransporte
{
    private NodoRuta[] listaAdyacencia;
    private string[] codigosParadas;
    private int cantidadActualParadas;


    public GrafoTransporte(int cantidadParadas)
    {
        listaAdyacencia = new NodoRuta[cantidadParadas];
        codigosParadas = new string[cantidadParadas];
        cantidadActualParadas = 0;
    }

    // Agregar parada, Agrega ruta, bfs, dfs, dijkstra

    public int ObtenerIndiceParada(string codigo)
    {
        for (int i = 0; i < cantidadActualParadas; i++)
        {
            if (codigosParadas[i] == codigo)
            {
                return i;
            }
        }
        return -1;
    }

    public void AgregarParada(string codigo)
    {
        if (cantidadActualParadas >= codigosParadas.Length)
        {
            Console.WriteLine("ERROR: Grafo lleno, no se pueden agregar mas paradas.");
            return;
        }

        if (ObtenerIndiceParada(codigo) != -1)
        {
            Console.WriteLine($"La parada con codigo {codigo} ya existe.");
            return;
        }

        codigosParadas[cantidadActualParadas] = codigo;
        listaAdyacencia[cantidadActualParadas] = null;
        cantidadActualParadas++;
    }

    public void MostrarParadas()
    {
        Console.WriteLine("-- Paradas Registradas --");
        if (cantidadActualParadas == 0)
        {
            Console.WriteLine("No hay paradas en el sistema.");
            return;
        }

        for (int i = 0; i < cantidadActualParadas; i++)
        {
            if (codigosParadas[i] == null) continue;
            Console.WriteLine($"Índice {i}: {codigosParadas[i]}");
        }
        Console.WriteLine("---------------------------\n");
    }

    // registra nuevas rutas
    public void AgregarRuta(string origen, string destino, int tiempo, bool unSoloSentido = false)
    {
        int indiceOrigen = ObtenerIndiceParada(origen);
        int indiceDestino = ObtenerIndiceParada(destino);
        if (indiceOrigen == -1 || indiceDestino == -1)
        {
            Console.WriteLine("ERROR: Una o ambas paradas no existen.");
            return;
        }
        NodoRuta nuevaRuta = new NodoRuta(destino, tiempo);
        nuevaRuta.Siguiente = listaAdyacencia[indiceOrigen];
        listaAdyacencia[indiceOrigen] = nuevaRuta;
        if (!unSoloSentido)
        {
            NodoRuta rutaVuelta = new NodoRuta(origen, tiempo);
            rutaVuelta.Siguiente = listaAdyacencia[indiceDestino];
            listaAdyacencia[indiceDestino] = rutaVuelta;
        }
    }

    // imprime la lista de adyacencia completa 
    public void MostrarGrafo()
    {
        Console.WriteLine("-- Rutas del sistema --");
        for (int i = 0; i < cantidadActualParadas; i++)
        {
            if (codigosParadas[i] == null) continue;
            Console.Write($"Parada {codigosParadas[i]}: ");
            NodoRuta actual = listaAdyacencia[i];
            while (actual != null)
            {
                Console.Write($" -> {actual.CodigoDestino} ({actual.TiempoMinutos} min)");
                actual = actual.Siguiente;
            }
            Console.WriteLine("Nada");
        }
        Console.WriteLine("---------------------------\n");

    }

    public void RecorridoBFS(string codigoInicio)
    {
        int indiceInicio = ObtenerIndiceParada(codigoInicio);
        if (indiceInicio == -1)
        {
            Console.WriteLine($"Error: La parada {codigoInicio} no existe para iniciar el BFS.");
            return;
        }

        bool[] visitados = new bool[cantidadActualParadas];
        ColaCircular cola = new ColaCircular(cantidadActualParadas);
        visitados[indiceInicio] = true;
        cola.Encolar(indiceInicio);

        Console.WriteLine($"\n-- Recorrido BFS (Desde {codigoInicio}) --");

        while (!cola.EstaVacia())
        {
            int actual = cola.Desencolar();
            Console.Write($"{codigosParadas[actual]} -> ");
            NodoRuta vecino = listaAdyacencia[actual];
            while (vecino != null)
            {
                int indiceVecino = ObtenerIndiceParada(vecino.CodigoDestino);
                if (!visitados[indiceVecino])
                {
                    visitados[indiceVecino] = true;
                    cola.Encolar(indiceVecino);
                }

                vecino = vecino.Siguiente;
            }
        }
        Console.WriteLine("FIN\n------------------------------------");
    }

    public void RecorridoDFS(string codigoInicio)
    {
        int indiceInicio = ObtenerIndiceParada(codigoInicio);
        if (indiceInicio == -1)
        {
            Console.WriteLine($"Error: La parada {codigoInicio} no existe para iniciar el DFS.");
            return;
        }

        bool[] visitados = new bool[cantidadActualParadas];
        PilaEstatica pila = new PilaEstatica(cantidadActualParadas * 2);
        pila.Apilar(indiceInicio);

        Console.WriteLine($"\n--- Recorrido DFS (Desde {codigoInicio}) ---");

        while (!pila.EstaVacia())
        {
            int actual = pila.Desapilar();

            if (!visitados[actual])
            {
                Console.Write($"{codigosParadas[actual]} -> ");
                visitados[actual] = true;
                NodoRuta vecino = listaAdyacencia[actual];
                while (vecino != null)
                {
                    int indiceVecino = ObtenerIndiceParada(vecino.CodigoDestino);

                    if (!visitados[indiceVecino])
                    {
                        pila.Apilar(indiceVecino);
                    }

                    vecino = vecino.Siguiente;
                }
            }
        }
        Console.WriteLine("FIN\n------------------------------------");
    }

    public void CalcularRutaMasRapida(string origen, string destino)
    {
        int indiceOrigen = ObtenerIndiceParada(origen);
        int indiceDestino = ObtenerIndiceParada(destino);

        if (indiceOrigen == -1 || indiceDestino == -1)
        {
            Console.WriteLine($"Error: No se puede calcular la ruta. {origen} o {destino} no existen.");
            return;
        }

        // Arreglos para Dijkstra
        int[] distancias = new int[cantidadActualParadas];
        int[] padres = new int[cantidadActualParadas]; // Para reconstruir el camino exacto
        bool[] procesados = new bool[cantidadActualParadas];

        for (int i = 0; i < cantidadActualParadas; i++)
        {
            distancias[i] = int.MaxValue;
            padres[i] = -1;
        }

        distancias[indiceOrigen] = 0;
        ColaPrioridadMinima pq = new ColaPrioridadMinima(cantidadActualParadas * 10);
        pq.Insertar(indiceOrigen, 0);

        while (!pq.EstaVacia())
        {
            NodoPrioridad actual = pq.ExtraerMinimo();
            int u = actual.IndiceParada;
            if (procesados[u]) continue;
            procesados[u] = true;
            if (u == indiceDestino) break;

            NodoRuta vecino = listaAdyacencia[u];
            while (vecino != null)
            {
                int v = ObtenerIndiceParada(vecino.CodigoDestino);
                int peso = vecino.TiempoMinutos;
                if (!procesados[v] && distancias[u] != int.MaxValue && distancias[u] + peso < distancias[v])
                {
                    distancias[v] = distancias[u] + peso;
                    padres[v] = u; // Guardamos de dónde venimos para trazar la ruta luego
                    pq.Insertar(v, distancias[v]);
                }
                vecino = vecino.Siguiente;
            }
        }

        // Resultados
        if (distancias[indiceDestino] == int.MaxValue)
        {
            Console.WriteLine($"\nNo existe una ruta conectada entre {origen} y {destino}.");
            return;
        }

        Console.WriteLine($"\n--- Ruta mas rapida({origen} a {destino}) ---");
        Console.WriteLine($"Tiempo total estimado: {distancias[indiceDestino]} minutos");
        PilaEstatica camino = new PilaEstatica(cantidadActualParadas);
        int pasoActual = indiceDestino;

        while (pasoActual != -1)
        {
            camino.Apilar(pasoActual);
            pasoActual = padres[pasoActual];
        }

        Console.Write("Ruta a seguir: ");
        while (!camino.EstaVacia())
        {
            int nodo = camino.Desapilar();
            Console.Write(codigosParadas[nodo] + (camino.EstaVacia() ? "" : " -> "));
        }
        Console.WriteLine("\n----------------------------------------\n");
    }

    private NodoRuta EliminarNodoDeLista(NodoRuta cabeza, string destinoABorrar)
    {
        if (cabeza == null) return null;
        if (cabeza.CodigoDestino == destinoABorrar)
        {
            return cabeza.Siguiente;
        }

        NodoRuta actual = cabeza;
        while (actual.Siguiente != null && actual.Siguiente.CodigoDestino != destinoABorrar)
        {
            actual = actual.Siguiente;
        }

        if (actual.Siguiente != null)
        {
            actual.Siguiente = actual.Siguiente.Siguiente;
        }

        return cabeza;
    }

    // Método para eliminar la conexión entre dos paradas
    public void EliminarRuta(string origen, string destino, bool unSoloSentido = false)
    {
        int indiceOrigen = ObtenerIndiceParada(origen);
        if (indiceOrigen != -1)
        {
            listaAdyacencia[indiceOrigen] = EliminarNodoDeLista(listaAdyacencia[indiceOrigen], destino);
        }

        if (!unSoloSentido)
        {
            int indiceDestino = ObtenerIndiceParada(destino);
            if (indiceDestino != -1)
            {
                listaAdyacencia[indiceDestino] = EliminarNodoDeLista(listaAdyacencia[indiceDestino], origen);
            }
        }
    }

    // Método para borrar una parada (Soft-deletion)
    public void EliminarParada(string codigo)
    {
        int indice = ObtenerIndiceParada(codigo);
        if (indice == -1)
        {
            Console.WriteLine($"Error: La parada {codigo} no existe.");
            return;
        }
        // borrado logico
        codigosParadas[indice] = null;
        listaAdyacencia[indice] = null;

        // eliminar ruta que apunta a la parada 
        for (int i = 0; i < cantidadActualParadas; i++)
        {
            if (codigosParadas[i] != null)
            {
                listaAdyacencia[i] = EliminarNodoDeLista(listaAdyacencia[i], codigo);
            }
        }

        Console.WriteLine($"\nLa parada {codigo} y todas sus rutas entrantes/salientes fueron eliminadas.");
    }
}

// Estructura para el recorrido BFS
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
        if (tamañoActual == capacidad) return; // Cola llena

        final = (final + 1) % capacidad;
        arreglo[final] = elemento;
        tamañoActual++;
    }

    public int Desencolar()
    {
        if (EstaVacia()) return -1;

        int elemento = arreglo[frente];
        frente = (frente + 1) % capacidad;
        tamañoActual--;
        return elemento;
    }
}

// Estructura para el recorrido DFS
public class PilaEstatica
{
    private int[] arreglo;
    private int tope;
    private int capacidad;

    public PilaEstatica(int capacidad)
    {
        this.capacidad = capacidad;
        arreglo = new int[capacidad];
        tope = -1; // vacia
    }

    public bool EstaVacia()
    {
        return tope == -1;
    }

    public void Apilar(int elemento)
    {
        if (tope == capacidad - 1) return; // Pila llena

        tope++;
        arreglo[tope] = elemento;
    }

    public int Desapilar()
    {
        if (EstaVacia()) return -1;

        int elemento = arreglo[tope];
        tope--;
        return elemento;
    }

}