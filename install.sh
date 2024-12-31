dotnet build
dotnet pack -p:PackageID=sharpify
dotnet tool install --global sharpify --add-source ./nupkg

