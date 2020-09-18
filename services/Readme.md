# View documentation

Services are documented with the [Open API Specification](https://www.openapis.org/) in the YAML-format.

## Viewing a single service documentation

To render a single documentation as HTML with [Redoc](https://hub.docker.com/r/redocly/redoc/) the following docker command can be used

    docker run -it --rm \
        -p 8080:80 \
        -v $(pwd)/:/usr/share/nginx/html/specs \
        -e SPEC_URL=specs/Customer.yaml \
        redocly/redoc

Note: specify the desired serice-yaml file instead of `Customer.yaml` and open the documentation at <http://localhost:8080>! 

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