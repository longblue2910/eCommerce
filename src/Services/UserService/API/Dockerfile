# Giai đoạn base (runtime ASP.NET Core)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Giai đoạn build (SDK .NET)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy file .csproj trước để restore dependencies
COPY ["src/Services/UserService/API/API.csproj", "Services/UserService/API/"]
COPY ["src/Services/UserService/Application/Application.csproj", "Services/UserService/Application/"]
COPY ["src/Services/UserService/Domain/Domain.csproj", "Services/UserService/Domain/"]
COPY ["src/Services/UserService/Infrastructure/Infrastructure.csproj", "Services/UserService/Infrastructure/"]
COPY ["src/Shared/SharedKernel/SharedKernel.csproj", "Shared/SharedKernel/"]

# Restore dependencies
RUN dotnet restore "Services/UserService/API/API.csproj"

# Copy toàn bộ mã nguồn
COPY . .

# Debug: Kiểm tra xem thư mục Shared đã copy đúng chưa
RUN ls -R /src/Shared || echo "Shared folder not found!"

# Chuyển thư mục làm việc đến API
WORKDIR "/src/Services/UserService/API"

# Build project
RUN dotnet build "./API.csproj" -c Release --no-restore -o /app/build

# Giai đoạn publish
FROM build AS publish
RUN dotnet publish "./API.csproj" -c Release --no-restore -o /app/publish /p:UseAppHost=false

# Giai đoạn cuối cùng (sản xuất)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserService.API.dll"]
