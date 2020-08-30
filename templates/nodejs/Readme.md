# Using the template

The template uses `node.js` and was bootstrapped via the following commands

```sh
npm init
npm install express --save
npm install node-fetch --save
```

To customize the template for your service do the following steps:

1. copy the template into a new folder for your service
2. change the `nodejs` name in `package.json` into something more meaningful
3. choose a free development port for your service as part of the `index.js` `PORT` setting

## Running the template

To run the application locally use

  npm install
  npm start

The template is based on [Express.js](http://expressjs.com/) and provides the following additional examples:

- A `/configuration` endpoint which reads and returns a `MY_TARGET_CONFIGURATION` configuration value
- A `/outgoingRequest` endpoint which issues an outward facing HTTP request with [node-fetch](https://github.com/node-fetch/node-fetch) and returns the result.

### Customizing the execution

To change the exposed HTTP port override the `PORT` variable via the following command:

  PORT=1234 npm start

To override other configuration values us the following pattern

  <<configuration key>>=<<configuration value>> npm start

As example

  MY_TARGET_CONFIGURATION="http://orf.at" npm start