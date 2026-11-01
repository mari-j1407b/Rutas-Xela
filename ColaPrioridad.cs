public class NonoPrioridad
{
    public int IndiceParada;
    public int DistanciaAcumulada;

    public NonoPrioridad(int indiceParada, int distanciaAcumulada)
    {
        IndiceParada = indiceParada;
        DistanciaAcumulada = distanciaAcumulada;
    }
}

public class ColaPrioridadMinima
{
    private NodoPrioridad[] monticulo;
    private int tamanio;

    public ColaPrioridadMinima(int capacidadMaxima)
    {
        monticulo = new NodoPrioridad[capacidadMaxima + 1];
        tamanio = 0;
    }

    public bool EstaVacia()
    {
        return tamanio == 0;
    }

    // Insertar y extraerminimo 

    public void Insertar(int indiceParado, int distancia)
    {
        tamanio++;
        monticulo[tamanio] = new NodoPrioridad(indiceParado, distancia);
        Subir(tamanio);
    }

    public NodoPridad Extraerminimo()
    {
        NodoPrioridad minimo = monticulo[1];
        monticulo[1] = monticulo[tamanio];
        tamanio--;
        Bajar(1);
        return minimo;
    }

    public void Subir(int k)
    {
        while (k > 1 && monticulo[k / 2].DistanciaAcumulada > monticulo[k].DistanciaAcumulada)
        {
            Intercambiar(k, k / 2);
            k = k / 2;
        }
    }

    public void Bajar(int k)
    {
        while (2 * k <= tamanio)
        {
            int j = 2 * k;
            if (j < tamanio && monticulo[j].DistanciaAcumulada > monticulo[j + 1].DistanciaAcumulada)
            {
                j++;
            }

            if (monticulo[k].DistanciaAcumulada <= monticulo[j].DistanciaAcumulada)
            {
                break;
            }

            Intercambiar(k, j);
            k = j;
        }
    }

    public void Intercambiar(int i, int j)
    {
        NodoPrioridad temporal = monticulo[i];
        monticulo[i] = monticulo[j];
        monticulo[j] = temporal;
    }

}