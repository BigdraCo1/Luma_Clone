FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

WORKDIR /App

COPY ./alma.csproj ./

RUN dotnet restore

COPY . ./

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime

RUN apk add --update --no-cache icu-libs

WORKDIR /App

COPY --from=build /App/out .

ENV ASPNETCORE_ENVIRONMENT=Production
CMD ["dotnet", "alma.dll"]
