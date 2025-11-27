using System.Collections.Generic;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.BusinessLogicLayer.IService
{
    public interface IRevenueReportDetailService
    {
        IEnumerable<RevenueReportDetailDTO> GetAll();
        IEnumerable<RevenueReportDetailDTO> GetByMonthYear(int month, int year);
        RevenueReportDetailDTO GetByDate(int day, int month, int year);
    }
}