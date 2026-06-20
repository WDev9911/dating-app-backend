-- ============================================================================
-- SEED: Quán hẹn hò + Combo ưu đãi (Date Pass)
-- Chạy trên Neon SAU KHI đã deploy code mới (migration AddVenues + AddDatePass
-- tự chạy lúc khởi động → bảng chat."Venues" & chat."VenueCombos" đã tồn tại).
-- Toạ độ quanh trung tâm TP.HCM, gần seed users (10.7769, 106.7009) để hiện trong gợi ý.
-- Chạy lại được: xoá theo Id cố định rồi tạo lại (cascade xoá combo kèm theo).
-- ============================================================================

BEGIN;

DELETE FROM chat."Venues" WHERE "Id" IN (
  'aaaa1111-0000-0000-0000-000000000001',
  'aaaa1111-0000-0000-0000-000000000002',
  'aaaa1111-0000-0000-0000-000000000003',
  'aaaa1111-0000-0000-0000-000000000004',
  'aaaa1111-0000-0000-0000-000000000005',
  'aaaa1111-0000-0000-0000-000000000006'
);

-- 1) QUÁN
INSERT INTO chat."Venues"
  ("Id","Name","Category","Address","District","City","Latitude","Longitude","ImageUrl","PriceRange","Description","IsActive","CreatedAt")
VALUES
  ('aaaa1111-0000-0000-0000-000000000001','The Cozy Corner Café','cafe','12 Lê Lợi','Quận 1','TP.HCM',10.7740,106.7020,'https://picsum.photos/seed/cozycafe/600/400','$$','Quán cà phê yên tĩnh, không gian hẹn hò ấm cúng.',true, now() at time zone 'utc'),
  ('aaaa1111-0000-0000-0000-000000000002','Sài Gòn Bistro','restaurant','45 Nguyễn Huệ','Quận 1','TP.HCM',10.7745,106.7035,'https://picsum.photos/seed/bistro/600/400','$$$','Nhà hàng phong cách Âu, lý tưởng cho bữa tối lãng mạn.',true, now() at time zone 'utc'),
  ('aaaa1111-0000-0000-0000-000000000003','Sweet Moments Dessert','dessert','88 Hai Bà Trưng','Quận 3','TP.HCM',10.7790,106.6960,'https://picsum.photos/seed/dessert/600/400','$','Tiệm tráng miệng & trà sữa dễ thương.',true, now() at time zone 'utc'),
  ('aaaa1111-0000-0000-0000-000000000004','Skyline Rooftop Bar','bar','200 Lê Thánh Tôn','Quận 1','TP.HCM',10.7760,106.7000,'https://picsum.photos/seed/rooftop/600/400','$$$','Bar tầng thượng, view thành phố về đêm.',true, now() at time zone 'utc'),
  ('aaaa1111-0000-0000-0000-000000000005','CGV Vincom','cinema','72 Lê Thánh Tôn','Quận 1','TP.HCM',10.7775,106.7015,'https://picsum.photos/seed/cinema/600/400','$$','Rạp chiếu phim — combo xem phim cho cặp đôi.',true, now() at time zone 'utc'),
  ('aaaa1111-0000-0000-0000-000000000006','Tao Đàn Park Café','park','Công viên Tao Đàn','Quận 1','TP.HCM',10.7725,106.6945,'https://picsum.photos/seed/parkcafe/600/400','$','Cà phê sân vườn trong công viên, thoáng mát.',true, now() at time zone 'utc');

-- 2) COMBO ƯU ĐÃI (mỗi quán nhiều combo khác nhau)
INSERT INTO chat."VenueCombos"
  ("VenueId","Title","Description","OriginalPriceVnd","SalePriceVnd","CommissionPercent","IsActive","CreatedAt")
VALUES
  -- The Cozy Corner Café
  ('aaaa1111-0000-0000-0000-000000000001','Combo Cà Phê Đôi','2 ly cà phê signature + 1 bánh ngọt',150000,99000,15,true, now() at time zone 'utc'),
  ('aaaa1111-0000-0000-0000-000000000001','Combo Trà Chiều','2 trà trái cây + 2 bánh tart',180000,129000,15,true, now() at time zone 'utc'),

  -- Sài Gòn Bistro
  ('aaaa1111-0000-0000-0000-000000000002','Set Bữa Tối Lãng Mạn','2 món chính + khai vị + tráng miệng + nến',650000,499000,20,true, now() at time zone 'utc'),
  ('aaaa1111-0000-0000-0000-000000000002','Combo Pasta Đôi','2 pasta + 2 nước ép tươi',350000,259000,18,true, now() at time zone 'utc'),

  -- Sweet Moments Dessert
  ('aaaa1111-0000-0000-0000-000000000003','Combo Trà Sữa Đôi','2 trà sữa topping + 1 bánh flan',120000,79000,15,true, now() at time zone 'utc'),
  ('aaaa1111-0000-0000-0000-000000000003','Combo Kem Ngọt Ngào','2 ly kem ý + 1 waffle',140000,99000,15,true, now() at time zone 'utc'),

  -- Skyline Rooftop Bar
  ('aaaa1111-0000-0000-0000-000000000004','Combo Mocktail Hoàng Hôn','2 mocktail + snack view thành phố',300000,229000,20,true, now() at time zone 'utc'),

  -- CGV Vincom
  ('aaaa1111-0000-0000-0000-000000000005','Combo Xem Phim Cặp Đôi','2 vé phim + 1 bắp lớn + 2 nước',280000,199000,18,true, now() at time zone 'utc'),

  -- Tao Đàn Park Café
  ('aaaa1111-0000-0000-0000-000000000006','Combo Picnic Mini','2 cà phê + 1 set bánh picnic',130000,89000,15,true, now() at time zone 'utc');

COMMIT;
