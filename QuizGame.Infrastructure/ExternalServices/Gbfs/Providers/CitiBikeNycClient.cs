namespace QuizGame.Infrastructure.ExternalServices.Gbfs.Providers;

public class CitiBikeNycClient(HttpClient http)
    : GbfsClient(http, "https://gbfs.citibikenyc.com/gbfs/2.3/gbfs.json")
{
    public override string ProviderId => "citi-bike-nyc";
    protected override string City => "New York";
}