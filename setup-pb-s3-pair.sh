#!/bin/bash
set -e

# === CONFIGURATION ===
PROJECT_NAME="${1:-pb-s3-test}"           # Pass project name as first arg, or default
PB_SERVICE_NAME="pocketbase"
BUCKET_NAME="pb-bucket-$(date +%s)"

echo "🚀 Creating Railway project: $PROJECT_NAME"
railway init --name "$PROJECT_NAME" --yes || true
railway link --project "$PROJECT_NAME"

echo "📦 Creating PocketBase service..."
railway add --template pocketbase --name "$PB_SERVICE_NAME"

echo "🪣 Creating S3 Bucket..."
railway add --resource bucket --name "$BUCKET_NAME"

echo "🔗 Linking bucket to PocketBase service..."
railway variables --service "$PB_SERVICE_NAME" --link-resource "$BUCKET_NAME"

echo "📝 Adding PocketBase S3 environment variables..."
railway variables --service "$PB_SERVICE_NAME" set \
  PB_STORAGE_S3_ENABLED=true \
  PB_STORAGE_S3_ENDPOINT='${{AWS_ENDPOINT_URL}}' \
  PB_STORAGE_S3_BUCKET='${{AWS_S3_BUCKET_NAME}}' \
  PB_STORAGE_S3_ACCESS_KEY='${{AWS_ACCESS_KEY_ID}}' \
  PB_STORAGE_S3_SECRET='${{AWS_SECRET_ACCESS_KEY}}' \
  PB_STORAGE_S3_REGION=auto \
  PB_STORAGE_S3_FORCE_PATH_STYLE=true \
  PB_STORAGE_S3_USE_SSL=true

echo "✅ Deployment triggered..."
railway up --service "$PB_SERVICE_NAME"

echo ""
echo "🎉 Setup complete!"
echo ""
echo "Next manual step (required):"
echo "1. Open the PocketBase admin panel"
echo "2. Go to Settings → Files"
echo "3. Turn on 'Use S3 storage'"
echo "4. Fill in the fields (they should auto-populate from env vars)"
echo ""
echo "Bucket name: $BUCKET_NAME"
echo "PocketBase service: $PB_SERVICE_NAME"
