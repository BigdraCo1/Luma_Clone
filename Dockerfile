FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

WORKDIR /App

COPY . ./

RUN dotnet restore
RUN dotnet publish -o out


FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime

WORKDIR /App

COPY --from=build /App/out .

ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "alma.dll"]
