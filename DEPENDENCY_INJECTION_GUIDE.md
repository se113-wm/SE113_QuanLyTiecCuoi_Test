# H??ng D?n Dependency Injection - D? án Qu?n Lý Ti?c C??i

## ?? T?ng Quan

?ã áp d?ng **Dependency Injection Pattern** cho toàn b? d? án v?i c?u trúc:

```
View 
  ? (l?y t? DI Container ho?c Factory Method)
ViewModel
  ? (inject)
Service (Business Logic Layer)
  ? (inject)
Repository (Data Access Layer)
  ?
Database (Entity Framework DbContext)
```

---

## ?? C?u Trúc DI

### 1. **Infrastructure/ServiceContainer.cs**
**Vai trò**: DI Container qu?n lý t?t c? dependencies + Factory methods cho ViewModels có tham s?

#### Services ?ã ??ng ký:

**Repositories (Transient):**
- `ICaRepository` ? `CaRepository`
- `IBaoCaoDsRepository` ? `BaoCaoDsRepository`
- `IChiTietDVRepository` ? `ChiTietDVRepository`
- `IDichVuRepository` ? `DichVuRepository`
- `ILoaiSanhRepository` ? `LoaiSanhRepository`
- `IMonAnRepository` ? `MonAnRepository`
- `INguoiDungRepository` ? `NguoiDungRepository`
- `IPhieuDatTiecRepository` ? `PhieuDatTiecRepository`
- `ISanhRepository` ? `SanhRepository`
- `IThamSoRepository` ? `ThamSoRepository`
- `IThucDonRepository` ? `ThucDonRepository`

**Services (Transient):**
- `ICaService` ? `CaService`
- `IChiTietDVService` ? `ChiTietDVService`
- `IDichVuService` ? `DichVuService`
- `ILoaiSanhService` ? `LoaiSanhService`
- `IMonAnService` ? `MonAnService`
- `INguoiDungService` ? `NguoiDungService`
- `IPhieuDatTiecService` ? `PhieuDatTiecService`
- `ISanhService` ? `SanhService`
- `IThamSoService` ? `ThamSoService`
- `IThucDonService` ? `ThucDonService`

**ViewModels (Transient):**
- `ShiftViewModel`

**Factory Methods:**
- `CreateWeddingDetailViewModel(int maPhieuDat)`
- `CreateAddWeddingViewModel()`
- `CreateInvoiceViewModel(int invoiceId)`

---

## ?? Cách S? D?ng

### A. ViewModel Không Có Tham S? (VD: ShiftViewModel)

#### Trong View Code-Behind:
```csharp
using QuanLyTiecCuoi.Infrastructure;

public ShiftView()
{
    InitializeComponent();
    this.DataContext = ServiceContainer.GetService<ShiftViewModel>();
}
```

#### Trong ViewModel khác:
```csharp
// MainViewModel.cs
ShiftCommand = new RelayCommand<object>((p) => true, (p) =>
{
    CurrentView = new ShiftView(); // ShiftView t? l?y ViewModel t? DI
    //...
});
```

---

### B. ViewModel Có Tham S? (VD: WeddingDetailViewModel)

#### S? d?ng Factory Method:
```csharp
using QuanLyTiecCuoi.Infrastructure;

// Trong MainViewModel.cs
public void SwitchToWeddingDetailTab(int weddingId)
{
    var viewModel = ServiceContainer.CreateWeddingDetailViewModel(weddingId);
    
    CurrentView = new WeddingDetailView()
    {
        DataContext = viewModel
    };
}
```

#### Ho?c trong View/Dialog:
```csharp
// Show Invoice dialog
ShowInvoiceCommand = new RelayCommand<object>((p) => true, (p) =>
{
    var invoiceView = new InvoiceView() 
    {
        DataContext = ServiceContainer.CreateInvoiceViewModel(_maPhieuDat)
    };
    invoiceView.ShowDialog();
});
```

---

## ?? ?ã Áp D?ng DI Cho

### ? Repositories (100%)
T?t c? Repository ?ã h? tr? constructor injection:
- CaRepository
- BaoCaoDsRepository
- ChiTietDVRepository
- DichVuRepository
- LoaiSanhRepository
- MonAnRepository
- NguoiDungRepository
- PhieuDatTiecRepository
- SanhRepository
- ThamSoRepository
- ThucDonRepository

### ? Services (100%)
T?t c? Service ?ã inject Repository:
- CaService
- ChiTietDVService
- DichVuService
- LoaiSanhService
- MonAnService
- PhieuDatTiecService
- SanhService
- ThamSoService
- ThucDonService

### ? ViewModels
- **ShiftViewModel** - inject `ICaService`, `IPhieuDatTiecService`
- **WeddingDetailViewModel** - inject 8 services (via factory)
- **AddWeddingViewModel** - inject 8 services (via factory)
- **InvoiceViewModel** - inject 6 services (via factory)

---

## ?? Pattern Áp D?ng

### 1. Constructor Injection (Repositories & Services)

**Repository Example:**
```csharp
public class CaRepository : ICaRepository
{
    private readonly QuanLyTiecCuoiEntities _context;

    // Constructor injection
    public CaRepository(QuanLyTiecCuoiEntities context)
    {
        _context = context ?? new QuanLyTiecCuoiEntities();
    }

    // Constructor m?c ??nh (backward compatibility)
    public CaRepository() : this(null) { }
    
    // ...methods
}
```

**Service Example:**
```csharp
public class CaService : ICaService
{
    private readonly ICaRepository _caRepository;

    // Constructor injection
    public CaService(ICaRepository caRepository)
    {
        _caRepository = caRepository;
    }
    
    // ...methods
}
```

### 2. Factory Pattern (ViewModels v?i tham s?)

```csharp
// ServiceContainer.cs
public static WeddingDetailViewModel CreateWeddingDetailViewModel(int maPhieuDat)
{
    return new WeddingDetailViewModel(
        maPhieuDat,
        GetService<ISanhService>(),
        GetService<ICaService>(),
        GetService<IPhieuDatTiecService>(),
        GetService<IMonAnService>(),
        GetService<IDichVuService>(),
        GetService<IThucDonService>(),
        GetService<IChiTietDVService>(),
        GetService<IThamSoService>()
    );
}
```

---

## ? L?i Ích ?ã ??t ???c

### 1. **Loose Coupling**
- ViewModels không ph? thu?c tr?c ti?p vào Services c? th?
- Services không ph? thu?c tr?c ti?p vào Repositories c? th?
- Có th? thay th? implementation b?t k? lúc nào

### 2. **Testability**
```csharp
// Mock service cho unit test
var mockCaService = new Mock<ICaService>();
var mockPhieuDatTiecService = new Mock<IPhieuDatTiecService>();
var viewModel = new ShiftViewModel(
    mockCaService.Object, 
    mockPhieuDatTiecService.Object
);
```

### 3. **Single Responsibility**
- M?i class ch? lo vi?c c?a mình
- Không lo kh?i t?o dependencies
- Code s?ch và d? ??c h?n

### 4. **Maintainability**
- Thay ??i implementation ch? c?n s?a ? `ServiceContainer`
- Không c?n s?a code ? nhi?u n?i
- D? dàng thêm logging, caching, validation...

---

## ?? So Sánh Tr??c & Sau

### TR??C (Kh?i t?o th? công):
```csharp
public ShiftViewModel()
{
    _caService = new CaService(); // ? Tight coupling
    _phieuDatTiecService = new PhieuDatTiecService();
    // ...
}
```

### SAU (Dependency Injection):
```csharp
public ShiftViewModel(
    ICaService caService, 
    IPhieuDatTiecService phieuDatTiecService) // ? Loose coupling
{
    _caService = caService;
    _phieuDatTiecService = phieuDatTiecService;
    // ...
}
```

---

## ?? Thêm Module M?i

### B??c 1: T?o Repository v?i DI
```csharp
public class MyRepository : IMyRepository
{
    private readonly QuanLyTiecCuoiEntities _context;

    public MyRepository(QuanLyTiecCuoiEntities context)
    {
        _context = context ?? new QuanLyTiecCuoiEntities();
    }

    public MyRepository() : this(null) { }
}
```

### B??c 2: T?o Service v?i DI
```csharp
public class MyService : IMyService
{
    private readonly IMyRepository _myRepository;

    public MyService(IMyRepository myRepository)
    {
        _myRepository = myRepository;
    }
}
```

### B??c 3: ??ng ký vào ServiceContainer
```csharp
// Infrastructure/ServiceContainer.cs
services.AddTransient<IMyRepository, MyRepository>();
services.AddTransient<IMyService, MyService>();
services.AddTransient<MyViewModel>(); // N?u không có tham s?
```

### B??c 4: S? d?ng
```csharp
// Trong View
this.DataContext = ServiceContainer.GetService<MyViewModel>();

// Ho?c t?o Factory Method n?u có tham s?
public static MyViewModel CreateMyViewModel(int id)
{
    return new MyViewModel(id, GetService<IMyService>());
}
```

---

## ?? Tài Li?u Tham Kh?o

- [Microsoft.Extensions.DependencyInjection Documentation](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
- [Dependency Injection in .NET](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-guidelines)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)

---

## ?? Best Practices

1. ? **Luôn inject interface**, không inject concrete class
2. ? **Constructor injection** là preferred method
3. ? **Transient lifetime** cho ViewModels và Services (m?i request t?o m?i)
4. ? **Factory pattern** cho ViewModels có tham s? ph?c t?p
5. ? **Backward compatibility** - gi? constructor m?c ??nh cho Repository n?u c?n
6. ? **Single source of truth** - t?t c? config DI ? m?t ch? (ServiceContainer)

---

**C?p nh?t l?n cu?i**: ?ã hoàn thành DI cho t?t c? Repositories, Services và các ViewModel chính.
