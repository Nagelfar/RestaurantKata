#! /bin/bash
# if you are using podman instead of docker,
# use host.containers.internal instead of host.docker.internal to connect to the services on your host
docker run --rm -it \
    -p 5000:5000 \
    -e ASPNETCORE_URLS=http://+:5000 \
    -e DOTNET_ENVIRONMENT=DockerLocal \
    -e APIs__GuestExperience="http://host.docker.internal:8081/" \
    -e APIS__TableService="http://host.docker.internal:8082/" \
    -e APIS__Billing="http://host.docker.internal:8083/" \
    $(docker build -q customer)
 