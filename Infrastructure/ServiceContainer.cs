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
    /// Container quản lý Dependency Injection cho toàn bộ ứng dụng
    /// </summary>
    public static class ServiceContainer
    {
        private static IServiceProvider _serviceProvider;

        /// <summary>
        /// Service Provider singleton - tự động khởi tạo nếu chưa có
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
        /// Cấu hình tất cả dependencies cho ứng dụng
        /// </summary>
        public static void Configure()
        {
            var services = new ServiceCollection();

            // ============================================
            // REGISTER REPOSITORIES (Transient - mỗi lần tạo mới)
            // ============================================
            services.AddTransient<IAppUserRepository, AppUserRepository>();
            services.AddTransient<IBookingRepository, BookingRepository>();
            services.AddTransient<IDishRepository, DishRepository>();
            services.AddTransient<IHallRepository, HallRepository>();
            services.AddTransient<IHallTypeRepository, HallTypeRepository>();
            services.AddTransient<IMenuRepository, MenuRepository>();
            services.AddTransient<IParameterRepository, ParameterRepository>();
            services.AddTransient<IPermissionRepository, PermissionRepository>();
            services.AddTransient<IRevenueReportRepository, RevenueReportRepository>();
            services.AddTransient<IRevenueReportDetailRepository, RevenueReportDetailRepository>();
            services.AddTransient<IServiceRepository, ServiceRepository>();
            services.AddTransient<IServiceDetailRepository, ServiceDetailRepository>();
            services.AddTransient<IShiftRepository, ShiftRepository>();
            services.AddTransient<IUserGroupRepository, UserGroupRepository>();

            // ============================================
            // REGISTER SERVICES (Transient - Business Logic Layer)
            // ============================================
            services.AddTransient<IAppUserService, AppUserService>();
            services.AddTransient<IBookingService, BookingService>();
            services.AddTransient<IDishService, DishService>();
            services.AddTransient<IHallService, HallService>();
            services.AddTransient<IHallTypeService, HallTypeService>();
            services.AddTransient<IMenuService, MenuService>();
            services.AddTransient<IParameterService, ParameterService>();
            services.AddTransient<IPermissionService, PermissionService>();
            services.AddTransient<IRevenueReportService, RevenueReportService>();
            services.AddTransient<IRevenueReportDetailService, RevenueReportDetailService>();
            services.AddTransient<IServiceService, ServiceService>();
            services.AddTransient<IServiceDetailService, ServiceDetailService>();
            services.AddTransient<IShiftService, ShiftService>();
            services.AddTransient<IUserGroupService, UserGroupService>();

            // ============================================
            // REGISTER VIEWMODELS (Transient - mỗi View mới sẽ có ViewModel mới)
            // ============================================
            services.AddTransient<LoginViewModel>();
            services.AddTransient<AccountViewModel>();
            services.AddTransient<UserViewModel>();
            services.AddTransient<PermissionViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<ShiftViewModel>();
            services.AddTransient<MenuItemViewModel>();
            services.AddTransient<ServiceDetailItemViewModel>();
            services.AddTransient<ParameterViewModel>();
            services.AddTransient<FoodViewModel>();
            services.AddTransient<ServiceViewModel>();
            services.AddTransient<HallViewModel>();
            services.AddTransient<HallTypeViewModel>();
            services.AddTransient<ReportViewModel>();
            
            // HomeViewModel cần MainViewModel, sẽ được tạo trong MainViewModel constructor
            // WeddingViewModel cần MainViewModel, sẽ được tạo trong MainViewModel
            // Thêm các ViewModel khác ở đây khi cần...

            _serviceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// Lấy service từ container theo kiểu generic
        /// </summary>
        /// <typeparam name="T">Kiểu service cần lấy</typeparam>
        /// <returns>Instance của service</returns>
        public static T GetService<T>()
        {
            return ServiceProvider.GetService<T>();
        }

        /// <summary>
        /// Tạo scope mới cho một phạm vi hoạt động
        /// </summary>
        public static IServiceScope CreateScope()
        {
            return ServiceProvider.CreateScope();
        }

        // ============================================
        // FACTORY METHODS cho ViewModels có parameters
        // ============================================

        /// <summary>
        /// Tạo WeddingDetailViewModel với DI
        /// </summary>
        public static WeddingDetailViewModel CreateWeddingDetailViewModel(int bookingId)
        {
            return new WeddingDetailViewModel(
                bookingId,
                GetService<IHallService>(),
                GetService<IShiftService>(),
                GetService<IBookingService>(),
                GetService<IDishService>(),
                GetService<IServiceService>(),
                GetService<IMenuService>(),
                GetService<IServiceDetailService>(),
                GetService<IParameterService>()
            );
        }

        /// <summary>
        /// Tạo AddWeddingViewModel với DI
        /// </summary>
        public static AddWeddingViewModel CreateAddWeddingViewModel()
        {
            return new AddWeddingViewModel(
                GetService<IHallService>(),
                GetService<IShiftService>(),
                GetService<IBookingService>(),
                GetService<IDishService>(),
                GetService<IServiceService>(),
                GetService<IMenuService>(),
                GetService<IServiceDetailService>(),
                GetService<IParameterService>()
            );
        }

        /// <summary>
        /// Tạo InvoiceViewModel với DI
        /// </summary>
        public static InvoiceViewModel CreateInvoiceViewModel(int invoiceId)
        {
            return new InvoiceViewModel(
                invoiceId,
                GetService<IBookingService>(),
                GetService<IShiftService>(),
                GetService<IHallService>(),
                GetService<IServiceDetailService>(),
                GetService<IMenuService>(),
                GetService<IParameterService>()
            );
        }
    }
}
