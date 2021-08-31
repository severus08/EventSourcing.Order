FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Order.EventSubscribers/Order.EventSubscribers.csproj", "Order.EventSubscribers/"]
RUN dotnet restore "Order.EventSubscribers/Order.EventSubscribers.csproj"
COPY . .
WORKDIR "/src/Order.EventSubscribers"
RUN dotnet build "Order.EventSubscribers.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Order.EventSubscribers.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Order.EventSubscribers.dll"]

#docker build -t orderes -f Order.EventSubscribers\Dockerfile .
#docker save ccae4d44328c -o order.worker
#docker load -i order.worker && docker tag ccae4d44328c order.worker && docker run --name order.worker -e ASPNETCORE_ENVIRONMENT="Production" -d order.worker