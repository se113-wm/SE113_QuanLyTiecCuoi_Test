# Ti?n Trình Migration sang Dependency Injection

## ? ?ã Hoàn Thành (9/15 ViewModels - 60%)

### Services & Repositories (100%)
- ? All Repositories (11 repos) - Constructor injection support
- ? All Services (10 services) - Inject repositories

### ViewModels ?ã Chuy?n Sang DI

1. ? **ShiftViewModel** - Hoàn toàn DI
   - Inject: `ICaService`, `IPhieuDatTiecService`
   - S? d?ng: `ServiceContainer.GetService<ShiftViewModel>()`
   - ??ng ký: `services.AddTransient<ShiftViewModel>()`

2. ? **WeddingDetailViewModel** - Factory Pattern
   - Inject: 8 services
   - S? d?ng: `ServiceContainer.CreateWeddingDetailViewModel(maPhieuDat)`
   - Factory method ?ã t?o s?n

3. ? **MenuItemViewModel** - DI
   - Inject: `IMonAnService`
   - S? d?ng: `ServiceContainer.GetService<MenuItemViewModel>()`
   - ??ng ký: `services.AddTransient<MenuItemViewModel>()`
   
4. ? **ServiceDetailItemViewModel** - DI
   - Inject: `IDichVuService`
   - S? d?ng: `ServiceContainer.GetService<ServiceDetailItemViewModel>()`
   - ??ng ký: `services.AddTransient<ServiceDetailItemViewModel>()`

5. ? **InvoiceViewModel** - Factory Pattern
   - Inject: 6 services (IPhieuDatTiecService, ICaService, ISanhService, IChiTietDVService, IThucDonService, IThamSoService)
   - S? d?ng: `ServiceContainer.CreateInvoiceViewModel(invoiceId)`
   - Factory method ?ã t?o s?n

6. ? **AddWeddingViewModel** - Factory Pattern
   - Inject: 8 services
   - S? d?ng: `ServiceContainer.CreateAddWeddingViewModel()`
   - Factory method ?ã t?o s?n
   
7. ? **MainViewModel** - DI
   - Inject: `IPhieuDatTiecService`
   - S? d?ng: `ServiceContainer.GetService<MainViewModel>()`
   - ??ng ký: `services.AddTransient<MainViewModel>()`

8. ? **HomeViewModel** - Constructor Injection
   - Inject: `IPhieuDatTiecService` (MainViewModel truy?n vào)
   - Constructor: `HomeViewModel(MainViewModel mainVM, IPhieuDatTiecService phieuDatTiecService)`
   - T?o trong MainViewModel: `new HomeViewModel(this, _phieuDatTiecService)`

9. ? **ParameterViewModel** - DI
   - Inject: `IThamSoService`
   - S? d?ng: `ServiceContainer.GetService<ParameterViewModel>()`
   - ??ng ký: `services.AddTransient<ParameterViewModel>()`

---

## ?? C?N S?A (5 ViewModels còn l?i - 40%)

### 1. **WeddingViewModel** ?? QUAN TR?NG
**Files**: `Presentation\ViewModel\WeddingViewModel.cs`

**L?i**:
- Line 101-103: `new PhieuDatTiecService()`, `new ThucDonService()`, `new ChiTietDVService()`
- Line 135: `new WeddingDetailViewModel(SelectedItem.MaPhieuDat)` ? S? d?ng Factory
- Line 257: `new AddWeddingViewModel()` ? S? d?ng Factory
- Line 265: `new PhieuDatTiecService()`

**C?n làm**:
```csharp
public WeddingViewModel(
    MainViewModel mainVM,
    IPhieuDatTiecService phieuDatTiecService,
    IThucDonService thucDonService,
    IChiTietDVService chiTietDVService)
{
    _mainViewModel = mainVM;
    _phieuDatTiecService = phieuDatTiecService;
    _thucDonService = thucDonService;
    _chiTietDichVuService = chiTietDVService;
    
    // XÓA: dòng 101-103, 265
    
    // Line 135 s?a:
    DataContext = Infrastructure.ServiceContainer.CreateWeddingDetailViewModel(SelectedItem.MaPhieuDat)
    
    // Line 257 s?a:
    DataContext = Infrastructure.ServiceContainer.CreateAddWeddingViewModel()
}
```

**??ng ký**: Không c?n, ???c t?o trong MainViewModel

---

### 2. **FoodViewModel** ??
**Files**: `Presentation\ViewModel\FoodViewModel.cs`

**L?i**:
- Line 516-517: `new MonAnService()`, `new ThucDonService()`

**C?n làm**:
```csharp
public FoodViewModel(
    IMonAnService monAnService,
    IThucDonService thucDonService)
{
    _foodService = monAnService;
    _thucDonService = thucDonService;
    // XÓA: dòng 516-517
}
```

**??ng ký**: 
```csharp
// ServiceContainer.cs
services.AddTransient<FoodViewModel>();
```

**S? d?ng trong MainViewModel**:
```csharp
FoodCommand = new RelayCommand<object>((p) => true, (p) =>
{
    CurrentView = new FoodView()
    {
        DataContext = Infrastructure.ServiceContainer.GetService<FoodViewModel>()
    };
    // ...
});
```

---

### 3. **HallViewModel** ??
**Files**: `Presentation\ViewModel\HallViewModel.cs`

**L?i**:
- Line 585-587: `new SanhService()`, `new LoaiSanhService()`, `new PhieuDatTiecService()`

**C?n làm**:
```csharp
public HallViewModel(
    ISanhService sanhService,
    ILoaiSanhService loaiSanhService,
    IPhieuDatTiecService phieuDatTiecService)
{
    _sanhService = sanhService;
    _loaiSanhService = loaiSanhService;
    _phieuDatTiecService = phieuDatTiecService;
    // XÓA: dòng 585-587
}
```

**??ng ký**: 
```csharp
services.AddTransient<HallViewModel>();
```

**S? d?ng trong MainViewModel**:
```csharp
HallCommand = new RelayCommand<object>((p) => true, (p) =>
{
    CurrentView = new HallView()
    {
        DataContext = Infrastructure.ServiceContainer.GetService<HallViewModel>()
    };
    // ...
});
```

---

### 4. **HallTypeViewModel** ??
**Files**: `Presentation\ViewModel\HallTypeViewModel.cs`

**L?i**:
- Line 229: `new LoaiSanhService()`
- Line 363: `new SanhService()` (trong method)

**C?n làm**:
```csharp
private readonly ILoaiSanhService _loaiSanhService;
private readonly ISanhService _sanhService;

public HallTypeViewModel(
    ILoaiSanhService loaiSanhService,
    ISanhService sanhService)
{
    _loaiSanhService = loaiSanhService;
    _sanhService = sanhService;
    // XÓA: dòng 229
    
    // Dòng 363 s?a: s? d?ng _sanhService thay vì new SanhService()
}
```

**??ng ký**: 
```csharp
services.AddTransient<HallTypeViewModel>();
```

**S? d?ng trong MainViewModel**:
```csharp
HallTypeCommand = new RelayCommand<object>((p) => true, (p) =>
{
    CurrentView = new HallTypeView()
    {
        DataContext = Infrastructure.ServiceContainer.GetService<HallTypeViewModel>()
    };
    // ...
});
```

---

### 5. **ServiceViewModel** ??
**Files**: `Presentation\ViewModel\ServiceViewModel.cs`

**L?i**:
- Line 563-564: `new DichVuService()`, `new ChiTietDVService()`

**C?n làm**:
```csharp
public ServiceViewModel(
    IDichVuService dichVuService,
    IChiTietDVService chiTietDVService)
{
    _dichVuService = dichVuService;
    _chiTietDVService = chiTietDVService;
    // XÓA: dòng 563-564
}
```

**??ng ký**: 
```csharp
services.AddTransient<ServiceViewModel>();
```

**S? d?ng trong MainViewModel**:
```csharp
ServiceCommand = new RelayCommand<object>((p) => true, (p) =>
{
    CurrentView = new ServiceView()
    {
        DataContext = Infrastructure.ServiceContainer.GetService<ServiceViewModel>()
    };
    // ...
});
```

---

## ?? Quy Trình Chung (Copy-Paste Pattern)

### B??c 1: S?a Constructor
```csharp
// TR??C
public MyViewModel()
{
    _myService = new MyService();
}

// SAU
public MyViewModel(IMyService myService)
{
    _myService = myService;
}
```

### B??c 2: Xóa T?t C? `new XxxService()`
- Trong constructor
- Trong các methods
- S? d?ng `using QuanLyTiecCuoi.BusinessLogicLayer.IService;`
- XÓA `using QuanLyTiecCuoi.BusinessLogicLayer.Service;`

### B??c 3: ??ng Ký Vào ServiceContainer (N?u không có tham s?)
```csharp
// Infrastructure/ServiceContainer.cs
services.AddTransient<MyViewModel>();
```

### B??c 4: C?p Nh?t N?i S? d?ng
```csharp
// TR??C
DataContext = new MyViewModel()

// SAU
DataContext = Infrastructure.ServiceContainer.GetService<MyViewModel>()
```

---

## ?? ?u Tiên S?a

**Th? t? khuy?n ngh?**:
1. **FoodViewModel** - ??n gi?n (2 services), ch? s?a constructor
2. **ServiceViewModel** - ??n gi?n (2 services), ch? s?a constructor
3. **HallTypeViewModel** - 2 services + 1 method s? d?ng service
4. **HallViewModel** - 3 services
5. **WeddingViewModel** - Ph?c t?p nh?t (3 services + Factory calls)

---

## ? Checklist Khi S?a M?i ViewModel

- [ ] Thêm parameters vào constructor
- [ ] Xóa t?t c? `new XxxService()`
- [ ] Xóa `using QuanLyTiecCuoi.BusinessLogicLayer.Service;`
- [ ] ??ng ký vào ServiceContainer (n?u c?n)
- [ ] C?p nh?t MainViewModel (ho?c n?i s? d?ng)
- [ ] Build và ki?m tra l?i

---

**Ti?n ??**: 9/15 ViewModels (60%) ?
**Còn l?i**: 5 ViewModels (33%)

M?i ViewModel còn l?i m?t kho?ng 5-10 phút ?? s?a theo pattern ?ã có!
