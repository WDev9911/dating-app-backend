-- SameMess test seed: 23 users (mat khau: Test@12345). Chay duoc tren Neon ke ca khi chua deploy migration moi.
INSERT INTO auth."Users"
  ("Id","Email","PasswordHash","IsEmailVerified","IsPhoneVerified","Role","Status","CreatedAt")
VALUES
  ('c0000000-0000-0000-0000-000000000001','admin@samemess.test','$2a$11$4Tl5Th7HaI7d30N/7g5G0O1ilc.7EAwcX22pGSTTl6ikr98PkMMgi',true,false,'Admin','Active',now() at time zone 'utc'),
  ('c0000000-0000-0000-0000-000000000002','alice@samemess.test','$2a$11$4Tl5Th7HaI7d30N/7g5G0O1ilc.7EAwcX22pGSTTl6ikr98PkMMgi',true,false,'User','Active',now() at time zone 'utc'),
  ('c0000000-0000-0000-0000-000000000003','bob@samemess.test','$2a$11$4Tl5Th7HaI7d30N/7g5G0O1ilc.7EAwcX22pGSTTl6ikr98PkMMgi',true,false,'User','Active',now() at time zone 'utc'),
  ('e0000000-0000-0000-0000-000000000001','testuser1@samemess.test','$2a$11$4Tl5Th7HaI7d30N/7g5G0O1ilc.7EAwcX22pGSTTl6ikr98PkMMgi',true,false,'User','Active',now() at time zone 'utc'),
  ('e0000000-0000-0000-0000-000000000002','testuser2@samemess.test','$2a$11$4Tl5Th7HaI7d30N/7g5G0O1ilc.7EAwcX22pGSTTl6ikr98PkMMgi',true,false,'User','Active',now() at time zone 'utc'),
  ('e0000000-0000-0000-0000-000000000003','testuser3@samemess.test','$2a$11$4Tl5Th7HaI7d30N/7g5G0O1ilc.7EAwcX22pGSTTl6ikr98PkMMgi',true,false,'User','Active',now() at time zone 'utc'),
  ('e0000000-0000-0000-0000-000000000004','testuser4@samemess.test','$2a$11$4Tl5Th7HaI7d30N/7g5G0O1ilc.7EAwcX22pGSTTl6ikr98PkMMgi',true,false,'User','Active',now() at time zone 'utc'),
  ('e0000000-0000-0000-0000-000000000005','testuser5@samemess.test','$2a$11$4Tl5Th7HaI7d30N/7g5G0O1ilc.7EAwcX22pGSTTl6ikr98PkMMgi',true,false,'User','Active',now() at time zone 'utc'),
  ('e0000000-0000-0000-0000-000000000006','testuser6@samemess.test','$2a$11$4Tl5Th7HaI7d30N/7g5G0O1ilc.7EAwcX22pGSTTl6ikr98PkMMgi',true,false,'User','Active',now() at time zone 'utc'),
  ('e0000000-0000-0000-0000-000000000007','testuser7@samemess.test','$2a$11$4Tl5Th7HaI7d30N/7g5G0O1ilc.7EAwcX22pGSTTl6ikr98PkMMgi',true,false,'User','Active',now() at time zone 'utc'),
  ('e0000000-0000-0000-0000-000000000008','testuser8@samemess.test','$2a$11$4Tl5Th7HaI7d30N/7g5G0O1ilc.7EAwcX22pGSTTl6ikr98PkMMgi',true,false,'User','Active',now() at time zone 'utc'),
  ('e0000000-0000-0000-0000-000000000009','testuser9@samemess.test','$2a$11$4Tl5Th7HaI7d30N/7g5G0O1ilc.7EAwcX22pGSTTl6ikr98PkMMgi',true,false,'User','Active',now() at time zone 'utc'),
  ('e0000000-0000-0000-0000-00000000000a','testuser10@samemess.test','$2a$11$4Tl5Th7HaI7d30N/7g5G0O1ilc.7EAwcX22pGSTTl6ikr98PkMMgi',true,false,'User','Active',now() at time zone 'utc'),
  ('e0000000-0000-0000-0000-00000000000b','testuser11@samemess.test','$2a$11$4Tl5Th7HaI7d30N/7g5G0O1ilc.7EAwcX22pGSTTl6ikr98PkMMgi',true,false,'User','Active',now() at time zone 'utc'),
  ('e0000000-0000-0000-0000-00000000000c','testuser12@samemess.test','$2a$11$4Tl5Th7HaI7d30N/7g5G0O1ilc.7EAwcX22pGSTTl6ikr98PkMMgi',true,false,'User','Active',now() at time zone 'utc'),
  ('e0000000-0000-0000-0000-00000000000d','testuser13@samemess.test','$2a$11$4Tl5Th7HaI7d30N/7g5G0O1ilc.7EAwcX22pGSTTl6ikr98PkMMgi',true,false,'User','Active',now() at time zone 'utc'),
  ('e0000000-0000-0000-0000-00000000000e','testuser14@samemess.test','$2a$11$4Tl5Th7HaI7d30N/7g5G0O1ilc.7EAwcX22pGSTTl6ikr98PkMMgi',true,false,'User','Active',now() at time zone 'utc'),
  ('e0000000-0000-0000-0000-00000000000f','testuser15@samemess.test','$2a$11$4Tl5Th7HaI7d30N/7g5G0O1ilc.7EAwcX22pGSTTl6ikr98PkMMgi',true,false,'User','Active',now() at time zone 'utc'),
  ('e0000000-0000-0000-0000-000000000010','testuser16@samemess.test','$2a$11$4Tl5Th7HaI7d30N/7g5G0O1ilc.7EAwcX22pGSTTl6ikr98PkMMgi',true,false,'User','Active',now() at time zone 'utc'),
  ('e0000000-0000-0000-0000-000000000011','testuser17@samemess.test','$2a$11$4Tl5Th7HaI7d30N/7g5G0O1ilc.7EAwcX22pGSTTl6ikr98PkMMgi',true,false,'User','Active',now() at time zone 'utc'),
  ('e0000000-0000-0000-0000-000000000012','testuser18@samemess.test','$2a$11$4Tl5Th7HaI7d30N/7g5G0O1ilc.7EAwcX22pGSTTl6ikr98PkMMgi',true,false,'User','Active',now() at time zone 'utc'),
  ('e0000000-0000-0000-0000-000000000013','testuser19@samemess.test','$2a$11$4Tl5Th7HaI7d30N/7g5G0O1ilc.7EAwcX22pGSTTl6ikr98PkMMgi',true,false,'User','Active',now() at time zone 'utc'),
  ('e0000000-0000-0000-0000-000000000014','testuser20@samemess.test','$2a$11$4Tl5Th7HaI7d30N/7g5G0O1ilc.7EAwcX22pGSTTl6ikr98PkMMgi',true,false,'User','Active',now() at time zone 'utc')
ON CONFLICT ("Id") DO NOTHING;

INSERT INTO auth."UserProfiles"
  ("Id","UserId","DisplayName","Gender","DateOfBirth","Location","Latitude","Longitude","IsProfileCompleted","IsPhotoVerified","VerificationStatus","CreatedAt")
VALUES
  ('d0000000-0000-0000-0000-000000000001','c0000000-0000-0000-0000-000000000001','Admin Tester','Male','1995-01-01','Hà Nội',21.0285,105.8542,true,false,'None',now() at time zone 'utc'),
  ('d0000000-0000-0000-0000-000000000002','c0000000-0000-0000-0000-000000000002','Alice','Female','1999-03-15','Hà Nội',21.03,105.85,true,false,'None',now() at time zone 'utc'),
  ('d0000000-0000-0000-0000-000000000003','c0000000-0000-0000-0000-000000000003','Bob','Male','1997-07-20','Hà Nội',21.025,105.86,true,false,'None',now() at time zone 'utc'),
  ('f0000000-0000-0000-0000-000000000001','e0000000-0000-0000-0000-000000000001','Linh','Female','2006-06-15','Hà Nội',21,105.83,true,false,'None',now() at time zone 'utc'),
  ('f0000000-0000-0000-0000-000000000002','e0000000-0000-0000-0000-000000000002','Mai','Female','2005-06-15','Hà Nội',21.005,105.834,true,false,'None',now() at time zone 'utc'),
  ('f0000000-0000-0000-0000-000000000003','e0000000-0000-0000-0000-000000000003','Hương','Female','2004-06-15','Hà Nội',21.01,105.838,true,false,'None',now() at time zone 'utc'),
  ('f0000000-0000-0000-0000-000000000004','e0000000-0000-0000-0000-000000000004','Trang','Female','2003-06-15','Hà Nội',21.015,105.842,true,false,'None',now() at time zone 'utc'),
  ('f0000000-0000-0000-0000-000000000005','e0000000-0000-0000-0000-000000000005','Ngọc','Female','2002-06-15','Hà Nội',21.02,105.846,true,false,'None',now() at time zone 'utc'),
  ('f0000000-0000-0000-0000-000000000006','e0000000-0000-0000-0000-000000000006','Thảo','Female','2001-06-15','Hà Nội',21.025,105.85,true,false,'None',now() at time zone 'utc'),
  ('f0000000-0000-0000-0000-000000000007','e0000000-0000-0000-0000-000000000007','Vy','Female','2000-06-15','Hà Nội',21.03,105.854,true,false,'None',now() at time zone 'utc'),
  ('f0000000-0000-0000-0000-000000000008','e0000000-0000-0000-0000-000000000008','Hà','Female','1999-06-15','Hà Nội',21.035,105.858,true,false,'None',now() at time zone 'utc'),
  ('f0000000-0000-0000-0000-000000000009','e0000000-0000-0000-0000-000000000009','Quỳnh','Female','1998-06-15','Hà Nội',21.04,105.862,true,false,'None',now() at time zone 'utc'),
  ('f0000000-0000-0000-0000-00000000000a','e0000000-0000-0000-0000-00000000000a','Lan','Female','1997-06-15','Hà Nội',21.045,105.866,true,false,'None',now() at time zone 'utc'),
  ('f0000000-0000-0000-0000-00000000000b','e0000000-0000-0000-0000-00000000000b','Minh','Male','1996-06-15','Hà Nội',21.05,105.87,true,false,'None',now() at time zone 'utc'),
  ('f0000000-0000-0000-0000-00000000000c','e0000000-0000-0000-0000-00000000000c','Tuấn','Male','1995-06-15','Hà Nội',21.055,105.874,true,false,'None',now() at time zone 'utc'),
  ('f0000000-0000-0000-0000-00000000000d','e0000000-0000-0000-0000-00000000000d','Nam','Male','1994-06-15','Hà Nội',21.06,105.878,true,false,'None',now() at time zone 'utc'),
  ('f0000000-0000-0000-0000-00000000000e','e0000000-0000-0000-0000-00000000000e','Khoa','Male','1993-06-15','Hà Nội',21.065,105.882,true,false,'None',now() at time zone 'utc'),
  ('f0000000-0000-0000-0000-00000000000f','e0000000-0000-0000-0000-00000000000f','Phúc','Male','1992-06-15','Hà Nội',21.07,105.886,true,false,'None',now() at time zone 'utc'),
  ('f0000000-0000-0000-0000-000000000010','e0000000-0000-0000-0000-000000000010','Long','Male','2006-06-15','Hà Nội',21.075,105.89,true,false,'None',now() at time zone 'utc'),
  ('f0000000-0000-0000-0000-000000000011','e0000000-0000-0000-0000-000000000011','Sơn','Male','2005-06-15','Hà Nội',21.08,105.894,true,false,'None',now() at time zone 'utc'),
  ('f0000000-0000-0000-0000-000000000012','e0000000-0000-0000-0000-000000000012','Hùng','Male','2004-06-15','Hà Nội',21.085,105.898,true,false,'None',now() at time zone 'utc'),
  ('f0000000-0000-0000-0000-000000000013','e0000000-0000-0000-0000-000000000013','Dũng','Male','2003-06-15','Hà Nội',21.09,105.902,true,false,'None',now() at time zone 'utc'),
  ('f0000000-0000-0000-0000-000000000014','e0000000-0000-0000-0000-000000000014','Bình','Male','2002-06-15','Hà Nội',21.095,105.906,true,false,'None',now() at time zone 'utc')
ON CONFLICT ("Id") DO NOTHING;

INSERT INTO auth."Photos"
  ("Id","UserId","Url","OrderIndex","IsPrimary","CreatedAt")
VALUES
  ('a1110000-0000-0000-0000-000000000001','c0000000-0000-0000-0000-000000000001','https://randomuser.me/api/portraits/men/0.jpg',0,true,now() at time zone 'utc'),
  ('a1110000-0000-0000-0000-000000000002','c0000000-0000-0000-0000-000000000002','https://randomuser.me/api/portraits/women/0.jpg',0,true,now() at time zone 'utc'),
  ('a1110000-0000-0000-0000-000000000003','c0000000-0000-0000-0000-000000000003','https://randomuser.me/api/portraits/men/1.jpg',0,true,now() at time zone 'utc'),
  ('a1110000-0000-0000-0000-000000100001','e0000000-0000-0000-0000-000000000001','https://randomuser.me/api/portraits/women/2.jpg',0,true,now() at time zone 'utc'),
  ('a1110000-0000-0000-0000-000000100002','e0000000-0000-0000-0000-000000000002','https://randomuser.me/api/portraits/women/3.jpg',0,true,now() at time zone 'utc'),
  ('a1110000-0000-0000-0000-000000100003','e0000000-0000-0000-0000-000000000003','https://randomuser.me/api/portraits/women/4.jpg',0,true,now() at time zone 'utc'),
  ('a1110000-0000-0000-0000-000000100004','e0000000-0000-0000-0000-000000000004','https://randomuser.me/api/portraits/women/5.jpg',0,true,now() at time zone 'utc'),
  ('a1110000-0000-0000-0000-000000100005','e0000000-0000-0000-0000-000000000005','https://randomuser.me/api/portraits/women/6.jpg',0,true,now() at time zone 'utc'),
  ('a1110000-0000-0000-0000-000000100006','e0000000-0000-0000-0000-000000000006','https://randomuser.me/api/portraits/women/7.jpg',0,true,now() at time zone 'utc'),
  ('a1110000-0000-0000-0000-000000100007','e0000000-0000-0000-0000-000000000007','https://randomuser.me/api/portraits/women/8.jpg',0,true,now() at time zone 'utc'),
  ('a1110000-0000-0000-0000-000000100008','e0000000-0000-0000-0000-000000000008','https://randomuser.me/api/portraits/women/9.jpg',0,true,now() at time zone 'utc'),
  ('a1110000-0000-0000-0000-000000100009','e0000000-0000-0000-0000-000000000009','https://randomuser.me/api/portraits/women/10.jpg',0,true,now() at time zone 'utc'),
  ('a1110000-0000-0000-0000-00000010000a','e0000000-0000-0000-0000-00000000000a','https://randomuser.me/api/portraits/women/11.jpg',0,true,now() at time zone 'utc'),
  ('a1110000-0000-0000-0000-00000010000b','e0000000-0000-0000-0000-00000000000b','https://randomuser.me/api/portraits/men/12.jpg',0,true,now() at time zone 'utc'),
  ('a1110000-0000-0000-0000-00000010000c','e0000000-0000-0000-0000-00000000000c','https://randomuser.me/api/portraits/men/13.jpg',0,true,now() at time zone 'utc'),
  ('a1110000-0000-0000-0000-00000010000d','e0000000-0000-0000-0000-00000000000d','https://randomuser.me/api/portraits/men/14.jpg',0,true,now() at time zone 'utc'),
  ('a1110000-0000-0000-0000-00000010000e','e0000000-0000-0000-0000-00000000000e','https://randomuser.me/api/portraits/men/15.jpg',0,true,now() at time zone 'utc'),
  ('a1110000-0000-0000-0000-00000010000f','e0000000-0000-0000-0000-00000000000f','https://randomuser.me/api/portraits/men/16.jpg',0,true,now() at time zone 'utc'),
  ('a1110000-0000-0000-0000-000000100010','e0000000-0000-0000-0000-000000000010','https://randomuser.me/api/portraits/men/17.jpg',0,true,now() at time zone 'utc'),
  ('a1110000-0000-0000-0000-000000100011','e0000000-0000-0000-0000-000000000011','https://randomuser.me/api/portraits/men/18.jpg',0,true,now() at time zone 'utc'),
  ('a1110000-0000-0000-0000-000000100012','e0000000-0000-0000-0000-000000000012','https://randomuser.me/api/portraits/men/19.jpg',0,true,now() at time zone 'utc'),
  ('a1110000-0000-0000-0000-000000100013','e0000000-0000-0000-0000-000000000013','https://randomuser.me/api/portraits/men/20.jpg',0,true,now() at time zone 'utc'),
  ('a1110000-0000-0000-0000-000000100014','e0000000-0000-0000-0000-000000000014','https://randomuser.me/api/portraits/men/21.jpg',0,true,now() at time zone 'utc')
ON CONFLICT ("Id") DO NOTHING;
