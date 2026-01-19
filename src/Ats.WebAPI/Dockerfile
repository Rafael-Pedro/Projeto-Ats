FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Ats.WebAPI/Ats.WebAPI.csproj", "Ats.WebAPI/"]
COPY ["Ats.Application/Ats.Application.csproj", "Ats.Application/"]
COPY ["Ats.Domain/Ats.Domain.csproj", "Ats.Domain/"]
COPY ["Ats.Infrastructure/Ats.Infrastructure.csproj", "Ats.Infrastructure/"]

RUN dotnet restore "Ats.WebAPI/Ats.WebAPI.csproj"

COPY . .

WORKDIR "/src/Ats.WebAPI"
RUN dotnet build "Ats.WebAPI.csproj" -c Release -o /app/build
RUN dotnet publish "Ats.WebAPI.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Ats.WebAPI.dll"]