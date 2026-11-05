using System.Globalization;
using System.IO;

public sealed class CargadorDatos
{
    public TablaHash TablaHash { get; }
    public GrafoTransporte Grafo { get; }
    public Trie Trie { get; }
    public UnionFind UnionFind { get; private set; }
    public IReadOnlyList<Parada> Paradas { get; }
    public string RutaArchivo { get; }

    public CargadorDatos()
        : this(Path.Combine(AppContext.BaseDirectory, "data", "paradas.csv"))
    {
    }

    public CargadorDatos(string rutaArchivo)
    {
        if (string.IsNullOrWhiteSpace(rutaArchivo))
        {
            throw new ArgumentException("La ruta del CSV es obligatoria.", nameof(rutaArchivo));
        }

        RutaArchivo = Path.GetFullPath(rutaArchivo);
        string[] lineas = File.ReadAllLines(RutaArchivo);
        Parada[] paradasLeidas = new Parada[lineas.Length];
        int[] lineasParadas = new int[lineas.Length];
        RutaCsv[] rutasLeidas = new RutaCsv[lineas.Length];
        int cantidadParadas = 0;
        int cantidadRutas = 0;

        for (int indice = 0; indice < lineas.Length; indice++)
        {
            string linea = lineas[indice].Trim();
            int numeroLinea = indice + 1;

            if (linea.Length == 0 || linea.StartsWith('#'))
            {
                continue;
            }

            string[] campos = linea.Split(',');
            string tipo = campos[0].Trim();

            if (tipo.Equals("PARADA", StringComparison.OrdinalIgnoreCase))
            {
                if (campos.Length > 1 && campos[1].Trim().Equals("codigo", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                paradasLeidas[cantidadParadas] = LeerParada(campos, numeroLinea);
                lineasParadas[cantidadParadas] = numeroLinea;
                cantidadParadas++;
            }
            else if (tipo.Equals("RUTA", StringComparison.OrdinalIgnoreCase))
            {
                if (campos.Length > 1 && campos[1].Trim().Equals("origen", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                rutasLeidas[cantidadRutas++] = LeerRuta(campos, numeroLinea);
            }
            else
            {
                throw Error(numeroLinea, $"tipo de registro desconocido: '{tipo}'.");
            }
        }

        if (cantidadParadas == 0)
        {
            throw new InvalidDataException("El CSV no contiene registros PARADA.");
        }

        Array.Resize(ref paradasLeidas, cantidadParadas);
        Array.Resize(ref lineasParadas, cantidadParadas);
        Array.Resize(ref rutasLeidas, cantidadRutas);

        var tablaHash = new TablaHash(Math.Max(11, cantidadParadas * 2 + 1));
        var grafo = new GrafoTransporte(Math.Max(10, cantidadParadas));
        var trie = new Trie();
        var unionFind = new UnionFind();

        for (int indice = 0; indice < paradasLeidas.Length; indice++)
        {
            Parada parada = paradasLeidas[indice];
            int numeroLinea = lineasParadas[indice];

            if (!tablaHash.Insertar(parada))
            {
                throw Error(numeroLinea, $"no se pudo registrar la parada '{parada.Codigo}' (código duplicado o inválido).");
            }

            grafo.AgregarParada(parada.Codigo);
            if (!grafo.ExisteParada(parada.Codigo))
            {
                throw Error(numeroLinea, $"no se pudo agregar la parada '{parada.Codigo}' al grafo.");
            }

            if (!trie.Insertar(parada.Nombre, parada.Codigo))
            {
                throw Error(numeroLinea, $"el nombre de la parada '{parada.Nombre}' no puede insertarse en el Trie.");
            }

            if (!unionFind.AgregarParada(parada.Codigo))
            {
                throw Error(numeroLinea, $"no se pudo registrar la parada '{parada.Codigo}' en Union-Find.");
            }
        }

        foreach (RutaCsv ruta in rutasLeidas)
        {
            Parada? origen = tablaHash.Buscar(ruta.Origen);
            Parada? destino = tablaHash.Buscar(ruta.Destino);

            if (origen == null || destino == null)
            {
                string codigoFaltante = origen == null ? ruta.Origen : ruta.Destino;
                throw Error(ruta.NumeroLinea, $"la ruta referencia la parada inexistente '{codigoFaltante}'.");
            }

            grafo.AgregarRuta(
                origen.Codigo,
                destino.Codigo,
                ruta.Tiempo,
                ruta.Costo,
                ruta.NumeroLineaServicio,
                ruta.UnSoloSentido);

            unionFind.Union(origen.Codigo, destino.Codigo);
        }

        TablaHash = tablaHash;
        Grafo = grafo;
        Trie = trie;
        UnionFind = unionFind;
        Paradas = Array.AsReadOnly(paradasLeidas);
    }

    public void ReconstruirUnionFind()
    {
        UnionFind = Grafo.CrearUnionFind();
    }

    private static Parada LeerParada(string[] campos, int numeroLinea)
    {
        if (campos.Length != 5)
        {
            throw Error(numeroLinea, "un registro PARADA debe tener exactamente 5 campos.");
        }

        string codigo = campos[1].Trim();
        string nombre = campos[2].Trim();

        if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(nombre))
        {
            throw Error(numeroLinea, "el código y el nombre de la parada son obligatorios.");
        }

        if (!double.TryParse(campos[3].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double latitud) ||
            !double.TryParse(campos[4].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double longitud) ||
            !double.IsFinite(latitud) || !double.IsFinite(longitud) ||
            latitud < -90 || latitud > 90 || longitud < -180 || longitud > 180)
        {
            throw Error(numeroLinea, $"las coordenadas de la parada '{codigo}' son inválidas.");
        }

        return new Parada(codigo, nombre, latitud, longitud);
    }

    private static RutaCsv LeerRuta(string[] campos, int numeroLinea)
    {
        if (campos.Length != 7)
        {
            throw Error(numeroLinea, "un registro RUTA debe tener exactamente 7 campos.");
        }

        string origen = campos[1].Trim();
        string destino = campos[2].Trim();

        if (string.IsNullOrWhiteSpace(origen) || string.IsNullOrWhiteSpace(destino))
        {
            throw Error(numeroLinea, "el origen y el destino de la ruta son obligatorios.");
        }

        if (!int.TryParse(campos[3].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int tiempo) || tiempo < 0)
        {
            throw Error(numeroLinea, "el tiempo de la ruta debe ser un entero no negativo.");
        }

        if (!double.TryParse(campos[4].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double costo) ||
            !double.IsFinite(costo) || costo < 0)
        {
            throw Error(numeroLinea, "el costo de la ruta debe ser un número no negativo.");
        }

        string sentidoTexto = campos[5].Trim();
        if (sentidoTexto != "0" && sentidoTexto != "1")
        {
            throw Error(numeroLinea, "el sentido de la ruta debe ser 0 o 1.");
        }

        if (!int.TryParse(campos[6].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int numeroLineaServicio) ||
            numeroLineaServicio < 1)
        {
            throw Error(numeroLinea, "el número de línea debe ser un entero positivo.");
        }

        return new RutaCsv(
            origen,
            destino,
            tiempo,
            costo,
            sentidoTexto == "1",
            numeroLineaServicio,
            numeroLinea);
    }

    private static InvalidDataException Error(int numeroLinea, string mensaje)
    {
        return new InvalidDataException($"Error en la línea {numeroLinea} del CSV: {mensaje}");
    }

    private readonly record struct RutaCsv(
        string Origen,
        string Destino,
        int Tiempo,
        double Costo,
        bool UnSoloSentido,
        int NumeroLineaServicio,
        int NumeroLinea);
}
