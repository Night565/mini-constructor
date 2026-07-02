FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Constructor/Constructor.csproj Constructor/
RUN dotnet restore Constructor/Constructor.csproj

COPY Constructor/ Constructor/
RUN dotnet publish Constructor/Constructor.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ConnectionStrings__DefaultConnection="Data Source=/app/data/constructor.db"

COPY --from=build /app/publish/ ./
RUN mkdir -p /app/data /app/wwwroot/uploads
COPY Constructor/constructor.db /app/data/constructor.db

EXPOSE 8080

ENTRYPOINT ["dotnet", "Constructor.dll"]
