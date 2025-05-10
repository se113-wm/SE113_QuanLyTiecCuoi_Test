# QuanLyTiecCuoi

## Giới thiệu
Dự án **QuanLyTiecCuoi** là một ứng dụng quản lý tiệc cưới được xây dựng bằng WPF trên nền tảng .NET Framework 4.8. Dự án bao gồm các chức năng như quản lý sảnh, ca, thực đơn, dịch vụ, báo cáo, và phân quyền người dùng.

## Yêu cầu hệ thống
- **Hệ điều hành**: Windows 10 hoặc mới hơn
- **.NET Framework**: 4.8
- **IDE**: Visual Studio 2022
- **SQL Server**: SQL Server 2016 hoặc mới hơn

## Hướng dẫn cài đặt và chạy dự án

### 1. Thiết lập cơ sở dữ liệu
1. Mở SQL Server Management Studio (SSMS).
2. Chạy file SQL script để tạo cơ sở dữ liệu và các bảng cần thiết. File script này nằm ở thư mục gốc của dự án, cạnh file `.sln`. Tên file là `database_script.sql`.
3. Đảm bảo kết nối cơ sở dữ liệu trong file cấu hình của dự án (thường là `App.config` hoặc `Web.config`) đã được thiết lập đúng với thông tin SQL Server của bạn.

### 2. Mở và build dự án
1. Mở file `QuanLyTiecCuoi.sln` bằng Visual Studio 2022.
2. Đảm bảo đã cài đặt đầy đủ các gói NuGet cần thiết. Nếu chưa, vào __Tools > NuGet Package Manager > Manage NuGet Packages for Solution__ và cài đặt các gói bị thiếu.
3. Nhấn __F5__ hoặc chọn __Start__ để chạy dự án.

### 3. Đăng nhập
- Khi chạy ứng dụng, bạn sẽ được chuyển đến màn hình đăng nhập.
- Sử dụng tài khoản mặc định (nếu có) hoặc tài khoản đã được tạo trong cơ sở dữ liệu.

## Cấu trúc dự án
- **Model**: Chứa các lớp mô hình và file EDMX để kết nối với cơ sở dữ liệu.
- **ViewModel**: Chứa logic xử lý và các command cho giao diện.
- **View**: Chứa các file XAML để hiển thị giao diện người dùng.

## Ghi chú
- Nếu gặp lỗi kết nối cơ sở dữ liệu, kiểm tra lại chuỗi kết nối trong file cấu hình.
- Đảm bảo SQL Server đang chạy và có quyền truy cập vào cơ sở dữ liệu.

## Liên hệ
Nếu có bất kỳ vấn đề nào, vui lòng liên hệ qua email: `support@example.com`.
