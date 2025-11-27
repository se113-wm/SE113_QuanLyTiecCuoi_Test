using System.Collections.Generic;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.DataAccessLayer.IRepository
{
    public interface IRevenueReportRepository
    {
        IEnumerable<RevenueReport> GetAll();
        RevenueReport GetByMonthYear(int month, int year);
    }
}