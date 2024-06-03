# .NET Get ID Token Sample

This Google C# sample demonstrates authenticated service to service calls using an ID token with Cloud Run.  Service A (this service) -> Service B (Authenticated).  Add the following entry to your `appsettings.json` file for the location of Service B.

```json
  "ServiceUrl": "https://helloworld-XXXXXX-uc.a.run.app"
```
## Git Tags

`start` is without authentication
`end` is with authentication

