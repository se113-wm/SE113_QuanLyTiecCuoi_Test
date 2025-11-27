using System.Collections.Generic;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.DataAccessLayer.IRepository
{
    public interface IRevenueReportDetailRepository
    {
        IEnumerable<RevenueReportDetail> GetAll();
        IEnumerable<RevenueReportDetail> GetByMonthYear(int month, int year);
        RevenueReportDetail GetByDate(int day, int month, int year);
    }
}