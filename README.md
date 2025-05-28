# 💒 QuanLyTiecCuoi

**QuanLyTiecCuoi** là một ứng dụng WPF theo mô hình **MVVM kết hợp kiến trúc 3 lớp (Presentation, BLL, DAL)**, dùng để quản lý hoạt động tổ chức tiệc cưới. Ứng dụng hỗ trợ đầy đủ các chức năng như: quản lý sảnh, ca làm việc, thực đơn, dịch vụ, tiệc cưới, báo cáo, phân quyền người dùng, và tùy chỉnh tham số hệ thống.

---

## 🛠️ Công nghệ sử dụng

- **Ngôn ngữ:** C#, XAML  
- **Nền tảng:** WPF (.NET Framework 4.8)  
- **IDE:** Visual Studio 2022  
- **Cơ sở dữ liệu:** SQL Server 2016 hoặc mới hơn  
- **Kiến trúc:** MVVM + 3 Layer (Presentation, BLL, DAL)  
- **Thư viện:** Material Design in XAML Toolkit, Enity FrameWork

---

## ⚙️ Yêu cầu hệ thống

- Hệ điều hành: Windows 10 hoặc mới hơn  
- .NET Framework: 4.8  
- SQL Server: 2016 hoặc mới hơn  
- Visual Studio: 2022 (khuyến nghị bản Professional hoặc Community với hỗ trợ WPF)

---

## 🚀 Hướng dẫn cài đặt & chạy dự án

### 1. Thiết lập cơ sở dữ liệu

1. Mở **SQL Server Management Studio (SSMS)**  
2. Chạy file script `QuanLyTiecCuoi.sql` (nằm trong thư mục gốc, cùng cấp với `.sln`) để tạo cơ sở dữ liệu và các bảng cần thiết  
3. Kiểm tra và cập nhật **chuỗi kết nối** trong file `App.config` (trong project Presentation/WPF) cho phù hợp với SQL Server trên máy bạn

### 2. Mở và build dự án

1. Mở file `QuanLyTiecCuoi.sln` bằng **Visual Studio 2022**  
2. Cài đặt các gói NuGet bị thiếu nếu có (vào `Tools > NuGet Package Manager > Manage NuGet Packages for Solution`)  
3. Nhấn `F5` để chạy ứng dụng

### 3. Đăng nhập

- Khi khởi động, ứng dụng sẽ đưa bạn đến **màn hình đăng nhập**  
- Bạn có thể đăng nhập bằng **tài khoản mặc định** hoặc tự thêm tài khoản vào bảng `NGUOIDUNG` trong SQL Server (qua SSMS)

---

## 📁 Cấu trúc thư mục

```
QuanLyTiecCuoi/
│
├── Presentation/       # Dự án WPF: View (XAML), ViewModel, giao diện người dùng
│
├── BLL/                # Business Logic Layer: xử lý logic nghiệp vụ
│
├── DAL/                # Data Access Layer: kết nối và thao tác với SQL Server qua ADO.NET
│
├── DataTransferObject/ # Các lớp DTO đại diện cho dữ liệu trao đổi giữa các lớp
│
├── QuanLyTiecCuoi.sql  # File script tạo CSDL
│
└── App.config          # Cấu hình chuỗi kết nối và tài nguyên WPF
```

---

## 🔐 Phân quyền người dùng

Ứng dụng hỗ trợ phân quyền thông qua:
- Bảng `CHUCNANG`, `NHOMNGUOIDUNG`, `PHANQUYEN`, và `NGUOIDUNG`  
- Giao diện quản lý phân quyền cho phép thiết lập **chức năng nào được nhóm nào truy cập**

---

## 🧩 Các chức năng chính

| Tên chức năng       | Mô tả                                |
|---------------------|----------------------------------------|
| Quản lý sảnh         | Thêm, sửa, xóa thông tin sảnh cưới     |
| Quản lý loại sảnh    | Phân loại và định giá theo loại sảnh   |
| Quản lý ca làm việc  | Thiết lập các ca tổ chức trong ngày    |
| Quản lý món ăn       | Thực đơn cho từng tiệc cưới            |
| Quản lý dịch vụ      | Các dịch vụ phụ trợ như âm thanh, ánh sáng, MC... |
| Quản lý tiệc cưới    | Thông tin chi tiết từng tiệc cưới      |
| Tham số hệ thống     | Cấu hình quy định chung (ví dụ: số lượng bàn tối thiểu) |
| Báo cáo thống kê     | Xuất báo cáo doanh thu, tần suất đặt tiệc |
| Quản lý người dùng   | Tạo tài khoản, phân nhóm, phân quyền   |

---

## ❗ Lưu ý

- Nếu ứng dụng **không kết nối được cơ sở dữ liệu**, hãy kiểm tra lại chuỗi kết nối trong `App.config`  
- Đảm bảo **SQL Server đã khởi động** và **có quyền truy cập** đến CSDL `QuanLyTiecCuoi`  
- Với dữ liệu lớn (ví dụ: danh sách sảnh, dịch vụ...), cần tối ưu bộ nhớ nếu gặp độ trễ khi binding

---

## 📬 Liên hệ

Mọi thắc mắc, vui lòng liên hệ qua email: **23521476@gm.uit.edu.vn**
