/* This code is equivalent to the following JavaScript code:

    import {GoogleAuth} from 'google-auth-library';
    const auth = new GoogleAuth();
    const url = 'https://helloworld-xxxxx-uc.a.run.app';
    const client = await auth.getIdTokenClient(url);
*/

// The documentation at cloud.google.com/docs/authentication/get-id-token should bee updated to include C# sample.

using Google.Apis.Auth.OAuth2;
using Google.Cloud.Iam.Credentials.V1;

string url = "https://helloworld-xxxxx-uc.a.run.app";

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
    var credential = await GoogleCredential.GetApplicationDefaultAsync();

    // Get the underlying ServiceAccountCredential
    var serviceAccount = credential.UnderlyingCredential as ServiceAccountCredential;

    if(serviceAccount == null)
        throw new Exception("Could not get ServiceAccountCredential from ApplicationDefaultCredentials");
 
    // Create IAMCredentialsClient
    var client = IAMCredentialsClient.Create();

    // Generate ID token
    var tokenResponse = await client.GenerateIdTokenAsync(serviceAccount.Id, null, url, true);

    // Set ID token on the client
    serviceAccount.HttpClient.DefaultRequestHeaders.Add(
        "Authorization", $"Bearer {tokenResponse.Token}");

    return serviceAccount.HttpClient;
}
