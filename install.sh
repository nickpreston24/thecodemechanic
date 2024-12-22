dotnet build
dotnet pack -p:PackageID=cm
dotnet tool install --global cm --add-source ./nupkg

