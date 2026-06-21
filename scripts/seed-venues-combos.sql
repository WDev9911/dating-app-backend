-- ============================================================================
-- SEED: Quán hẹn hò + Combo ưu đãi (Date Pass) — THƯƠNG HIỆU NỔI TIẾNG
-- Xoá sạch quán/voucher cũ rồi tạo lại bằng các brand thật (Starbucks, Pizza 4P's...).
-- Ảnh = logo brand local trong frontend public/assets/brands/*.png → card hiển thị logo căn giữa nền trắng.
-- Toạ độ quanh trung tâm TP.HCM (gần seed users 10.7769, 106.7009).
-- Chạy lại được nhiều lần.
-- ============================================================================

BEGIN;

-- Xoá voucher đã đặt trước (tránh chặn khoá ngoại), rồi xoá toàn bộ combo + quán cũ
DELETE FROM billing."DatePassOrders";
DELETE FROM chat."VenueCombos";
DELETE FROM chat."Venues";

-- 1) QUÁN — thương hiệu nổi tiếng
INSERT INTO chat."Venues"
  ("Id","Name","Category","Address","District","City","Latitude","Longitude","ImageUrl","PriceRange","Description","IsActive","CreatedAt")
VALUES
  ('bbbb2222-0000-0000-0000-000000000001','Starbucks Coffee','cafe','Rex Hotel, 141 Nguyễn Huệ','Quận 1','TP.HCM',10.7745,106.7030,'/assets/brands/starbucks.png','$$$','Cà phê & Frappuccino chuẩn quốc tế, không gian sang trọng.',true, now() at time zone 'utc'),
  ('bbbb2222-0000-0000-0000-000000000002','Highlands Coffee','cafe','72 Lê Thánh Tôn','Quận 1','TP.HCM',10.7775,106.7012,'/assets/brands/highlands.png','$$','Phin sữa đá & Freeze trứ danh, quán phủ khắp Việt Nam.',true, now() at time zone 'utc'),
  ('bbbb2222-0000-0000-0000-000000000003','The Coffee House','cafe','86-88 Cao Thắng','Quận 3','TP.HCM',10.7720,106.6820,'/assets/brands/coffeehouse.png','$$','Không gian trẻ trung, trà đào cam sả best-seller.',true, now() at time zone 'utc'),
  ('bbbb2222-0000-0000-0000-000000000004','Katinat Saigon Kafe','cafe','91 Đồng Khởi','Quận 1','TP.HCM',10.7765,106.7040,'/assets/brands/katinat.png','$$','Cà phê sữa đá & trà sữa hot trend giới trẻ Sài Gòn.',true, now() at time zone 'utc'),
  ('bbbb2222-0000-0000-0000-000000000005','Phúc Long Coffee & Tea','cafe','Vincom Đồng Khởi','Quận 1','TP.HCM',10.7780,106.7025,'/assets/brands/phuclong.png','$$','Trà & cà phê đậm vị, thương hiệu Việt lâu đời.',true, now() at time zone 'utc'),
  ('bbbb2222-0000-0000-0000-000000000006','Gong Cha','dessert','35 Nguyễn Trãi','Quận 1','TP.HCM',10.7690,106.6920,'/assets/brands/gongcha.png','$','Trà sữa trân châu hoàng kim chuẩn Đài Loan.',true, now() at time zone 'utc'),
  ('bbbb2222-0000-0000-0000-000000000007','Pizza 4P''s','restaurant','8 Thủ Khoa Huân','Quận 1','TP.HCM',10.7730,106.6985,'/assets/brands/pizza4ps.png','$$$','Pizza phô mai burrata nhà làm — điểm hẹn hò được yêu thích.',true, now() at time zone 'utc'),
  ('bbbb2222-0000-0000-0000-000000000008','Haidilao Hotpot','restaurant','Vincom Lê Thánh Tôn','Quận 1','TP.HCM',10.7778,106.7018,'/assets/brands/haidilao.png','$$$','Lẩu Tứ Xuyên, phục vụ 5 sao — ấm cúng cho cặp đôi.',true, now() at time zone 'utc'),
  ('bbbb2222-0000-0000-0000-000000000009','KFC','restaurant','Saigon Centre, 65 Lê Lợi','Quận 1','TP.HCM',10.7738,106.7008,'/assets/brands/kfc.png','$','Gà rán giòn cay trứ danh — combo cặp đôi tiện lợi.',true, now() at time zone 'utc'),
  ('bbbb2222-0000-0000-0000-000000000010','CGV Cinemas','cinema','Vincom Đồng Khởi','Quận 1','TP.HCM',10.7782,106.7026,'/assets/brands/cgv.png','$$','Rạp chiếu phim hiện đại — ghế đôi Sweetbox cho cặp đôi.',true, now() at time zone 'utc');

-- 2) COMBO ƯU ĐÃI
INSERT INTO chat."VenueCombos"
  ("VenueId","Title","Description","OriginalPriceVnd","SalePriceVnd","CommissionPercent","IsActive","CreatedAt")
VALUES
  -- Starbucks
  ('bbbb2222-0000-0000-0000-000000000001','Combo Frappuccino Đôi','2 Frappuccino size Tall + 1 bánh ngọt',230000,179000,15,true, now() at time zone 'utc'),
  ('bbbb2222-0000-0000-0000-000000000001','Combo Trà Chiều Starbucks','2 Tea Latte + 2 cookie bơ',200000,149000,15,true, now() at time zone 'utc'),
  -- Highlands
  ('bbbb2222-0000-0000-0000-000000000002','Combo Phin Sữa Đá Đôi','2 phin sữa đá + 2 bánh mì que',120000,89000,15,true, now() at time zone 'utc'),
  ('bbbb2222-0000-0000-0000-000000000002','Combo Freeze Trà Xanh','2 Freeze trà xanh size L',130000,99000,15,true, now() at time zone 'utc'),
  -- The Coffee House
  ('bbbb2222-0000-0000-0000-000000000003','Combo Trà Đào Đôi','2 trà đào cam sả + 2 bánh mousse',130000,95000,15,true, now() at time zone 'utc'),
  -- Katinat
  ('bbbb2222-0000-0000-0000-000000000004','Combo Cà Phê Sữa Đá Đôi','2 cà phê sữa đá + 2 bánh croissant',110000,85000,15,true, now() at time zone 'utc'),
  -- Phúc Long
  ('bbbb2222-0000-0000-0000-000000000005','Combo Trà Sữa Phúc Long Đôi','2 trà sữa Phúc Long + 2 bánh',120000,89000,15,true, now() at time zone 'utc'),
  -- Gong Cha
  ('bbbb2222-0000-0000-0000-000000000006','Combo Trân Châu Hoàng Kim Đôi','2 trà sữa size L full topping',110000,79000,15,true, now() at time zone 'utc'),
  -- Pizza 4P's
  ('bbbb2222-0000-0000-0000-000000000007','Set Pizza Đôi Lãng Mạn','1 pizza nửa-nửa + 2 salad + 2 nước',450000,359000,20,true, now() at time zone 'utc'),
  ('bbbb2222-0000-0000-0000-000000000007','Combo Pasta & Burrata','2 pasta + 1 phô mai burrata nhà làm',520000,399000,20,true, now() at time zone 'utc'),
  -- Haidilao
  ('bbbb2222-0000-0000-0000-000000000008','Combo Lẩu Cặp Đôi','Lẩu 2 ngăn + 2 set thịt + rau & đồ nhúng',700000,549000,20,true, now() at time zone 'utc'),
  -- KFC
  ('bbbb2222-0000-0000-0000-000000000009','Combo Gà Rán Đôi','6 miếng gà rán + 2 burger + 2 nước ngọt',250000,189000,18,true, now() at time zone 'utc'),
  -- CGV
  ('bbbb2222-0000-0000-0000-000000000010','Combo Xem Phim Cặp Đôi','2 vé phim 2D + 1 bắp lớn + 2 nước',320000,229000,18,true, now() at time zone 'utc'),
  ('bbbb2222-0000-0000-0000-000000000010','Combo Sweetbox Couple','2 ghế đôi Sweetbox + 1 bắp + 2 nước',400000,299000,20,true, now() at time zone 'utc');

COMMIT;
