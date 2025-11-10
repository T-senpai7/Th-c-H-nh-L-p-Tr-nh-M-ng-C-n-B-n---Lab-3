# BÀI 4: QUẢN LÝ PHÒNG VÉ RẠP PHIM (1 Server - Multi Client)

## 📋 MÔ TẢ

Hệ thống quản lý phòng vé rạp phim sử dụng TCP/IP với kiến trúc 1 Server - Multi Client:
- **Server**: Lưu trữ dữ liệu trong SQLite database, xử lý yêu cầu từ nhiều client
- **Client**: Giao diện đặt vé, kết nối đến server để xem và đặt vé
- **Đồng bộ**: Khi một client đặt vé, tất cả các client khác sẽ được cập nhật ngay lập tức

## 🚀 HƯỚNG DẪN SỬ DỤNG

### 1. Khởi động Server

1. Chạy ứng dụng: `dotnet run` trong thư mục `Bai4`
2. Chọn **"TCP Server"** từ menu
3. Click nút **"Listen"** để bắt đầu lắng nghe kết nối từ clients
4. Server sẽ tự động tạo database `cinema_database.db` nếu chưa có
5. Database sẽ được khởi tạo với dữ liệu mặc định:
   - 4 phim: "Đào, phở và piano", "Mai", "Gặp lại chị bầu", "Tarot"
   - 3 phòng chiếu: Phòng 1, Phòng 2, Phòng 3
   - Mỗi phòng có 15 ghế (3 hàng A, B, C x 5 cột 1-5)

### 2. Khởi động Client

1. Chạy ứng dụng: `dotnet run` trong thư mục `Bai4` (có thể chạy nhiều instance)
2. Chọn **"TCP Client"** từ menu
3. Nhập **Server IP** (mặc định: `127.0.0.1` cho localhost)
4. Click nút **"Kết nối"** để kết nối đến server
5. Sau khi kết nối thành công:
   - ComboBox "Tên phim" sẽ được load từ server
   - Chọn phim → ComboBox "Phòng chiếu" sẽ hiển thị các phòng có phim đó
   - Chọn phòng → Ghế sẽ được hiển thị với trạng thái (trống/đã đặt)

### 3. Đặt Vé

1. Nhập **Họ và tên** khách hàng
2. Chọn **Phim** và **Phòng chiếu**
3. Click vào các **ghế** để chọn (click lần nữa để bỏ chọn)
4. Xem **Tổng tiền** được tính tự động
5. Click nút **"Đặt Vé"** để gửi yêu cầu đến server
6. Server sẽ kiểm tra và đặt vé nếu ghế còn trống
7. Tất cả clients sẽ được cập nhật trạng thái ghế ngay lập tức

### 4. Đồng Bộ Dữ Liệu

- Khi một client đặt vé thành công, server sẽ broadcast cập nhật cho tất cả clients đang kết nối
- Các client khác sẽ tự động cập nhật trạng thái ghế (ghế đã đặt sẽ chuyển sang màu xám và bị disable)
- Nếu client đang cố gắng đặt ghế đã được đặt bởi client khác, server sẽ trả về lỗi

## 🗄️ CẤU TRÚC DATABASE

Database SQLite (`cinema_database.db`) gồm các bảng:

### Movies
- `Id`: Primary key
- `Name`: Tên phim
- `BasePrice`: Giá vé cơ bản

### Rooms
- `Id`: Primary key
- `RoomNumber`: Số phòng
- `Name`: Tên phòng (ví dụ: "Phòng 1")

### MovieRooms
- `MovieId`: Foreign key đến Movies
- `RoomId`: Foreign key đến Rooms
- Quan hệ many-to-many giữa Movies và Rooms

### Seats
- `Id`: Primary key
- `RoomId`: Foreign key đến Rooms
- `SeatName`: Tên ghế (ví dụ: "A1", "B2")
- `SeatType`: Loại vé ("Vé vớt", "Vé thường", "Vé VIP")
- `IsBooked`: Trạng thái đặt (0 = chưa đặt, 1 = đã đặt)

### Bookings
- `Id`: Primary key
- `CustomerName`: Tên khách hàng
- `MovieId`: Foreign key đến Movies
- `RoomId`: Foreign key đến Rooms
- `BookingTime`: Thời gian đặt vé
- `TotalPrice`: Tổng tiền

### BookingSeats
- `BookingId`: Foreign key đến Bookings
- `SeatId`: Foreign key đến Seats
- Quan hệ many-to-many giữa Bookings và Seats

## 📡 PROTOCOL GIAO TIẾP

### Client → Server

1. **GET_MOVIES**: Lấy danh sách phim
   ```
   GET_MOVIES|
   ```

2. **GET_ROOMS**: Lấy danh sách phòng cho một phim
   ```
   GET_ROOMS|Tên phim
   ```

3. **GET_SEATS**: Lấy trạng thái ghế của một phòng
   ```
   GET_SEATS|Tên phim|Tên phòng
   ```

4. **BOOK_SEATS**: Đặt vé
   ```
   BOOK_SEATS|Tên khách hàng|Tên phim|Tên phòng|Ghế1,Ghế2,...|Tổng tiền
   ```

### Server → Client

1. **MOVIES**: Danh sách phim
   ```
   MOVIES|Tên phim1:Giá1;Tên phim2:Giá2;...
   ```

2. **ROOMS**: Danh sách phòng
   ```
   ROOMS|Phòng 1;Phòng 2;...
   ```

3. **SEATS**: Trạng thái ghế
   ```
   SEATS|Ghế1:Loại:Trạng thái;Ghế2:Loại:Trạng thái;...
   ```
   Trạng thái: 0 = chưa đặt, 1 = đã đặt

4. **BOOK_SUCCESS**: Đặt vé thành công
   ```
   BOOK_SUCCESS|
   ```

5. **BOOK_ERROR**: Lỗi đặt vé
   ```
   BOOK_ERROR|Thông báo lỗi
   ```

6. **UPDATE_SEATS**: Cập nhật trạng thái ghế (broadcast)
   ```
   UPDATE_SEATS|Tên phòng|Ghế1,Ghế2,...
   ```

7. **ERROR**: Lỗi chung
   ```
   ERROR|Thông báo lỗi
   ```

## 🎫 LOẠI VÉ VÀ GIÁ

- **Vé vớt**: Giá = 25% giá cơ bản (Ghế: A1, A5, C1, C5)
- **Vé thường**: Giá = 100% giá cơ bản (Ghế: A2, A3, A4, C2, C3, C4)
- **Vé VIP**: Giá = 200% giá cơ bản (Ghế: B1, B2, B3, B4, B5)

## 📊 DỮ LIỆU MẶC ĐỊNH

### Phim và Giá
- Đào, phở và piano: 45,000 VNĐ
- Mai: 100,000 VNĐ
- Gặp lại chị bầu: 70,000 VNĐ
- Tarot: 90,000 VNĐ

### Phim và Phòng
- Đào, phở và piano: Phòng 1, 2, 3
- Mai: Phòng 2, 3
- Gặp lại chị bầu: Phòng 1
- Tarot: Phòng 3

## ⚠️ LƯU Ý

1. **Port**: Server mặc định chạy trên port `8080`
2. **Database**: Database được tạo tự động trong thư mục chạy ứng dụng
3. **Đồng bộ**: Đảm bảo server đang chạy trước khi clients kết nối
4. **Nhiều Client**: Có thể mở nhiều cửa sổ client để test đồng bộ
5. **Firewall**: Nếu test qua mạng, cần mở port 8080 trên firewall

## 🔧 TROUBLESHOOTING

### Client không kết nối được
- Kiểm tra server đã click "Listen" chưa
- Kiểm tra IP address có đúng không
- Kiểm tra firewall có chặn port 8080 không

### Vé không đồng bộ
- Đảm bảo server đang chạy
- Kiểm tra kết nối mạng giữa client và server
- Xem log trên server để kiểm tra lỗi

### Database lỗi
- Xóa file `cinema_database.db` và chạy lại server để tạo database mới
- Kiểm tra quyền ghi file trong thư mục chạy ứng dụng

## 📁 CẤU TRÚC FILE

```
Bai4/
├── Bai4.csproj          # Project file
├── Program.cs           # Entry point với menu
├── CinemaDatabase.cs    # Database helper class
├── Bai4Server.cs        # TCP Server form
├── Bai4Client.cs        # TCP Client form
├── Test1.cs             # File test mẫu (không sử dụng)
└── README.md            # File này
```

## 🎯 TÍNH NĂNG

✅ 1 Server - Multi Client  
✅ SQLite Database  
✅ Đồng bộ vé giữa các client  
✅ Kiểm tra ghế đã đặt  
✅ Tính giá vé tự động  
✅ Giao diện đẹp, dễ sử dụng  
✅ Xử lý lỗi và thông báo rõ ràng  

---

**Ngày tạo**: 2024  
**Phiên bản**: 1.0  
**Ứng dụng**: Bai4 - Quản lý phòng vé rạp phim

