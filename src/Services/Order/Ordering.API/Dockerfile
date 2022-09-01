#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Services/Order/Ordering.API/Ordering.API.csproj", "Services/Order/Ordering.API/"]
COPY ["Services/Order/Order.Infrastructure/Ordering.Infrastructure.csproj", "Services/Order/Order.Infrastructure/"]
COPY ["Services/Order/Order.Application/Ordering.Application.csproj", "Services/Order/Order.Application/"]
COPY ["Services/Order/Order.Domain/Ordering.Domain.csproj", "Services/Order/Order.Domain/"]
COPY ["BuildingBlocks/EventBus.Messages/EventBus.Messages.csproj", "BuildingBlocks/EventBus.Messages/"]
RUN dotnet restore "Services/Order/Ordering.API/Ordering.API.csproj"
COPY . .
WORKDIR "/src/Services/Order/Ordering.API"
RUN dotnet build "Ordering.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ordering.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ordering.API.dll"]