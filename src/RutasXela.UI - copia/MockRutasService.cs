using System.Globalization;

namespace RutasXela.UI;

// ─────────────────────────────────────────────────────────────
//  SERVICIO DE PRUEBA (solo para ver la interfaz funcionando).
//  OJO: aquí usamos List<T> por rapidez. Es SOLO un "relleno":
//  tu implementación real debe usar TUS estructuras propias.
// ─────────────────────────────────────────────────────────────
public class MockRutasService : IRutasService
{
    private readonly List<Parada> _p = new();
    private readonly List<Ruta> _r = new();

    public MockRutasService()
    {
        // Datos de demostración (en tu proyecto real vendrán de CSV / menú)
        P("P001", "Parque Central", 14.8347, -91.5180);
        P("P002", "Terminal", 14.8300, -91.5260);
        P("P003", "Universidad", 14.8390, -91.5300);
        P("P004", "Hospital", 14.8420, -91.5120);
        P("P005", "Mercado", 14.8330, -91.5080);
        P("P006", "Estación Norte", 14.8480, -91.5200);
        P("P007", "Olintepeque", 14.8200, -91.5000);   // zona aislada para probar "no conectadas"
        P("P008", "Salcajá", 14.8150, -91.4900);

        R("P001", "P002", 5, 1.50, false, 1);
        R("P002", "P003", 8, 2.00, false, 1);
        R("P001", "P004", 6, 2.00, false, 2);
        R("P004", "P005", 4, 1.00, false, 2);
        R("P001", "P005", 10, 1.50, true, 3);   // un solo sentido
        R("P005", "P006", 5, 2.00, false, 3);
        R("P003", "P006", 7, 3.00, false, 4);
        R("P007", "P008", 12, 4.00, false, 5);
    }

    private void P(string c, string n, double lat, double lon) => _p.Add(new Parada { Codigo = c, Nombre = n, Latitud = lat, Longitud = lon });
    private void R(string o, string d, int t, double c, bool u, int l) => _r.Add(new Ruta { Origen = o, Destino = d, Tiempo = t, Costo = c, UnSentido = u, Linea = l });

    private int Idx(string codigo) => _p.FindIndex(x => string.Equals(x.Codigo, codigo?.Trim(), StringComparison.OrdinalIgnoreCase));

    public Parada[] ObtenerParadas() => _p.ToArray();
    public Ruta[] ObtenerRutas() => _r.ToArray();

    public bool AgregarParada(Parada p)
    {
        if (string.IsNullOrWhiteSpace(p.Codigo) || string.IsNullOrWhiteSpace(p.Nombre) || Idx(p.Codigo) >= 0) return false;
        p.Codigo = p.Codigo.Trim().ToUpper();
        _p.Add(p);
        return true;
    }

    public bool AgregarRuta(Ruta r)
    {
        int a = Idx(r.Origen), b = Idx(r.Destino);
        if (a < 0 || b < 0 || a == b) return false;
        r.Origen = _p[a].Codigo; r.Destino = _p[b].Codigo;
        _r.Add(r);
        return true;
    }

    public bool ModificarParada(Parada parada)
    {
        int indice = Idx(parada?.Codigo);
        if (indice < 0 || string.IsNullOrWhiteSpace(parada.Nombre) ||
            !AreaGeograficaXela.Contiene(parada.Latitud, parada.Longitud) ||
            _p.Any(actual => actual != _p[indice] && string.Equals(actual.Nombre, parada.Nombre.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        _p[indice].Nombre = parada.Nombre.Trim();
        _p[indice].Latitud = parada.Latitud;
        _p[indice].Longitud = parada.Longitud;
        return true;
    }

    public int ContarRutasConectadas(string codigo)
    {
        return _r.Count(ruta => string.Equals(ruta.Origen, codigo, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(ruta.Destino, codigo, StringComparison.OrdinalIgnoreCase));
    }

    public bool EliminarParada(string codigo)
    {
        int indice = Idx(codigo);
        if (indice < 0) return false;
        string codigoParada = _p[indice].Codigo;
        _r.RemoveAll(ruta => string.Equals(ruta.Origen, codigoParada, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(ruta.Destino, codigoParada, StringComparison.OrdinalIgnoreCase));
        _p.RemoveAt(indice);
        return true;
    }

    public Parada BuscarParada(string codigo)
    {
        int i = Idx(codigo);
        return i < 0 ? null : _p[i];
    }

    public Parada[] Autocompletar(string prefijo)
    {
        prefijo = prefijo.Trim();
        return _p.Where(x => x.Codigo.StartsWith(prefijo, StringComparison.OrdinalIgnoreCase)
                          || x.Nombre.StartsWith(prefijo, StringComparison.OrdinalIgnoreCase))
                 .Take(8).ToArray();
    }

    private IEnumerable<(int, Ruta)> Vecinos(int u, bool respetarSentido)
    {
        foreach (var r in _r)
        {
            if (Idx(r.Origen) == u) yield return (Idx(r.Destino), r);
            else if ((!respetarSentido || !r.UnSentido) && Idx(r.Destino) == u) yield return (Idx(r.Origen), r);
        }
    }

    public bool MismaZona(string a, string b)
    {
        int s = Idx(a), t = Idx(b);
        if (s < 0 || t < 0) return false;
        var vis = new bool[_p.Count];
        var pila = new Stack<int>();
        pila.Push(s); vis[s] = true;
        while (pila.Count > 0)
        {
            int u = pila.Pop();
            if (u == t) return true;
            foreach (var (v, _) in Vecinos(u, false))
                if (!vis[v]) { vis[v] = true; pila.Push(v); }
        }
        return false;
    }

    public ResultadoRuta CalcularRuta(string origen, string destino, Criterio c)
    {
        int s = Idx(origen), t = Idx(destino), n = _p.Count;
        if (s < 0 || t < 0) return new ResultadoRuta { Mensaje = "Parada no encontrada." };

        var dist = Enumerable.Repeat(double.PositiveInfinity, n).ToArray();
        var prev = Enumerable.Repeat(-1, n).ToArray();
        var prevR = new Ruta[n];
        var vis = new bool[n];
        dist[s] = 0;

        for (int k = 0; k < n; k++)
        {
            int u = -1;
            for (int i = 0; i < n; i++) if (!vis[i] && (u < 0 || dist[i] < dist[u])) u = i;
            if (u < 0 || double.IsInfinity(dist[u])) break;
            vis[u] = true;
            if (u == t) break;
            foreach (var (v, r) in Vecinos(u, true))
            {
                double w = c == Criterio.Rapida ? r.Tiempo : r.Costo;
                if (dist[u] + w < dist[v]) { dist[v] = dist[u] + w; prev[v] = u; prevR[v] = r; }
            }
        }

        if (double.IsInfinity(dist[t]))
            return new ResultadoRuta { Mensaje = "No hay ruta posible respetando los sentidos." };

        var camino = new List<string>();
        int tiempo = 0; double costo = 0;
        for (int x = t; x != -1; x = prev[x])
        {
            camino.Insert(0, _p[x].Codigo);
            if (prevR[x] != null) { tiempo += prevR[x].Tiempo; costo += prevR[x].Costo; }
        }
        return new ResultadoRuta { Exito = true, Camino = camino.ToArray(), TiempoTotal = tiempo, CostoTotal = costo, Mensaje = "OK" };
    }

    public string[] RecorridoBFS(string inicio)
    {
        int s = Idx(inicio);
        if (s < 0) return Array.Empty<string>();
        var vis = new bool[_p.Count];
        var orden = new List<string>();
        var cola = new Queue<int>();
        cola.Enqueue(s); vis[s] = true;
        while (cola.Count > 0)
        {
            int u = cola.Dequeue();
            orden.Add(_p[u].Codigo);
            foreach (var (v, _) in Vecinos(u, true))
                if (!vis[v]) { vis[v] = true; cola.Enqueue(v); }
        }
        return orden.ToArray();
    }

    public string[] RecorridoDFS(string inicio)
    {
        int s = Idx(inicio);
        if (s < 0) return Array.Empty<string>();
        var vis = new bool[_p.Count];
        var orden = new List<string>();
        void Visitar(int u)
        {
            vis[u] = true;
            orden.Add(_p[u].Codigo);
            foreach (var (v, _) in Vecinos(u, true)) if (!vis[v]) Visitar(v);
        }
        Visitar(s);
        return orden.ToArray();
    }

    // Formato CSV:
    //   PARADA,codigo,nombre,latitud,longitud
    //   RUTA,origen,destino,tiempo,costo,unSentido(0/1),linea
    public string CargarCSV(string path)
    {
        int paradas = 0, rutas = 0, omitidas = 0;
        var inv = CultureInfo.InvariantCulture;
        foreach (var linea in File.ReadAllLines(path))
        {
            if (string.IsNullOrWhiteSpace(linea) || linea.TrimStart().StartsWith("#")) continue;
            var c = linea.Split(',').Select(x => x.Trim()).ToArray();
            try
            {
                if (c[0].Equals("PARADA", StringComparison.OrdinalIgnoreCase))
                {
                    if (AgregarParada(new Parada { Codigo = c[1], Nombre = c[2], Latitud = double.Parse(c[3], inv), Longitud = double.Parse(c[4], inv) })) paradas++; else omitidas++;
                }
                else if (c[0].Equals("RUTA", StringComparison.OrdinalIgnoreCase))
                {
                    var r = new Ruta
                    {
                        Origen = c[1], Destino = c[2], Tiempo = int.Parse(c[3]), Costo = double.Parse(c[4], inv),
                        UnSentido = c[5] == "1", Linea = c.Length > 6 ? int.Parse(c[6]) : 1
                    };
                    if (AgregarRuta(r)) rutas++; else omitidas++;
                }
                else omitidas++;
            }
            catch { omitidas++; }
        }
        return $"CSV cargado: {paradas} paradas, {rutas} rutas, {omitidas} líneas omitidas.";
    }
}
