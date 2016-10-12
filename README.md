# TodoApiCore

A Todo Web API created in .Net Core with a docker file to host it in a docker container. 
The values are stored in a Redis database, which is hosted in another container.


# Getting started

    cd src/TodoApiCore
    
    docker build -t <yourusername>/imagename .
Build the docker image

    docker run -d -p 8080:5000 --net=yournetwork_name --name container_name imagename
Run the container in the same network as the redis container

    docker pull redis
    docker run --net=yournetwork_name --name=redis redis
Run the redis container in the same network. Make sure to set the name of the container as 'redis'
