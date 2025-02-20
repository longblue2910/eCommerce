# 🛍️ Ecommerce Backend

## 🚀 Giới thiệu
Đây là hệ thống backend cho nền tảng thương mại điện tử, được xây dựng theo **Microservices Architecture** với các công nghệ hiện đại như **.NET**, **gRPC**, **RabbitMQ**, **Redis**, **Docker**, và **Azure Cloud**.

## 🏗️ Công nghệ sử dụng
- **.NET 8** - Backend chính cho các Microservices
- **gRPC** - Giao tiếp giữa **InventoryService** và các service khác
- **SQL Server, PostgreSQL, MongoDB, Redis** - Cơ sở dữ liệu phân tán
- **RabbitMQ** - Message Broker để giao tiếp giữa các service
- **Docker & Kubernetes** - Triển khai containerized microservices
- **Azure** - Cloud infrastructure

## 🏛️ Kiến trúc hệ thống
Hệ thống sử dụng **Microservices** kết hợp với **Event-Driven Architecture**:
- **UserService**: Quản lý thông tin người dùng (**SQL Server**)
- **OrderService**: Quản lý đơn hàng (**PostgreSQL**)
- **ProductService**: Quản lý sản phẩm (**MongoDB**)
- **BasketService**: Quản lý giỏ hàng (**Redis**)
- **ShippingService**: Xử lý vận chuyển đơn hàng
- **InventoryService**: Quản lý kho hàng (**gRPC**)
- **API Gateway**: Reverse proxy điều hướng request

## 🛠️ Hướng dẫn cài đặt
### 1️⃣ Clone repository
```sh
git clone https://github.com/yourusername/ecommerce-backend.git
cd ecommerce-backend


📁 Cấu trúc thư mục

/ecommerce-backend
 ├── src/                     # Chứa source code của từng service
 │    ├── UserService/        # Service quản lý User (SQL Server)
 │    ├── OrderService/       # Service quản lý Order (PostgreSQL)
 │    ├── ProductService/     # Service quản lý Product (MongoDB)
 │    ├── BasketService/      # Service giỏ hàng (Redis)
 │    ├── ShippingService/    # Service vận chuyển
 │    ├── InventoryService/   # Service quản lý kho hàng (gRPC)
 │    ├── ApiGateway/         # API Gateway (Reverse Proxy)
 ├── proto/                   # Chứa file .proto định nghĩa gRPC
 │    ├── inventory.proto     # Định nghĩa gRPC cho Inventory Service
 ├── shared/                  # Chứa code dùng chung (DTOs, Utilities, Authentication)
 ├── infra/                   # Chứa config CI/CD, Docker, Terraform
 ├── tests/                   # Chứa Unit Test và Integration Test
 ├── docs/                    # Tài liệu thiết kế hệ thống
 ├── .gitignore                # Bỏ qua thư mục /bin, /obj, /.vs
 ├── README.md                 # Mô tả dự án
 ├── docker-compose.yml        # Docker Compose để chạy services cục bộ
 ├── EcommerceBackend.sln      # Solution file của .NET
