using System;
public class NodoPrioridad
{
    public int IndiceParada;
    public int DistanciaAcumulada;

    public NodoPrioridad(int indiceParada, int distanciaAcumulada)
    {
        IndiceParada = indiceParada;
        DistanciaAcumulada = distanciaAcumulada;
    }
}

public class ColaPrioridadMinima
{
    private NodoPrioridad?[] monticulo;
    private int tamanio;

    public ColaPrioridadMinima(int capacidadMaxima)
    {
        if (capacidadMaxima < 1)
        {
            capacidadMaxima = 1;
        }

        monticulo = new NodoPrioridad?[capacidadMaxima + 1];
        tamanio = 0;
    }

    public bool EstaVacia()
    {
        return tamanio == 0;
    }

    // Inserta una nueva distancia en el montículo
    public void Insertar(int indiceParada, int distancia)
    {
        // Aumenta la capacidad si el arreglo está lleno.
        if (tamanio == monticulo.Length - 1)
        {
            Array.Resize(ref monticulo, monticulo.Length * 2);
        }

        tamanio++;

        monticulo[tamanio] = new NodoPrioridad(indiceParada, distancia);

        Subir(tamanio);
    }

    // Extremos el elemento con menor distancia.
    public NodoPrioridad ExtraerMinimo()
    {
        if (EstaVacia())
        {
            throw new InvalidOperationException("La cola de prioridad está vacía.");
        }

        NodoPrioridad minimo = monticulo[1]!;

        monticulo[1] = monticulo[tamanio];
        monticulo[tamanio] = null;

        tamanio--;

        if (tamanio > 0)
        {
            Bajar(1);
        }

        return minimo;
    }

    // Subim un elemento hasta recuperar la propiedad del montículo.
    public void Subir(int k)
    {
        while (k > 1 &&
               monticulo[k / 2]!.DistanciaAcumulada >
               monticulo[k]!.DistanciaAcumulada)
        {
            Intercambiar(k, k / 2);
            k = k / 2;
        }
    }

    // Baja un elemento hasta recuperar la propiedad del montículo.
    public void Bajar(int k)
    {
        while (2 * k <= tamanio)
        {
            int j = 2 * k;

            if (j < tamanio &&
                monticulo[j]!.DistanciaAcumulada >
                monticulo[j + 1]!.DistanciaAcumulada)
            {
                j++;
            }

            if (monticulo[k]!.DistanciaAcumulada <=
                monticulo[j]!.DistanciaAcumulada)
            {
                break;
            }

            Intercambiar(k, j);
            k = j;
        }
    }

    // Intercambiando dos posiciones en el monticulo
    public void Intercambiar(int i, int j)
    {
        NodoPrioridad? temporal = monticulo[i];

        monticulo[i] = monticulo[j];
        monticulo[j] = temporal;
    }
}
