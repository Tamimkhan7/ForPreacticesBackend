From mcr.microsoft.com/dotnet/sdk:9.0 As build

WORKDIR /app

copy *.csproj .

run dotnet restore

copy . .

run dotnet publish -c Release -o /app/publish




From mcr.microsoft.com/dotnet/aspnet:9.0

workdir /app

copy --from=build /app/publish /app

Expose 8080

ENTRYPOINT ["dotnet", "ForPractices.dll"]