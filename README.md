# .NET Get ID Token Sample

This Google C# sample demonstrates authenticated service to service calls using an ID token with Cloud Run.  Service A (this service) -> Service B (Authenticated).

## Git Tags

This repo is setup with 2 tags to follow along:

* `start` is without authentication
* `end` is with authentication

## App Settings

Add the following entry to your `appsettings.json` file for the location of Service B.

```json
  {
    ...,
    "ServiceUrl": "https://helloworld-XXXXXX-uc.a.run.app"
  }
```

## User & Service Accounts

Google's [Best practices for using service accounts](https://cloud.google.com/iam/docs/best-practices-service-accounts#using_service_accounts) suggests using [service account impersonation](https://cloud.google.com/docs/authentication/use-service-account-impersonation) when developing locally.

1. Set your service account email, for example: 
    ```bash
    export SERVICE_ACCT_EMAIL=serviceaccount@your-project.iam.gserviceaccount.com
    ```

1. Ensure that your **service account** has the `roles/run.invoker` role in order to be able to invoke another Cloud Run service.

1. Ensure that your **user account** that you login with has the `Service Account OpenID Connect Identity Token Creator` role.  **NOTE** Even if you are an administrator in your GCP Project, you will need this role or similar role which has the `iam.serviceAccounts.getOpenIdToken` permission.

1. Login using Service Account Impersonation with [Application Default Credentials](https://cloud.google.com/docs/authentication/provide-credentials-adc):

    ```bash
    gcloud auth application-default login --impersonate-service-account $SERVICE_ACCT_EMAIL

    export GOOGLE_APPLICATION_CREDENTIALS=$HOME/.config/gcloud/application_default_credentials.json
    ```
