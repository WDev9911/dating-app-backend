# SameMess — E2E User Flow (cho Frontend)

Mô tả thứ tự gọi API phía **user** (không gồm admin). Mỗi bước ghi: **request → cái nhận về cần giữ lại → bước tiếp theo**.

**Quy ước chung**
- Mọi API (trừ `register`/`verify-email`/`login`/`plans`) cần header: `Authorization: Bearer <accessToken>`.
- `accessToken` lấy ở bước login/verify-email, dùng cho TẤT CẢ bước sau.
- Base URL (cloud): `https://dating-app-backend-q5gk.onrender.com`
- ⚠️ Các luồng **Match/Chat/Plant/Connection/Block** cần **2 user** (A và B) đã hoàn thiện hồ sơ.

---

## STAGE 0 — Tài khoản (làm cho từng user)

| # | Gọi | Body | Nhận về → dùng cho |
|---|-----|------|--------|
| 1 | `POST /api/auth/register` | `{ email, password, displayName, phoneNumber? }` | gửi OTP email → bước 2 |
| 2 | `POST /api/auth/verify-email` | `{ email, otpCode }` | `{ accessToken, user }` + cookie refresh → **token cho mọi bước sau** |
| — | *(user đã seed thì bỏ qua 1-2, dùng login)* | | |
| 3 | `POST /api/auth/login` | `{ email, password }` | `{ accessToken, user }` → **token** |
| 4 | `GET /api/auth/me` | — | kiểm tra token hợp lệ |

**→ Sau khi có `accessToken`, qua STAGE 1.**

---

## STAGE 1 — Hoàn thiện hồ sơ (điều kiện để vào Discovery)

| # | Gọi | Body | Ghi chú |
|---|-----|------|--------|
| 5 | `PUT /api/profile` | `{ displayName, gender, dateOfBirth:"1999-03-15", bio, height, datingGoal }` | cập nhật info |
| 6 | `PUT /api/profile/location` | `{ latitude, longitude }` | **BẮT BUỘC** để discovery hoạt động |
| 7 | `POST /api/profile/photos` | multipart field `file` | trả `{ id, url }` → giữ `photoId` |
| 8 | `PUT /api/profile/photos/{photoId}/primary` | — | đặt ảnh chính (verify-face so với ảnh này) |
| 9 | `GET /api/profile/me` | — | xác nhận `isProfileCompleted = true` |

**→ Đủ info + location + ≥1 ảnh ⇒ `IsProfileCompleted=true` ⇒ user xuất hiện & dùng được Discovery.**

Phụ (bất kỳ lúc nào): `PUT /api/profile/photos/order` `{ photoIds:[] }` · `DELETE /api/profile/photos/{photoId}` · `POST /api/profile/boost`

---

## STAGE 2 — Tiêu chí & Cài đặt

| # | Gọi | Body | Ghi chú |
|---|-----|------|--------|
| 10 | `GET /api/search/filters` | — | trả `interests[]` → **giữ interestId** cho bước 12 |
| 11 | `PUT /api/preferences` | `{ interestedInGender:"Male"\|"Female"\|"Everyone", minAge, maxAge, maxDistanceKm }` | lọc ai hiện trong feed |
| 12 | `PUT /api/settings/interests` | `{ interestIds: [guid,...] }` (tối đa 10) | dùng id từ bước 10 |
| 13 | `GET /api/settings/security` · `GET /api/settings/devices` | — | xem trạng thái |

Phụ: `GET/PUT /api/settings/discovery` (= preferences) · `GET /api/settings/interests` · `PUT /api/settings/security` `{ twoFactorEnabled?, loginAlertsEnabled? }`
⚠️ `PUT /api/settings/password` `{ currentPassword, newPassword }` → **để CUỐI cùng** (thu hồi mọi session, phải login lại).

---

## STAGE 3 — Khám phá / Tìm kiếm

| # | Gọi | Query/Body | Nhận về → dùng cho |
|---|-----|------|--------|
| 14 | `GET /api/search/results` | `?gender=&city=&minAge=&maxAge=&interests=&distanceKm=&sort=distance` | danh sách hồ sơ lọc |
| 15 | `GET /api/discovery?limit=10` | — | feed swipe → **giữ `userId` đối phương** cho bước 16 |

**→ Có `userId` mục tiêu, qua STAGE 4.**

---

## STAGE 4 — Swipe & Match (CẦN 2 USER)

| # | Gọi | Body | Nhận về |
|---|-----|------|--------|
| 16 | `POST /api/swipes` (User A) | `{ targetUserId: B, action:"Like" }` | `{ isMatch:false }` (B chưa like lại) |
| 17 | `GET /api/swipes/liked-me` (User B) | — | thấy A đã like |
| 18 | `POST /api/swipes` (User B) | `{ targetUserId: A, action:"Like" }` | **`{ isMatch:true, matchId }`** 🎉 → giữ `matchId` |

Phụ: `GET /api/swipes/superliked-me` · `POST /api/swipes/undo` (gói Plus, Free → 403) · action `"SuperLike"` / `"Pass"`.

**→ Có `matchId`, qua STAGE 5.**

---

## STAGE 5 — Match & Chat (CẦN match từ STAGE 4)

| # | Gọi | Body | Nhận về → dùng cho |
|---|-----|------|--------|
| 19 | `GET /api/matches` | — | danh sách match → `matchId` |
| 20 | `POST /api/conversations/by-match/{matchId}` | — | trả `{ conversationId }` → giữ |
| 21 | `POST /api/conversations/{conversationId}/messages` | `{ content:"Chào bạn!" }` | gửi tin |
| 22 | `GET /api/conversations/{conversationId}/messages?limit=30` | — | lịch sử tin |
| 23 | `POST /api/conversations/{conversationId}/read` | — | đánh dấu đã đọc |
| 24 | `GET /api/conversations` | — | danh sách hội thoại + chưa đọc |
| 25 | `POST /api/ai/icebreakers/{matchId}` | — | gợi ý mở lời (AI) |

> Realtime: nên dùng **SignalR hub** `/hubs/chat?access_token=<token>` để nhận tin tức thì; REST ở trên là fallback.
Phụ: `DELETE /api/matches/{matchId}` (unmatch).

---

## STAGE 6 — Cây tình yêu & Gamification (CẦN match)

| # | Gọi | Body |
|---|-----|------|
| 26 | `GET /api/plants/{matchId}` | — |
| 27 | `POST /api/plants/{matchId}/water` | — (tưới → cây lớn) |
| 28 | `GET /api/tasks` | — (nhiệm vụ + tự điểm danh login) |
| 29 | `GET /api/inventory` | — (vật phẩm) |

---

## STAGE 7 — Daily & Connection

| # | Gọi | Body | Ghi chú |
|---|-----|------|--------|
| 30 | `GET /api/daily/connection` | — | `{ quests[], totalXp, userXp }` → giữ `code` quest |
| 31 | `POST /api/daily/complete` | `{ questIds:["daily-login","daily-message"] }` | cộng XP |
| 32 | `GET /api/connection/reminders` | — | nhắc kết nối |
| 33 | `GET /api/connection/nudges/{conversationId}` | — | gợi ý hội thoại → giữ `id` (nudge code) |
| 34 | `POST /api/connection/nudges/{conversationId}/dismiss` | `{ nudgeId:"reengage" }` | bỏ qua gợi ý |
| 35 | `POST /api/connection/meetup/{conversationId}/propose` | `{ venueId:"cafe-1", proposedAt:"2026-07-01T18:00:00Z", note }` | đề xuất gặp |

---

## STAGE 8 — Uy tín & Thông báo

| # | Gọi | Body |
|---|-----|------|
| 36 | `GET /api/reputation/me` | — (điểm + hạng) |
| 37 | `GET /api/notifications` | — |
| 38 | `POST /api/notifications/read` | `{ ids:[] }` (hoặc rỗng = tất cả) |
| 39 | `GET /api/notifications/vapid-public-key` | — (để đăng ký Web Push) |
| 40 | `POST /api/notifications/subscribe` | `{ endpoint, keys:{p256dh,auth} }` | cần Web Push từ trình duyệt |
| 41 | `POST /api/notifications/unsubscribe` | `{ endpoint }` |

---

## STAGE 9 — An toàn / PIN

| # | Gọi | Body |
|---|-----|------|
| 42 | `GET /api/safety/settings` | — |
| 43 | `PUT /api/safety/settings` | `{ pinEnabled?, emergencyAlertEnabled?, checkinEnabled? }` |
| 44 | `POST /api/safety/pin/setup` | `{ pin:"1234" }` (4-6 số) |
| 45 | `PUT /api/safety/emergency` | `{ alertMessage, contacts:[{name,phoneNumber,relationship}] }` |
| 46 | `GET /api/safety/emergency` | — |
| 47 | `POST /api/safety/checkin` | `{ status:"safe" }` (hoặc `"help"`) |
| 48 | `POST /api/safety/pin/forgot` | `{ channel:"email" }` → gửi OTP → giữ OTP |
| 49 | `POST /api/safety/pin/verify-otp` | `{ otp:"123456" }` → gỡ PIN để đặt lại |

---

## STAGE 10 — Sự kiện (admin phải tạo & publish trước)

| # | Gọi | Body | Nhận về → dùng cho |
|---|-----|------|--------|
| 50 | `GET /api/events` | — | danh sách → giữ `eventId` |
| 51 | `GET /api/events/{eventId}` | — | chi tiết |
| 52 | `POST /api/events/{eventId}/register` | — | `{ registrationId, status }` |
| 53 | `GET /api/events/history` | — | sự kiện đã đăng ký |
| 54 | `GET /api/events/reward?eventId={eventId}` | — | XP + badge |

---

## STAGE 11 — Thanh toán / Gói

| # | Gọi | Body | Nhận về → dùng cho |
|---|-----|------|--------|
| 55 | `GET /api/plans` | — (public) | danh sách gói → `code` ("Plus"/"Gold") |
| 56 | `GET /api/subscription/me` | — | gói hiện tại |
| 57 | `POST /api/subscription/order` | `{ planCode:"Plus" }` | `{ txnRef, paymentUrl }` → mở `paymentUrl` (VNPay) |
| 58a | **Thật:** mở `paymentUrl` → thanh toán → VNPay redirect `GET /api/payments/vnpay/return` | — | hiển thị kết quả |
| 58b | **DEV/test:** `POST /api/subscription/mock-confirm/{txnRef}` | — | kích hoạt gói ngay |
| 59 | `GET /api/subscription/me` | — | xác nhận đã lên Plus/Gold |

> `GET /api/payments/vnpay/ipn` là server→server (VNPay gọi), frontend không gọi.

---

## STAGE 12 — Xác minh khuôn mặt (cần ảnh primary ở STAGE 1)

| # | Gọi | Body | Ghi chú |
|---|-----|------|--------|
| 60 | `POST /api/profile/verify-face` | multipart field `file` (selfie) | **Frontend mở camera → chụp → gửi file**. So với ảnh **primary**. Trùng → Approved; khác → Pending |
| 61 | `GET /api/profile/verification` | — | trạng thái (Approved/Pending/Rejected). Pending thì chờ admin duyệt |

---

## STAGE 13 — Chặn & Báo cáo (cần user khác)

| # | Gọi | Body |
|---|-----|------|
| 62 | `POST /api/users/{userId}/report` | `{ reason:"spam", description? }` |
| 63 | `POST /api/users/{userId}/block` | — |
| 64 | `GET /api/blocks` | — |
| 65 | `DELETE /api/users/{userId}/block` | — (bỏ chặn) |

---

## STAGE 14 — Kết thúc
| # | Gọi | Ghi chú |
|---|-----|------|
| 66 | `POST /api/auth/refresh` | (cookie) → accessToken mới — test gia hạn |
| 67 | `POST /api/auth/logout` | thu hồi refresh token |

---

## Tóm tắt phụ thuộc (chuỗi bắt buộc)
```
register → verify-email → (accessToken)
  → PUT profile + location + upload photo  (IsProfileCompleted=true)
      → preferences/interests
      → discovery/search  (lấy userId)
          → [2 user] swipe A→B, swipe B→A  (matchId)
              → conversation by-match  (conversationId)
                  → messages / read / ai / nudges / propose-meetup / plant
verify-face  ← cần ảnh primary
events       ← cần admin publish trước
subscription/order → (txnRef) → mock-confirm / VNPay return
password đổi → CUỐI CÙNG (logout hết)
```

## Lưu ý cho Frontend
- **Ảnh:** field `url` trả về là **URL tuyệt đối** (Cloudinary / randomuser). Dùng thẳng, **không** nối base URL. (Xử lý an toàn: `url.startsWith('http') ? url : base+url`.)
- **2 user:** dùng sẵn `alice@samemess.test` + `bob@samemess.test` (mật khẩu `Test@12345`) để test match/chat.
- **Gói Free** chặn vài tính năng (undo swipe, …) → trả `403` là đúng, không phải lỗi.
- **Realtime chat:** SignalR `/hubs/chat?access_token=<token>`.
