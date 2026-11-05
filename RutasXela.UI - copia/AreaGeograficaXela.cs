namespace RutasXela.UI;

internal static class AreaGeograficaXela
{
    public const double CoordenadaYMinima = 0;
    public const double CoordenadaYMaxima = 30;
    public const double CoordenadaXMinima = 0;
    public const double CoordenadaXMaxima = 30;

    public static bool Contiene(double coordenadaY, double coordenadaX)
    {
        return double.IsFinite(coordenadaY) && double.IsFinite(coordenadaX) &&
            coordenadaY >= CoordenadaYMinima && coordenadaY <= CoordenadaYMaxima &&
            coordenadaX >= CoordenadaXMinima && coordenadaX <= CoordenadaXMaxima;
    }
}
