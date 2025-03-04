# 🔹 Stage 1: Base Image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80


# 🔹 Stage 2: Build & Restore Dependencies
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Sao chép file dự án trước
COPY ["src/Services/UserService/API/API.csproj", "Services/UserService/API/"]
COPY ["src/Services/UserService/Application/Application.csproj", "Services/UserService/Application/"]
COPY ["src/Services/UserService/Domain/Domain.csproj", "Services/UserService/Domain/"]
COPY ["src/Services/UserService/Infrastructure/Infrastructure.csproj", "Services/UserService/Infrastructure/"]
COPY ["src/Shared/SharedKernel/SharedKernel.csproj", "Shared/SharedKernel/"]

# 🔹 Restore Dependencies
RUN dotnet restore "Services/UserService/API/API.csproj"

# 🔹 Sao chép toàn bộ mã nguồn vào container
COPY src/Services/UserService/API/ Services/UserService/API/
COPY src/Services/UserService/Application/ Services/UserService/Application/
COPY src/Services/UserService/Domain/ Services/UserService/Domain/
COPY src/Services/UserService/Infrastructure/ Services/UserService/Infrastructure/
COPY src/Shared/SharedKernel/ Shared/SharedKernel/

# Chuyển đến thư mục chứa API
WORKDIR "/src/Services/UserService/API"

# 🔹 Build dự án
RUN dotnet build "API.csproj" -c Release -o /app/build

# 🔹 Publish dự án
FROM build AS publish
RUN dotnet publish "API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 🔹 Stage 3: Run Application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "API.dll"]
