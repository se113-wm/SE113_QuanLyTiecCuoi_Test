using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Service
{
    public class RevenueReportService : IRevenueReportService
    {
        private readonly IRevenueReportRepository _revenueReportRepository;

        public RevenueReportService(IRevenueReportRepository revenueReportRepository)
        {
            _revenueReportRepository = revenueReportRepository;
        }

        public IEnumerable<RevenueReportDTO> GetAll()
        {
            return _revenueReportRepository.GetAll()
                .Select(x => new RevenueReportDTO
                {
                    Month = x.Month,
                    Year = x.Year,
                    TotalRevenue = x.TotalRevenue,
                    RevenueReportDetails = x.RevenueReportDetails?.Select(d => new RevenueReportDetailDTO
                    {
                        Day = d.Day,
                        Month = d.Month,
                        Year = d.Year,
                        WeddingCount = d.WeddingCount,
                        Revenue = d.Revenue,
                        Ratio = d.Ratio
                    }).ToList()
                });
        }

        public RevenueReportDTO GetByMonthYear(int month, int year)
        {
            var entity = _revenueReportRepository.GetByMonthYear(month, year);
            if (entity == null) return null;
            return new RevenueReportDTO
            {
                Month = entity.Month,
                Year = entity.Year,
                TotalRevenue = entity.TotalRevenue,
                RevenueReportDetails = entity.RevenueReportDetails?.Select(d => new RevenueReportDetailDTO
                {
                    Day = d.Day,
                    Month = d.Month,
                    Year = d.Year,
                    WeddingCount = d.WeddingCount,
                    Revenue = d.Revenue,
                    Ratio = d.Ratio
                }).ToList()
            };
        }
    }
}