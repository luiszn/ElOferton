namespace ElOferton.Constants;

public static class Reloj
{
    private static readonly TimeZoneInfo ZonaPeru = ObtenerZonaPeru();

    public static DateTime Ahora => TimeZoneInfo.ConvertTime(DateTime.UtcNow, ZonaPeru);

    public static DateTime InicioDia => Ahora.Date;

    public static DateTime InicioMes => new(Ahora.Year, Ahora.Month, 1);

    private static TimeZoneInfo ObtenerZonaPeru()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("America/Lima");
        }
        catch (TimeZoneNotFoundException)
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            }
            catch
            {
                return TimeZoneInfo.Utc;
            }
        }
        catch (InvalidTimeZoneException)
        {
            return TimeZoneInfo.Utc;
        }
    }
}
