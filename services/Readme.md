# Work with service specifications

Services are documented in the YAML-format and we use [Open API Specification](https://www.openapis.org/) for HTTP based services and [AsyncAPI](https://www.asyncapi.com/) for messages.
Note: separate files need to be created for the contracts.

Note: the following commands in the documentation are expected to be executed from the `services` directory!

## On using OpenAPI

### Viewing a single service documentation

To render a single documentation as HTML several options exist, including the following:

- a quick preview can be done via the online tool <https://editor.swagger.io/> and copy-paste of the `YAML` content
- locally with the help of [Redoc](https://hub.docker.com/r/redocly/redoc/) and the following docker command:

        docker run -it --rm \
            -p 8000:80 \
            -v $(pwd)/:/usr/share/nginx/html/specs \
            -e SPEC_URL=specs/Customer.yaml \
            redocly/redoc

    Note: specify the desired service-yaml file instead of `Customer.yaml` and open the documentation at <http://localhost:8000>!

### Viewing all service documentations

To view all service documentation in one document use the following command

    docker run -it --rm \
        -p 8080:80 \
        -v $(pwd)/:/var/www/html/static/api-files \
        -e URLS="[
            { url: '/static/api-files/Customer.yaml', name: 'Customer'},
            { url: '/static/api-files/Billing.yaml', name: 'Billing'},
            { url: '/static/api-files/GuestExperience.yaml', name: 'Guest Experience'},
            { url: '/static/api-files/TableService.yaml', name: 'Table Service'},
            ]" \
        -e PAGE_TITLE="Restaurant Kata" \
        volbrene/redoc


### Building service proxies from the OpenAPI files

Note: you don't need to generate service proxies in order to use `OpenApi` specifications!
You can always hand-roll your own objects/types and use them within your project.

Service proxies for clients can be generated with the help of the <https://github.com/OpenAPITools/openapi-generator> tool.
The tool can be installed or executed via the following `docker` command:

    docker run --rm -v "${PWD}:/services" openapitools/openapi-generator-cli generate \
        --input-spec /services/GuestExperience.yaml  \
        --generator-name  csharp-netcore \
        --output /services/generated/guest \
        --global-property models,modelDocs=false,modelTests=false \
        --package-name Api.GuestExperience

The following things need to be customized:

- input file for the service via `input-spec /services/<<filename>>.yaml`
- language the proxies use with `generator-name` (see supported list: https://openapi-generator.tech/docs/generators)
- output folder via `output /services/generated/<<foldername>>`
- package namespace via `package-name <<namespace>>`
- if the whole API should be generated the `global-property` setting can be omitted (see [customization](https://openapi-generator.tech/docs/customization) and [global properties](https://openapi-generator.tech/docs/globals) for details)

The generated files in the `generated/<<foldername>>` folder can then be used to bootstrap your API implementation.

### Running a fake server

[Fakeit](https://github.com/justinfeng/fakeit) can be used to test an implementation with a fake server

    docker run -t \
        -v "${PWD}:/services" \
        -p 8010:8080 realfengjia/fakeit:latest \
        --spec /services/GuestExperience.yaml

The fake server is reachable via <http://localhost:8010/>.
If you want to use the provided examples of an OpenAPI file append `--use-example` as additional parameter.


## On using AsyncAPI

At the moment the tooling for AsyncApi is not as mature!
There exist a couple of [templates and generators](https://github.com/search?q=topic%3Aasyncapi+topic%3Agenerator+topic%3Atemplate) we can be used to create local stubs.
For not supported languages local message implementations need to be created manually.

### Viewing a single service documentation

A single AsyncAPI service documentation can be generated with the help of [AsyncAPI Generator](https://github.com/asyncapi/generator#cli-usage-with-docker) and the following command:

    docker run --rm -it \
        -v [ASYNCAPI SPEC FILE LOCATION]:/app/asyncapi.yml \
        -v [GENERATED FILES LOCATION]:/app/output \
        asyncapi/generator [COMMAND HERE]

As example:

    docker run --rm -it \
        -v ${PWD}/TableService-Async.yaml:/app/asyncapi.yml \
        -v ${PWD}/output:/app/output \
        asyncapi/generator -o /app/output /app/asyncapi.yml @asyncapi/html-template --force-write