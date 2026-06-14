# Hướng dẫn Deploy — SameMess Backend

App .NET 8 + PostgreSQL. DB cloud dùng **Neon** (free). Code đóng gói bằng **Docker**.
Khi khởi động, app **tự chạy migration** (`db.Database.Migrate()`) → DB tự tạo/cập nhật schema.

---

## 1. Biến môi trường cần set trên host

ASP.NET đọc env var đè lên `appsettings.json`. Quy tắc: dấu `:` trong key → đổi thành `__` (hai gạch dưới).
**Tất cả secret đặt ở đây, KHÔNG commit vào git.**

| Biến | Bắt buộc | Ghi chú |
|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | ✅ | đặt `Production` |
| `ConnectionStrings__DefaultConnection` | ✅ | chuỗi Npgsql tới Neon (xem mục 2) |
| `Jwt__SecretKey` | ✅ | chuỗi bí mật ≥ 32 ký tự |
| `Jwt__Issuer` | — | mặc định `SameMess` (đã có trong appsettings.json) |
| `Jwt__Audience` | — | mặc định `SameMessClient` |
| `Smtp__Username` | ✅ | email gửi OTP |
| `Smtp__Password` | ✅ | Gmail App Password |
| `Smtp__FromEmail` | ✅ | email hiển thị người gửi |
| `Ai__GeminiApiKey` | ✅ | key Gemini (gợi ý mở lời + kiểm duyệt chat) |
| `VNPay__TmnCode` | ✅ | mã merchant VNPay |
| `VNPay__HashSecret` | ✅ | secret ký HMAC VNPay |
| `VNPay__ReturnUrl` | ✅ | `https://<domain-that>/api/payments/vnpay/return` |
| `Cors__AllowedOrigins__0` | ✅ | domain frontend thật, vd `https://samemess.app` |
| `WebPush__PublicKey` / `WebPush__PrivateKey` | — | chỉ khi bật push notification |

> `VNPay__BaseUrl` để mặc định (sandbox) trong appsettings.json; lên production VNPay thật thì set lại.

---

## 2. Connection string Neon (định dạng Npgsql)

Lấy từ Neon dashboard (chuỗi `postgresql://...`) rồi chuyển sang dạng Npgsql:

```
Host=<ep-xxxx>.ap-southeast-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=<password>;SSL Mode=Require;Trust Server Certificate=true
```

- `SSL Mode=Require` — Neon bắt buộc SSL.
- Migration đã được áp dụng sẵn lên Neon; lần deploy đầu app cũng tự `Migrate()` nên không cần làm tay.

---

## 3. Build & chạy bằng Docker (local thử)

```bash
# build image
docker build -t samemess-api .

# chạy thử, truyền env qua --env (hoặc --env-file .env)
docker run -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="Host=...;Database=neondb;Username=neondb_owner;Password=...;SSL Mode=Require;Trust Server Certificate=true" \
  -e Jwt__SecretKey="..." \
  -e Smtp__Username="..." -e Smtp__Password="..." -e Smtp__FromEmail="..." \
  -e Ai__GeminiApiKey="..." \
  -e VNPay__TmnCode="..." -e VNPay__HashSecret="..." -e VNPay__ReturnUrl="https://.../api/payments/vnpay/return" \
  samemess-api
```

Mở http://localhost:8080/swagger để kiểm tra.

---

## 4. Deploy lên Render (gợi ý — free)

1. Push code lên GitHub (file `.dockerignore` + `.gitignore` đã loại secret).
2. Render → **New → Web Service** → kết nối repo.
3. Render tự nhận `Dockerfile`. Runtime: **Docker**.
4. Tab **Environment** → thêm toàn bộ biến ở mục 1 (đặc biệt `ConnectionStrings__DefaultConnection`).
   - Không cần set `PORT` — Render tự inject, Program.cs đã đọc.
5. Create Web Service → Render build image & chạy. Lần đầu app tự migrate lên Neon.

> Lưu ý: Render free **ngủ sau 15 phút** không có request (request đầu chậm ~30s). Cần chạy 24/7 thì nâng gói hoặc dùng VPS.

---

## 5. Ảnh upload (làm khi cần)

Hiện ảnh lưu ở `wwwroot/uploads/` (ổ đĩa container) → **mất khi redeploy**.
Production cần đổi `IPhotoStorageService` sang object storage (Cloudinary / S3 / Supabase Storage).
Chưa có user thật thì để sau.
