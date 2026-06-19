# Hướng dẫn gọi API SameMess (cho Cursor) — LUỒNG USER

Tài liệu để AI (Cursor) sinh code frontend gọi API **đúng**. Chỉ luồng **user** (không có admin).

---

## 0. Cấu hình chung

- **Base URL:** `https://dating-app-backend-q5gk.onrender.com`
- **Content-Type mặc định:** `application/json` cho mọi request có body — TRỪ 2 endpoint upload file (xem mục 4).
- **Lưu ý Render free:** request đầu sau khi server "ngủ" có thể mất ~30-50s. Đặt timeout client ≥ 60s và có retry nhẹ cho lần đầu.

---

## 1. Xác thực & Token (QUAN TRỌNG NHẤT)

Backend dùng **JWT access token** (gắn ở header) + **refresh token** (cookie HttpOnly do server set).

### Quy tắc bắt buộc
1. **Access token** lấy từ body của `login` / `verify-email`:
   ```json
   { "accessToken": "eyJ...", "user": { ... } }
   ```
   → Lưu `accessToken` trong **bộ nhớ (memory/state)**, KHÔNG cần tự lưu refresh token (server đã set cookie).
2. Mọi API cần đăng nhập → thêm header:
   ```
   Authorization: Bearer <accessToken>
   ```
3. **Refresh token nằm trong cookie HttpOnly** → các request `login`, `verify-email`, `refresh`, `logout` PHẢI gửi kèm cookie:
   - `fetch(url, { credentials: 'include', ... })`
   - hoặc axios: `withCredentials: true`
4. **Access token hết hạn (15 phút)** → server trả **401**. Khi gặp 401:
   - Gọi `POST /api/auth/refresh` (kèm cookie) → nhận `accessToken` mới → **gọi lại request cũ**.
   - Nếu refresh cũng 401 → xóa token, chuyển về màn login.

### Mẫu API client (axios) — Cursor nên sinh theo kiểu này
```ts
import axios from 'axios';

const api = axios.create({
  baseURL: 'https://dating-app-backend-q5gk.onrender.com',
  withCredentials: true,           // để gửi/nhận cookie refresh token
  timeout: 60000,
});

let accessToken: string | null = null;
export const setAccessToken = (t: string | null) => { accessToken = t; };

// Gắn Bearer vào mọi request
api.interceptors.request.use((config) => {
  if (accessToken) config.headers.Authorization = `Bearer ${accessToken}`;
  return config;
});

// Tự refresh khi 401 (1 lần)
api.interceptors.response.use(
  (res) => res,
  async (error) => {
    const original = error.config;
    if (error.response?.status === 401 && !original._retried) {
      original._retried = true;
      try {
        const { data } = await api.post('/api/auth/refresh');
        setAccessToken(data.accessToken);
        original.headers.Authorization = `Bearer ${data.accessToken}`;
        return api(original);
      } catch {
        setAccessToken(null);
        // redirect về /login
      }
    }
    return Promise.reject(error);
  }
);
export default api;
```

---

## 2. Định dạng lỗi (server trả ProblemDetails)

```json
{ "title": "...", "status": 400, "detail": "...", "instance": "...", "traceId": "..." }
```
- **400** (validation) → có thêm field `errors: { "Field": ["message"] }`. Hiển thị lỗi theo field.
- **401** → token hết hạn/sai → refresh hoặc login lại (mục 1).
- **403** → **bị chặn quyền** (vd gói Free không được dùng tính năng) → đây là **hành vi đúng**, hiển thị "Nâng cấp gói", KHÔNG coi là bug.
- **404** → không tìm thấy.
- **409** → trùng (vd email đã tồn tại, đã swipe rồi).

---

## 3. Quy ước dữ liệu (Cursor phải đúng)

- **action swipe** viết hoa kiểu PascalCase: `"Like"` | `"Pass"` | `"SuperLike"`.
- **gender / interestedInGender:** `"Male"` | `"Female"` | (preferences còn `"Everyone"`).
- **dateOfBirth:** chuỗi `"YYYY-MM-DD"` (vd `"1999-03-15"`).
- **proposedAt / thời gian:** ISO 8601 UTC `"2026-07-01T18:00:00Z"`.
- **Ảnh (`url` trong response):** là **URL TUYỆT ĐỐI** (Cloudinary). Dùng thẳng làm `<img src>`, **TUYỆT ĐỐI KHÔNG** nối base URL vào trước.

---

## 4. Upload file (2 endpoint đặc biệt — KHÔNG phải JSON)

Dùng `multipart/form-data`, field tên **`file`**:
- `POST /api/profile/photos` — upload ảnh hồ sơ.
- `POST /api/profile/verify-face` — gửi selfie (frontend tự mở camera chụp → tạo file → gửi).

```ts
const form = new FormData();
form.append('file', fileBlob, 'photo.jpg');
await api.post('/api/profile/photos', form); // KHÔNG set Content-Type thủ công, để browser tự set boundary
```

---

## 5. THỨ TỰ GỌI API (luồng user)

> Mọi bước (trừ register/verify-email/login/plans) cần `Authorization: Bearer`.

### Bước A — Đăng nhập (lấy token)
1. `POST /api/auth/login` body `{ email, password }` → lưu `accessToken`.
   *(User mới: `POST /api/auth/register` → `POST /api/auth/verify-email {email,otpCode}` → có token.)*
2. `GET /api/auth/me` → thông tin user hiện tại.

### Bước B — Hoàn thiện hồ sơ (điều kiện vào Discovery)
3. `PUT /api/profile` `{ displayName, gender, dateOfBirth, bio, height, datingGoal }`
4. `PUT /api/profile/location` `{ latitude, longitude }`  ← **bắt buộc**
5. `POST /api/profile/photos` (multipart `file`) → lưu `photoId` từ response
6. `PUT /api/profile/photos/{photoId}/primary`
7. `GET /api/profile/me` → kiểm tra `isProfileCompleted === true`

### Bước C — Tiêu chí & sở thích
8. `GET /api/search/filters` → lấy mảng `interests` (mỗi cái có `id`)
9. `PUT /api/preferences` `{ interestedInGender, minAge, maxAge, maxDistanceKm }`
10. `PUT /api/settings/interests` `{ interestIds: [<id từ bước 8>] }` (tối đa 10)

### Bước D — Khám phá → Swipe → Match  (CẦN 2 USER)
11. `GET /api/discovery?limit=10` → mỗi item có `userId`
12. `POST /api/swipes` `{ targetUserId, action: "Like" }` → `{ isMatch, matchId? }`
    - `isMatch === true` khi đối phương đã like mình từ trước → có `matchId`.
13. (test) đăng nhập user B → `POST /api/swipes { targetUserId: A, action:"Like" }` → tạo match.
14. `GET /api/swipes/liked-me` → ai đã like mình.

### Bước E — Match & Chat  (cần `matchId`)
15. `GET /api/matches` → danh sách match
16. `POST /api/conversations/by-match/{matchId}` → lưu `conversationId`
17. `POST /api/conversations/{conversationId}/messages` `{ content }`
18. `GET /api/conversations/{conversationId}/messages?limit=30`
19. `POST /api/conversations/{conversationId}/read`
20. `GET /api/conversations` → danh sách hội thoại (badge chưa đọc)
21. `POST /api/ai/icebreakers/{matchId}` → gợi ý mở lời
- **Realtime (khuyến nghị):** kết nối SignalR `/hubs/chat?access_token=<accessToken>` để nhận tin tức thì; REST ở trên là fallback.

### Bước F — Tính năng phụ (gọi bất kỳ lúc nào sau đăng nhập)
- **Cây tình yêu** (cần match): `GET /api/plants/{matchId}` → `POST /api/plants/{matchId}/water`
- **Gamification:** `GET /api/tasks`, `GET /api/inventory`
- **Daily:** `GET /api/daily/connection` → `POST /api/daily/complete { questIds:[] }`
- **Connection** (cần conversationId): `GET /api/connection/reminders`, `GET /api/connection/nudges/{conversationId}`, `POST .../dismiss { nudgeId }`, `POST /api/connection/meetup/{conversationId}/propose { venueId, proposedAt, note }`
- **Uy tín:** `GET /api/reputation/me`
- **Thông báo:** `GET /api/notifications`, `POST /api/notifications/read { ids:[] }`
- **An toàn:** `GET/PUT /api/safety/settings`, `POST /api/safety/pin/setup { pin }`, `POST /api/safety/checkin { status:"safe" }`, `PUT /api/safety/emergency { alertMessage, contacts:[] }`
- **Sự kiện:** `GET /api/events` → `POST /api/events/{eventId}/register`
- **Xác minh khuôn mặt:** `POST /api/profile/verify-face` (multipart selfie) → `GET /api/profile/verification`
- **Chặn/Báo cáo:** `POST /api/users/{userId}/block`, `GET /api/blocks`, `POST /api/users/{userId}/report { reason, description }`

### Bước G — Thanh toán
22. `GET /api/plans` → lấy `code` ("Plus" | "Gold")
23. `POST /api/subscription/order` `{ planCode }` → `{ txnRef, paymentUrl }`
24. Redirect trình duyệt sang `paymentUrl` (VNPay). Sau thanh toán VNPay tự gọi về `GET /api/payments/vnpay/return`.
    - **Test nhanh (không qua VNPay):** `POST /api/subscription/mock-confirm/{txnRef}`.
25. `GET /api/subscription/me` → xác nhận đã lên gói.

### Bước H — Đổi mật khẩu & đăng xuất (LÀM CUỐI)
- `PUT /api/settings/password` `{ currentPassword, newPassword }` → **thu hồi mọi session** → phải login lại.
- `POST /api/auth/logout`.

---

## 6. Checklist cho Cursor (đừng làm sai)
- [ ] Gắn `Authorization: Bearer <accessToken>` cho mọi call cần auth.
- [ ] `withCredentials: true` / `credentials:'include'` cho login, verify-email, refresh, logout.
- [ ] Gặp 401 → refresh 1 lần rồi thử lại; thất bại → login.
- [ ] Upload ảnh & verify-face: `multipart/form-data`, field `file`, KHÔNG tự set Content-Type.
- [ ] `url` ảnh là tuyệt đối → dùng thẳng, không nối base.
- [ ] swipe action: `"Like"/"Pass"/"SuperLike"`; dateOfBirth `"YYYY-MM-DD"`.
- [ ] 403 = tính năng bị khóa theo gói → hiện thông báo nâng cấp, không phải lỗi hệ thống.
- [ ] Match/Chat cần **2 user** đã hoàn thiện hồ sơ.
- [ ] Chat realtime qua SignalR `/hubs/chat?access_token=`.

## 7. Tài khoản test sẵn (mật khẩu: `Test@12345`)
- `alice@samemess.test` (nữ) · `bob@samemess.test` (nam) → dùng cặp này test match/chat.
- `testuser1..20@samemess.test`.

---

## 8. PHỤ LỤC — Danh sách ĐẦY ĐỦ 80 endpoint user theo thứ tự gọi
(Mọi call cần `Authorization: Bearer` trừ register/verify-email/login/plans.)

```
AUTH
1.  POST   /api/auth/register
2.  POST   /api/auth/verify-email        → accessToken
3.  POST   /api/auth/login               → accessToken
4.  GET    /api/auth/me

PROFILE
5.  PUT    /api/profile
6.  PUT    /api/profile/location         (bắt buộc cho discovery)
7.  POST   /api/profile/photos           (multipart 'file') → photoId
8.  PUT    /api/profile/photos/{photoId}/primary
9.  PUT    /api/profile/photos/order
10. GET    /api/profile/me
11. POST   /api/profile/boost
12. DELETE /api/profile/photos/{photoId}

SETTINGS / PREFERENCES
13. GET    /api/preferences
14. PUT    /api/preferences
15. GET    /api/search/filters           → interestId
16. GET    /api/settings/interests
17. PUT    /api/settings/interests
18. GET    /api/settings/discovery
19. PUT    /api/settings/discovery
20. GET    /api/settings/security
21. PUT    /api/settings/security
22. GET    /api/settings/devices

SEARCH / DISCOVERY
23. GET    /api/search/results
24. GET    /api/discovery                → userId mục tiêu

SWIPE / MATCH (cần 2 user)
25. POST   /api/swipes                    {targetUserId, action:"Like"}
26. GET    /api/swipes/liked-me
27. GET    /api/swipes/superliked-me
28. POST   /api/swipes/undo               (Plus; Free → 403)
29. GET    /api/matches                   → matchId
30. DELETE /api/matches/{matchId}

CHAT
31. POST   /api/conversations/by-match/{matchId}  → conversationId
32. GET    /api/conversations
33. POST   /api/conversations/{conversationId}/messages   {content}
34. GET    /api/conversations/{conversationId}/messages
35. POST   /api/conversations/{conversationId}/read
36. POST   /api/ai/icebreakers/{matchId}

CÂY TÌNH YÊU / GAMIFICATION (cần match)
37. GET    /api/plants/{matchId}
38. POST   /api/plants/{matchId}/water
39. GET    /api/tasks
40. GET    /api/inventory

DAILY / CONNECTION
41. GET    /api/daily/connection
42. POST   /api/daily/complete            {questIds:[]}
43. GET    /api/connection/reminders
44. GET    /api/connection/nudges/{conversationId}
45. POST   /api/connection/nudges/{conversationId}/dismiss   {nudgeId}
46. POST   /api/connection/meetup/{conversationId}/propose   {venueId,proposedAt,note}

UY TÍN / THÔNG BÁO
47. GET    /api/reputation/me
48. GET    /api/notifications
49. POST   /api/notifications/read        {ids:[]}
50. GET    /api/notifications/vapid-public-key
51. POST   /api/notifications/subscribe
52. POST   /api/notifications/unsubscribe

AN TOÀN / PIN
53. GET    /api/safety/settings
54. PUT    /api/safety/settings
55. POST   /api/safety/pin/setup          {pin}
56. PUT    /api/safety/emergency          {alertMessage, contacts:[]}
57. GET    /api/safety/emergency
58. POST   /api/safety/checkin            {status:"safe"}
59. POST   /api/safety/pin/forgot         {channel:"email"}
60. POST   /api/safety/pin/verify-otp     {otp}

SỰ KIỆN (admin publish trước)
61. GET    /api/events                    → eventId
62. GET    /api/events/{eventId}
63. POST   /api/events/{eventId}/register
64. GET    /api/events/history
65. GET    /api/events/reward?eventId=

XÁC MINH KHUÔN MẶT (cần ảnh primary)
66. POST   /api/profile/verify-face       (multipart selfie)
67. GET    /api/profile/verification

THANH TOÁN
68. GET    /api/plans                     → planCode
69. GET    /api/subscription/me
70. POST   /api/subscription/order        {planCode} → txnRef, paymentUrl
71. POST   /api/subscription/mock-confirm/{txnRef}
72. GET    /api/payments/vnpay/return     (VNPay redirect về; frontend KHÔNG tự gọi)
    (GET /api/payments/vnpay/ipn = server→server, frontend KHÔNG gọi)

CHẶN / BÁO CÁO (cần user khác)
73. POST   /api/users/{userId}/report     {reason, description}
74. POST   /api/users/{userId}/block
75. GET    /api/blocks
76. DELETE /api/users/{userId}/block

CUỐI CÙNG
77. PUT    /api/settings/password         {currentPassword,newPassword}  (logout hết)
78. POST   /api/auth/refresh
79. POST   /api/auth/logout
```
