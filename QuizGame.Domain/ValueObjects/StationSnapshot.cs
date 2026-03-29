namespace QuizGame.Domain.ValueObjects;

public record StationSnapshot(
    string StationId,
    string Name,
    string ProviderId,
    string City,
    double Latitude,
    double Longitude,
    int BikesAvailable,
    int DocksAvailable,
    int Capacity,
    DateTime FetchedAt
)
{
    public double DistanceTo(StationSnapshot other)
    {
        // Haversine formula
        const double R = 6371;
        var dLat = ToRad(other.Latitude - Latitude);
        var dLon = ToRad(other.Longitude - Longitude);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                + Math.Cos(ToRad(Latitude)) * Math.Cos(ToRad(other.Latitude))
                                            * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }
 
    private static double ToRad(double deg) => deg * Math.PI / 180;
}