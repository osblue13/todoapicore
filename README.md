# TodoApiCore

A Todo Web API created in .Net Core with a docker file to host it in a docker container. 
The values are stored in a Redis database, which is hosted in another container.


# Getting started

    cd src/TodoApiCore

## Easy way

    docker-compose up
This will build the images and bring up the containers.

## The hard way
    
Get the docker image by following 1st option or 2nd option

    docker pull osblue13/todoapicore
1st option - pull from docker hub    

    docker build -t <yourusername>/imagename .
2nd option - Build the docker image

    docker network create --driver bridge yournetwork_name
Create a network to run our environment in

    docker run -d -p 8080:5000 --net=yournetwork_name --name container_name imagename
Run the app container in the network set up above

    docker pull redis
    docker run --net=yournetwork_name --name=redis redis
Run the redis container in the same network. Make sure to set the name of the container as 'redis'
