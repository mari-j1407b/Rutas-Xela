
using System;

class UnionFind
{
    // Guarda los códigos de las paradas.
    private string[] CodigosParadas;

    // Guarda el padre de cada elemento.
    private int[] padre;

    // Guarda el tamaño de cada conjunto.
    private int[] tamanio;

    // Indica cuántas paradas están registradas.
    private int cantidad;

    // Constructor.
    public UnionFind()
    {
        CodigosParadas = new string[10];
        padre = new int[10];
        tamanio = new int[10];

        cantidad = 0;
    }

    // Agrega una parada a la estructura.
    public bool AgregarParada(string codigoParada)
    {
        if (string.IsNullOrWhiteSpace(codigoParada))
        {
            return false;
        }

        codigoParada = codigoParada.Trim();

        if (BuscarIndice(codigoParada) != -1)
        {
            return false;
        }

        // Aumenta la capacidad cuando sea necesario.
        if (cantidad == CodigosParadas.Length)
        {
            int nuevaCapacidad = CodigosParadas.Length * 2;

            Array.Resize(ref CodigosParadas, nuevaCapacidad);
            Array.Resize(ref padre, nuevaCapacidad);
            Array.Resize(ref tamanio, nuevaCapacidad);
        }

        CodigosParadas[cantidad] = codigoParada;

        // Cada elemento comienza siendo su propio representante.
        padre[cantidad] = cantidad;
        tamanio[cantidad] = 1;

        cantidad++;

        return true;
    }

    // Busca la posición de una parada mediante su código.
    private int BuscarIndice(string codigoParada)
    {
        if (string.IsNullOrWhiteSpace(codigoParada))
        {
            return -1;
        }

        codigoParada = codigoParada.Trim();

        for (int indice = 0; indice < cantidad; indice++)
        {
            if (CodigosParadas[indice] == codigoParada)
            {
                return indice;
            }
        }

        return -1;
    }

    // Busca el representante del grupo.
    private int BuscarRaiz(int x)
    {
        // Si x es su propio padre, x es la raíz.
        if (padre[x] == x)
        {
            return x;
        }

        // Busca la raíz y aplica compresión de camino.
        padre[x] = BuscarRaiz(padre[x]);

        return padre[x];
    }

    // Busca el representante del grupo mediante su código.
    public string Find(string codigoParada)
    {
        int indice = BuscarIndice(codigoParada);

        if (indice == -1)
        {
            return "";
        }

        int raiz = BuscarRaiz(indice);

        return CodigosParadas[raiz];
    }

    // Une dos grupos.
    public bool Union(string primerCodigo, string segundoCodigo)
    {
        int x = BuscarIndice(primerCodigo);
        int y = BuscarIndice(segundoCodigo);

        if (x == -1 || y == -1)
        {
            return false;
        }

        // Buscamos la raíz de cada elemento.
        int raizX = BuscarRaiz(x);
        int raizY = BuscarRaiz(y);

        // Si las raíces son iguales, ya pertenecen al mismo grupo.
        if (raizX == raizY)
        {
            return false;
        }

        // Unimos el conjunto más pequeño al más grande.
        if (tamanio[raizX] < tamanio[raizY])
        {
            int temporal = raizX;
            raizX = raizY;
            raizY = temporal;
        }

        padre[raizY] = raizX;
        tamanio[raizX] += tamanio[raizY];

        return true;
    }

    // Comprueba si dos elementos pertenecen al mismo conjunto.
    public bool EstanConectados(string primerCodigo, string segundoCodigo)
    {
        int x = BuscarIndice(primerCodigo);
        int y = BuscarIndice(segundoCodigo);

        if (x == -1 || y == -1)
        {
            return false;
        }

        return BuscarRaiz(x) == BuscarRaiz(y);
    }
}
