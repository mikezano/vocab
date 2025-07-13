# PowerShell script to build, tag, and push Docker image to Azure Container Registry using a service principal

# Set your Azure Container Registry name and image name
$ACR_NAME = "vocab"      # <-- replace with your ACR name
$IMAGE_NAME = "vocab"
$TAG = "latest"

# Service principal credentials (replace with your actual values)
$APP_ID     = $env:AZURE_APP_ID      # from az ad sp create-for-rbac
$PASSWORD   = $env:AZURE_PASSWORD   # from az ad sp create-for-rbac
$TENANT_ID  = $env:AZURE_TENANT_ID     # from az ad sp create-for-rbac

# Build the Docker image
docker build -t $IMAGE_NAME .

# Tag the image for ACR
$fullTag = "$ACR_NAME.azurecr.io/$IMAGE_NAME`:$TAG"
docker tag $IMAGE_NAME $fullTag

# Log in to Azure and ACR non-interactively
az login --service-principal -u $APP_ID -p $PASSWORD --tenant $TENANT_ID --only-show-errors | Out-Null
az acr login --name $ACR_NAME --only-show-errors

# Push the image to ACR
docker push $fullTag

az containerapp update --name testing-azure-vocab-app --resource-group Default-Web-WestUS --image "$fullTag"