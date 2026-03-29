namespace QuizGame.Infrastructure.ExternalServices.Gbfs.Providers;

public class CitiBikeNycClient(HttpClient http)
    : GbfsClient(http, "https://gbfs.lyft.com/gbfs/1.1/babs/gbfs.json")
{
    public override string ProviderId => "citi-bike-nyc";
    protected override string City => "New York";
}