
using System;

class NodoTrie
{
    // Guarda los posibles caracteres siguientes.
    public NodoTrie[] Hijos;

    // Indica si en este nodo termina una palabra.
    public bool EsFinPalabra;

    // Guarda el código de la parada si termina una palabra.
    public string CodigoParada;

    public NodoTrie()
    {
        Hijos = new NodoTrie[256];
        EsFinPalabra = false;
        CodigoParada = "";
    }
}

public class Trie
{
    private NodoTrie Raiz;

    public Trie()
    {
        Raiz = new NodoTrie();
    }

    // Inserta el nombre de una parada y su código.
    public bool Insertar(string nombreParada, string codigoParada)
    {
        if (string.IsNullOrWhiteSpace(nombreParada) ||
            string.IsNullOrWhiteSpace(codigoParada))
        {
            return false;
        }

        nombreParada = nombreParada.Trim().ToLowerInvariant();

        // Comprueba que todos los caracteres sean compatibles.
        foreach (char caracter in nombreParada)
        {
            if (caracter >= 256)
            {
                return false;
            }
        }

        NodoTrie nodoActual = Raiz;

        foreach (char caracter in nombreParada)
        {
            if (nodoActual.Hijos[caracter] == null)
            {
                nodoActual.Hijos[caracter] = new NodoTrie();
            }

            nodoActual = nodoActual.Hijos[caracter];
        }

        nodoActual.EsFinPalabra = true;
        nodoActual.CodigoParada = codigoParada;

        return true;
    }

    public bool Eliminar(string nombreParada, string codigoParada)
    {
        if (string.IsNullOrWhiteSpace(nombreParada) || string.IsNullOrWhiteSpace(codigoParada))
        {
            return false;
        }

        nombreParada = nombreParada.Trim().ToLowerInvariant();
        foreach (char caracter in nombreParada)
        {
            if (caracter >= 256)
            {
                return false;
            }
        }

        return Eliminar(Raiz, nombreParada, codigoParada, 0);
    }

    private static bool Eliminar(NodoTrie nodo, string nombre, string codigo, int indice)
    {
        if (indice == nombre.Length)
        {
            if (!nodo.EsFinPalabra || !string.Equals(nodo.CodigoParada, codigo, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            nodo.EsFinPalabra = false;
            nodo.CodigoParada = "";
            return true;
        }

        NodoTrie hijo = nodo.Hijos[nombre[indice]];
        return hijo != null && Eliminar(hijo, nombre, codigo, indice + 1);
    }

    // Busca los códigos de las paradas que coinciden con un prefijo.
    public string[] BuscarPrefijo(string prefijo, int maximoResultados = 10)
    {
        if (string.IsNullOrWhiteSpace(prefijo) || maximoResultados <= 0)
        {
            return new string[0];
        }

        prefijo = prefijo.Trim().ToLowerInvariant();

        NodoTrie nodoActual = Raiz;

        // Recorre el Trie hasta encontrar el prefijo.
        foreach (char caracter in prefijo)
        {
            if (caracter >= 256 || nodoActual.Hijos[caracter] == null)
            {
                return new string[0];
            }

            nodoActual = nodoActual.Hijos[caracter];
        }

        string[] codigosEncontrados = new string[maximoResultados];
        int cantidadEncontrada = 0;

        // Recopila los códigos de las palabras que comienzan con el prefijo.
        RecopilarCodigos(nodoActual, codigosEncontrados, ref cantidadEncontrada);

        Array.Resize(ref codigosEncontrados, cantidadEncontrada);

        return codigosEncontrados;
    }

    // Recorre los nodos y recopila los códigos registrados.
    private void RecopilarCodigos(
        NodoTrie nodoActual,
        string[] codigosEncontrados,
        ref int cantidadEncontrada)
    {
        if (cantidadEncontrada >= codigosEncontrados.Length)
        {
            return;
        }

        if (nodoActual.EsFinPalabra)
        {
            codigosEncontrados[cantidadEncontrada] = nodoActual.CodigoParada;
            cantidadEncontrada++;
        }

        for (int indice = 0; indice < nodoActual.Hijos.Length; indice++)
        {
            if (nodoActual.Hijos[indice] != null)
            {
                RecopilarCodigos(
                    nodoActual.Hijos[indice],
                    codigosEncontrados,
                    ref cantidadEncontrada);
            }

            if (cantidadEncontrada >= codigosEncontrados.Length)
            {
                return;
            }
        }
    }
}
