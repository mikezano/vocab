#!/bin/bash
# filepath: push-to-acr.sh

# Set your Azure Container Registry name and image name
ACR_NAME="vocab"      # <-- replace with your ACR name (no angle brackets)
IMAGE_NAME="vocab"
TAG="latest"

# Build the Docker image
docker build -t $IMAGE_NAME .

# Tag the image for ACR
docker tag $IMAGE_NAME $ACR_NAME.azurecr.io/$IMAGE_NAME:$TAG

# Log in to Azure and ACR
az login
az acr login --name $ACR_NAME

# Push the image to ACR
docker push $ACR_NAME.azurecr.io/$IMAGE_NAME:$TAG