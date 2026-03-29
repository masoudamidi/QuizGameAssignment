namespace QuizGame.Infrastructure.ExternalServices.Gbfs.Providers;

public class VelibParisClient(HttpClient http)
    : GbfsClient(http, "https://data.lime.bike/api/partners/v2/gbfs/paris/gbfs.json")
{
    public override string ProviderId => "paris";
    protected override string City => "Paris";
}