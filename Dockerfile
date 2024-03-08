FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR webapp

EXPOSE 8080
EXPOSE 8081

#COPY PROJECT FILES
COPY ./*.csproj ./
RUN dotnet restore PerfectPickUsers_MS.csproj

#COPY ALL FILES
COPY . .
RUN dotnet publish -c Release -o out

#BUILD THE IMAGE
FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /webapp
COPY --from=build /webapp/out .
ENTRYPOINT ["dotnet", "PerfectPickUsers_MS.dll"]