# Using the template

The template was bootstrapped via the following [article](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-6.0&tabs=visual-studio-code) and the `dotnet new webapi -o .` command on MacOS.
Please make sure you have all requirements installed and you have an idea about the [fundamentals](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/) of dotnet core!

To customize the template for your service do the following steps:

1. copy the template into a new folder for your service
2. change the namespace & dll name `csharp` into something more meaningful
3. choose a free development port for your service as part of the `Properties/launchSettings.json`

## Running the template

To run the application locally use

    dotnet run

The template provides the following additional examples in the `WeatherForecastController`:

- A `WeatherForecast/configuration` endpoint which reads and returns a `myTargetConfiguration` configuration value
- A `WeatherForecast/outgoingRequest` endpoint which issues an outward facing HTTP request and returns the result

### Customizing the execution

#### Change the exposed HTTP port

To override the exposed HTTP the following methods can be used

- use the `--urls` parameter : `dotnet run --urls "http://localhost:1234"`
- ignore the launch profiles and use `ASPNETCORE_URLS=http://*:1234 dotnet run --no-launch-profile`
- publish the application and launch it via the following command (note: you need to apply your custom dll name)

  ```sh
    dotnet publish -o artifacts
    cd ./artifacts
    ASPNETCORE_URLS=http://*:1234 dotnet csharp.dll
  ```

#### Changing configuration parameters

When running the application without any changes, a request to `http://localhost:5000/WeatherForecast/configuration` should return the default value specified in `appsettings.json``

To override the value from the command line use the following command pattern

    <<configuration key>>=<<configuration value>> dotnet run

An example:

    myTargetConfiguration="http://orf.at" dotnet run
