FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Payment/Payment.API/Payment.API.csproj", "Services/Payment/Payment.API/"]
COPY ["Services/Payment/Payment.Application/Payment.Application.csproj", "Services/Payment/Payment.Application/"]
COPY ["Services/Payment/Payment.Domain/Payment.Domain.csproj", "Services/Payment/Payment.Domain/"]
COPY ["Services/Payment/Payment.Infrastructure/Payment.Infrastructure.csproj", "Services/Payment/Payment.Infrastructure/"]
COPY ["Common/EventBus.Messages/EventBus.Messages.csproj", "Common/EventBus.Messages/"]
RUN dotnet restore "Services/Payment/Payment.API/Payment.API.csproj"
COPY . .
WORKDIR "/src/Services/Payment/Payment.API"
RUN dotnet publish "Payment.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Payment.API.dll"]
