## 1️. Xác thực & phân quyền (Authorization & Role Management)
- Xác thực Token (JWT): Middleware kiểm tra Token hợp lệ khi gọi API.
- Refresh Token: Cấp lại Access Token mới khi hết hạn.
- Quản lý vai trò (Roles & Permissions): Thêm quyền (Admin, User, ...) vào hệ thống.
## 2️. Quản lý tài khoản người dùng
- Cập nhật thông tin cá nhân: User có thể sửa tên, email, avatar, v.v.
- Đổi mật khẩu: Xác thực mật khẩu cũ & cập nhật mật khẩu mới.
- Quên mật khẩu (Forgot Password): Gửi email khôi phục mật khẩu.
## 3. Danh sách & quản lý người dùng (Admin Feature)
- Lấy danh sách người dùng: API trả danh sách user (chỉ admin truy cập).
- Vô hiệu hóa tài khoản: Admin có thể khóa/mở tài khoản user.
- Gán quyền (Assign Roles): Thay đổi quyền của user.