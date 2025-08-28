FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["NotesMicroService/NotesMicroService.csproj", "NotesMicroService/"]
RUN dotnet restore "NotesMicroService/NotesMicroService.csproj"
COPY . .
WORKDIR "/src/NotesMicroService"
RUN dotnet build "NotesMicroService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NotesMicroService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotesMicroService.dll"]