# ===== Build stage =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy riêng các .csproj trước để tận dụng cache layer restore (chỉ restore lại khi đổi dependency)
COPY SameMess.Domain/SameMess.Domain.csproj SameMess.Domain/
COPY SameMess.Application/SameMess.Application.csproj SameMess.Application/
COPY SameMess.Infrastructure/SameMess.Infrastructure.csproj SameMess.Infrastructure/
COPY SameMess.API/SameMess.API.csproj SameMess.API/
RUN dotnet restore SameMess.API/SameMess.API.csproj

# Copy toàn bộ source rồi publish bản Release
COPY . .
RUN dotnet publish SameMess.API/SameMess.API.csproj -c Release -o /app/publish /p:UseAppHost=false

# ===== Runtime stage =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Cổng mặc định trong container. Host PaaS (Render/Fly/Railway) sẽ inject biến PORT
# và Program.cs sẽ lắng nghe theo PORT đó (ghi đè dòng này).
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["dotnet", "SameMess.API.dll"]
