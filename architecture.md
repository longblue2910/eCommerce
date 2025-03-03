🏗️ Kiến Trúc Hệ Thống eCommerce Backend

📌 Giới Thiệu

Hệ thống eCommerce Backend được thiết kế theo mô hình Microservices Architecture, áp dụng Clean Architecture và Domain-Driven Design (DDD). Hệ thống sử dụng các công nghệ hiện đại để đảm bảo khả năng mở rộng, bảo trì và hiệu suất cao.

📂 Cấu Trúc Thư Mục

/ecommerce-backend
 ├── src/                    # Chứa source code của từng service
 │    ├── UserService/        # Service quản lý User (SQL Server)
 │    ├── OrderService/       # Service quản lý Order (PostgreSQL)
 │    ├── ProductService/     # Service quản lý Product (MongoDB)
 │    ├── BasketService/      # Service giỏ hàng (Redis)
 │    ├── ShippingService/    # Service vận chuyển
 │    ├── InventoryService/   # Service quản lý kho hàng (gRPC)
 │    ├── ApiGateway/         # API Gateway (Reverse Proxy)
 ├── shared/                 # Chứa code dùng chung
 │    ├── SharedKernel/       # Chứa Entity, ValueObject, AggregateRoot, Events...
 │    ├── DTOs/               # Chứa các DTO dùng chung
 │    ├── Utilities/          # Helper function (Logging, Extensions)
 │    ├── Authentication/     # Code xác thực, Authorization, JWT
 ├── infra/                   # CI/CD, Docker, Terraform
 ├── tests/                   # Chứa Unit Test và Integration Test
 ├── docs/                    # Tài liệu thiết kế hệ thống
 ├── .gitignore               # Bỏ qua thư mục /bin, /obj, /.vs
 ├── README.md                # Mô tả dự án
 ├── docker-compose.yml       # Docker Compose để chạy services cục bộ
 ├── EcommerceBackend.sln     # Solution file của .NET

🏛️ Kiến Trúc Tổng Quan

Hệ thống áp dụng Microservices Architecture kết hợp với Event-Driven Architecture để xử lý dữ liệu bất đồng bộ, giao tiếp giữa các service thông qua gRPC và RabbitMQ.

🏢 Các Service Chính

UserService: Quản lý thông tin người dùng (SQL Server).

OrderService: Quản lý đơn hàng (PostgreSQL).

ProductService: Quản lý sản phẩm (MongoDB).

BasketService: Quản lý giỏ hàng (Redis).

ShippingService: Xử lý vận chuyển đơn hàng.

InventoryService: Quản lý kho hàng (gRPC).

API Gateway: Reverse proxy điều hướng request, hỗ trợ load balancing.

🗄️ Cơ Sở Dữ Liệu

Service

Database

Mô tả

UserService

SQL Server

Lưu trữ thông tin tài khoản người dùng

OrderService

PostgreSQL

Lưu trữ đơn hàng và trạng thái

ProductService

MongoDB

Lưu trữ thông tin sản phẩm dạng JSON

BasketService

Redis

Lưu trữ giỏ hàng tạm thời

InventoryService

SQL Server

Quản lý kho hàng

🔗 Giao Tiếp Giữa Các Service

gRPC: Dùng để giao tiếp giữa các service có độ trễ thấp.

RabbitMQ: Dùng để xử lý sự kiện bất đồng bộ giữa các service.

REST API: Cung cấp API cho frontend và mobile.

🔑 Authentication & Authorization

Hệ thống sử dụng JWT (JSON Web Token) để xác thực và phân quyền người dùng. Người dùng đăng nhập sẽ nhận được access token, sử dụng token này để truy cập các API.

🛠️ DevOps & Triển Khai

Docker & Kubernetes: Tất cả các service được containerized và triển khai qua Kubernetes.

Azure Cloud: Dùng để host backend.

CI/CD: Sử dụng GitHub Actions và Jenkins để tự động triển khai.

📈 Logging & Monitoring

Serilog: Logging hệ thống.

ElasticSearch + Kibana: Giám sát và phân tích log.

Prometheus + Grafana: Theo dõi hiệu suất hệ thống.

🔍 Test & Quality Assurance

Unit Test: Xác thực logic từng module.

Integration Test: Kiểm thử sự phối hợp giữa các service.

Load Test: Kiểm tra hiệu suất hệ thống với nhiều request đồng thời.

🏁 Kết Luận

Kiến trúc trên giúp hệ thống eCommerce đạt hiệu suất cao, dễ mở rộng và bảo trì. Hệ thống sử dụng các công nghệ tiên tiến để đảm bảo hoạt động ổn định trong môi trường thực tế.