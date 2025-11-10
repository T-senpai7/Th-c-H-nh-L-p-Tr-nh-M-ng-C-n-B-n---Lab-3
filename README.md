## Lab 3 LTMCB — Tổng hợp Bài 1 → Bài 6

Dự án gồm 6 bài thực hành về lập trình mạng trên Windows Forms (.NET), bao gồm UDP, TCP (single/multi client), đồng bộ dữ liệu theo mô hình 1 Server - nhiều Client, thao tác cơ sở dữ liệu SQLite, và phòng chat thời gian thực.

### Yêu cầu môi trường
- **.NET SDK**: .NET 8.0 (ưu tiên) hoặc .NET 9.0 for Windows
- **OS**: Windows 10+
- IDE khuyến nghị: Visual Studio 2022 (Windows) hoặc chạy `dotnet run` trong từng thư mục bài
- Nếu chạy qua mạng LAN: mở port tương ứng trên Windows Firewall

### Cấu trúc thư mục chính
- `Bai1/` — UDP Client/Server đơn giản
- `Bai2/` — TCP Server đọc luồng ký tự kết thúc bằng newline
- `Bai3/` — TCP Client/Server (Form: Dashboard, TCP Client, TCP Server)
- `Bai4/` — Quản lý phòng vé rạp phim (1 Server - Multi Client, SQLite, đồng bộ)
- `Bai5/` — “Hôm nay ăn gì?” (1 Server - Multi Client, SQLite, hình ảnh)
- `Bai6/` — Chat room TCP (multi-client, nhắn tin chung/riêng, gửi file)
- `Lab3_LTMCB.sln` — Solution tổng

---

### Bài 1 — UDP Client/Server
- Thư mục: `Bai1/`
- Entry: `Program.cs` chạy `Menu` để mở `UServer` (UDP server) hoặc `UClient` (UDP client).
- Tính năng:
  - Server: lắng nghe UDP trên port do người dùng nhập; hiển thị thông điệp kèm địa chỉ IP nguồn.
  - Client: gửi chuỗi UTF-8 tới IP/Port chỉ định.
- Cách chạy nhanh:
  - Visual Studio: Set Startup Project = `Bai1`, F5.
  - CLI: `dotnet run --project Bai1`
  - Trong cửa sổ `Menu` chọn Server hoặc Client.
- Gợi ý test: chạy 1 Server + nhiều Client gửi thông điệp.

---

### Bài 2 — TCP (đơn kết nối)
- Thư mục: `Bai2/`
- Entry: `Program.cs` chạy Form `Bai2` (TCP server).
- Tính năng:
  - Server TCP tại `127.0.0.1:8080`, chấp nhận 1 client, đọc dữ liệu theo từng byte đến khi gặp `\n`, hiển thị lên ListView.
- Cách chạy nhanh:
  - Visual Studio: Set Startup Project = `Bai2`, F5.
  - CLI: `dotnet run --project Bai2`
  - Dùng telnet/netcat hoặc 1 client TCP khác để gửi dữ liệu có hậu tố newline.

---

### Bài 3 — TCP Client/Server (Windows Forms)
- Thư mục: `Bai3/`
- Entry: `Program.cs` chạy `Dashboard` để điều hướng sang `TCP Server` hoặc `TCP Client`.
- Tính năng tiêu biểu (tham khảo `TCP Server.cs`):
  - Server tại `127.0.0.1:8080`, nhận chuỗi kết thúc `\n`, log kết nối và nội dung client.
  - Có form riêng cho Client và Server (xem các file `TCP Client.*`, `TCP Server.*`).
- Cách chạy nhanh:
  - Visual Studio: Set Startup Project = `Bai3`, F5.
  - CLI: `dotnet run --project Bai3`
  - Từ `Dashboard`, mở Server và (tùy chọn) mở Client để kiểm thử.

---

### Bài 4 — Quản lý phòng vé rạp phim (1 Server - Multi Client, SQLite, đồng bộ)
- Thư mục: `Bai4/`
- Entry: `Program.cs` hiển thị menu chọn Server/Client.
- Database: `cinema_database.db` (tự tạo khi chạy Server lần đầu).
- Port mặc định: `8080` (có thể thay đổi trong code nếu cần).
- Hướng dẫn chi tiết: xem `Bai4/README.md` (đã có sẵn).
- Tính năng chính:
  - Server lưu trữ, xử lý đặt vé; Client hiển thị phim/phòng/ghế.
  - Đồng bộ thời gian thực: đặt vé trên 1 client cập nhật ngay các client còn lại.
  - Phân loại vé và tính tiền tự động (Vớt/Thường/VIP).
- Cách chạy nhanh:
  - Visual Studio: Set Startup Project = `Bai4`, F5.
  - CLI: `dotnet run --project Bai4`
  - Mở Server → Listen → mở nhiều Client để test đồng bộ.

---

### Bài 5 — “Hôm nay ăn gì?” (1 Server - Multi Client, SQLite, ảnh)
- Thư mục: `Bai5/`
- Entry: `Program.cs` chạy `MainForm` (menu mở `Bai5Server` hoặc `Bai5Client`).
- Database & thư mục ảnh: tự tạo trong thư mục chạy
  - File DB: `food_database.db`
  - Thư mục ảnh: `Images/` (tự tạo nếu chưa có)
- Schema chính (xem `DatabaseHelper.cs`):
  - Bảng `NguoiDung(IDNCC, HoVaTen, QuyenHan)`
  - Bảng `MonAn(IDMA, TenMonAn, HinhAnh, IDNCC)` (FK tới `NguoiDung`)
  - Hỗ trợ dữ liệu mẫu và chọn món ngẫu nhiên theo người đóng góp.
- Cách chạy nhanh:
  - Visual Studio: Set Startup Project = `Bai5`, F5.
  - CLI: `dotnet run --project Bai5`
  - Mở Server trước, sau đó mở 1+ Client để sử dụng.

---

### Bài 6 — TCP Chat Room (Multi-Client)
- Thư mục: `Bai6/`
- Entry: `Program.cs` mở menu chọn `Bai6Server` hoặc `Bai6Client`.
- Port mặc định: nhập trong UI (mặc định `8080`), tự động thử port kế tiếp nếu bị chiếm.
- Giao thức (trích `Bai6Server.cs`):
  - `JOIN|username` — đăng nhập
  - `MSG_ALL|sender|text` — nhắn tất cả
  - `MSG_PRIV|sender|toUser|text` — nhắn riêng
  - `FILE_ALL|sender|filename|mime|base64` — gửi file toàn phòng
  - `FILE_PRIV|sender|toUser|filename|mime|base64` — gửi file riêng
- Server quản lý danh sách người dùng, broadcast danh sách `USERS|a,b,c` và thông báo hệ thống `SYS|...`.
- Cách chạy nhanh:
  - Visual Studio: Set Startup Project = `Bai6`, F5.
  - CLI: `dotnet run --project Bai6`
  - Mở Server → mở nhiều Client → JOIN với username khác nhau.

---

### Cách chạy chung
1. Mở solution `Lab3_LTMCB.sln` bằng Visual Studio 2022, chọn project (Bài) làm Startup Project, bấm F5.
2. Hoặc dùng CLI: `dotnet run --project <đường-dẫn-project.csproj>` trong thư mục bài tương ứng.
3. Với các bài có mô hình Client/Server, luôn chạy Server trước (Listen) rồi mới mở Client.

### Các truy vấn SQL mẫu (SQLite)

#### Bài 4 — cinema_database.db

- Liệt kê phim và giá:
```sql
SELECT Id, Name, BasePrice FROM Movies ORDER BY Name;
```

- Liệt kê phòng:
```sql
SELECT Id, RoomNumber, Name FROM Rooms ORDER BY RoomNumber;
```

- Phim và các phòng đang chiếu:
```sql
SELECT m.Name AS Movie, r.Name AS Room
FROM MovieRooms mr
JOIN Movies m ON mr.MovieId = m.Id
JOIN Rooms r ON mr.RoomId = r.Id
ORDER BY m.Name, r.RoomNumber;
```

- Danh sách ghế và trạng thái (0: trống, 1: đã đặt):
```sql
SELECT s.Id, r.Name AS Room, s.SeatName, s.SeatType, s.IsBooked
FROM Seats s
JOIN Rooms r ON s.RoomId = r.Id
ORDER BY r.RoomNumber, s.SeatName;
```

- Tạo một booking (ví dụ):
```sql
-- 1) Tạo đơn đặt vé
INSERT INTO Bookings (CustomerName, MovieId, RoomId, BookingTime, TotalPrice)
VALUES ('Nguyen Van A', 1, 1, datetime('now','localtime'), 180000);

-- 2) Gắn ghế vào đơn đặt
INSERT INTO BookingSeats (BookingId, SeatId) VALUES (last_insert_rowid(), 5);
INSERT INTO BookingSeats (BookingId, SeatId) VALUES (last_insert_rowid(), 6);

-- 3) Đánh dấu ghế đã đặt
UPDATE Seats SET IsBooked = 1 WHERE Id IN (5,6);
```

- Thống kê doanh thu theo phim:
```sql
SELECT m.Name AS Movie, SUM(b.TotalPrice) AS Revenue, COUNT(b.Id) AS NumBookings
FROM Bookings b
JOIN Movies m ON b.MovieId = m.Id
GROUP BY m.Id
ORDER BY Revenue DESC;
```

#### Bài 5 — food_database.db

- Danh sách người dùng (người đóng góp):
```sql
SELECT IDNCC, HoVaTen, QuyenHan FROM NguoiDung ORDER BY IDNCC;
```

- Thêm người dùng:
```sql
INSERT INTO NguoiDung (HoVaTen, QuyenHan) VALUES ('Tran Thi B', 'User');
```

- Danh sách món ăn kèm thông tin người đóng góp:
```sql
SELECT m.IDMA, m.TenMonAn, m.HinhAnh, n.HoVaTen, n.QuyenHan
FROM MonAn m
JOIN NguoiDung n ON m.IDNCC = n.IDNCC
ORDER BY m.IDMA;
```

- Thêm món ăn (có/không có ảnh):
```sql
INSERT INTO MonAn (TenMonAn, HinhAnh, IDNCC)
VALUES ('Pho Bo', 'pho_bo.jpg', 1);

INSERT INTO MonAn (TenMonAn, HinhAnh, IDNCC)
VALUES ('Banh Mi', NULL, 2);
```

- Lọc món theo người đóng góp:
```sql
SELECT m.IDMA, m.TenMonAn, m.HinhAnh
FROM MonAn m
WHERE m.IDNCC = 1
ORDER BY m.IDMA;
```

- Xóa một món ăn:
```sql
DELETE FROM MonAn WHERE IDMA = 10;
```

- Cập nhật đường dẫn ảnh của món:
```sql
UPDATE MonAn SET HinhAnh = 'com_tam.jpg' WHERE IDMA = 3;
```

