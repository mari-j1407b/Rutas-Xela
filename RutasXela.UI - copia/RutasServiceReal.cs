using System.Globalization;
using System.Text;
using OriginalParada = global::Parada;

namespace RutasXela.UI;

public sealed class RutasServiceReal : IRutasService
{
    private CargadorDatos _datos;
    private readonly List<Parada> _paradasPresentacion = new();

    public RutasServiceReal()
        : this(Path.Combine(AppContext.BaseDirectory, "data", "paradas.csv"))
    {
    }

    public RutasServiceReal(string rutaCsv)
    {
        _datos = new CargadorDatos(rutaCsv);
        ActualizarParadasPresentacion();
    }

    public Parada[] ObtenerParadas() => _paradasPresentacion.ToArray();

    public Ruta[] ObtenerRutas()
    {
        return _datos.Grafo.ObtenerConexiones()
            .Select(conexion => new Ruta
            {
                Origen = conexion.CodigoOrigen,
                Destino = conexion.CodigoDestino,
                Tiempo = conexion.TiempoMinutos,
                Costo = conexion.Costo,
                UnSentido = conexion.UnSoloSentido,
                Linea = conexion.NumeroLinea
            })
            .ToArray();
    }

    public bool AgregarParada(Parada p)
    {
        if (p == null || string.IsNullOrWhiteSpace(p.Codigo) || string.IsNullOrWhiteSpace(p.Nombre) ||
            !AreaGeograficaXela.Contiene(p.Latitud, p.Longitud) ||
            p.Nombre.Any(caracter => caracter >= 256) ||
            _datos.TablaHash.Buscar(p.Codigo) != null ||
            _paradasPresentacion.Any(parada => string.Equals(parada.Nombre, p.Nombre.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        var paradaOriginal = new OriginalParada(p.Codigo, p.Nombre, p.Latitud, p.Longitud);
        if (!_datos.TablaHash.Insertar(paradaOriginal))
        {
            return false;
        }

        _datos.Grafo.AgregarParada(paradaOriginal.Codigo);
        if (!_datos.Grafo.ExisteParada(paradaOriginal.Codigo) ||
            !_datos.Trie.Insertar(paradaOriginal.Nombre, paradaOriginal.Codigo) ||
            !_datos.UnionFind.AgregarParada(paradaOriginal.Codigo))
        {
            throw new InvalidOperationException($"No se pudieron inicializar todas las estructuras para la parada {paradaOriginal.Codigo}.");
        }

        _paradasPresentacion.Add(ConvertirParada(paradaOriginal));
        return true;
    }

    public bool AgregarRuta(Ruta r)
    {
        if (r == null || string.IsNullOrWhiteSpace(r.Origen) || string.IsNullOrWhiteSpace(r.Destino) ||
            r.Tiempo < 0 || !double.IsFinite(r.Costo) || r.Costo < 0 || r.Linea < 1)
        {
            return false;
        }

        OriginalParada? origen = _datos.TablaHash.Buscar(r.Origen);
        OriginalParada? destino = _datos.TablaHash.Buscar(r.Destino);
        if (origen == null || destino == null ||
            string.Equals(origen.Codigo, destino.Codigo, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (_datos.Grafo.ObtenerConexiones().Any(conexion =>
            (string.Equals(conexion.CodigoOrigen, origen.Codigo, StringComparison.OrdinalIgnoreCase) &&
             string.Equals(conexion.CodigoDestino, destino.Codigo, StringComparison.OrdinalIgnoreCase)) ||
            (!conexion.UnSoloSentido &&
             string.Equals(conexion.CodigoOrigen, destino.Codigo, StringComparison.OrdinalIgnoreCase) &&
             string.Equals(conexion.CodigoDestino, origen.Codigo, StringComparison.OrdinalIgnoreCase))))
        {
            return false;
        }

        _datos.Grafo.AgregarRuta(
            origen.Codigo,
            destino.Codigo,
            r.Tiempo,
            r.Costo,
            r.Linea,
            r.UnSentido);
        _datos.UnionFind.Union(origen.Codigo, destino.Codigo);
        return true;
    }

    public bool ModificarParada(Parada parada)
    {
        if (parada == null || string.IsNullOrWhiteSpace(parada.Codigo) || string.IsNullOrWhiteSpace(parada.Nombre) ||
            !AreaGeograficaXela.Contiene(parada.Latitud, parada.Longitud) ||
            parada.Nombre.Any(caracter => caracter >= 256 || caracter == ',') ||
            parada.Codigo.Contains(','))
        {
            return false;
        }

        OriginalParada? original = _datos.TablaHash.Buscar(parada.Codigo);
        if (original == null || _paradasPresentacion.Any(actual =>
            !string.Equals(actual.Codigo, original.Codigo, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(actual.Nombre, parada.Nombre.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        string nombreNuevo = parada.Nombre.Trim();
        bool cambioNombre = !string.Equals(original.Nombre, nombreNuevo, StringComparison.Ordinal);
        Parada[] paradasCandidatas = _paradasPresentacion.Select(actual =>
            string.Equals(actual.Codigo, original.Codigo, StringComparison.OrdinalIgnoreCase)
                ? new Parada { Codigo = original.Codigo, Nombre = nombreNuevo, Latitud = parada.Latitud, Longitud = parada.Longitud }
                : actual).ToArray();
        PersistirCambios(paradasCandidatas, ObtenerRutas());

        if (cambioNombre && !_datos.Trie.Eliminar(original.Nombre, original.Codigo))
        {
            throw new InvalidOperationException($"No se encontró el nombre actual de la parada {original.Codigo} en el Trie.");
        }

        original.Nombre = nombreNuevo;
        original.Latitud = parada.Latitud;
        original.Longitud = parada.Longitud;
        if (cambioNombre && !_datos.Trie.Insertar(original.Nombre, original.Codigo))
        {
            throw new InvalidOperationException($"No se pudo actualizar el nombre de la parada {original.Codigo} en el Trie.");
        }

        Parada presentacion = _paradasPresentacion.First(actual =>
            string.Equals(actual.Codigo, original.Codigo, StringComparison.OrdinalIgnoreCase));
        presentacion.Nombre = original.Nombre;
        presentacion.Latitud = original.Latitud;
        presentacion.Longitud = original.Longitud;
        return true;
    }

    public int ContarRutasConectadas(string codigo)
    {
        if (_datos.TablaHash.Buscar(codigo) == null)
        {
            return 0;
        }

        return _datos.Grafo.ObtenerConexiones().Count(conexion =>
            string.Equals(conexion.CodigoOrigen, codigo, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(conexion.CodigoDestino, codigo, StringComparison.OrdinalIgnoreCase));
    }

    public bool EliminarParada(string codigo)
    {
        OriginalParada? parada = _datos.TablaHash.Buscar(codigo);
        if (parada == null)
        {
            return false;
        }

        Ruta[] rutasRestantes = ObtenerRutas().Where(ruta =>
            !string.Equals(ruta.Origen, parada.Codigo, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(ruta.Destino, parada.Codigo, StringComparison.OrdinalIgnoreCase)).ToArray();
        Parada[] paradasRestantes = _paradasPresentacion.Where(actual =>
            !string.Equals(actual.Codigo, parada.Codigo, StringComparison.OrdinalIgnoreCase)).ToArray();
        PersistirCambios(paradasRestantes, rutasRestantes);

        if (!_datos.Trie.Eliminar(parada.Nombre, parada.Codigo) ||
            !_datos.Grafo.EliminarParada(parada.Codigo) ||
            !_datos.TablaHash.Eliminar(parada.Codigo))
        {
            throw new InvalidOperationException($"No se pudieron eliminar todas las referencias de la parada {parada.Codigo}.");
        }

        _paradasPresentacion.RemoveAll(actual =>
            string.Equals(actual.Codigo, parada.Codigo, StringComparison.OrdinalIgnoreCase));
        _datos.ReconstruirUnionFind();
        return true;
    }

    private void PersistirCambios(IEnumerable<Parada> paradas, IEnumerable<Ruta> rutas)
    {
        var contenido = new StringBuilder();
        contenido.AppendLine("# PARADA,codigo,nombre,coordenadaY,coordenadaX (cuadricula ficticia; no son ubicaciones reales)");
        foreach (Parada parada in paradas)
        {
            if (parada.Codigo.Contains(',') || parada.Nombre.Contains(','))
            {
                throw new InvalidOperationException("El formato actual del CSV no admite comas en códigos o nombres.");
            }

            contenido.Append("PARADA,").Append(parada.Codigo).Append(',').Append(parada.Nombre).Append(',')
                .Append(parada.Latitud.ToString("R", CultureInfo.InvariantCulture)).Append(',')
                .Append(parada.Longitud.ToString("R", CultureInfo.InvariantCulture)).AppendLine();
        }

        contenido.AppendLine("# RUTA,origen,destino,tiempo,costo,unSentido(0/1),linea");
        foreach (Ruta ruta in rutas)
        {
            contenido.Append("RUTA,").Append(ruta.Origen).Append(',').Append(ruta.Destino).Append(',')
                .Append(ruta.Tiempo.ToString(CultureInfo.InvariantCulture)).Append(',')
                .Append(ruta.Costo.ToString("R", CultureInfo.InvariantCulture)).Append(',')
                .Append(ruta.UnSentido ? '1' : '0').Append(',')
                .Append(ruta.Linea.ToString(CultureInfo.InvariantCulture)).AppendLine();
        }

        string rutaCsv = _datos.RutaArchivo;
        string directorio = Path.GetDirectoryName(rutaCsv)!;
        string temporal = Path.Combine(directorio, $"paradas.{Guid.NewGuid():N}.tmp");
        try
        {
            File.WriteAllText(temporal, contenido.ToString(), new UTF8Encoding(false));
            _ = new CargadorDatos(temporal);
            File.Move(temporal, rutaCsv, true);
        }
        finally
        {
            if (File.Exists(temporal))
            {
                File.Delete(temporal);
            }
        }
    }

    public Parada? BuscarParada(string codigo)
    {
        OriginalParada? parada = _datos.TablaHash.Buscar(codigo);
        return parada == null ? null : ConvertirParada(parada);
    }

    public Parada[] Autocompletar(string prefijo)
    {
        if (string.IsNullOrWhiteSpace(prefijo))
        {
            return Array.Empty<Parada>();
        }

        string texto = prefijo.Trim();
        var resultados = new List<Parada>(8);

        foreach (Parada parada in _paradasPresentacion)
        {
            if (parada.Codigo.StartsWith(texto, StringComparison.OrdinalIgnoreCase))
            {
                resultados.Add(parada);
                if (resultados.Count == 8)
                {
                    return resultados.ToArray();
                }
            }
        }

        foreach (string codigo in _datos.Trie.BuscarPrefijo(texto, 8))
        {
            OriginalParada? parada = _datos.TablaHash.Buscar(codigo);
            if (parada != null && !resultados.Any(resultado =>
                string.Equals(resultado.Codigo, parada.Codigo, StringComparison.OrdinalIgnoreCase)))
            {
                resultados.Add(ConvertirParada(parada));
                if (resultados.Count == 8)
                {
                    break;
                }
            }
        }

        return resultados.ToArray();
    }

    public bool MismaZona(string codigoA, string codigoB)
    {
        OriginalParada? paradaA = _datos.TablaHash.Buscar(codigoA);
        OriginalParada? paradaB = _datos.TablaHash.Buscar(codigoB);
        return paradaA != null && paradaB != null &&
            _datos.UnionFind.EstanConectados(paradaA.Codigo, paradaB.Codigo);
    }

    public ResultadoRuta CalcularRuta(string origen, string destino, Criterio c)
    {
        OriginalParada? paradaOrigen = _datos.TablaHash.Buscar(origen);
        OriginalParada? paradaDestino = _datos.TablaHash.Buscar(destino);
        if (paradaOrigen == null || paradaDestino == null)
        {
            return new ResultadoRuta { Mensaje = "Parada no encontrada." };
        }

        ResultadoRutaGrafo resultado = c == Criterio.Economica
            ? _datos.Grafo.CalcularRutaMasEconomica(paradaOrigen.Codigo, paradaDestino.Codigo)
            : _datos.Grafo.CalcularRutaMasRapida(paradaOrigen.Codigo, paradaDestino.Codigo);
        if (!resultado.Exito)
        {
            return new ResultadoRuta { Mensaje = resultado.Mensaje };
        }

        var detalles = new List<DetalleConexionRuta>();
        for (int indice = 0; indice < resultado.Camino.Length - 1; indice++)
        {
            string desde = resultado.Camino[indice];
            string hacia = resultado.Camino[indice + 1];
            ConexionRuta? conexion = _datos.Grafo.ObtenerConexiones().FirstOrDefault(ruta =>
                (string.Equals(ruta.CodigoOrigen, desde, StringComparison.OrdinalIgnoreCase) &&
                 string.Equals(ruta.CodigoDestino, hacia, StringComparison.OrdinalIgnoreCase)) ||
                (!ruta.UnSoloSentido &&
                 string.Equals(ruta.CodigoOrigen, hacia, StringComparison.OrdinalIgnoreCase) &&
                 string.Equals(ruta.CodigoDestino, desde, StringComparison.OrdinalIgnoreCase)));

            if (conexion == null)
            {
                return new ResultadoRuta { Mensaje = $"No se pudieron recuperar los datos de la conexión {desde} - {hacia}." };
            }

            detalles.Add(new DetalleConexionRuta
            {
                Origen = desde,
                Destino = hacia,
                Linea = conexion.NumeroLinea,
                Tiempo = conexion.TiempoMinutos,
                Costo = conexion.Costo
            });
        }

        return new ResultadoRuta
        {
            Exito = true,
            Mensaje = "OK",
            Camino = resultado.Camino,
            TiempoTotal = resultado.TiempoTotal,
            CostoTotal = resultado.CostoTotal,
            DetalleConexiones = detalles.ToArray()
        };
    }

    public string[] RecorridoBFS(string inicio)
    {
        OriginalParada? parada = _datos.TablaHash.Buscar(inicio);
        return parada == null ? Array.Empty<string>() : _datos.Grafo.RecorridoBFS(parada.Codigo);
    }

    public string[] RecorridoDFS(string inicio)
    {
        OriginalParada? parada = _datos.TablaHash.Buscar(inicio);
        return parada == null ? Array.Empty<string>() : _datos.Grafo.RecorridoDFS(parada.Codigo);
    }

    public string CargarCSV(string rutaArchivo)
    {
        CargadorDatos nuevosDatos = new(rutaArchivo);
        _datos = nuevosDatos;
        ActualizarParadasPresentacion();
        return $"CSV cargado: {_datos.Paradas.Count} paradas, {_datos.Grafo.ObtenerConexiones().Count()} rutas.";
    }

    private void ActualizarParadasPresentacion()
    {
        _paradasPresentacion.Clear();
        foreach (OriginalParada parada in _datos.Paradas)
        {
            _paradasPresentacion.Add(ConvertirParada(parada));
        }
    }

    private static Parada ConvertirParada(OriginalParada parada)
    {
        return new Parada
        {
            Codigo = parada.Codigo,
            Nombre = parada.Nombre,
            Latitud = parada.Latitud,
            Longitud = parada.Longitud
        };
    }
}
