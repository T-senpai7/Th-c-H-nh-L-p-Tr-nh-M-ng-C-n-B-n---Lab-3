# TEST CASE CHECKLIST - BÀI 1

## ✅ CHECKLIST TEST NHANH

### Setup Cơ Bản
- [ ] Chạy ứng dụng: `dotnet run`
- [ ] Màn hình Menu hiển thị 2 nút: "UDP Server" và "UDP Client"

### Test Server
- [ ] Click "UDP Server" → Cửa sổ Server mở
- [ ] Nhập Port: `8080` → Click "Listen"
- [ ] Nút "Listen" bị vô hiệu hóa sau khi click
- [ ] Server đang chờ nhận tin nhắn

### Test Client
- [ ] Click "UDP Client" → Cửa sổ Client mở
- [ ] Nhập IP: `127.0.0.1`
- [ ] Nhập Port: `8080`
- [ ] Nhập tin nhắn: `Hello`
- [ ] Click "Gửi"

### Test Kết Quả
- [ ] Server hiển thị: `127.0.0.1:Hello`

---

## 📋 TEST CASES CHI TIẾT

### TC1: Tin nhắn đơn giản
```
Server: Port 8080 → Listen
Client: IP 127.0.0.1, Port 8080, Message "Hello Server"
✅ Kết quả: Server hiển thị "127.0.0.1:Hello Server"
```

### TC2: Nhiều tin nhắn
```
Server: Port 8080 → Listen
Client gửi: "Message 1", "Message 2", "Message 3"
✅ Kết quả: Server hiển thị 3 dòng
```

### TC3: Nhiều Client
```
Server: Port 8080 → Listen
Client 1: Gửi "Client 1"
Client 2: Gửi "Client 2"
✅ Kết quả: Server nhận cả 2 tin nhắn
```

### TC4: Tiếng Việt
```
Server: Port 8080 → Listen
Client: Gửi "Xin chào! Đây là test"
✅ Kết quả: Server hiển thị đúng tiếng Việt
```

### TC5: Port không hợp lệ
```
Server: Port "abc" → Listen
✅ Kết quả: Hiển thị lỗi "Vui lòng nhập định dạng số cho Port"
```

### TC6: Port sai
```
Server: Port 8080 → Listen
Client: Port 8081 → Gửi
✅ Kết quả: Server không nhận được
```

### TC7: Tin nhắn rỗng
```
Server: Port 8080 → Listen
Client: Gửi tin nhắn rỗng
✅ Kết quả: Server hiển thị "127.0.0.1:"
```

---

## 🎯 TEST SCENARIOS THỰC TẾ

### Scenario 1: Chat đơn giản
1. Mở Server (Port: 8080)
2. Mở Client
3. Gửi: "Hello"
4. Gửi: "How are you?"
5. Gửi: "Goodbye"
✅ Kiểm tra: Server hiển thị 3 tin nhắn

### Scenario 2: Test với IP thực
1. Lấy IP máy: `ipconfig` (ví dụ: 192.168.1.100)
2. Server trên máy A (Port: 8080)
3. Client trên máy B (IP: 192.168.1.100, Port: 8080)
4. Gửi tin nhắn
✅ Kiểm tra: Server nhận được tin nhắn từ IP máy B

### Scenario 3: Stress Test
1. Server (Port: 8080)
2. Mở 3 Client cùng lúc
3. Mỗi client gửi 5 tin nhắn
✅ Kiểm tra: Server nhận đủ 15 tin nhắn

---

## ⚠️ LỖI THƯỜNG GẶP

| Lỗi | Nguyên nhân | Giải pháp |
|-----|-------------|-----------|
| Server không nhận tin | Port không khớp | Kiểm tra port Client = Server |
| Lỗi "Port already in use" | Port đang được dùng | Đổi port khác |
| Tin nhắn bị lỗi ký tự | Encoding sai | Đã dùng UTF-8, kiểm tra lại |
| Client không gửi được | IP sai | Kiểm tra IP server |

---

**Ghi chú**: Đánh dấu ✅ sau mỗi test case đã hoàn thành!

