# Dangl.OpenCDE

This is an open source sample implementation of the buildingSMART OpenCDE Documents API. A live demo can be found here:  
<https://opencde-dev.dangl.dev/>

## User Documentation

Documentation is available either at <https://docs.dangl-it.com/Projects/Dangl.OpenCDE> or directly here in this repository at [index.md](./docs/index.md).

## Downloads

Documentation and downloads are available here: <https://docs.dangl-it.com/Projects/Dangl.OpenCDE>

## Local Development

To run the backend, `Azurite` must be running in Docker to provide a local environment for Azure Blob Storage. Simply run it with this command:

    docker run -p 10000:10000 -p 10001:10001 -p 10002:10002 mcr.microsoft.com/azure-storage/azurite

### Client

To run the client, please run `ng serve` in the client UI directory and `electronize start /watch /args ASPNETCORE_ENVIRONMENT="Development"` in the client backend directory.

## CI/CD

The `Jenkinsfile` is only executed for the repository at [GeorgDangl/Dangl.OpenCDE](https://github.com/GeorgDangl/Dangl.OpenCDE), to have deployments controlled by a separate repository.

The pipeline in GitHub Actions does currently not correctly execute, since this repository is referencing some internal packages that are not yet made available publicly.

## Tests

To run the integration tests, you need access to some internal Dangl**IT** packages. However, both the server and the client can be compiled and run with public packages.
