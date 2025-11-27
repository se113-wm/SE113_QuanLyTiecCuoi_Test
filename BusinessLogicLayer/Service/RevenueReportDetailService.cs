using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Service
{
    public class RevenueReportDetailService : IRevenueReportDetailService
    {
        private readonly IRevenueReportDetailRepository _revenueReportDetailRepository;

        public RevenueReportDetailService(IRevenueReportDetailRepository revenueReportDetailRepository)
        {
            _revenueReportDetailRepository = revenueReportDetailRepository;
        }

        public IEnumerable<RevenueReportDetailDTO> GetAll()
        {
            return _revenueReportDetailRepository.GetAll()
                .Select(x => new RevenueReportDetailDTO
                {
                    Day = x.Day,
                    Month = x.Month,
                    Year = x.Year,
                    WeddingCount = x.WeddingCount,
                    Revenue = x.Revenue,
                    Ratio = x.Ratio
                });
        }

        public IEnumerable<RevenueReportDetailDTO> GetByMonthYear(int month, int year)
        {
            return _revenueReportDetailRepository.GetByMonthYear(month, year)
                .Select(x => new RevenueReportDetailDTO
                {
                    Day = x.Day,
                    Month = x.Month,
                    Year = x.Year,
                    WeddingCount = x.WeddingCount,
                    Revenue = x.Revenue,
                    Ratio = x.Ratio
                });
        }

        public RevenueReportDetailDTO GetByDate(int day, int month, int year)
        {
            var entity = _revenueReportDetailRepository.GetByDate(day, month, year);
            if (entity == null) return null;
            return new RevenueReportDetailDTO
            {
                Day = entity.Day,
                Month = entity.Month,
                Year = entity.Year,
                WeddingCount = entity.WeddingCount,
                Revenue = entity.Revenue,
                Ratio = entity.Ratio
            };
        }
    }
}