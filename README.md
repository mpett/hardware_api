# Hardware API

A simple server-client for listing and leasing hardware for PC, PS4 and XBOZ

## Getting the sample

The easiest way is to clone this repository.

```console
git clone https://github.com/mpett/hardware_api.git
```

## Build and run the backend from Docker

You can build and run the sample in Docker using the following commands. The instructions assume that you are in the root of the repository.

```console
cd backend
docker build -t backend .
docker run -p 2204:2204 backend
```

## Build and run the frontend from Docker

You can build and run the sample in Docker using the following commands. The instructions assume that you are in the root of the repository.

```console
cd frontend
docker build -t frontend .
docker run --rm -i frontend <ip_of_backend_container>
```

Note: You can get the ip for the backend container by running:

```console
docker ps
docker inspect <backend_container_id>
```

## Build and run locally

You can also build and run the sample locally. The instructions assume that you are in the root of the repository.

```console
cd backend
python backend.py
cd ..
cd frontend
dotnet restore
dotnet run
```