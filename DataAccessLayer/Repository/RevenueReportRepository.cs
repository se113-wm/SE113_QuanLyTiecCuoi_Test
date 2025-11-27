using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.DataAccessLayer.Repository
{
    public class RevenueReportRepository : IRevenueReportRepository
    {
        private readonly QuanLyTiecCuoiEntities _context;

        public RevenueReportRepository()
        {
            _context = new QuanLyTiecCuoiEntities();
        }

        public IEnumerable<RevenueReport> GetAll()
        {
            return _context.RevenueReports.Include("RevenueReportDetails").ToList();
        }

        public RevenueReport GetByMonthYear(int month, int year)
        {
            return _context.RevenueReports
                .Include("RevenueReportDetails")
                .FirstOrDefault(r => r.Month == month && r.Year == year);
        }
    }
}