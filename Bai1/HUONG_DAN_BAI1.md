# HƯỚNG DẪN SỬ DỤNG VÀ TEST CASE - BÀI 1: UDP CLIENT-SERVER

## 📋 MÔ TẢ ỨNG DỤNG

Bai1 là ứng dụng giao tiếp UDP (User Datagram Protocol) cho phép:
- **UDP Server**: Lắng nghe và nhận tin nhắn từ các client qua UDP
- **UDP Client**: Gửi tin nhắn đến server qua UDP

## 🚀 HƯỚNG DẪN SỬ DỤNG

### Bước 1: Khởi động ứng dụng
1. Chạy ứng dụng bằng lệnh: `dotnet run` trong thư mục `Lab3_LTMCB\Lab3_LTMCB`
2. Màn hình **Menu** sẽ hiển thị với 2 nút:
   - **UDP Server**: Mở cửa sổ Server
   - **UDP Client**: Mở cửa sổ Client

### Bước 2: Thiết lập UDP Server
1. Click nút **"UDP Server"** trên màn hình Menu
2. Trong cửa sổ **UDP Server**:
   - Nhập **Port** (ví dụ: `8080`) vào ô textbox
   - Click nút **"Listen"** để bắt đầu lắng nghe
   - Sau khi click "Listen", nút sẽ bị vô hiệu hóa (không thể click lại)
3. Server sẽ hiển thị các tin nhắn nhận được trong RichTextBox với định dạng: `IP:Message`

### Bước 3: Thiết lập UDP Client
1. Click nút **"UDP Client"** trên màn hình Menu (có thể mở nhiều client)
2. Trong cửa sổ **UDP Client**:
   - Nhập **IP Remote Host** (ví dụ: `127.0.0.1` cho localhost hoặc IP của máy server)
   - Nhập **Port** (phải trùng với port của server, ví dụ: `8080`)
   - Nhập **tin nhắn** vào ô Chat
   - Click nút **"Gửi"** để gửi tin nhắn

### Bước 4: Kiểm tra kết quả
- Tin nhắn sẽ xuất hiện trên màn hình Server với định dạng: `IP_Client:Tin_nhắn`

## 📝 TEST CASES

### **Test Case 1: Gửi tin nhắn đơn giản (Localhost)**
**Mục đích**: Kiểm tra chức năng cơ bản gửi/nhận tin nhắn trên cùng máy

**Các bước thực hiện**:
1. Khởi động ứng dụng
2. Mở UDP Server:
   - Port: `8080`
   - Click "Listen"
3. Mở UDP Client:
   - IP Remote Host: `127.0.0.1`
   - Port: `8080`
   - Chat: `Hello Server`
   - Click "Gửi"

**Kết quả mong đợi**:
- Server hiển thị: `127.0.0.1:Hello Server`

---

### **Test Case 2: Gửi nhiều tin nhắn liên tiếp**
**Mục đích**: Kiểm tra server có thể nhận nhiều tin nhắn từ cùng một client

**Các bước thực hiện**:
1. Khởi động Server (Port: `8080`)
2. Mở Client và gửi lần lượt:
   - Tin nhắn 1: `Message 1`
   - Tin nhắn 2: `Message 2`
   - Tin nhắn 3: `Test UDP`

**Kết quả mong đợi**:
- Server hiển thị 3 dòng:
  ```
   127.0.0.1:Message 1
   127.0.0.1:Message 2
   127.0.0.1:Test UDP
  ```

---

### **Test Case 3: Nhiều Client gửi đến cùng Server**
**Mục đích**: Kiểm tra server có thể nhận tin nhắn từ nhiều client khác nhau

**Các bước thực hiện**:
1. Khởi động Server (Port: `8080`)
2. Mở Client 1:
   - IP: `127.0.0.1`, Port: `8080`
   - Gửi: `Client 1 message`
3. Mở Client 2 (cửa sổ mới):
   - IP: `127.0.0.1`, Port: `8080`
   - Gửi: `Client 2 message`

**Kết quả mong đợi**:
- Server hiển thị 2 tin nhắn từ 2 IP khác nhau (hoặc cùng IP nhưng khác thời điểm)

---

### **Test Case 4: Tin nhắn tiếng Việt có dấu**
**Mục đích**: Kiểm tra encoding UTF-8 hoạt động đúng với ký tự đặc biệt

**Các bước thực hiện**:
1. Khởi động Server (Port: `8080`)
2. Client gửi: `Xin chào! Đây là test tiếng Việt`

**Kết quả mong đợi**:
- Server hiển thị đúng: `127.0.0.1:Xin chào! Đây là test tiếng Việt`

---

### **Test Case 5: Tin nhắn dài**
**Mục đích**: Kiểm tra xử lý tin nhắn có độ dài lớn

**Các bước thực hiện**:
1. Khởi động Server (Port: `8080`)
2. Client gửi tin nhắn dài (ví dụ: 500 ký tự)

**Kết quả mong đợi**:
- Server nhận và hiển thị đầy đủ tin nhắn

---

### **Test Case 6: Port không hợp lệ (Server)**
**Mục đích**: Kiểm tra validation port trên Server

**Các bước thực hiện**:
1. Mở UDP Server
2. Nhập Port: `abc` (không phải số)
3. Click "Listen"

**Kết quả mong đợi**:
- Hiển thị message box lỗi: "Vui lòng nhập định dạng số cho Port"
- Nút "Listen" vẫn có thể click lại

---

### **Test Case 7: Port ngoài phạm vi hợp lệ**
**Mục đích**: Kiểm tra xử lý port không hợp lệ

**Các bước thực hiện**:
1. Server: Port `99999` (quá lớn)
2. Click "Listen"

**Kết quả mong đợi**:
- Có thể xảy ra exception hoặc lỗi (tùy hệ thống)

---

### **Test Case 8: Client gửi đến IP không tồn tại**
**Mục đích**: Kiểm tra xử lý khi không có server lắng nghe

**Các bước thực hiện**:
1. Chỉ mở Client (không mở Server)
2. Client:
   - IP: `127.0.0.1`
   - Port: `8080`
   - Gửi: `Test message`

**Kết quả mong đợi**:
- Client gửi thành công (UDP không có ACK)
- Không có tin nhắn hiển thị (vì không có server nhận)

---

### **Test Case 9: Client gửi đến Port sai**
**Mục đích**: Kiểm tra khi port client không khớp với port server

**Các bước thực hiện**:
1. Server: Port `8080`, click "Listen"
2. Client:
   - IP: `127.0.0.1`
   - Port: `8081` (sai port)
   - Gửi: `Test`

**Kết quả mong đợi**:
- Client gửi thành công
- Server không nhận được tin nhắn

---

### **Test Case 10: Tin nhắn rỗng**
**Mục đích**: Kiểm tra xử lý tin nhắn trống

**Các bước thực hiện**:
1. Khởi động Server (Port: `8080`)
2. Client gửi tin nhắn rỗng (không nhập gì)

**Kết quả mong đợi**:
- Server nhận và hiển thị: `127.0.0.1:` (chỉ có IP, không có message)

---

### **Test Case 11: Tin nhắn đặc biệt (ký tự đặc biệt, emoji)**
**Mục đích**: Kiểm tra xử lý ký tự đặc biệt

**Các bước thực hiện**:
1. Khởi động Server (Port: `8080`)
2. Client gửi: `!@#$%^&*()_+-=[]{}|;':\",./<>?`

**Kết quả mong đợi**:
- Server hiển thị đúng các ký tự đặc biệt

---

### **Test Case 12: Server nhận tin nhắn liên tục**
**Mục đích**: Kiểm tra server có thể xử lý nhiều tin nhắn liên tục

**Các bước thực hiện**:
1. Khởi động Server (Port: `8080`)
2. Client gửi 10 tin nhắn liên tiếp, mỗi tin cách nhau 1 giây

**Kết quả mong đợi**:
- Server nhận và hiển thị đầy đủ tất cả 10 tin nhắn

---

## ⚠️ LƯU Ý

1. **Port**: Phải là số nguyên dương, thường từ 1024-65535 (tránh các port hệ thống < 1024)
2. **IP Address**: 
   - `127.0.0.1` hoặc `localhost` cho giao tiếp trên cùng máy
   - IP thực của máy (ví dụ: `192.168.1.100`) cho giao tiếp qua mạng LAN
3. **Firewall**: Nếu test qua mạng, cần mở port trên firewall
4. **UDP Protocol**: UDP là giao thức không đảm bảo (unreliable), tin nhắn có thể bị mất
5. **Thread Safety**: Ứng dụng sử dụng `CheckForIllegalCrossThreadCalls = false`, có thể gây vấn đề trong môi trường production

## 🔧 TROUBLESHOOTING

### Vấn đề: Server không nhận được tin nhắn
- **Kiểm tra**: Port trên Client và Server có khớp không
- **Kiểm tra**: IP address có đúng không
- **Kiểm tra**: Server đã click "Listen" chưa

### Vấn đề: Lỗi "Port already in use"
- **Giải pháp**: Đổi sang port khác hoặc đóng ứng dụng đang sử dụng port đó

### Vấn đề: Tin nhắn bị lỗi encoding
- **Kiểm tra**: Đảm bảo sử dụng UTF-8 encoding (đã được code sử dụng)

## 📊 BẢNG TÓM TẮT TEST CASES

| Test Case | Mô tả | Kết quả mong đợi | Trạng thái |
|-----------|-------|------------------|------------|
| TC1 | Gửi tin nhắn đơn giản | Server nhận và hiển thị đúng | ✅ |
| TC2 | Nhiều tin nhắn liên tiếp | Server hiển thị tất cả | ✅ |
| TC3 | Nhiều client | Server nhận từ nhiều client | ✅ |
| TC4 | Tiếng Việt có dấu | Hiển thị đúng encoding | ✅ |
| TC5 | Tin nhắn dài | Nhận đầy đủ | ✅ |
| TC6 | Port không hợp lệ | Hiển thị lỗi | ✅ |
| TC7 | Port quá lớn | Xử lý exception | ⚠️ |
| TC8 | IP không tồn tại | Client gửi nhưng không có server nhận | ✅ |
| TC9 | Port sai | Client gửi nhưng server không nhận | ✅ |
| TC10 | Tin nhắn rỗng | Server nhận và hiển thị | ✅ |
| TC11 | Ký tự đặc biệt | Hiển thị đúng | ✅ |
| TC12 | Tin nhắn liên tục | Nhận tất cả | ✅ |

---

**Ngày tạo**: 2024  
**Phiên bản**: 1.0  
**Ứng dụng**: Bai1 - UDP Client-Server Communication

