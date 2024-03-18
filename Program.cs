using Google.Apis.Auth.OAuth2;
using Google.Cloud.Iam.Credentials.V1;

/* This code is equivalent to the following JavaScript code:

    import {GoogleAuth} from 'google-auth-library';
    const auth = new GoogleAuth();
    const url = 'https://helloworld-xxxxx-uc.a.run.app';
    const client = await auth.getIdTokenClient(url);

* The documentation at cloud.google.com/docs/authentication/get-id-token should bee updated to include C# sample.
*/

const string url = "https://helloworld-xxxxx-uc.a.run.app";

var client = await GetIdTokenClient(url);

using var response = await client.GetAsync(url);
Console.WriteLine(await response.Content.ReadAsStringAsync());


/// <summary>
/// Gets an HTTP client with an ID token set.
/// </summary>
/// <param name="url">The URL to use when generating the ID token.</param>
/// <returns>An HTTP client with an ID token set.</returns>
async Task<HttpClient> GetIdTokenClient(string url) 
{
    // Get default Google credential
    var credential = await GoogleCredential.GetApplicationDefaultAsync()
        .ConfigureAwait(false);

    // Get the underlying ServiceAccountCredential
    var serviceAccount = credential.UnderlyingCredential as ServiceAccountCredential;

    if(serviceAccount == null)
        throw new Exception("Could not get ServiceAccountCredential from ApplicationDefaultCredentials");
 
    var idToken = await GetIdTokenAsync(url, serviceAccount.Id);

    // Set ID token on the client
    serviceAccount.HttpClient.DefaultRequestHeaders.Add(
        "Authorization", $"Bearer {idToken}");

    return serviceAccount.HttpClient;
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
