# 🛒 Order Saga - Orchestration Pattern

## 📌 Giới thiệu
`OrderSaga` là một **Worker Service** trong hệ thống eCommerce, được sử dụng để quản lý **Orchestration Saga Pattern** cho quy trình đặt hàng. Nó giúp xử lý các bước như thanh toán, kiểm tra kho và xác nhận đơn hàng bằng cách sử dụng **Event-Driven Architecture** với **RabbitMQ** làm message broker.

## 📂 Cấu trúc thư mục
```sh
/order-saga
 ├── OrderSaga.sln                # Solution file
 ├── src/
 │   ├── OrderSagaWorker/         # Service chính
 │   │   ├── OrderSagaWorker.cs   # Worker chạy nền
 │   │   ├── OrderSagaOrchestrator.cs  # Xử lý quy trình Saga
 │   ├── Messaging/               # Hệ thống messaging
 │   │   ├── IMessageBus.cs       # Interface cho Message Bus
 │   │   ├── RabbitMqMessageBus.cs # Triển khai Message Bus với RabbitMQ
 │   ├── Events/                  # Định nghĩa các sự kiện trong Saga
 │   │   ├── OrderCreatedEvent.cs
 │   │   ├── PaymentRequestedEvent.cs
 │   │   ├── PaymentProcessedEvent.cs
 │   │   ├── InventoryReservedEvent.cs
 │   │   ├── OrderConfirmedEvent.cs
 │   │   ├── OrderCanceledEvent.cs
 │   │   ├── RefundRequestedEvent.cs
 │   ├── appsettings.json         # Cấu hình RabbitMQ
 ├── README.md                    # Tài liệu dự án
```

## 🔄 Quy trình Saga
Hệ thống hoạt động dựa trên các bước sau:
1️⃣ **OrderCreatedEvent**: Khi người dùng đặt hàng, sự kiện `OrderCreatedEvent` được phát đi.
2️⃣ **PaymentRequestedEvent**: Saga yêu cầu xử lý thanh toán.
3️⃣ **PaymentProcessedEvent**: Hệ thống thanh toán phản hồi kết quả.
   - ✅ Nếu thanh toán thành công → Kiểm tra kho (`InventoryReservedEvent`).
   - ❌ Nếu thanh toán thất bại → Hủy đơn hàng (`OrderCanceledEvent`).
4️⃣ **InventoryReservedEvent**: Hệ thống kiểm tra tồn kho.
   - ✅ Nếu có hàng → Xác nhận đơn (`OrderConfirmedEvent`).
   - ❌ Nếu hết hàng → Hoàn tiền (`RefundRequestedEvent`).

## 🚀 Cách chạy ứng dụng
### 1️⃣ Cấu hình RabbitMQ
Hệ thống sử dụng RabbitMQ để giao tiếp giữa các microservices. Chạy RabbitMQ bằng Docker:
```sh
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:management
```
Truy cập RabbitMQ Management UI tại: `http://localhost:15672` (User: `guest`, Pass: `guest`).

### 2️⃣ Chạy OrderSaga Worker
Mở terminal và chạy lệnh sau:
```sh
dotnet run --project src/OrderSagaWorker/OrderSagaWorker.csproj
```

## ⚙️ Cấu hình appsettings.json
```json
{
  "RabbitMQ": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest"
  }
}
```

## 🛠 Công nghệ sử dụng
- **.NET 8 Worker Service**
- **RabbitMQ (Message Broker)**
- **Event-Driven Architecture**
- **Orchestration Saga Pattern**
- **Docker** (Tùy chọn)

---
🚀 *Hãy đảm bảo bạn đã triển khai đầy đủ các microservices liên quan trước khi chạy Saga!*