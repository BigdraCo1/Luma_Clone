# Alma: an event management platform

## Recommended Extensions
- [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
- [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
- [Prettier](https://marketplace.visualstudio.com/items?itemName=esbenp.prettier-vscode)
- [Razor / Blazor CSS IntelliSense](https://marketplace.visualstudio.com/items?itemName=kevin-chatham.aspnetcorerazor-html-css-class-completion)


## Running

### Development
To run a development server, follow the steps outlined below
    1. rename or copy the file `.env.example` to `.env`
    2. edit `.env` configuration as needed
    3. run `docker compose -f docker-compose.dev.yaml up -d --build` to start the developmen dependencies (this will not start the main application)
    4. run `dotnet run` or `dotnet watch` to start development server
