choco install dotnet
choco install dotnet-sdk

dotnet new webapi -n WindowsMediaController -o WindowsMediaController


dotnet build WindowsMediaController.csproj

dotnet run WindowsMediaController.csproj

