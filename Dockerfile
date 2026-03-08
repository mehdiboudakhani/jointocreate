FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /jointocreate

COPY src/JTC.csproj src/
RUN dotnet restore src/JTC.csproj

COPY src/ src/
RUN dotnet publish src/JTC.csproj -c Release -o /jointocreate/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /jointocreate
COPY --from=build /jointocreate/publish .
ENTRYPOINT ["dotnet", "JTC.dll"]