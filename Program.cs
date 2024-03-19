using Google.Api;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Iam.Credentials.V1;

/* This code is equivalent to the following JavaScript code:

    import {GoogleAuth} from 'google-auth-library';
    const auth = new GoogleAuth();
    const url = 'https://helloworld-xxxxx-uc.a.run.app';
    const client = await auth.getIdTokenClient(url);

* The documentation at cloud.google.com/docs/authentication/get-id-token should bee updated to include C# sample.
*/
if (args.Length != 2)
{
    Console.WriteLine("Usage: dotnet run <cloud-run url> <oidc|sa>");
    return;
}

string url = args[0];
string accessToken = args[1];

if (accessToken == "oidc")
    accessToken = await GetAccessTokenFromOidc(url);
else
    accessToken = await GetIdTokenFromServiceAccount(url);

var client = new HttpClient();
client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

using var response = await client.GetAsync(url);

Console.WriteLine(await response.Content.ReadAsStringAsync());


/// <summary>
/// Get the OIDC access token from the service account via Application Default Credentials
/// </summary>
/// <remarks>Based on: https://cloud.google.com/run/docs/tutorials/secure-services#run_secure_invoker-dotnet</remarks>
async Task<string> GetAccessTokenFromOidc(string url)
{    
    var credential = await GoogleCredential.GetApplicationDefaultAsync()
        .ConfigureAwait(false);
    
    var token = await credential.GetOidcTokenAsync(OidcTokenOptions.FromTargetAudience(url))
        .ConfigureAwait(false);
    
    string accessToken = await token.GetAccessTokenAsync().ConfigureAwait(false);

    return accessToken;
}

/// <summary>
/// Gets an HTTP client with an ID token set.
/// </summary>
async Task<string> GetIdTokenFromServiceAccount(string url) 
{
    // Get default Google credential
    var credential = await GoogleCredential.GetApplicationDefaultAsync()
        .ConfigureAwait(false);

    // Get the underlying ServiceAccountCredential
    var serviceAccount = credential.UnderlyingCredential as ServiceAccountCredential;

    if(serviceAccount == null)
        throw new Exception("Could not get ServiceAccountCredential from ApplicationDefaultCredentials");
 
    var idToken = await GetIdTokenAsync(url, serviceAccount.Id);

    return idToken;
}

/// <summary>
/// Gets the ID Token scoped to a url with a service account ID (usually the email address).
/// </summary>
async Task<string> GetIdTokenAsync(string url, string serviceAccountId)
{
    // Create IAMCredentialsClient
    var client = IAMCredentialsClient.Create();

    // Generate ID token
    var tokenResponse = await client.GenerateIdTokenAsync(serviceAccountId, null, url, true)
            .ConfigureAwait(false);

    return tokenResponse.Token;
}

