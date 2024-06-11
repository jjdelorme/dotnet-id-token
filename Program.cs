using System.Net.Http.Headers;
using Google.Apis.Auth.OAuth2;

const string SERVICE_CLIENT = "ServiceClient";

var builder = WebApplication.CreateBuilder(args);

// Get service URL from configuration
string serviceUrl = builder.Configuration["ServiceUrl"] ?? 
    throw new Exception("ServiceUrl not found");

// Get ID token that will automatically refresh
OidcToken token = await GetIdTokenAsync(serviceUrl);

// Configure HTTP client
builder.Services.AddHttpClient(SERVICE_CLIENT, client =>
{
    client.BaseAddress = new Uri(serviceUrl);
});

var app = builder.Build();

// Define the route
app.MapGet("/", async (IHttpClientFactory clientFactory) => 
{
    // Calling GetAccessTokenAsync handles token refresh automatically when you call
    // Despite the method being called AccessToken this is an IdToken
    var idToken = await token.GetAccessTokenAsync().ConfigureAwait(false);

    var client = clientFactory.CreateClient(SERVICE_CLIENT);
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

    using var response = await client.GetAsync("/ping");
    
    if (!response.IsSuccessStatusCode)
    { 
        return Results.BadRequest($"Error: {response.ReasonPhrase}");
    }

    string content = await response.Content.ReadAsStringAsync();    
    
    return Results.Ok($"The response was: {content} at {DateTime.Now.ToString("s")}");
});

app.Run();

/// <summary>
/// Gets a token from Application Default Credentials.
/// </summary>
static async Task<OidcToken> GetIdTokenAsync(string url)
{
    // Get default Google credential
    var credential = await GoogleCredential.GetApplicationDefaultAsync()
        .ConfigureAwait(false);

    var token = await credential.GetOidcTokenAsync(OidcTokenOptions.FromTargetAudience(url))
        .ConfigureAwait(false);

    return token;
}
