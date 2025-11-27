using System.Collections.Generic;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.BusinessLogicLayer.IService
{
    public interface IRevenueReportService
    {
        IEnumerable<RevenueReportDTO> GetAll();
        RevenueReportDTO GetByMonthYear(int month, int year);
    }
}