dotnet build
dotnet pack #-p:PackageID=cm
dotnet tool install --global thecodemechanic --add-source ./nupkg

