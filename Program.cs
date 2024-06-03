using Google.Apis.Auth.OAuth2;

const string SERVICE_CLIENT = "ServiceClient";

var builder = WebApplication.CreateBuilder(args);

string serviceUrl = builder.Configuration["ServiceUrl"] ?? 
    throw new Exception("ServiceUrl not found");

string idToken = await GetIdTokenFromApplicationDefaultAsync(serviceUrl);

builder.Services.AddHttpClient(SERVICE_CLIENT, client =>
{
    client.BaseAddress = new Uri(serviceUrl);
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {idToken}");
});

var app = builder.Build();

app.MapGet("/", async (IHttpClientFactory clientFactory) => 
{
    var client = clientFactory.CreateClient(SERVICE_CLIENT);
    using var response = await client.GetAsync("/ping");
    
    if (!response.IsSuccessStatusCode)
    { 
        return Results.BadRequest($"The response was: {response.ReasonPhrase}");
    }

    string content = await response.Content.ReadAsStringAsync();    
    return Results.Ok($"The response was: {content}");    
});

app.Run();

/// <summary>
/// Gets an ID token from Application Default Credentials.
/// </summary>
static async Task<string> GetIdTokenFromApplicationDefaultAsync(string url, CancellationToken cancellationToken = default)
{
    // Get default Google credential
    var credential = await GoogleCredential.GetApplicationDefaultAsync(cancellationToken)
        .ConfigureAwait(false);

    var token = await credential.GetOidcTokenAsync(OidcTokenOptions.FromTargetAudience(url), cancellationToken)
        .ConfigureAwait(false);

    // Despite the method being called AccessToken this is an IdToken
    var idToken = await token.GetAccessTokenAsync(cancellationToken)
        .ConfigureAwait(false);

    return idToken;
}
