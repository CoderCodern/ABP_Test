FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["src/TestABP.Web.Host/TestABP.Web.Host.csproj", "src/TestABP.Web.Host/"]
COPY ["src/TestABP.Web.Core/TestABP.Web.Core.csproj", "src/TestABP.Web.Core/"]
COPY ["src/TestABP.Application/TestABP.Application.csproj", "src/TestABP.Application/"]
COPY ["src/TestABP.Core/TestABP.Core.csproj", "src/TestABP.Core/"]
COPY ["src/TestABP.EntityFrameworkCore/TestABP.EntityFrameworkCore.csproj", "src/TestABP.EntityFrameworkCore/"]
WORKDIR "/src/src/TestABP.Web.Host"
RUN dotnet restore 

WORKDIR /src
COPY ["src/TestABP.Web.Host", "src/TestABP.Web.Host"]
COPY ["src/TestABP.Web.Core", "src/TestABP.Web.Core"]
COPY ["src/TestABP.Application", "src/TestABP.Application"]
COPY ["src/TestABP.Core", "src/TestABP.Core"]
COPY ["src/TestABP.EntityFrameworkCore", "src/TestABP.EntityFrameworkCore"]
WORKDIR "/src/src/TestABP.Web.Host"
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 80
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "TestABP.Web.Host.dll"]
