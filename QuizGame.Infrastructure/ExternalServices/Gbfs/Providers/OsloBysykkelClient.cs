namespace QuizGame.Infrastructure.ExternalServices.Gbfs.Providers;

public class OsloBysykkelClient(HttpClient http)
    : GbfsClient(http, "https://gbfs.urbansharing.com/oslobysykkel.no/gbfs.json")
{
    public override string ProviderId => "oslo-bysykkel";
    protected override string City => "Oslo";
}