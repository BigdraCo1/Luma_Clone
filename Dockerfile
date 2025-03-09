FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

WORKDIR /App

COPY . ./

RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime

RUN apk add --update --no-cache icu-libs

WORKDIR /App

COPY --from=build /App/out .
COPY ./Databases/template.sqlite ./Databases/database.sqlite

ENV ASPNETCORE_ENVIRONMENT=Production
CMD ["dotnet", "alma.dll"]
