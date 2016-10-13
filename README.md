# TodoApiCore

A Todo Web API created in .Net Core with a docker file to host it in a docker container. 
The values are stored in a Redis database, which is hosted in another container.


# Getting started

    cd src/TodoApiCore

## Easy way

    docker-compose up
This will build the images and bring up the containers.

## The hard way
    
    docker build -t <yourusername>/imagename .
Build the docker image

    docker run -d -p 8080:5000 --link yourrediscontainer:redis imagename
Run the container by linking it to your redis container
