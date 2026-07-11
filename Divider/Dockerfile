# Estágio 1: build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Divider/Divider/*.csproj Divider/Divider/
RUN dotnet restore Divider/Divider/Divider.csproj

COPY Divider/Divider/. Divider/Divider/
WORKDIR /src/Divider/Divider
RUN dotnet publish Divider.csproj -c Release -o /app/publish

# Estágio 2: imagem final, mais leve, só com o runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Divider.dll"]