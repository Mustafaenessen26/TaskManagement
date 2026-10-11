# ---------- Build aşaması ----------
    FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
    WORKDIR /src
    
    # Önce sadece .csproj dosyaları: paket restore katmanı, kod değişse de cache'ten gelir
    COPY src/TaskManagement.Domain/TaskManagement.Domain.csproj               src/TaskManagement.Domain/
    COPY src/TaskManagement.Application/TaskManagement.Application.csproj     src/TaskManagement.Application/
    COPY src/TaskManagement.Infrastructure/TaskManagement.Infrastructure.csproj src/TaskManagement.Infrastructure/
    COPY src/TaskManagement.API/TaskManagement.API.csproj                     src/TaskManagement.API/
    RUN dotnet restore src/TaskManagement.API/TaskManagement.API.csproj
    
    COPY src/ src/
    RUN dotnet publish src/TaskManagement.API/TaskManagement.API.csproj \
        -c Release -o /app/publish --no-restore
    
    # ---------- Runtime aşaması ----------
    FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
    WORKDIR /app
    
    ENV ASPNETCORE_URLS=http://+:8080
    EXPOSE 8080
    
    COPY --from=build /app/publish .
    
    # Root olmayan kullanıcı (.NET 8 imajlarında hazır gelir)
    USER app
    
    ENTRYPOINT ["dotnet", "TaskManagement.API.dll"]