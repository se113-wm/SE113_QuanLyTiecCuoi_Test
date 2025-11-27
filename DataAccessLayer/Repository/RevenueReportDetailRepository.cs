using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.DataAccessLayer.Repository
{
    public class RevenueReportDetailRepository : IRevenueReportDetailRepository
    {
        private readonly QuanLyTiecCuoiEntities _context;

        public RevenueReportDetailRepository()
        {
            _context = new QuanLyTiecCuoiEntities();
        }

        public IEnumerable<RevenueReportDetail> GetAll()
        {
            return _context.RevenueReportDetails.ToList();
        }

        public IEnumerable<RevenueReportDetail> GetByMonthYear(int month, int year)
        {
            return _context.RevenueReportDetails
                .Where(d => d.Month == month && d.Year == year)
                .ToList();
        }

        public RevenueReportDetail GetByDate(int day, int month, int year)
        {
            return _context.RevenueReportDetails
                .FirstOrDefault(d => d.Day == day && d.Month == month && d.Year == year);
        }
    }
}