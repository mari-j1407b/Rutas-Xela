
using System;
using System.Collections.Generic;

public sealed class ConexionRuta
{
    public string CodigoOrigen { get; }
    public string CodigoDestino { get; }
    public int TiempoMinutos { get; }
    public double Costo { get; }
    public int NumeroLinea { get; }
    public bool UnSoloSentido { get; }

    public ConexionRuta(
        string codigoOrigen,
        string codigoDestino,
        int tiempoMinutos,
        double costo,
        int numeroLinea,
        bool unSoloSentido)
    {
        CodigoOrigen = codigoOrigen;
        CodigoDestino = codigoDestino;
        TiempoMinutos = tiempoMinutos;
        Costo = costo;
        NumeroLinea = numeroLinea;
        UnSoloSentido = unSoloSentido;
    }
}

public sealed class ResultadoRutaGrafo
{
    public bool Exito { get; }
    public string Mensaje { get; }
    public string[] Camino { get; }
    public int TiempoTotal { get; }
    public double CostoTotal { get; }

    public ResultadoRutaGrafo(bool exito, string mensaje, string[] camino, int tiempoTotal)
        : this(exito, mensaje, camino, tiempoTotal, 0)
    {
    }

    public ResultadoRutaGrafo(bool exito, string mensaje, string[] camino, int tiempoTotal, double costoTotal)
    {
        Exito = exito;
        Mensaje = mensaje;
        Camino = camino;
        TiempoTotal = tiempoTotal;
        CostoTotal = costoTotal;
    }
}

public class NodoRuta
{
    public string CodigoOrigen;
    public string CodigoDestino;
    public int TiempoMinutos;
    public double Costo;
    public int NumeroLinea;
    public bool UnSoloSentido;
    public NodoRuta? Siguiente;

    public NodoRuta(string destino, int tiempo)
        : this("", destino, tiempo, 0, 1, true)
    {
    }

    public NodoRuta(
        string origen,
        string destino,
        int tiempo,
        double costo,
        int numeroLinea,
        bool unSoloSentido)
    {
        CodigoOrigen = origen;
        CodigoDestino = destino;
        TiempoMinutos = tiempo;
        Costo = costo;
        NumeroLinea = numeroLinea;
        UnSoloSentido = unSoloSentido;
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

        int nuevaCapacidad = codigosParadas.Length * 2;
        Array.Resize(ref codigosParadas, nuevaCapacidad);
        Array.Resize(ref listaAdyacencia, nuevaCapacidad);
        codigosParadas[cantidadActualParadas] = codigo;
        listaAdyacencia[cantidadActualParadas] = null;
        cantidadActualParadas++;
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
        AgregarRuta(origen, destino, tiempo, 0, 1, unSoloSentido);
    }

    // Agrega una conexión con todos sus metadatos.
    public void AgregarRuta(
        string origen,
        string destino,
        int tiempo,
        double costo,
        int numeroLinea,
        bool unSoloSentido = false)
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

        AgregarConexion(
            indiceOrigen, origen, destino, tiempo, costo, numeroLinea, unSoloSentido);

        if (!unSoloSentido)
        {
            AgregarConexion(
                indiceDestino, destino, origen, tiempo, costo, numeroLinea, false);
        }
    }

    // Agrega una conexión o actualiza sus datos si ya existe.
    private void AgregarConexion(
        int indiceOrigen,
        string origen,
        string destino,
        int tiempo,
        double costo,
        int numeroLinea,
        bool unSoloSentido)
    {
        NodoRuta? actual = listaAdyacencia[indiceOrigen];

        while (actual != null)
        {
            if (string.Equals(actual.CodigoDestino, destino, StringComparison.OrdinalIgnoreCase))
            {
                actual.CodigoOrigen = origen;
                actual.TiempoMinutos = tiempo;
                actual.Costo = costo;
                actual.NumeroLinea = numeroLinea;
                actual.UnSoloSentido = unSoloSentido;
                return;
            }

            actual = actual.Siguiente;
        }

        NodoRuta nuevaRuta = new NodoRuta(
            origen, destino, tiempo, costo, numeroLinea, unSoloSentido);
        nuevaRuta.Siguiente = listaAdyacencia[indiceOrigen];
        listaAdyacencia[indiceOrigen] = nuevaRuta;
    }

    // Devuelve cada ruta lógica una sola vez, aunque una bidireccional use dos aristas.
    public IEnumerable<ConexionRuta> ObtenerConexiones()
    {
        for (int i = 0; i < listaAdyacencia.Length; i++)
        {
            NodoRuta? actual = listaAdyacencia[i];

            while (actual != null)
            {
                if (actual.UnSoloSentido ||
                    StringComparer.OrdinalIgnoreCase.Compare(
                        actual.CodigoOrigen, actual.CodigoDestino) <= 0 ||
                    !ExisteAristaBidireccionalInversa(actual))
                {
                    yield return new ConexionRuta(
                        actual.CodigoOrigen,
                        actual.CodigoDestino,
                        actual.TiempoMinutos,
                        actual.Costo,
                        actual.NumeroLinea,
                        actual.UnSoloSentido);
                }

                actual = actual.Siguiente;
            }
        }
    }

    private bool ExisteAristaBidireccionalInversa(NodoRuta conexion)
    {
        int indiceInverso = ObtenerIndiceParada(conexion.CodigoDestino);

        if (indiceInverso == -1)
        {
            return false;
        }

        NodoRuta? inversa = listaAdyacencia[indiceInverso];

        while (inversa != null)
        {
            if (!inversa.UnSoloSentido &&
                string.Equals(inversa.CodigoDestino, conexion.CodigoOrigen, StringComparison.OrdinalIgnoreCase) &&
                inversa.TiempoMinutos == conexion.TiempoMinutos &&
                inversa.Costo == conexion.Costo &&
                inversa.NumeroLinea == conexion.NumeroLinea)
            {
                return true;
            }

            inversa = inversa.Siguiente;
        }

        return false;
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
    public string[] RecorridoBFS(string codigoInicio)
    {
        int indiceInicio = ObtenerIndiceParada(codigoInicio);

        if (indiceInicio == -1)
        {
            Console.WriteLine($"Error: La parada {codigoInicio} no existe para iniciar el BFS.");
            return Array.Empty<string>();
        }

        bool[] visitados = new bool[codigosParadas.Length];
        string[] recorrido = new string[cantidadActualParadas];
        int cantidadRecorrida = 0;
        ColaCircular cola = new ColaCircular(codigosParadas.Length);

        visitados[indiceInicio] = true;
        cola.Encolar(indiceInicio);

        Console.WriteLine($"\n-- Recorrido BFS (Desde {codigosParadas[indiceInicio]}) --");

        while (!cola.EstaVacia())
        {
            int actual = cola.Desencolar();

            recorrido[cantidadRecorrida++] = codigosParadas[actual]!;
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
        Array.Resize(ref recorrido, cantidadRecorrida);
        return recorrido;
    }

    // Realiza el recorrido en profundidad.
    public string[] RecorridoDFS(string codigoInicio)
    {
        int indiceInicio = ObtenerIndiceParada(codigoInicio);

        if (indiceInicio == -1)
        {
            Console.WriteLine($"Error: La parada {codigoInicio} no existe para iniciar el DFS.");
            return Array.Empty<string>();
        }

        bool[] visitados = new bool[codigosParadas.Length];
        string[] recorrido = new string[cantidadActualParadas];
        int cantidadRecorrida = 0;
        PilaEstatica pila = new PilaEstatica(codigosParadas.Length);

        visitados[indiceInicio] = true;
        pila.Apilar(indiceInicio);

        Console.WriteLine($"\n--- Recorrido DFS (Desde {codigosParadas[indiceInicio]}) ---");

        while (!pila.EstaVacia())
        {
            int actual = pila.Desapilar();

            recorrido[cantidadRecorrida++] = codigosParadas[actual]!;
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
        Array.Resize(ref recorrido, cantidadRecorrida);
        return recorrido;
    }

    // Calcula la ruta de menor tiempo mediante Dijkstra.
    public ResultadoRutaGrafo CalcularRutaMasRapida(string origen, string destino)
    {
        return CalcularRutaDijkstra(origen, destino, ruta => ruta.TiempoMinutos, "más rápida");
    }

    // Calcula la ruta de menor costo mediante Dijkstra.
    public ResultadoRutaGrafo CalcularRutaMasEconomica(string origen, string destino)
    {
        return CalcularRutaDijkstra(origen, destino, ruta => ruta.Costo, "más económica");
    }

    private ResultadoRutaGrafo CalcularRutaDijkstra(
        string origen,
        string destino,
        Func<NodoRuta, double> obtenerPeso,
        string criterio)
    {
        int indiceOrigen = ObtenerIndiceParada(origen);
        int indiceDestino = ObtenerIndiceParada(destino);

        if (indiceOrigen == -1 || indiceDestino == -1)
        {
            string mensaje = $"No se puede calcular la ruta. {origen} o {destino} no existen.";
            Console.WriteLine($"Error: {mensaje}");
            return new ResultadoRutaGrafo(false, mensaje, Array.Empty<string>(), 0);
        }

        int capacidad = codigosParadas.Length;
        double[] distancias = new double[capacidad];
        int[] padres = new int[capacidad];
        NodoRuta?[] aristasAnteriores = new NodoRuta?[capacidad];
        bool[] procesados = new bool[capacidad];

        for (int i = 0; i < capacidad; i++)
        {
            distancias[i] = double.PositiveInfinity;
            padres[i] = -1;
        }

        distancias[indiceOrigen] = 0;
        PriorityQueue<int, double> cola = new();
        cola.Enqueue(indiceOrigen, 0);

        while (cola.Count > 0)
        {
            cola.TryDequeue(out int u, out double distanciaActual);
            if (distanciaActual != distancias[u] || procesados[u])
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
                if (v != -1 && !procesados[v])
                {
                    double peso = obtenerPeso(vecino);
                    if (!double.IsFinite(peso) || peso < 0)
                    {
                        string mensaje = $"No se puede calcular la ruta: la conexión {vecino.CodigoOrigen} - {vecino.CodigoDestino} tiene un peso inválido.";
                        return new ResultadoRutaGrafo(false, mensaje, Array.Empty<string>(), 0);
                    }

                    double nuevaDistancia = distancias[u] + peso;
                    if (double.IsFinite(nuevaDistancia) && nuevaDistancia < distancias[v])
                    {
                        distancias[v] = nuevaDistancia;
                        padres[v] = u;
                        aristasAnteriores[v] = vecino;
                        cola.Enqueue(v, nuevaDistancia);
                    }
                }

                vecino = vecino.Siguiente;
            }
        }

        if (double.IsPositiveInfinity(distancias[indiceDestino]))
        {
            string mensaje = $"No existe una ruta conectada entre {origen} y {destino}.";
            Console.WriteLine($"\n{mensaje}");
            return new ResultadoRutaGrafo(false, mensaje, Array.Empty<string>(), 0);
        }

        int[] caminoIndices = new int[capacidad];
        int cantidadCamino = 0;
        int pasoActual = indiceDestino;
        while (pasoActual != -1 && cantidadCamino < capacidad)
        {
            caminoIndices[cantidadCamino++] = pasoActual;
            pasoActual = padres[pasoActual];
        }

        Array.Reverse(caminoIndices, 0, cantidadCamino);
        string[] codigosCamino = new string[cantidadCamino];
        long tiempoTotal = 0;
        double costoTotal = 0;
        for (int i = 0; i < cantidadCamino; i++)
        {
            int indice = caminoIndices[i];
            codigosCamino[i] = codigosParadas[indice]!;
            if (i == 0)
            {
                continue;
            }

            NodoRuta arista = aristasAnteriores[indice]!;
            tiempoTotal += arista.TiempoMinutos;
            costoTotal += arista.Costo;
        }

        if (tiempoTotal > int.MaxValue || !double.IsFinite(costoTotal))
        {
            string mensaje = "El total de tiempo o costo de la ruta excede el rango permitido.";
            return new ResultadoRutaGrafo(false, mensaje, Array.Empty<string>(), 0);
        }

        Console.WriteLine($"\n--- Ruta {criterio} ({codigosCamino[0]} a {codigosCamino[^1]}) ---");
        Console.WriteLine($"Tiempo total: {tiempoTotal} minutos; costo total: Q{costoTotal:F2}");
        Console.WriteLine("Ruta a seguir: " + string.Join(" -> ", codigosCamino));
        Console.WriteLine("----------------------------------------\n");
        return new ResultadoRutaGrafo(true, "OK", codigosCamino, (int)tiempoTotal, costoTotal);
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
