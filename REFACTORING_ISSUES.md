# Refactoring Issues - Services & DI Container

## ? ?ã hoàn thành
1. DTO - T?t c? DTO ?ã ???c refactor sang ti?ng Anh
2. Repository Interfaces & Implementations - ?ã refactor xong
3. Service Interfaces & Implementations - ?ã refactor xong
4. ServiceContainer - ?ã c?p nh?t v?i t?t c? Repository và Service

## ? V?n ?? c?n s?a

### 1. Services không s? d?ng DI (Dependency Injection)

#### UserViewModel.cs
```csharp
// HI?N T?I (SAI):
public UserViewModel() {
    _AppUserService = new AppUserService();
    _UserGroupService = new UserGroupService();
}

// NÊN S?A THÀNH:
public UserViewModel(IAppUserService appUserService, IUserGroupService userGroupService) {
    _appUserService = appUserService;
    _userGroupService = userGroupService;
}
```

#### ReportView.xaml.cs
```csharp
// HI?N T?I (SAI):
DataContext = new ReportViewModel(new CtRevenueReportService());

// NÊN S?A THÀNH:
DataContext = new ReportViewModel(ServiceContainer.GetService<IRevenueReportDetailService>());
```

#### ReportViewModel.cs
```csharp
// HI?N T?I - Constructor parameter type sai:
public ReportViewModel(IRevenueReportDetailService ctBaoCaoService)

// Tên class ?ã ??i:
// CtRevenueReportService ? RevenueReportDetailService
// CTRevenueReportDTO ? RevenueReportDetailDTO
```

### 2. PermissionViewModel - Truy c?p tr?c ti?p Database

```csharp
// HI?N T?I (SAI - truy c?p tr?c ti?p DB):
DataProvider.Ins.DB.NHOMNGUOIDUNGs
DataProvider.Ins.DB.CHUCNANGs
DataProvider.Ins.DB.NGUOIDUNGs

// NÊN S?A - S? d?ng Service Layer:
- S? d?ng IUserGroupService
- S? d?ng IPermissionService
- S? d?ng IAppUserService
```

### 3. ServiceContainer - Thi?u ViewModel registration

```csharp
// C?N THÊM VÀO ServiceContainer.cs:
services.AddTransient<UserViewModel>();
services.AddTransient<PermissionViewModel>();
services.AddTransient<ReportViewModel>();
```

### 4. Tên bi?n/parameter ch?a nh?t quán

#### AppUserService.cs
```csharp
// Private field naming:
private readonly IAppUserRepository _appUserRepository; // ? ?ÚNG

// Nh?ng trong UserViewModel:
private readonly IAppUserService _AppUserService; // ? SAI - nên là _appUserService
```

#### ReportViewModel.cs
```csharp
// Field naming:
private readonly IRevenueReportDetailService _ctBaoCaoService; // ? SAI

// NÊN LÀ:
private readonly IRevenueReportDetailService _revenueReportDetailService; // ? ?ÚNG
```

### 5. DTO naming trong code c?

```csharp
// ReportViewModel.cs v?n dùng:
ObservableCollection<CTRevenueReportDTO> ReportList // ? SAI

// NÊN LÀ:
ObservableCollection<RevenueReportDetailDTO> ReportList // ? ?ÚNG
```

## ?? Action Items

### Priority 1 - Critical (Service Layer)
1. ? S?a ReportViewModel constructor parameter type
2. ? ??i tên CTRevenueReportDTO ? RevenueReportDetailDTO trong ReportViewModel
3. ? S?a ReportView.xaml.cs ?? dùng DI
4. ? Refactor PermissionViewModel ?? dùng Service thay vì truy c?p DB tr?c ti?p
5. ? Refactor UserViewModel ?? dùng DI

### Priority 2 - Code Quality
6. ? Chu?n hóa tên bi?n private fields (camelCase with _)
7. ? Thêm missing ViewModels vào ServiceContainer
8. ? Ki?m tra t?t c? ViewModels khác xem có dùng DI ch?a

### Priority 3 - Testing
9. ? Build project và ki?m tra compilation errors
10. ? Test t?ng module ?ã refactor
11. ? Update documentation

## ?? Files c?n ki?m tra thêm

- [ ] T?t c? ViewModel files trong Presentation/ViewModel/
- [ ] T?t c? View files trong Presentation/View/
- [ ] App.xaml.cs - initialization code
- [ ] MainWindow.xaml.cs

## ?? Notes

- Entity Framework Model entities (NGUOIDUNG, NHOMNGUOIDUNG, etc.) v?n gi? nguyên tên ti?ng Vi?t vi?t hoa
- Ch? DTO, Service, Repository ???c refactor sang ti?ng Anh
- Navigation properties trong Model entities c?ng gi? nguyên (vd: x.NHOMNGUOIDUNG)
