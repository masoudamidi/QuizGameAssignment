namespace QuizGame.Infrastructure.ExternalServices.Gbfs.Providers;

public class VelibParisClient(HttpClient http)
    : GbfsClient(http, "https://velib-metropole-opendata.smovengo.fr/opendata/Velib_Metropole/gbfs.json")
{
    public override string ProviderId => "velib-paris";
    protected override string City => "Paris";
}