namespace RutasXela.UI;

// ─────────────────────────────────────────────────────────────
//  MODELOS + CONTRATO ENTRE LA INTERFAZ Y TU LÓGICA
//  La UI SOLO habla con IRutasService. Para conectar tu proyecto
//  real, crea una clase que implemente esta interfaz y que use tu
//  Tabla Hash, Grafo, Union-Find y Trie por dentro.
// ─────────────────────────────────────────────────────────────

public class Parada
{
    public string Codigo { get; set; } = "";
    public string Nombre { get; set; } = "";
    public double Latitud { get; set; }
    public double Longitud { get; set; }
    public override string ToString() => $"{Codigo} · {Nombre}";
}

public class Ruta
{
    public string Origen { get; set; } = "";
    public string Destino { get; set; } = "";
    public int Tiempo { get; set; }          // minutos
    public double Costo { get; set; }        // quetzales
    public bool UnSentido { get; set; }
    public int Linea { get; set; } = 1;      // solo para el color en el mapa
}

public enum Criterio { Rapida, Economica }

public sealed class DetalleConexionRuta
{
    public string Origen { get; set; } = "";
    public string Destino { get; set; } = "";
    public int Linea { get; set; }
    public int Tiempo { get; set; }
    public double Costo { get; set; }
}

public class ResultadoRuta
{
    public bool Exito { get; set; }
    public string Mensaje { get; set; } = "";
    public string[] Camino { get; set; } = Array.Empty<string>();   // códigos de parada en orden
    public int TiempoTotal { get; set; }
    public double CostoTotal { get; set; }
    public DetalleConexionRuta[] DetalleConexiones { get; set; } = Array.Empty<DetalleConexionRuta>();
}

public interface IRutasService
{
    Parada[] ObtenerParadas();                 // para dibujar el mapa y llenar combos (recorrido de la Tabla Hash)
    Ruta[] ObtenerRutas();                     // aristas del Grafo

    bool AgregarParada(Parada p);              // Tabla Hash + Trie + Grafo (vértice) + Union-Find (MakeSet)
    bool AgregarRuta(Ruta r);                  // Grafo (arista) + Union-Find (Union)
    bool ModificarParada(Parada parada);
    int ContarRutasConectadas(string codigo);
    bool EliminarParada(string codigo);

    Parada BuscarParada(string codigo);
    Parada[] Autocompletar(string prefijo);    // Trie (prefijo) → Tabla Hash (registro completo)

    bool MismaZona(string codigoA, string codigoB);                          // Union-Find (Find con compresión)
    ResultadoRuta CalcularRuta(string origen, string destino, Criterio c);   // Dijkstra
    string[] RecorridoBFS(string inicio);                                    // Grafo BFS
    string[] RecorridoDFS(string inicio);                                    // Grafo DFS

    string CargarCSV(string rutaArchivo);      // devuelve un mensaje resumen
}
