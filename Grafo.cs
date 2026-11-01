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
    private int cantidadActuvalParadas;

    public GrafoTransporte(int cantidadParadas)
    {
        listaAdyacencia = new NodoRuta[cantidadParadas];
        codigosParadas = new string[cantidadParadas];
        cantidadActuvalParadas = 0;
    }

    // Agregar parada, Agrega ruta, bfs, dfs, dijkstra

}