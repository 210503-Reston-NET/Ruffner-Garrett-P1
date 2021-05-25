# Base Image
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

WORKDIR /app

COPY *.sln ./
COPY Data/*.csproj Data/
COPY Model/*.csproj Model/
COPY Service/*.csproj Service/
COPY WebUI/*.csproj WebUI/
COPY Tests/*.csproj Tests/

RUN cd WebUI && dotnet restore

#get source code
COPY . ./

RUN dotnet publish WebUI -c Release -o publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:5.0 as runtime

WORKDIR /app

COPY --from=build /app/publish ./

CMD ["dotnet", "WebUI.dll"]
