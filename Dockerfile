FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /App

COPY . ./

RUN dotnet restore
RUN dotnet publish -o out


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /App

COPY --from=build /App/out .

ENTRYPOINT ["dotnet", "DotNet.Docker.dll"]
