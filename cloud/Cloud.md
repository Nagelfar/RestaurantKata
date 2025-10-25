# Cloud Workshop

The goal is to experiment with a Hyperscaler, deploy a couple of workloads and eventually run a version of the Kata in the cloud.

## Prerequisites

- Install [Docker](https://www.docker.com/products/docker-desktop/) or similar tools (e.g. [Podman](https://podman-desktop.io))
- Install [Azure CLI](https://learn.microsoft.com/en-gb/cli/azure/install-azure-cli?view=azure-cli-latest) (`brew install azure-cli`) and make [yourself familiar](https://learn.microsoft.com/en-gb/cli/azure/what-is-azure-cli?view=azure-cli-latest) with it
    - Make sure you have an existing Azure Account ([free Student Account](https://learn.microsoft.com/en-us/azure/education-hub/about-azure-for-students) can be created [here](https://azure.microsoft.com/en-gb/free/students/))
    - Run `az login` to connect your CLI with Azure account.

## Execute the Kata in the cloud

We will deploy the first version of the Kata as distributed application in Azure.

### Prepare everything for the images

First we need to create a private container registry to hold our images

![Container Registry Dialog](container_registry.png)

First create a resource group to hold our application

    az group create --name dsa --location eastaustria TODO

Then we add the container registry

    az acr create --name das25<group-number> --group dsa --sku Basic

Login & Configure the registry

    az acr login --name das25<group-number>

### Build and Push Images to the registry

Ensure we have multi build enabled (or make sure you build images for `linux/arm64` otherwise)

    docker buildx create --use

Then build and push all docker images

//    docker buildx build --platform linux/amd64,linux/arm64 . -t restaurant-implementation
    docker buildx build --platform linux/amd64,linux/arm64 --tag das25<group-number>.azurecr.io/restaurant/restaurant-implementation --push . 
//  docker buildx build --platform linux/amd64,linux/arm64 customer -t restaurant-customer
    docker buildx build --platform linux/amd64,linux/arm64 --tag das25<group-number>.azurecr.io/restaurant/restaurant-customer:latest --push customer 


Then push the image to the container registry

    docker tag restaurant-implementation dsa4cf.azurecr.io/restaurant/restaurant-implementation
    docker push --platform linux/amd64,linux/arm64 dsa4cf.azurecr.io/restaurant/restaurant-implementation

    docker tag restaurant-customer dsa4cf.azurecr.io/restaurant/restaurant-customer
    docker push --platform linux/amd64,linux/arm64 dsa4cf.azurecr.io/restaurant/restaurant-customer

#### Verify images work locally

Now pull and start the images you've pushed to the registry back to your machine to verify everything worked fine sofar.

    docker pull das25<group-number>.azurecr.io/restaurant/restaurant-customer:latest

![Container on Registry Dialog](container_on_registry.png)

### Prepare the Container App to run our images

    az containerapp create


Run Image with correct configuration (locally)

    docker run --rm -it \
        -p 5100:5100 \
        -e ASPNETCORE_URLS=http://+:5100 \
        -e DOTNET_ENVIRONMENT=DockerLocal \
        -e APIS__FoodPreparation="http://host.docker.internal:5100/FoodPreparation/" \
        -e APIS__Delivery="http://host.docker.internal:5100/delivery/" \
        -e APIs__GuestExperience="http://host.docker.internal:5100/guestexperience/" \
        -e APIS__TableService="http://host.docker.internal:5100/tableservice/" \
        -e APIS__Billing="http://host.docker.internal:5100/billing/" \
        -e APIS__Customer="http://host.docker.internal:5000/" \
        restaurant-implementation
 

Run customer

    docker run --rm -it \
        -p 5000:5000 \
        -e ASPNETCORE_URLS=http://+:5000 \
        -e DOTNET_ENVIRONMENT=DockerLocal \
        -e APIs__GuestExperience="http://host.docker.internal:5100/guestexperience/" \
        -e APIS__TableService="http://host.docker.internal:5100/tableservice/" \
        -e APIS__Billing="http://host.docker.internal:5100/billing/" \
        restaurant-customer
 

 Application URLs

 https://implementation.wonderfuldesert-8dec7f6a.westeurope.azurecontainerapps.io
 https://customer.wonderfuldesert-8dec7f6a.westeurope.azurecontainerapps.io 