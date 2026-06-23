# 💖 SameMess — Backend API

Ứng dụng **hẹn hò (dating app)** kiểu Tinder/Bumble: ghép đôi người dùng theo sở thích & vị trí, **tích hợp AI** (gợi ý mở lời + kiểm duyệt tin nhắn). Điểm khác biệt là mô hình **O2O "Date Pass"** — sau khi match, các cặp đặt **combo ưu đãi tại quán đối tác**, nhận **voucher QR qua email**; ứng dụng **thu hoa hồng** trên mỗi đơn.

> REST API xây bằng **.NET 8** theo **Clean Architecture**.

---

## 🚀 Công nghệ

| Hạng mục | Công nghệ |
|---|---|
| Nền tảng | .NET 8 · ASP.NET Core Web API |
| Kiến trúc | Clean Architecture (Domain / Application / Infrastructure / API) |
| CSDL | PostgreSQL (Neon) · Entity Framework Core (Npgsql) |
| Realtime | SignalR (chat) |
| Xác thực | JWT + Refresh Token (HttpOnly cookie) · BCrypt |
| AI | Google Gemini API (gợi ý chat & kiểm duyệt nội dung) |
| Thanh toán | VNPay |
| Email | Gmail SMTP / Resend (OTP, voucher QR) |
| Lưu ảnh | Cloudinary |
| Khác | AutoMapper · Swagger · Docker · Render |

---

## 🏗️ Kiến trúc

```
SameMess.Domain          # Entities, Enums, Interfaces (không phụ thuộc gì)
SameMess.Application     # Business logic, Services, DTOs, Interfaces
SameMess.Infrastructure  # EF Core, Repositories, AI/Email/Payment, Migrations
SameMess.API             # Controllers, Program.cs, cấu hình, SignalR Hub
```

Phụ thuộc một chiều: `API → Application → Domain`, `Infrastructure → Application/Domain`.

---

## ✨ Tính năng chính

- **Tài khoản & bảo mật:** đăng ký, xác minh email **OTP**, đăng nhập **JWT + Refresh Token**, phân quyền **User / Admin**.
- **Hồ sơ:** ảnh (Cloudinary), thông tin, vị trí, tuỳ chọn ghép đôi; **xác minh khuôn mặt** (admin duyệt).
- **Khám phá & Ghép đôi:** gợi ý theo vị trí/sở thích/uy tín, swipe (Like/Pass/Super Like), tạo **match**.
- **Chat realtime (SignalR):** nhắn tin, chia sẻ địa điểm, **AI gợi ý mở lời** & **AI kiểm duyệt nội dung**.
- **Cây tình yêu (Gamification):** nhiệm vụ ngày/tuần → nhận nguyên liệu → tưới cây lên cấp → mở khoá hẹn hò (Lv4).
- **Date Pass (O2O):** combo ưu đãi tại quán đối tác, thanh toán **VNPay**, gửi **voucher + mã QR** qua email, **thu hoa hồng**.
- **Gói thành viên:** Free / Plus / Gold (thích không giới hạn, Super Swipe, xem ai đã thích…).
- **Hệ thống uy tín (Reputation):** cộng/trừ điểm theo hành vi, ảnh hưởng hiển thị.
- **An toàn:** chặn người dùng, báo cáo vi phạm.
- **Admin Dashboard:** thống kê doanh thu (gói + hoa hồng voucher), duyệt xác minh ảnh, quản lý quán/combo, **cấm & xoá vĩnh viễn** tài khoản vi phạm.

---

## ⚙️ Cài đặt & chạy

### Yêu cầu
- .NET 8 SDK
- PostgreSQL (local hoặc [Neon](https://neon.tech))

### Cấu hình
Tạo `SameMess.API/appsettings.Development.json` (hoặc dùng User Secrets / biến môi trường):

```jsonc
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=...;Port=5432;Database=samemess;Username=...;Password=..."
  },
  "Jwt": {
    "SecretKey": "<chuỗi bí mật đủ dài>",
    "Issuer": "SameMess",
    "Audience": "SameMessApp",
    "AccessTokenMinutes": 30,
    "RefreshTokenDays": 7
  },
  "Cors": { "AllowedOrigins": [ "http://localhost:5173" ] },
  "Ai": { "GeminiApiKey": "<gemini key>", "GeminiModel": "gemini-2.0-flash-lite" },
  "VNPay": {
    "TmnCode": "<tmn>", "HashSecret": "<secret>",
    "BaseUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
    "ReturnUrl": "https://localhost:7097/api/payments/vnpay/return",
    "FrontendReturnUrl": "http://localhost:5173/premium"
  },
  "Smtp": {
    "Host": "smtp.gmail.com", "Port": 587,
    "Username": "<gmail>", "Password": "<app password>",
    "FromEmail": "<gmail>", "FromName": "SameMess"
  },
  "Cloudinary": { "CloudName": "...", "ApiKey": "...", "ApiSecret": "...", "Folder": "samemess" }
}
```

### Chạy
```bash
dotnet restore
dotnet run --project SameMess.API
```
- Migration **tự áp dụng** khi khởi động (`db.Database.Migrate()`).
- Swagger UI: `https://localhost:7097/swagger`

### Docker
```bash
docker build -t samemess-api .
docker run -p 8080:8080 samemess-api
```

---

## 🔌 Một số nhóm endpoint

| Nhóm | Đường dẫn |
|---|---|
| Auth | `POST /api/auth/register` · `login` · `refresh` · `GET /api/auth/me` · `DELETE /api/auth/account` |
| Discovery | `GET /api/discovery?limit=&includeSwiped=` |
| Swipe | `POST /api/swipes` · `GET /api/swipes/liked-me` |
| Chat | `GET /api/conversations` · SignalR Hub |
| Cây tình yêu | `GET /api/tasks` · `POST /api/tasks/{code}/claim` · `POST /api/plants/{matchId}/water` |
| Date Pass | `GET /api/date-pass/combos` · `POST /api/date-pass/orders` |
| Thanh toán | `POST /api/payments/vnpay` · `GET /api/payments/vnpay/return` |
| Admin | `/api/admin/dashboard` · `/api/admin/users` · `/api/admin/venues` · `/api/admin/combos` |

> Tài liệu đầy đủ xem tại **Swagger**.

---

## ☁️ Triển khai

Đóng gói **Docker**, deploy trên **Render** (Free); CSDL **PostgreSQL trên Neon**. Cấu hình các khoá bí mật qua **Environment Variables** của Render.

---

## 📄 License

Dự án học tập (đồ án khởi nghiệp). © 2026 SameMess.
