
const string SERVICE_CLIENT = "ServiceClient";

var builder = WebApplication.CreateBuilder(args);

string serviceUrl = builder.Configuration["ServiceUrl"] ?? 
    throw new Exception("ServiceUrl not found");

builder.Services.AddHttpClient(SERVICE_CLIENT, client =>
{
    client.BaseAddress = new Uri(serviceUrl);
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
