using Google.Apis.Auth.OAuth2;

const string SERVICE_CLIENT = "ServiceClient";

var builder = WebApplication.CreateBuilder(args);

// Get service URL from configuration
string serviceUrl = builder.Configuration["ServiceUrl"] ?? 
    throw new Exception("ServiceUrl not found");

// Configure HTTP client
builder.Services.AddHttpClient(SERVICE_CLIENT, client =>
{
    client.BaseAddress = new Uri(serviceUrl);
});

var app = builder.Build();

// Define the route
app.MapGet("/", async (IHttpClientFactory clientFactory) => 
{
    var client = clientFactory.CreateClient(SERVICE_CLIENT);

    using var response = await client.GetAsync("/ping");
    
    if (!response.IsSuccessStatusCode)
    { 
        return Results.BadRequest($"Error: {response.ReasonPhrase}");
    }

    string content = await response.Content.ReadAsStringAsync();    
    
    return Results.Ok($"The response was: {content}");    
});

app.Run();
