
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
        Hijos = new NodoTrie[char.MaxValue + 1];
        EsFinPalabra = false;
        CodigoParada = "";
    }
}
