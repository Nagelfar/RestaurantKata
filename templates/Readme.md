# Restaurant Kata Templates

These templates are a starting point for the implementation of one actor / one component.
Using them is not a requirement, but might speed up the initial process.

## .NET Core via C#

One template uses a C# implementation based on [.NET Core](https://dotnet.microsoft.com/).

- install dotnet core
- run the template in the `csharp` directory via `dotnet run`
- make a GET request to `http://localhost:5000/WeatherForecast/outgoingRequest` to verify it works
- customize environment variables on execution (in MacOs / Linux) via e.g. `APIS__MyDemoApi=http://orf.at dotnet run`
- copy the template into another folder & rename the `csharp`-namespace and `csharp.csproj` into something more meaningful

## NodeJS

One template uses [Node.JS](https://nodejs.org/) as implemenation

- install the lastest version of node
- run `npm install` to download and install all dependencies
- run `npm start` to start the application
- execute a GET request to `http://localhost:3000/outgoingRequest` to verify it works
- customize environment variables on execution (in MacOs / Linux) via e.g. `MY_TARGET_CONFIGURATION=http://orf.at npm start`
- copy the template into another folder and adapt the `package.json` file