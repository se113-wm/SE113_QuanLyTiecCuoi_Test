using Microsoft.Extensions.DependencyInjection;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataAccessLayer.Repository;
using QuanLyTiecCuoi.Presentation.ViewModel;
using QuanLyTiecCuoi.ViewModel;
using System;

namespace QuanLyTiecCuoi.Infrastructure
{
    /// <summary>
    /// Container qu?n lý Dependency Injection cho toàn b? ?ng d?ng
    /// </summary>
    public static class ServiceContainer
    {
        private static IServiceProvider _serviceProvider;

        /// <summary>
        /// Service Provider singleton - t? ??ng kh?i t?o n?u ch?a có
        /// </summary>
        public static IServiceProvider ServiceProvider
        {
            get
            {
                if (_serviceProvider == null)
                {
                    Configure();
                }
                return _serviceProvider;
            }
        }

        /// <summary>
        /// C?u hình t?t c? dependencies cho ?ng d?ng
        /// </summary>
        public static void Configure()
        {
            var services = new ServiceCollection();

            // ============================================
            // REGISTER REPOSITORIES (Transient - m?i l?n t?o m?i)
            // ============================================
            services.AddTransient<ICaRepository, CaRepository>();
            services.AddTransient<IBaoCaoDsRepository, BaoCaoDsRepository>();
            services.AddTransient<IChiTietDVRepository, ChiTietDVRepository>();
            services.AddTransient<IDichVuRepository, DichVuRepository>();
            services.AddTransient<ILoaiSanhRepository, LoaiSanhRepository>();
            services.AddTransient<IMonAnRepository, MonAnRepository>();
            services.AddTransient<INguoiDungRepository, NguoiDungRepository>();
            services.AddTransient<IPhieuDatTiecRepository, PhieuDatTiecRepository>();
            services.AddTransient<IHallRepository, HallRepository>();
            services.AddTransient<IThamSoRepository, ThamSoRepository>();
            services.AddTransient<IThucDonRepository, ThucDonRepository>();

            // ============================================
            // REGISTER SERVICES (Transient - Business Logic Layer)
            // ============================================
            services.AddTransient<ICaService, CaService>();
            services.AddTransient<IChiTietDVService, ChiTietDVService>();
            services.AddTransient<IDichVuService, DichVuService>();
            services.AddTransient<ILoaiSanhService, LoaiSanhService>();
            services.AddTransient<IMonAnService, MonAnService>();
            services.AddTransient<INguoiDungService, NguoiDungService>();
            services.AddTransient<IPhieuDatTiecService, PhieuDatTiecService>();
            services.AddTransient<IHallService, HallService>();
            services.AddTransient<IThamSoService, ThamSoService>();
            services.AddTransient<IThucDonService, ThucDonService>();

            // ============================================
            // REGISTER VIEWMODELS (Transient - m?i View m?i s? có ViewModel m?i)
            // ============================================
            services.AddTransient<MainViewModel>();
            services.AddTransient<ShiftViewModel>();
            services.AddTransient<MenuItemViewModel>();
            services.AddTransient<ServiceDetailItemViewModel>();
            services.AddTransient<ParameterViewModel>();
            services.AddTransient<FoodViewModel>();
            services.AddTransient<ServiceViewModel>();
            services.AddTransient<HallViewModel>();
            services.AddTransient<HallTypeViewModel>();
            
            // HomeViewModel c?n MainViewModel, s? ???c t?o trong MainViewModel constructor
            // WeddingViewModel c?n MainViewModel, s? ???c t?o trong MainViewModel
            // Thêm các ViewModel khác ? ?ây khi c?n...

            _serviceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// L?y service t? container theo ki?u generic
        /// </summary>
        /// <typeparam name="T">Ki?u service c?n l?y</typeparam>
        /// <returns>Instance c?a service</returns>
        public static T GetService<T>()
        {
            return ServiceProvider.GetService<T>();
        }

        /// <summary>
        /// T?o scope m?i cho m?t ph?m vi ho&t ??ng
        /// </summary>
        public static IServiceScope CreateScope()
        {
            return ServiceProvider.CreateScope();
        }

        // ============================================
        // FACTORY METHODS cho ViewModels có parameters
        // ============================================

        /// <summary>
        /// T?o WeddingDetailViewModel v?i DI
        /// </summary>
        public static WeddingDetailViewModel CreateWeddingDetailViewModel(int maPhieuDat)
        {
            return new WeddingDetailViewModel(
                maPhieuDat,
                GetService<IHallService>(),
                GetService<ICaService>(),
                GetService<IPhieuDatTiecService>(),
                GetService<IMonAnService>(),
                GetService<IDichVuService>(),
                GetService<IThucDonService>(),
                GetService<IChiTietDVService>(),
                GetService<IThamSoService>()
            );
        }

        /// <summary>
        /// T?o AddWeddingViewModel v?i DI
        /// </summary>
        public static AddWeddingViewModel CreateAddWeddingViewModel()
        {
            return new AddWeddingViewModel(
                GetService<IHallService>(),
                GetService<ICaService>(),
                GetService<IPhieuDatTiecService>(),
                GetService<IMonAnService>(),
                GetService<IDichVuService>(),
                GetService<IThucDonService>(),
                GetService<IChiTietDVService>(),
                GetService<IThamSoService>()
            );
        }

        /// <summary>
        /// T?o InvoiceViewModel v?i DI
        /// </summary>
        public static InvoiceViewModel CreateInvoiceViewModel(int invoiceId)
        {
            return new InvoiceViewModel(
                invoiceId,
                GetService<IPhieuDatTiecService>(),
                GetService<ICaService>(),
                GetService<IHallService>(),
                GetService<IChiTietDVService>(),
                GetService<IThucDonService>(),
                GetService<IThamSoService>()
            );
        }
    }
}
