# Using the template

## Running the template

To run the application locally use

```bash
$ npm install
$ npm start # or npm run watch
```

The template is based on [Express.js](http://expressjs.com/) and provides the following additional examples:

- A `/configuration` endpoint which reads and returns a `MY_TARGET_CONFIGURATION` configuration value
- A `/outgoingRequest` endpoint which issues an outward facing HTTP request with [node-fetch](https://github.com/node-fetch/node-fetch) and returns the result.

### Customizing the execution

To change the exposed HTTP port override the `PORT` variable via the following command:
```bash
$ PORT=1234 npm start
```

To override other configuration values us the following pattern

```bash
$ <<configuration key>>=<<configuration value>> npm start
```

As example

```bash
$ MY_TARGET_CONFIGURATION="http://orf.at" npm start
```