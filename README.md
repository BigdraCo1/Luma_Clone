# Alma: an event management platform

## Recommended Extensions

-   [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
-   [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
-   [Prettier](https://marketplace.visualstudio.com/items?itemName=esbenp.prettier-vscode)
-   [Razor / Blazor CSS IntelliSense](https://marketplace.visualstudio.com/items?itemName=kevin-chatham.aspnetcorerazor-html-css-class-completion)
-   [C# Format Usings With Code Action](https://github.com/Nat-thapas/vscode-csharp-format-usings/releases) (This is a custom extension. Download the .vsix file and use the install from VSIX option)

## Running

### Development

To run a development server, follow the steps outlined below

1. rename or copy the file `appsettings.Development.example.json` to `appsettings.Development.json`
2. edit `appsettings.Development.json` configuration as needed
3. run `dotnet restore` to install dotnet dependencies
4. run `dotnet tool restore` to install dotnet tools
5. run `dotnet ef database update` do apply database migrations
6. run `dotnet run` or `dotnet watch` to start development server
