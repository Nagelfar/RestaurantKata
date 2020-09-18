# View documentation

Services are documented with the [Open API Specification](https://www.openapis.org/) in the YAML-format.

Note: commands are expected to be executed from the `services` directory!

## Viewing a single service documentation

To render a single documentation as HTML several options exist.

- a quick preview can be done via the online tool <https://editor.swagger.io/>
- locally with the help of [Redoc](https://hub.docker.com/r/redocly/redoc/) and the following docker command:

        docker run -it --rm \
            -p 8000:80 \
            -v $(pwd)/:/usr/share/nginx/html/specs \
            -e SPEC_URL=specs/Customer.yaml \
            redocly/redoc

    Note: specify the desired service-yaml file instead of `Customer.yaml` and open the documentation at <http://localhost:8000>!

## Viewing all service documentations

To view all service documentaion in one document use the following command

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


## Building service proxies from the OpenAPI files

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


## Runing a fake server

[Fakeit](https://github.com/justinfeng/fakeit) can be used to test an implementation with a fake server

    docker run -t \
        -v "${PWD}:/services" \
        -p 8010:8080 realfengjia/fakeit:latest \
        --spec /services/GuestExperience.yaml

The fake server is reachable via <http://localhost:8010/>.
To use provided examples of the OpenAPI file use `--use-example` as additional parameter
