#
# Create a service account
#
PROJECT_ID=$(gcloud config get project)
SERVICE_ACCOUNT_NAME=cr-id-token
SERVICE_ACCOUNT=$SERVICE_ACCOUNT_NAME@$PROJECT_ID.iam.gserviceaccount.com

gcloud iam service-accounts create $SERVICE_ACCOUNT_NAME --project $PROJECT_ID

gcloud projects add-iam-policy-binding $PROJECT_ID \
  --member="serviceAccount:$SERVICE_ACCOUNT" \
  --role="roles/run.invoker"

#
# Build and deploy with cloud build, substituting the URL for the service
#
gcloud builds submit --config cloudbuild.yaml \
  --substitutions=_SERVICE_URL="$(gcloud run services describe helloworld --format='value(status.url)')",_SERVICE_ACCOUNT=$SERVICE_ACCOUNT
